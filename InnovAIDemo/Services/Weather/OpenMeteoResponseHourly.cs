using System.Text.Json.Serialization;

namespace InnovAIDemo.Services.Weather
{
    public class OpenMeteoResponseHourly
    {
        [JsonPropertyName("time")]
        public DateTime[] Time { get; set; }
        [JsonPropertyName("temperature_2m")]
        public float[] Temperature2m { get; set; }
        [JsonPropertyName("weather_code")]
        public int[] WeatherCode { get; set; }
    }

}
