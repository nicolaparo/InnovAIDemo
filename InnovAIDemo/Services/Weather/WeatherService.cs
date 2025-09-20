using InnovAIDemo.Services.Nominatim;

namespace InnovAIDemo.Services.Weather
{
    /// <summary>
    /// Service for retrieving weather information from Open-Meteo API.
    /// Supports weather lookup by location name or coordinates.
    /// </summary>
    /// <param name="httpClient">HTTP client for API calls</param>
    /// <param name="nomatimService">Service for location geocoding</param>
    /// <param name="logger">Logger for this service</param>
    public class WeatherService(HttpClient httpClient, NominatimService nomatimService, ILogger<WeatherService> logger)
    {
        /// <summary>
        /// Gets weather information for a specified location by name.
        /// Uses geocoding to convert location name to coordinates.
        /// </summary>
        /// <param name="location">The location name to get weather for</param>
        /// <param name="cancellationToken">Cancellation token for the operation</param>
        /// <returns>Array of weather data points</returns>
        public async Task<WeatherServiceResponseItem[]> GetWeatherAtLocationAsync(string location, CancellationToken cancellationToken = default)
        {
            logger.LogInformation($"Getting weather for location: {location}");

            var results = await nomatimService.SearchAsync(location, cancellationToken);
            if (results is null || results.Length == 0)
            {
                logger.LogInformation($"Location '{location}' not found.");
                return [];
            }
            var firstResult = results[0];

            return await GetWeatherAsync(firstResult.Longitude, firstResult.Latitude);
        }
        
        /// <summary>
        /// Gets weather information for specified coordinates.
        /// </summary>
        /// <param name="longitude">The longitude coordinate</param>
        /// <param name="latitude">The latitude coordinate</param>
        /// <param name="cancellationToken">Cancellation token for the operation</param>
        /// <returns>Array of weather data points</returns>
        public async Task<WeatherServiceResponseItem[]> GetWeatherAsync(string longitude, string latitude, CancellationToken cancellationToken = default)
        {
            logger.LogInformation($"Getting weather for coordinates: {latitude}, {longitude}");

            var url = $"https://api.open-meteo.com/v1/forecast?latitude={latitude}&longitude={longitude}&hourly=temperature_2m,weather_code";

            var response = await httpClient.GetFromJsonAsync<OpenMeteoResponse>(url, cancellationToken);

            var zip = Enumerable.Zip(response.Hourly.Time, response.Hourly.Temperature2m, response.Hourly.WeatherCode)
                .Select(z =>
                {
                    var (time, temperatureCelsius, wcode) = z;
                    var weatherCode = WeatherCode.GetWeatherCode(wcode);

                    return new WeatherServiceResponseItem
                    {
                        Time = time,
                        TemperatureCelsius = temperatureCelsius,
                        Weather = weatherCode?.Description,
                        Icon = weatherCode?.Emoji
                    };

                })
                .ToArray();

            return zip;
        }
    }

}
