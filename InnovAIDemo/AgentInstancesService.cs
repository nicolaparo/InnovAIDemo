using System.Collections.Concurrent;
using InnovAIDemo.Ollama;
using OllamaSharp;

using Chat = Telegram.Bot.Types.Chat;

namespace InnovAIDemo
{
    /// <summary>
    /// Service responsible for managing AI agent instances for different chat sessions.
    /// Each chat gets its own agent instance to maintain conversation context.
    /// </summary>
    /// <param name="ollamaApiClient">The Ollama API client for AI communication</param>
    /// <param name="agentTools">Service providing tools for the AI agent</param>
    /// <param name="logger">Logger for this service</param>
    public class AgentInstancesService(IOllamaApiClient ollamaApiClient, AgentToolsService agentTools, ILogger<AgentInstancesService> logger)
    {
        /// <summary>
        /// Thread-safe dictionary to store agent instances per chat ID
        /// </summary>
        private ConcurrentDictionary<object, Agent> agents = new();
        
        /// <summary>
        /// Gets or creates an agent instance for the specified chat.
        /// </summary>
        /// <param name="chat">The Telegram chat to get an agent for</param>
        /// <returns>An agent instance associated with the chat</returns>
        public Agent GetAgentInstance(Chat chat) => agents.GetOrAdd(chat.Id, CreateAgent(chat));
        
        /// <summary>
        /// Creates a new agent instance for the specified chat.
        /// </summary>
        /// <param name="chat">The Telegram chat to create an agent for</param>
        /// <returns>A new agent instance configured for the chat</returns>
        public Agent CreateAgent(Chat chat)
        {
            var systemPrompt = $"""
                You are Coparot. You are a helpful assistant. brief and concise.
                You are talking with {chat.FirstName} {chat.LastName}.
                """;

            var agent = new Agent(ollamaApiClient, logger, systemPrompt);

            agent.AddAgentToolsFrom(agentTools);

            return agent;
        }
    }
}
