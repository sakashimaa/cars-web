using cars_web.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace cars_web.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseApiUrl;

        public ApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseApiUrl = configuration["ApiSettings:BaseUrl"] ?? "http://localhost:3002";
        }

        public async Task<List<Car>> GetCarsAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<Car>>($"{_baseApiUrl}/cars");
                return response ?? new List<Car>();
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error fetching cars: {ex.Message}");
                return new List<Car>();
            }
        }

        public async Task<Car> GetCarByIdAsync(int id)
        {
            try
            {
                string url = $"{_baseApiUrl}/cars/{id}";
                Console.WriteLine($"Fetching car details from: {url}");
                
                var response = await _httpClient.GetFromJsonAsync<Car>(url);
                return response;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error fetching car details: {ex.Message}");
                return null;
            }
        }

        public async Task<List<CarApiResponse>?> GetCarDetails(string? carModel)
        {
            try
            {
                string url = $"https://api.api-ninjas.com/v1/cars?model={carModel}";
                Console.WriteLine($"fetching car details from external api: {url}");

                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("X-Api-Key", "DFvLYvpbIQJyaMKU4re8sw==4FIKvflOBbDsMbhG");

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<List<CarApiResponse>>();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching car details from external api: {ex.Message}");
                return null;
            }
        }

        public async Task<List<Worker>> GetWorkersAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<Worker>>($"{_baseApiUrl}/workers");
                return response ?? new List<Worker>();
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error fetching workers: {ex.Message}");
                return new List<Worker>();
            }
        }

        public async Task<Worker> GetWorkerByIdAsync(int id)
        {
            try
            {
                string url = $"{_baseApiUrl}/workers/{id}";
                Console.WriteLine($"Fetching worker details from: {url}");
                
                var response = await _httpClient.GetFromJsonAsync<Worker>(url);
                return response;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error fetching worker details: {ex.Message}");
                return null;
            }
        }

        public async Task<Task> CreateTask(TaskCreateDto taskCreateDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_baseApiUrl}/tasks", taskCreateDto);
                response.EnsureSuccessStatusCode();

                var createdTask = await response.Content.ReadFromJsonAsync<Task>();
                return createdTask;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error creating task: {ex.Message}");
                return null;
            }
        }

        public async Task<List<Review>> GetWorkerReviewsAsync(int workerId)
        {
            try
            {
                string url = $"{_baseApiUrl}/workers/{workerId}/reviews";
                Console.WriteLine($"Fetching worker reviews from: {url}");
                
                var response = await _httpClient.GetFromJsonAsync<List<Review>>(url);
                return response ?? new List<Review>();
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error fetching worker reviews: {ex.Message}");
                return new List<Review>();
            }
        }

        public async Task<List<Detail>> GetDetailsAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<Detail>>($"{_baseApiUrl}/details");
                return response ?? new List<Detail>();
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error fetching details: {ex.Message}");
                return new List<Detail>();
            }
        }

        public async Task<Detail> GetDetailByIdAsync(int id)
        {
            try
            {
                string url = $"{_baseApiUrl}/details/{id}";
                Console.WriteLine($"Fetching detail with ID {id} from: {url}");
                
                var response = await _httpClient.GetFromJsonAsync<Detail>(url);
                return response;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error fetching detail with ID {id}: {ex.Message}");
                return null;
            }
        }

        public async Task<List<Service>> GetServicesAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<Service>>($"{_baseApiUrl}/services");
                return response ?? new List<Service>();
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error fetching services: {ex.Message}");
                return new List<Service>();
            }
        }

        public async Task<Service> GetServiceByIdAsync(int id)
        {
            try
            {
                string url = $"{_baseApiUrl}/services/{id}";
                Console.WriteLine($"Fetching service with ID {id} from: {url}");
                
                var response = await _httpClient.GetFromJsonAsync<Service>(url);
                return response;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error fetching service with ID {id}: {ex.Message}");
                return null;
            }
        }
    }
} 