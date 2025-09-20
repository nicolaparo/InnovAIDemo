namespace InnovAIDemo.Services
{
    public class WikipediaSearchService(HttpClient client, OllamaQuickSummarizer summarizer, ILogger<WikipediaSearchService> logger)
    {
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
