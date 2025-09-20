namespace InnovAIDemo.Services.Weather
{
    public class WeatherCode
    {
        private WeatherCode() { }

        public required int Code { get; init; }
        public required string Description { get; init; }
        public required string Emoji { get; init; }

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
        public static WeatherCode? GetWeatherCode(int code) => GetWeatherCodes().FirstOrDefault(wc => wc.Code == code);
    }

}
