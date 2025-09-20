namespace InnovAIDemo.Services.Nominatim
{
    public class NominatimService(HttpClient client, ILogger<NominatimService> logger)
    {
        public async Task<NominatimSearchResponse[]?> SearchAsync(string query, CancellationToken cancellationToken = default)
        {
            logger.LogInformation($"Searching Nominatim for query: {query}");

            // set a custom user-agent as required by Nominatim's usage policy
            client.DefaultRequestHeaders.UserAgent.ParseAdd("InnovAIDemoApp/1.0");

            var url = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(query)}&format=json&addressdetails=1&limit=5";

            using var response = await client.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadFromJsonAsync<NominatimSearchResponse[]>(cancellationToken);
            return content;
        }
    }

}
