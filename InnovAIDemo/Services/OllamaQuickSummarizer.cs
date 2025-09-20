using OllamaSharp;

namespace InnovAIDemo.Services
{
    /// <summary>
    /// Service for creating brief summaries of text using the Ollama AI model.
    /// Uses a lightweight model for quick summarization tasks.
    /// </summary>
    /// <param name="ollamaApiClient">The Ollama API client</param>
    /// <param name="logger">Logger for this service</param>
    public class OllamaQuickSummarizer(IOllamaApiClient ollamaApiClient, ILogger<OllamaQuickSummarizer> logger)
    {
        /// <summary>
        /// Creates a brief summary of the provided text.
        /// </summary>
        /// <param name="text">The text to summarize</param>
        /// <returns>A brief summary of the input text</returns>
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
