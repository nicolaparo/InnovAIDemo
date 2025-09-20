using System.Text.Json.Serialization;

namespace InnovAIDemo.Services.Weather
{
    public class OpenMeteoResponseHourlyUnits
    {
        [JsonPropertyName("time")]
        public string Time { get; set; }
        [JsonPropertyName("temperature_2m")]
        public string Temperature2m { get; set; }
        [JsonPropertyName("weather_code")]
        public string WeatherCode { get; set; }
    }

}
