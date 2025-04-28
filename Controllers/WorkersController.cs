using Microsoft.AspNetCore.Mvc;
using cars_web.Models;
using cars_web.Services;
using System.Diagnostics;
using System.Threading.Tasks;
using cars_web.Filters;

namespace cars_web.Controllers
{
    [RequireEmailVerification]
    public class WorkersController : Controller
    {
        private readonly ApiService _apiService;
        private readonly ILogger<CarsController> _logger;

        public WorkersController(ApiService apiService, ILogger<CarsController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var workers = await _apiService.GetWorkersAsync();
            foreach (var worker in workers) 
            {
                Console.WriteLine($"image = {worker.ImagePath}");
            }
            return View(workers);
        }

        public async Task<IActionResult> Details(int id)
        {
            _logger.LogInformation("Запрос деталей работника с ID: {id}", id);
             
            try
            {
                var worker = await _apiService.GetWorkerByIdAsync(id);
                 
                if (worker == null)
                {
                    _logger.LogWarning("Работник с ID {id} не найден", id);
                    return NotFound();
                }
                
                // Fetch worker reviews
                var reviews = await _apiService.GetWorkerReviewsAsync(id);
                ViewBag.Reviews = reviews;
                 
                _logger.LogInformation("Работник найден: {name}", worker.Name);
                Console.WriteLine($"Calling view");
                ViewData["Token"] = HttpContext.Session.GetString("JWTToken");
                return View(worker);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении деталей работника с ID: {id}", id);
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