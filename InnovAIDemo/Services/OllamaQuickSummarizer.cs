using OllamaSharp;

namespace InnovAIDemo.Services
{
    public class OllamaQuickSummarizer(IOllamaApiClient ollamaApiClient, ILogger<OllamaQuickSummarizer> logger)
    {
        public async Task<string> SummarizeBrieflyAsync(string text)
        {
            logger.LogInformation("Summarizing...");

            var chat = new Chat(ollamaApiClient);
            chat.Model = "gemma3:270m";
            var result = await chat.SendAsync($"""
                create a brief summary {text}
                """).StreamToEndAsync();

            logger.LogInformation("Summarization done");

            return result;
        }
    }
}
