using Microsoft.AspNetCore.Mvc;
using cars_web.Models;
using cars_web.Services;
using System.Diagnostics;
using System.Threading.Tasks;
using cars_web.Filters;

namespace cars_web.Controllers
{
    [RequireEmailVerification]
    public class DetailsController : Controller
    {
        private readonly ApiService _apiService;
        private readonly ILogger<DetailsController> _logger;

        public DetailsController(ApiService apiService, ILogger<DetailsController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Запрос списка деталей");
            
            try
            {
                var details = await _apiService.GetDetailsAsync();
                return View(details);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка деталей");
                return StatusCode(500, "Internal server error");
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            _logger.LogInformation("Запрос детали с ID: {id}", id);
            
            try
            {
                var detail = await _apiService.GetDetailByIdAsync(id);
                
                if (detail == null)
                {
                    _logger.LogWarning("Деталь с ID {id} не найдена", id);
                    return NotFound();
                }
                
                _logger.LogInformation("Деталь найдена: {name}", detail.Name);
                return View(detail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении детали с ID {id}", id);
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