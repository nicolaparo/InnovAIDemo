using System.ComponentModel;
using InnovAIDemo.Ollama;
using InnovAIDemo.Services;
using InnovAIDemo.Services.Led;
using InnovAIDemo.Services.Weather;

namespace InnovAIDemo
{
    /// <summary>
    /// Service that provides tools for AI agents to interact with various services.
    /// These tools can be called by the AI agent to perform actions like getting weather,
    /// searching Wikipedia, or controlling LED lights.
    /// </summary>
    /// <param name="weatherService">Service for weather information</param>
    /// <param name="wikipediaSearch">Service for Wikipedia searches</param>
    /// <param name="ledService">Service for LED control</param>
    /// <param name="logger">Logger for this service</param>
    public class AgentToolsService(WeatherService weatherService
        , WikipediaSearchService wikipediaSearch
        , ILedService ledService
        , ILogger<AgentToolsService> logger)
    {
        /// <summary>
        /// Gets the current weather for a specified location.
        /// </summary>
        /// <param name="location">The location to get weather for</param>
        /// <returns>Current weather information including temperature and conditions</returns>
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


        /// <summary>
        /// Gets the weather forecast for the next 24 hours for a specified location.
        /// Returns weather data at 3-hour intervals.
        /// </summary>
        /// <param name="location">The location to get weather forecast for</param>
        /// <returns>Weather forecast data for the next 24 hours</returns>
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

        /// <summary>
        /// Searches Wikipedia for information about a specific topic.
        /// </summary>
        /// <param name="searchTerm">The term to search for on Wikipedia</param>
        /// <returns>Wikipedia search results and content</returns>
        [AgentTool(Description = "searches on wikipedia for information regarding a specific topic")]
        public async Task<object> SearchWikipedia([Description("single term to search for")] string searchTerm)
        {
            logger.LogInformation($"SearchWikipedia called with searchTerm: {searchTerm}");

            return await wikipediaSearch.SearchAsync(searchTerm);
        }

        /// <summary>
        /// Turns on the LED light.
        /// </summary>
        /// <returns>Status indicating the light was turned on</returns>
        [AgentTool(Description = "Turns the light on")]
        public object TurnOn()
        {
            logger.LogInformation("TurnOnLight called");
            ledService.TurnOn();
            return new { Status = "Light turned on" };
        }

        /// <summary>
        /// Turns off the LED light.
        /// </summary>
        /// <returns>Status indicating the light was turned off</returns>
        [AgentTool(Description = "Turns the light off")]
        public object TurnOff()
        {
            logger.LogInformation("TurnOffLight called");
            ledService.TurnOff();
            return new { Status = "Light turned off" };
        }

        /// <summary>
        /// Checks the current state of the LED light.
        /// </summary>
        /// <returns>Status indicating whether the light is on or off</returns>
        [AgentTool(Description = "Checks if the light is on or off")]
        public object IsLightOn()
        {
            logger.LogInformation("IsLightOn called");
            var isOn = ledService.IsLedOn;
            return new { IsOn = isOn };
        }
    }
}
