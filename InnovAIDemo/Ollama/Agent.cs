using OllamaSharp;
using OllamaSharp.Models.Chat;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace InnovAIDemo.Ollama
{
    public class Agent(IOllamaApiClient ollamaClient, ILogger logger)
    {
        public Agent(IOllamaApiClient ollamaClient, ILogger logger, string systemPrompt) : this(ollamaClient, logger)
        {
            chatMessages.Add(new Message(ChatRole.System, systemPrompt));
        }

        private readonly List<Message> chatMessages = [];
        private readonly List<AgentToolEntry> agentTools = [];

        public IEnumerable<AgentToolEntry> AgentTools => agentTools.AsEnumerable();


        private static AgentToolEntry CreateAgentToolEntry(string functionName, string description, Delegate function)
        {
            return CreateAgentToolEntry(functionName, description, function.Target, function.GetMethodInfo());
        }
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
        private static AgentToolEntry[] CreateAgentToolEntries(object instance)
        {
            var agentTools = instance.GetType().GetMethods()
                .Where(m => m.GetCustomAttributes(typeof(AgentToolAttribute), false).Length > 0)
                .Select(m => CreateAgentToolEntry(m.Name, ((AgentToolAttribute)m.GetCustomAttributes(typeof(AgentToolAttribute), false).First()).Description, instance, m));

            return agentTools.ToArray();
        }

        public Agent AddAgentTool(string functionName, string description, Delegate function)
        {
            agentTools.Add(CreateAgentToolEntry(functionName, description, function));
            return this;
        }
        public Agent AddAgentToolsFrom(object instance)
        {
            var tools = CreateAgentToolEntries(instance);
            agentTools.AddRange(tools);
            return this;
        }

        public IAsyncEnumerable<string> SendAsync(string message, IEnumerable<string>? imagesAsBase64 = null, CancellationToken cancellationToken = default)
        {
            return SendAsAsync(ChatRole.User, message, imagesAsBase64, cancellationToken);
        }
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

        public record AgentToolEntry(Tool Tool, object? Target, MethodInfo Method)
        {
            public string Name => Tool.Function.Name;
        }

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
