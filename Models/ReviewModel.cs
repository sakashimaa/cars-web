using System.Text.Json.Serialization;

namespace cars_web.Models
{
    public class Review
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("shortDescription")]
        public string? ShortDescription { get; set; }
        
        [JsonPropertyName("fullDescription")]
        public string? FullDescription { get; set; }
        
        [JsonPropertyName("rating")]
        public int Rating { get; set; }
    }
} 