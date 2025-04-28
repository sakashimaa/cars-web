using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using cars_web.Models;

namespace cars_web.Services
{
    public class PaintingServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "http://localhost:3002";

        public PaintingServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<PaintingService>> GetAllServices()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<PaintingService>>($"{_baseUrl}/painting-services");
                return response ?? new List<PaintingService>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception("Failed to fetch painting services", ex);
            }
        }

        public async Task<PaintingService> GetServiceById(int id)
        {
            try 
            {
                var response = await _httpClient.GetFromJsonAsync<PaintingService>($"{_baseUrl}/painting-services/{id}");
                return response ?? throw new Exception("Painting service not found");
            }
            catch (Exception ex) 
            {
                throw new Exception("Failed to fetch painting service by id", ex);
            }
        }
    }
} 