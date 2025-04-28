using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace cars_web.Models
{
    public class UserRegisterModel
    {

        [Required(ErrorMessage = "Фамилия обязательна")]
        [JsonPropertyName("firstName")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Имя обязательно")]
        [JsonPropertyName("lastName")]
        public string? LastName { get; set; }
        
        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный email")]
        [JsonPropertyName("email")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Пароль обязателен")]
        [MinLength(6, ErrorMessage = "Пароль должен содержать минимум 6 символов")]
        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
    
    public class UserLoginModel
    {
        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный email")]
        [JsonPropertyName("email")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Пароль обязателен")]
        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
    
    public class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
        
        [JsonPropertyName("user")]
        public UserLoginResponse? User { get; set; }
    }

    public class UserLoginResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("email")]
        public string Email { get; set; }
        
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string LastName { get; set; }
        
        [JsonPropertyName("isEmailConfirmed")]
        public bool IsEmailConfirmed { get; set; }
    }
    
    
    public class UserInfoModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("email")]
        public string Email { get; set; }
    }

    public class RegisterResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("email")]
        public string Email { get; set; }
    }

    public class UnverifiedEmailViewModel
    {
        public int UserId { get; set; }
        public string Email { get; set; }
    }

    public class EmailVerificationStatus
    {
        [JsonPropertyName("isEmailConfirmed")]
        public bool IsEmailConfirmed { get; set; }
        
        [JsonPropertyName("email")]
        public string Email { get; set; }
    }
} 