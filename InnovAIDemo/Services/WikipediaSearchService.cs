namespace InnovAIDemo.Services
{
    /// <summary>
    /// Service for searching Wikipedia and providing summarized results.
    /// Fetches Wikipedia page summaries and uses AI to create brief summaries.
    /// </summary>
    /// <param name="client">HTTP client for Wikipedia API calls</param>
    /// <param name="summarizer">Service for summarizing the Wikipedia content</param>
    /// <param name="logger">Logger for this service</param>
    public class WikipediaSearchService(HttpClient client, OllamaQuickSummarizer summarizer, ILogger<WikipediaSearchService> logger)
    {
        /// <summary>
        /// Searches Wikipedia for a given query and returns a summarized result.
        /// </summary>
        /// <param name="query">The search term to look up on Wikipedia</param>
        /// <param name="cancellationToken">Cancellation token for the operation</param>
        /// <returns>A summarized version of the Wikipedia page content</returns>
        public async Task<string> SearchAsync(string query, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Searching Wikipedia for query: {Query}", query);

            // user agent
            client.DefaultRequestHeaders.UserAgent.ParseAdd("InnovAIDemoApp/1.0");

            var url = $"https://en.wikipedia.org/api/rest_v1/page/summary/{Uri.EscapeDataString(query)}";
            var result = await client.GetStringAsync(url, cancellationToken);

            return await summarizer.SummarizeBrieflyAsync(result);
        }
    }
}
