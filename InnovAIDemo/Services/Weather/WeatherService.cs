using InnovAIDemo.Services.Nominatim;

namespace InnovAIDemo.Services.Weather
{
    public class WeatherService(HttpClient httpClient, NominatimService nomatimService, ILogger<WeatherService> logger)
    {
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
