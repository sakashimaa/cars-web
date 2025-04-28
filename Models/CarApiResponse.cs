using System.Text.Json.Serialization;

namespace cars_web.Models
{
    public class CarApiResponse
    {
        [JsonPropertyName("city_mpg")]
        public string? CityMpg { get; set; }

        [JsonPropertyName("class")]
        public string? Class { get; set; }

        [JsonPropertyName("combination_mpg")]
        public string? CombinationMpg { get; set; }

        [JsonPropertyName("cylinders")]
        public int Cylinders { get; set; }

        [JsonPropertyName("displacement")]
        public double Displacement { get; set; }

        [JsonPropertyName("drive")]
        public string? Drive { get; set; }

        [JsonPropertyName("fuel_type")]
        public string? FuelType { get; set; }

        [JsonPropertyName("highway_mpg")]
        public string? HighwayMpg { get; set; }

        [JsonPropertyName("make")]
        public string? Make { get; set; }
        [JsonPropertyName("model")]
        public string? Model { get; set; }

        [JsonPropertyName("transmission")]
        public string? Transmission { get; set; }

        [JsonPropertyName("year")]
        public int Year { get; set; }
    }
}