using System.Text.Json.Serialization;

namespace cars_web.Models
{
    public class Car
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        
        [JsonPropertyName("shortDescription")]
        public string? ShortDescription { get; set; }
        
        [JsonPropertyName("fullDescription")]
        public string? FullDescription { get; set; }
        
        [JsonPropertyName("price")]
        public double? Price { get; set; }
        
        [JsonPropertyName("quantity")]
        public int? Quantity { get; set; }
        
        [JsonPropertyName("imagePath")]
        public string? ImagePath { get; set; }
    }
} 