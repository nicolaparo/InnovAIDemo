namespace InnovAIDemo.Services.Weather
{
    public class WeatherServiceResponseItem
    {
        public DateTime Time { get; set; }
        public float TemperatureCelsius { get; set; }
        public string Weather { get; set; }
        public string Icon { get; set; }
    }

}
