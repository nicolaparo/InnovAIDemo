namespace InnovAIDemo.Services.Weather
{
    /// <summary>
    /// Represents a single weather data point with time, temperature, and conditions.
    /// </summary>
    public class WeatherServiceResponseItem
    {
        /// <summary>
        /// Gets or sets the time for this weather data point
        /// </summary>
        public DateTime Time { get; set; }
        
        /// <summary>
        /// Gets or sets the temperature in Celsius
        /// </summary>
        public float TemperatureCelsius { get; set; }
        
        /// <summary>
        /// Gets or sets the weather description
        /// </summary>
        public string? Weather { get; set; }
        
        /// <summary>
        /// Gets or sets the weather icon emoji
        /// </summary>
        public string? Icon { get; set; }
    }

}
