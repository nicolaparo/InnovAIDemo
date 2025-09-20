using OllamaSharp;
using OllamaSharp.Models.Chat;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace InnovAIDemo.Ollama
{
    /// <summary>
    /// Represents an AI agent that can interact with users and execute tools.
    /// Manages conversation history and handles tool execution for enhanced AI capabilities.
    /// </summary>
    /// <param name="ollamaClient">The Ollama API client for AI communication</param>
    /// <param name="logger">Logger for tracking agent operations</param>
    public class Agent(IOllamaApiClient ollamaClient, ILogger logger)
    {
        /// <summary>
        /// Initializes a new instance of the Agent class with a system prompt.
        /// </summary>
        /// <param name="ollamaClient">The Ollama API client for AI communication</param>
        /// <param name="logger">Logger for tracking agent operations</param>
        /// <param name="systemPrompt">The system prompt to initialize the agent with</param>
        public Agent(IOllamaApiClient ollamaClient, ILogger logger, string systemPrompt) : this(ollamaClient, logger)
        {
            chatMessages.Add(new Message(ChatRole.System, systemPrompt));
        }

        /// <summary>
        /// List of chat messages in the conversation history
        /// </summary>
        private readonly List<Message> chatMessages = [];
        
        /// <summary>
        /// List of available tools that the agent can execute
        /// </summary>
        private readonly List<AgentToolEntry> agentTools = [];

        /// <summary>
        /// Gets the available agent tools
        /// </summary>
        public IEnumerable<AgentToolEntry> AgentTools => agentTools.AsEnumerable();


        /// <summary>
        /// Creates a tool entry from a function name, description, and delegate.
        /// </summary>
        /// <param name="functionName">The name of the function</param>
        /// <param name="description">Description of what the function does</param>
        /// <param name="function">The delegate to execute</param>
        /// <returns>An agent tool entry</returns>
        private static AgentToolEntry CreateAgentToolEntry(string functionName, string description, Delegate function)
        {
            return CreateAgentToolEntry(functionName, description, function.Target, function.GetMethodInfo());
        }
        
        /// <summary>
        /// Creates a tool entry from method information and target object.
        /// </summary>
        /// <param name="functionName">The name of the function</param>
        /// <param name="description">Description of what the function does</param>
        /// <param name="invocationTarget">The object instance to invoke the method on</param>
        /// <param name="method">The method information</param>
        /// <returns>An agent tool entry</returns>
        private static AgentToolEntry CreateAgentToolEntry(string functionName, string description, object? invocationTarget, MethodInfo method)
        {
            var tool = new Tool()
            {
                Function = new Function()
                {
                    Name = functionName,
                    Description = description,
                    Parameters = new Parameters()
                    {
                        Properties = method.GetParameters().ToDictionary(
                            p => p.Name ?? throw new InvalidOperationException("Parameter name cannot be null"),
                            p => new Property()
                            {
                                Type = p.ParameterType.Name.ToLower(),
                                Description = p.GetCustomAttribute<DescriptionAttribute>()?.Description ?? string.Empty
                            }),
                        Required = method.GetParameters().Where(p => !p.IsOptional).Select(p => p.Name!).ToArray()
                    }
                }
            };
            return new AgentToolEntry(tool, invocationTarget, method);
        }
        
        /// <summary>
        /// Creates tool entries from all methods in an object that have the AgentTool attribute.
        /// </summary>
        /// <param name="instance">The object instance to scan for agent tools</param>
        /// <returns>Array of agent tool entries</returns>
        private static AgentToolEntry[] CreateAgentToolEntries(object instance)
        {
            var agentTools = instance.GetType().GetMethods()
                .Where(m => m.GetCustomAttributes(typeof(AgentToolAttribute), false).Length > 0)
                .Select(m => CreateAgentToolEntry(m.Name, ((AgentToolAttribute)m.GetCustomAttributes(typeof(AgentToolAttribute), false).First()).Description, instance, m));

            return agentTools.ToArray();
        }

        /// <summary>
        /// Adds a single tool to the agent.
        /// </summary>
        /// <param name="functionName">The name of the function</param>
        /// <param name="description">Description of what the function does</param>
        /// <param name="function">The delegate to execute</param>
        /// <returns>The agent instance for method chaining</returns>
        public Agent AddAgentTool(string functionName, string description, Delegate function)
        {
            agentTools.Add(CreateAgentToolEntry(functionName, description, function));
            return this;
        }
        
        /// <summary>
        /// Adds all tools from an object instance that have the AgentTool attribute.
        /// </summary>
        /// <param name="instance">The object instance to scan for agent tools</param>
        /// <returns>The agent instance for method chaining</returns>
        public Agent AddAgentToolsFrom(object instance)
        {
            var tools = CreateAgentToolEntries(instance);
            agentTools.AddRange(tools);
            return this;
        }

        /// <summary>
        /// Sends a message to the agent as a user and returns the response.
        /// </summary>
        /// <param name="message">The message to send</param>
        /// <param name="imagesAsBase64">Optional base64-encoded images to include</param>
        /// <param name="cancellationToken">Cancellation token for the operation</param>
        /// <returns>An async enumerable of response chunks</returns>
        public IAsyncEnumerable<string> SendAsync(string message, IEnumerable<string>? imagesAsBase64 = null, CancellationToken cancellationToken = default)
        {
            return SendAsAsync(ChatRole.User, message, imagesAsBase64, cancellationToken);
        }
        
        /// <summary>
        /// Sends a message to the agent with a specific role and returns the response.
        /// Handles tool execution if the agent decides to call tools.
        /// </summary>
        /// <param name="role">The role for the message (User, Assistant, etc.)</param>
        /// <param name="message">The message to send</param>
        /// <param name="imagesAsBase64">Optional base64-encoded images to include</param>
        /// <param name="cancellationToken">Cancellation token for the operation</param>
        /// <returns>An async enumerable of response chunks</returns>
        public async IAsyncEnumerable<string> SendAsAsync(ChatRole role, string message, IEnumerable<string>? imagesAsBase64 = null
            , [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            logger.LogInformation($"Sending {role}: {message}");

            var chatMessage = new Message();
            chatMessage.Role = role;
            chatMessage.Content = message;

            if (imagesAsBase64 is not null)
                chatMessage.Images = [.. imagesAsBase64];

            chatMessages.Add(chatMessage);

            while (!cancellationToken.IsCancellationRequested)
            {
                var request = new ChatRequest()
                {
                    Messages = [.. chatMessages],
                    Tools = agentTools.Select(at => at.Tool),
                    Model = ollamaClient.SelectedModel,
                    Stream = true,
                };

                var responseBuilder = new StringBuilder();
                Message.ToolCall? toolCall = null;

                await foreach (var part in ollamaClient.ChatAsync(request))
                {
                    if (part.Message.ToolCalls is not null)
                        toolCall = part.Message.ToolCalls.FirstOrDefault();

                    if (toolCall is null)
                    {
                        var content = part.Message.Content;
                        responseBuilder.Append(content);
                        yield return content;
                    }
                }

                if (toolCall is null)
                {
                    chatMessages.Add(new Message(ChatRole.Assistant, responseBuilder.ToString()));
                    break;
                }
                else
                {
                    logger.LogInformation($"Starting tool invocation: {toolCall.Function.Name}");
                    // invoke tool
                    string? errorMessage = null;

                    var toolExecutionId = Guid.NewGuid();

                    try
                    {
                        var result = await ExecuteToolAsync(toolCall);
                        var jsonResult = JsonSerializer.Serialize(result);
                        chatMessages.Add(new Message(ChatRole.Tool, jsonResult));
                    }
                    catch (Exception ex)
                    {
                        errorMessage = $"Error executing tool '{toolCall.Function.Name}': {ex.Message}";
                        chatMessages.Add(new Message(ChatRole.Tool, errorMessage));
                    }

                    if (errorMessage is not null)
                        yield return errorMessage;


                    logger.LogInformation($"Completed tool invocation: {toolCall.Function.Name}");
                }
            }
        }

        /// <summary>
        /// Represents an agent tool that can be executed by the AI.
        /// Contains the tool definition, target object, and method information.
        /// </summary>
        /// <param name="Tool">The tool definition for the AI</param>
        /// <param name="Target">The target object to invoke the method on</param>
        /// <param name="Method">The method information for execution</param>
        public record AgentToolEntry(Tool Tool, object? Target, MethodInfo Method)
        {
            /// <summary>
            /// Gets the name of the tool
            /// </summary>
            public string Name => Tool.Function.Name;
        }

        /// <summary>
        /// Executes a tool call from the AI agent.
        /// </summary>
        /// <param name="toolCall">The tool call to execute</param>
        /// <returns>The result of the tool execution</returns>
        private async Task<object> ExecuteToolAsync(Message.ToolCall toolCall)
        {
            var agentTool = agentTools.FirstOrDefault(at => at.Tool.Function.Name == toolCall.Function.Name) ?? throw new InvalidOperationException($"No plugin found for tool call: {toolCall.Function.Name}");
            var method = agentTool.Method ?? throw new InvalidOperationException($"No method found for tool call: {toolCall.Function.Name}");
            List<object?> arguments = ExtractToolCallArguments(toolCall);

            var invocationId = Guid.NewGuid();

            var result = method.Invoke(agentTool.Target, arguments.ToArray());

            if (result is Task taskResult)
            {
                await taskResult;
                result = taskResult.GetType().GetProperty("Result")?.GetValue(taskResult);
            }

            if (result is null)
                return "Done";
            else
                return result;
        }

        /// <summary>
        /// Extracts arguments from a tool call for method invocation.
        /// </summary>
        /// <param name="toolCall">The tool call containing arguments</param>
        /// <returns>List of extracted arguments</returns>
        private static List<object?> ExtractToolCallArguments(Message.ToolCall toolCall)
        {
            List<object?> arguments = [];

            foreach (var (index, (key, value)) in toolCall.Function.Arguments.Index())
            {
                if (value is JsonElement jsonElement)
                {
                    if (jsonElement.ValueKind == JsonValueKind.Null)
                    {
                        arguments.Add(null);
                        continue;
                    }
                    if (jsonElement.ValueKind == JsonValueKind.String)
                    {
                        arguments.Add(jsonElement.GetString());
                        continue;
                    }
                    if (jsonElement.ValueKind == JsonValueKind.Number)
                    {
                        if (jsonElement.TryGetInt32(out var intValue))
                        {
                            arguments.Add(intValue);
                            continue;
                        }
                        else if (jsonElement.TryGetDouble(out var doubleValue))
                        {
                            arguments.Add(doubleValue);
                            continue;
                        }
                    }
                }
            }

            return arguments;
        }
    }
}
