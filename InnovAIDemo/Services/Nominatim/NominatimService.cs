namespace InnovAIDemo.Services.Nominatim
{
    /// <summary>
    /// Service for geocoding location names using the Nominatim API from OpenStreetMap.
    /// Converts location names into geographic coordinates.
    /// </summary>
    /// <param name="client">HTTP client for API calls</param>
    /// <param name="logger">Logger for this service</param>
    public class NominatimService(HttpClient client, ILogger<NominatimService> logger)
    {
        /// <summary>
        /// Searches for geographic locations by name and returns coordinate information.
        /// </summary>
        /// <param name="query">The location name to search for</param>
        /// <param name="cancellationToken">Cancellation token for the operation</param>
        /// <returns>Array of location search results with coordinates</returns>
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
