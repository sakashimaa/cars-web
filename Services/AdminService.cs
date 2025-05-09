using cars_web.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace cars_web.Services
{
    public class AdminService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseApiUrl;
        private readonly ILogger<AdminService> _logger;

        public AdminService(HttpClient httpClient, IConfiguration configuration, ILogger<AdminService> logger)
        {
            _httpClient = httpClient;
            _baseApiUrl = configuration["ApiSettings:BaseUrl"] ?? "http://localhost:3002";
            _logger = logger;
        }

        // Метод для установки авторизационного заголовка
        private void SetAuthHeader(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        // Методы для работы с пользователями
        public async Task<List<UserViewModel>> GetUsersAsync(string adminToken)
        {
            try
            {
                SetAuthHeader(adminToken);
                var users = await _httpClient.GetFromJsonAsync<List<UserViewModel>>($"{_baseApiUrl}/users");
                return users ?? new List<UserViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка пользователей");
                return new List<UserViewModel>();
            }
            finally
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        public async Task<UserViewModel> GetUserByIdAsync(int id, string adminToken)
        {
            try
            {
                SetAuthHeader(adminToken);
                return await _httpClient.GetFromJsonAsync<UserViewModel>($"{_baseApiUrl}/users/{id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении пользователя по ID: {Id}", id);
                return null;
            }
            finally
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        public async Task<bool> UpdateUserAsync(int id, UserUpdateModel model, string adminToken)
        {
            try
            {
                SetAuthHeader(adminToken);
                var jsonContent = JsonSerializer.Serialize(model);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PutAsync($"{_baseApiUrl}/users/{id}", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении пользователя: {Id}", id);
                return false;
            }
            finally
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        public async Task<bool> DeleteUserAsync(int id, string adminToken)
        {
            try
            {
                SetAuthHeader(adminToken);
                var response = await _httpClient.DeleteAsync($"{_baseApiUrl}/users/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении пользователя: {Id}", id);
                return false;
            }
            finally
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        // Методы для работы с автомобилями
        public async Task<List<CarViewModel>> GetCarsAsync(string adminToken)
        {
            try
            {
                SetAuthHeader(adminToken);
                var cars = await _httpClient.GetFromJsonAsync<List<CarViewModel>>($"{_baseApiUrl}/cars");
                return cars ?? new List<CarViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка автомобилей");
                return new List<CarViewModel>();
            }
            finally
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        public async Task<CarViewModel> GetCarByIdAsync(int id, string adminToken)
        {
            try
            {
                SetAuthHeader(adminToken);
                return await _httpClient.GetFromJsonAsync<CarViewModel>($"{_baseApiUrl}/cars/{id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении автомобиля по ID: {Id}", id);
                return null;
            }
            finally
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        public async Task<CarViewModel> CreateCarAsync(CarCreateModel model, string adminToken)
        {
            try
            {
                if (string.IsNullOrEmpty(adminToken))
                {
                    _logger.LogError("Токен админа пуст или равен null при создании автомобиля");
                    return null;
                }
                
                _logger.LogInformation("Начало создания автомобиля. Название: {Name}, Путь к изображению: {ImagePath}", 
                    model.Name, model.ImagePath);
                
                _logger.LogInformation("Токен админа: {Token}", adminToken);

                SetAuthHeader(adminToken);

                var jsonContent = JsonSerializer.Serialize(model);
                _logger.LogInformation("Отправляемые данные: {JsonContent}", jsonContent);
                
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                
                _logger.LogInformation("Отправка запроса на {Url} с токеном авторизации", $"{_baseApiUrl}/cars");
                var response = await _httpClient.PostAsync($"{_baseApiUrl}/cars", content);
                
                _logger.LogInformation("Получен ответ от API: {StatusCode}", response.StatusCode);
                
                if (response.IsSuccessStatusCode)
                {
                    var carViewModel = await response.Content.ReadFromJsonAsync<CarViewModel>();
                    _logger.LogInformation("Автомобиль успешно создан с ID: {Id}", carViewModel?.Id);
                    return carViewModel;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("API вернул ошибку: {StatusCode}, {ErrorContent}", 
                        response.StatusCode, errorContent);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании автомобиля");
                return null;
            }
            finally
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        public async Task<bool> UpdateCarAsync(int id, CarUpdateModel model, string adminToken)
        {
            try
            {
                SetAuthHeader(adminToken);
                var jsonContent = JsonSerializer.Serialize(model);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PutAsync($"{_baseApiUrl}/cars/{id}", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении автомобиля: {Id}", id);
                return false;
            }
            finally
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        public async Task<bool> DeleteCarAsync(int id, string adminToken)
        {
            try
            {
                SetAuthHeader(adminToken);
                var response = await _httpClient.DeleteAsync($"{_baseApiUrl}/cars/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении автомобиля: {Id}", id);
                return false;
            }
            finally
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        // Методы для работы с работниками
        public async Task<List<WorkerViewModel>> GetWorkersAsync(string adminToken)
        {
            try
            {
                SetAuthHeader(adminToken);
                var workers = await _httpClient.GetFromJsonAsync<List<WorkerViewModel>>($"{_baseApiUrl}/workers");
                return workers ?? new List<WorkerViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка работников");
                return new List<WorkerViewModel>();
            }
            finally
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        public async Task<WorkerViewModel> GetWorkerByIdAsync(int id, string adminToken)
        {
            try
            {
                SetAuthHeader(adminToken);
                return await _httpClient.GetFromJsonAsync<WorkerViewModel>($"{_baseApiUrl}/workers/{id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении работника по ID: {Id}", id);
                return null;
            }
            finally
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        public async Task<WorkerViewModel> CreateWorkerAsync(WorkerCreateModel model, string adminToken)
        {
            try
            {
                SetAuthHeader(adminToken);
                var jsonContent = JsonSerializer.Serialize(model);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync($"{_baseApiUrl}/workers", content);
                
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<WorkerViewModel>();
                }
                
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании работника");
                return null;
            }
            finally
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        public async Task<bool> UpdateWorkerAsync(int id, WorkerUpdateModel model, string adminToken)
        {
            try
            {
                SetAuthHeader(adminToken);
                var jsonContent = JsonSerializer.Serialize(model);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PutAsync($"{_baseApiUrl}/workers/{id}", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении работника: {Id}", id);
                return false;
            }
            finally
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        public async Task<bool> DeleteWorkerAsync(int id, string adminToken)
        {
            try
            {
                SetAuthHeader(adminToken);
                var response = await _httpClient.DeleteAsync($"{_baseApiUrl}/workers/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении работника: {Id}", id);
                return false;
            }
            finally
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        // Методы для работы с услугами
        public async Task<List<ServiceViewModel>> GetServicesAsync(string adminToken)
        {
            try
            {
                SetAuthHeader(adminToken);
                var services = await _httpClient.GetFromJsonAsync<List<ServiceViewModel>>($"{_baseApiUrl}/services");
                return services ?? new List<ServiceViewModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка услуг");
                return new List<ServiceViewModel>();
            }
            finally
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        public async Task<ServiceViewModel> GetServiceByIdAsync(int id, string adminToken)
        {
            try
            {
                SetAuthHeader(adminToken);
                return await _httpClient.GetFromJsonAsync<ServiceViewModel>($"{_baseApiUrl}/services/{id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении услуги по ID: {Id}", id);
                return null;
            }
            finally
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        public async Task<ServiceViewModel> CreateServiceAsync(ServiceCreateModel model, string adminToken)
        {
            try
            {
                SetAuthHeader(adminToken);
                var jsonContent = JsonSerializer.Serialize(model);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync($"{_baseApiUrl}/services", content);
                
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ServiceViewModel>();
                }
                
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании услуги");
                return null;
            }
            finally
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        public async Task<bool> UpdateServiceAsync(int id, ServiceUpdateModel model, string adminToken)
        {
            try
            {
                SetAuthHeader(adminToken);
                var jsonContent = JsonSerializer.Serialize(model);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PutAsync($"{_baseApiUrl}/services/{id}", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении услуги: {Id}", id);
                return false;
            }
            finally
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        public async Task<bool> DeleteServiceAsync(int id, string adminToken)
        {
            try
            {
                SetAuthHeader(adminToken);
                var response = await _httpClient.DeleteAsync($"{_baseApiUrl}/services/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении услуги: {Id}", id);
                return false;
            }
            finally
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }
    }
} 