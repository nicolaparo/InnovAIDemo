using System.Text.Json.Serialization;

namespace InnovAIDemo.Services.Nominatim
{
    /// <summary>
    /// Represents a search result from the Nominatim geocoding API.
    /// Contains location information including coordinates and address details.
    /// </summary>
    public class NominatimSearchResponse
    {
        /// <summary>
        /// Gets or sets the unique place identifier
        /// </summary>
        [JsonPropertyName("place_id")]
        public int PlaceId { get; set; }
        
        /// <summary>
        /// Gets or sets the license information for the data
        /// </summary>
        [JsonPropertyName("licence")]
        public string? Licence { get; set; }
        
        /// <summary>
        /// Gets or sets the OpenStreetMap object type
        /// </summary>
        [JsonPropertyName("osm_type")]
        public string? OsmType { get; set; }
        
        /// <summary>
        /// Gets or sets the OpenStreetMap object ID
        /// </summary>
        [JsonPropertyName("osm_id")]
        public int OsmId { get; set; }
        
        /// <summary>
        /// Gets or sets the latitude coordinate
        /// </summary>
        [JsonPropertyName("lat")]
        public string? Latitude { get; set; }
        
        /// <summary>
        /// Gets or sets the longitude coordinate
        /// </summary>
        [JsonPropertyName("lon")]
        public string? Longitude { get; set; }
        
        /// <summary>
        /// Gets or sets the classification of the place
        /// </summary>
        [JsonPropertyName("class")]
        public string? Class { get; set; }
        
        /// <summary>
        /// Gets or sets the type of the place
        /// </summary>
        [JsonPropertyName("type")]
        public string? Type { get; set; }
        
        /// <summary>
        /// Gets or sets the place rank for importance ordering
        /// </summary>
        [JsonPropertyName("place_rank")]
        public int PlaceRank { get; set; }
        
        /// <summary>
        /// Gets or sets the importance score of the place
        /// </summary>
        [JsonPropertyName("importance")]
        public float Importance { get; set; }
        
        /// <summary>
        /// Gets or sets the address type
        /// </summary>
        [JsonPropertyName("addresstype")]
        public string? Addresstype { get; set; }
        
        /// <summary>
        /// Gets or sets the name of the place
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        
        /// <summary>
        /// Gets or sets the full display name including address
        /// </summary>
        [JsonPropertyName("display_name")]
        public string? DisplayName { get; set; }
        
        /// <summary>
        /// Gets or sets the bounding box coordinates for the place
        /// </summary>
        [JsonPropertyName("boundingbox")]
        public string[]? Boundingbox { get; set; }
    }

}
