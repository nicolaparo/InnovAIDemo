using System.Text.Json.Serialization;

namespace InnovAIDemo.Services.Nominatim
{
    public class NominatimSearchResponse
    {
        [JsonPropertyName("place_id")]
        public int PlaceId { get; set; }
        [JsonPropertyName("licence")]
        public string Licence { get; set; }
        [JsonPropertyName("osm_type")]
        public string OsmType { get; set; }
        [JsonPropertyName("osm_id")]
        public int OsmId { get; set; }
        [JsonPropertyName("lat")]
        public string Latitude { get; set; }
        [JsonPropertyName("lon")]
        public string Longitude { get; set; }
        [JsonPropertyName("class")]
        public string Class { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("place_rank")]
        public int PlaceRank { get; set; }
        [JsonPropertyName("importance")]
        public float Importance { get; set; }
        [JsonPropertyName("addresstype")]
        public string Addresstype { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; }
        [JsonPropertyName("boundingbox")]
        public string[] Boundingbox { get; set; }
    }

}
