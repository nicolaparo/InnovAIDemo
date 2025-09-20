using System.Collections.Concurrent;
using InnovAIDemo.Ollama;
using OllamaSharp;

using Chat = Telegram.Bot.Types.Chat;

namespace InnovAIDemo
{
    public class AgentInstancesService(IOllamaApiClient ollamaApiClient, AgentToolsService agentTools, ILogger<AgentInstancesService> logger)
    {
        private ConcurrentDictionary<object, Agent> agents = new();
        public Agent GetAgentInstance(Chat chat) => agents.GetOrAdd(chat.Id, CreateAgent(chat));
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
