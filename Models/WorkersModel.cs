using System.Text.Json.Serialization;

namespace cars_web.Models
{
    public class Worker 
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        
        [JsonPropertyName("shortDescription")]
        public string? ShortDescription { get; set; }
        
        [JsonPropertyName("fullDescription")]
        public string? FullDescription { get; set; }
        
        [JsonPropertyName("imagePath")]
        public string? ImagePath { get; set; }
        
        [JsonPropertyName("workTime")]
        public int? WorkTime { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime? CreatedAt { get; set; }

        [JsonPropertyName("updatedAt")]
        public DateTime? UpdatedAt { get; set; }
    }
} 