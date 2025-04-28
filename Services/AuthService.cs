using cars_web.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace cars_web.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseApiUrl;
        private readonly ILogger<AuthService> _logger;

        public AuthService(HttpClient httpClient, IConfiguration configuration, ILogger<AuthService> logger)
        {
            _httpClient = httpClient;
            _baseApiUrl = configuration["ApiSettings:BaseUrl"] ?? "http://localhost:3002";
            _logger = logger;
        }

        public async Task<(bool Success, int? UserId)> RegisterAsync(UserRegisterModel model)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(model);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync($"{_baseApiUrl}/user/register", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var registerResponse = await response.Content.ReadFromJsonAsync<RegisterResponse>();
                    
                    if (registerResponse != null)
                    {
                        return (true, registerResponse.Id);
                    }
                    
                    return (true, null);
                }
                
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Error registering user: {ErrorContent}", errorContent);
                return (false, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during user registration");
                return (false, null);
            }
        }

        public async Task<UserProfileViewModel> GetUserProfile(string token)
        {
            try {
                string url = $"{_baseApiUrl}/users/profile";
                
                // Устанавливаем токен авторизации в заголовки запроса
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                
                var response = await _httpClient.GetFromJsonAsync<UserProfileViewModel>(url);
                
                // Проверяем полученные данные
                if (response == null)
                {
                    // Если данные некорректны, создаем базовую модель
                    return new UserProfileViewModel
                    {
                        Email = response?.Email ?? "user@example.com",
                        FirstName = response?.FirstName ?? "user@example.com",
                        LastName = response?.LastName ?? "user@example.com",
                    };
                }
                
                return response;
            } 
            catch (Exception e) {
                Console.WriteLine($"Error fetching profile: {e.Message}");
                // Возвращаем заглушку в случае ошибки
                return new UserProfileViewModel
                {
                    Email = "user@example.com",
                    FirstName = "user@example.com",
                    LastName = "user@example.com",
                };
            }
        }

        public async Task<TokenResponse> LoginAsync(UserLoginModel model)
        {
            try
            {
                // Согласно API, для логина нужен FormUrlEncodedContent
                var formContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("email", model.Email),
                    new KeyValuePair<string, string>("password", model.Password)
                });

                _logger.LogInformation($"email: {model.Email}, password: {model.Password}");
                
                var response = await _httpClient.PostAsync($"{_baseApiUrl}/user/login", formContent);
                var responseString = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"response: {responseString}");
                if (response.IsSuccessStatusCode)
                {
                    var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
                    return tokenResponse;
                }
                
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Error logging in: {ErrorContent}", errorContent);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during login");
                return null;
            }
        }

        public async Task<bool> SendEmailVerificationAsync(int userId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"{_baseApiUrl}/email/send-verification/{userId}", null);
                
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Email verification sent for user ID: {UserId}", userId);
                    return true;
                }
                
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Error sending email verification: {ErrorContent}", errorContent);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during email verification sending for user ID: {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> ConfirmEmailAsync(string token)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseApiUrl}/email/confirm?token={token}");
                
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Email confirmed successfully with token: {Token}", token);
                    return true;
                }
                
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Error confirming email: {ErrorContent}", errorContent);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during email confirmation with token: {Token}", token);
                return false;
            }
        }

        public async Task<EmailVerificationStatus> CheckEmailVerificationStatusAsync(string token)
        {
            try
            {
                // Устанавливаем Bearer токен
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                
                var response = await _httpClient.GetFromJsonAsync<EmailVerificationStatus>($"{_baseApiUrl}/email/verification-status");
                
                if (response != null)
                {
                    _logger.LogInformation("Email verification status checked: Email {Email}, Status: {Status}", 
                        response.Email, response.IsEmailConfirmed ? "Confirmed" : "Not confirmed");
                    return response;
                }
                
                _logger.LogWarning("Empty response received from email verification status check");
                return new EmailVerificationStatus { IsEmailConfirmed = false };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking email verification status");
                return new EmailVerificationStatus { IsEmailConfirmed = false };
            }
            finally
            {
                // Очищаем заголовки после запроса
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }
    }
} 