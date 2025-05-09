using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace cars_web.Models
{
    public class AdminLoginModel
    {
        [Required(ErrorMessage = "Имя пользователя обязательно")]
        [JsonPropertyName("username")]
        public string Username { get; set; }
        
        [Required(ErrorMessage = "Пароль обязателен")]
        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
    
    public class AdminTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
        
        [JsonPropertyName("admin")]
        public AdminResponse Admin { get; set; }
    }

    public class AdminResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("username")]
        public string Username { get; set; }
        
        [JsonPropertyName("fullName")]
        public string FullName { get; set; }
        
        [JsonPropertyName("isSuperAdmin")]
        public bool IsSuperAdmin { get; set; }
        
        [JsonPropertyName("permissions")]
        public List<string> Permissions { get; set; }
    }
} 