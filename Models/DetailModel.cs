using System.Text.Json.Serialization;

namespace cars_web.Models
{
    public class Detail
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        
        [JsonPropertyName("article")]
        public string? Article { get; set; }
        
        [JsonPropertyName("creatorCode")]
        public string? CreatorCode { get; set; }
        
        [JsonPropertyName("creator")]
        public string? Creator { get; set; }
        
        [JsonPropertyName("detailCategory")]
        public string? DetailCategory { get; set; }
        
        [JsonPropertyName("imagePath")]
        public string? ImagePath { get; set; }
        
        [JsonPropertyName("price")]
        public decimal Price { get; set; }
        
        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
    }
} 