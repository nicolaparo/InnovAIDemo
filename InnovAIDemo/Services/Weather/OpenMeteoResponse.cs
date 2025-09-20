using System.Text.Json.Serialization;

namespace InnovAIDemo.Services.Weather
{
    public class OpenMeteoResponse
    {
        [JsonPropertyName("latitude")]
        public float Latitude { get; set; }
        [JsonPropertyName("longitude")]
        public float Longitude { get; set; }
        [JsonPropertyName("generationtime_ms")]
        public float GenerationtimeMs { get; set; }
        [JsonPropertyName("utc_offset_seconds")]
        public int UtcOffsetSeconds { get; set; }
        [JsonPropertyName("timezone")]
        public string Timezone { get; set; }
        [JsonPropertyName("timezone_abbreviation")]
        public string TimezoneAbbreviation { get; set; }
        [JsonPropertyName("elevation")]
        public float Elevation { get; set; }
        [JsonPropertyName("hourly_units")]
        public OpenMeteoResponseHourlyUnits HourlyUnits { get; set; }
        [JsonPropertyName("hourly")]
        public OpenMeteoResponseHourly Hourly { get; set; }
    }

}
