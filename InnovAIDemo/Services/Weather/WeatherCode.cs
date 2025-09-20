namespace InnovAIDemo.Services.Weather
{
    /// <summary>
    /// Represents weather condition codes from Open-Meteo API with descriptions and emoji icons.
    /// Provides mapping between numeric weather codes and human-readable descriptions.
    /// </summary>
    public class WeatherCode
    {
        /// <summary>
        /// Private constructor to prevent external instantiation
        /// </summary>
        private WeatherCode() { }

        /// <summary>
        /// Gets the numeric weather code
        /// </summary>
        public required int Code { get; init; }
        
        /// <summary>
        /// Gets the human-readable description of the weather condition
        /// </summary>
        public required string Description { get; init; }
        
        /// <summary>
        /// Gets the emoji icon representing the weather condition
        /// </summary>
        public required string Emoji { get; init; }

        /// <summary>
        /// Gets all available weather codes with their descriptions and emojis.
        /// </summary>
        /// <returns>List of all weather codes</returns>
        public static List<WeatherCode> GetWeatherCodes() => new List<WeatherCode>
        {
            new() { Code = 0, Description = "Clear sky", Emoji = "☀️" },
            new() { Code = 1, Description = "Mainly clear", Emoji = "🌤️" },
            new() { Code = 2, Description = "Partly cloudy", Emoji = "⛅" },
            new() { Code = 3, Description = "Overcast", Emoji = "☁️" },
            new() { Code = 45, Description = "Fog", Emoji = "🌫️" },
            new() { Code = 48, Description = "Depositing rime fog", Emoji = "🌁" },
            new() { Code = 51, Description = "Light drizzle", Emoji = "🌦️" },
            new() { Code = 53, Description = "Moderate drizzle", Emoji = "🌦️" },
            new() { Code = 55, Description = "Dense drizzle", Emoji = "🌧️" },
            new() { Code = 56, Description = "Light freezing drizzle", Emoji = "🧊🌦️" },
            new() { Code = 57, Description = "Dense freezing drizzle", Emoji = "🧊🌧️" },
            new() { Code = 61, Description = "Slight rain", Emoji = "🌦️" },
            new() { Code = 63, Description = "Moderate rain", Emoji = "🌧️" },
            new() { Code = 65, Description = "Heavy rain", Emoji = "🌧️💧" },
            new() { Code = 66, Description = "Light freezing rain", Emoji = "🧊🌧️" },
            new() { Code = 67, Description = "Heavy freezing rain", Emoji = "🧊🌧️💧" },
            new() { Code = 71, Description = "Slight snow fall", Emoji = "🌨️" },
            new() { Code = 73, Description = "Moderate snow fall", Emoji = "❄️🌨️" },
            new() { Code = 75, Description = "Heavy snow fall", Emoji = "❄️❄️🌨️" },
            new() { Code = 77, Description = "Snow grains", Emoji = "🌨️🔹" },
            new() { Code = 80, Description = "Slight rain showers", Emoji = "🌦️" },
            new() { Code = 81, Description = "Moderate rain showers", Emoji = "🌧️" },
            new() { Code = 82, Description = "Violent rain showers", Emoji = "🌧️🌩️" },
            new() { Code = 85, Description = "Slight snow showers", Emoji = "🌨️" },
            new() { Code = 86, Description = "Heavy snow showers", Emoji = "🌨️❄️" },
            new() { Code = 95, Description = "Thunderstorm", Emoji = "⛈️" },
            new() { Code = 96, Description = "Thunderstorm with slight hail", Emoji = "⛈️🧊" },
            new() { Code = 99, Description = "Thunderstorm with heavy hail", Emoji = "⛈️🧊🧊" }
        };
        
        /// <summary>
        /// Gets the weather code information for a specific numeric code.
        /// </summary>
        /// <param name="code">The numeric weather code to look up</param>
        /// <returns>The weather code information, or null if not found</returns>
        public static WeatherCode? GetWeatherCode(int code) => GetWeatherCodes().FirstOrDefault(wc => wc.Code == code);
    }

}
