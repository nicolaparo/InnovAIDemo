using System.ComponentModel;
using InnovAIDemo.Ollama;
using InnovAIDemo.Services;
using InnovAIDemo.Services.Led;
using InnovAIDemo.Services.Weather;

namespace InnovAIDemo
{
    public class AgentToolsService(WeatherService weatherService
        , WikipediaSearchService wikipediaSearch
        , LedService ledService
        , ILogger<AgentToolsService> logger)
    {
        [AgentTool(Description = "Gets the current weather given a location")]
        public async Task<object> GetWeather([Description("location")] string location)
        {
            logger.LogInformation($"GetWeather called with location: {location}");

            var result = await weatherService.GetWeatherAtLocationAsync(location);

            var now = DateTime.Now;

            var entries = result
                .Where(r => r.Time >= now)
                .OrderBy(r => r.Time)
                .Select(x => new
                {
                    x.Time,
                    tempCelsius = x.TemperatureCelsius,
                    Weather = $"{x.Icon} {x.Weather}"
                })
                .FirstOrDefault();

            return entries;
        }


        [AgentTool(Description = "Gets the weather forecast for a day given a location")]
        public async Task<object> GetWeatherForecast([Description("location")] string location)
        {
            logger.LogInformation($"GetWeatherForecast called with location: {location}");

            var result = await weatherService.GetWeatherAtLocationAsync(location);

            var now = DateTime.Now;

            var entries = result
                .Where(r => r.Time >= now)
                .Where(r => r.Time <= now.AddHours(24))
                .OrderBy(r => r.Time)
                .Index()
                .Where(x => x.Index % 3 == 0) // every 3rd hour
                .Select(x => x.Item)
                .ToArray();

            return entries.Select(x => new
            {
                x.Time,
                tempCelsius = x.TemperatureCelsius,
                Weather = $"{x.Icon} {x.Weather}"
            });
        }

        [AgentTool(Description = "searches on wikipedia for information regarding a specific topic")]
        public async Task<object> SearchWikipedia([Description("single term to search for")] string searchTerm)
        {
            logger.LogInformation($"SearchWikipedia called with searchTerm: {searchTerm}");

            return await wikipediaSearch.SearchAsync(searchTerm);
        }

        [AgentTool(Description = "Turns the light on")]
        public object TurnOn()
        {
            logger.LogInformation("TurnOnLight called");
            ledService.TurnOn();
            return new { Status = "Light turned on" };
        }

        [AgentTool(Description = "Turns the light off")]
        public object TurnOff()
        {
            logger.LogInformation("TurnOffLight called");
            ledService.TurnOff();
            return new { Status = "Light turned off" };
        }

        [AgentTool(Description = "Checks if the light is on or off")]
        public object IsLightOn()
        {
            logger.LogInformation("IsLightOn called");
            var isOn = ledService.IsLedOn;
            return new { IsOn = isOn };
        }
    }
}
