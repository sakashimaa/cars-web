using Microsoft.AspNetCore.Mvc;
using cars_web.Models;
using cars_web.Services;
using System.Diagnostics;
using System.Threading.Tasks;
using cars_web.Filters;

namespace cars_web.Controllers
{
    [RequireEmailVerification]
    public class ServicesController : Controller
    {
        private readonly ApiService _apiService;
        private readonly ILogger<ServicesController> _logger;

        public ServicesController(ApiService apiService, ILogger<ServicesController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Запрос списка услуг");
            
            try
            {
                var services = await _apiService.GetServicesAsync();
                return View(services);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка услуг");
                return StatusCode(500, "Internal server error");
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            _logger.LogInformation("Запрос услуги с ID: {id}", id);
            
            try
            {
                var service = await _apiService.GetServiceByIdAsync(id);
                
                if (service == null)
                {
                    _logger.LogWarning("Услуга с ID {id} не найдена", id);
                    return NotFound();
                }
                
                _logger.LogInformation("Услуга найдена: {name}", service.Name);
                return View(service);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении услуги с ID {id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
} 