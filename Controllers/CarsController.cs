using Microsoft.AspNetCore.Mvc;
using cars_web.Models;
using cars_web.Services;
using System.Diagnostics;
using System.Threading.Tasks;
using cars_web.Filters;

namespace cars_web.Controllers
{
    [RequireEmailVerification]
    public class CarsController : Controller
    {
        private readonly ApiService _apiService;
        private readonly ILogger<CarsController> _logger;

        public CarsController(ApiService apiService, ILogger<CarsController> logger)
        {
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var cars = await _apiService.GetCarsAsync();
            foreach (var car in cars) 
            {
                Console.WriteLine($"image = {car.ImagePath}");
            }
            return View(cars);
        }

        public async Task<IActionResult> Details(int id)
        {
            _logger.LogInformation("Запрос деталей автомобиля с ID: {id}", id);
            
            try
            {
                var car = await _apiService.GetCarByIdAsync(id);
                
                if (car == null)
                {
                    _logger.LogWarning("Автомобиль с ID {id} не найден", id);
                    return NotFound();
                }
                
                _logger.LogInformation("Автомобиль найден: {name}", car.Name);
                var parts = car?.Name?.Split(' ');
    
                if (parts != null)
                {
                    var str = string.Join(" ", parts.Skip(1));
                    Console.WriteLine($"STRING: {str}");
                    var model = str ?? "camry";
                    var result = await _apiService.GetCarDetails(model);
                    
                    if (result != null && result.Count > 0)
                    {
                        var carDetails = result[0]; // Берем первый автомобиль из результатов
                        ViewBag.Year = carDetails.Year;
                        ViewBag.Cylinders = carDetails.Cylinders;
                        ViewBag.Transmission = carDetails.Transmission;
                        ViewBag.FuelType = carDetails.FuelType;
                        ViewBag.Make = carDetails.Make;
                        ViewBag.Class = carDetails.Class;
                        // Другие свойства, если нужно
                    }
                    else
                    {
                        // Значения по умолчанию, если данные не найдены
                        ViewBag.Year = "N/A";
                        ViewBag.Cylinders = "N/A";
                        ViewBag.Transmission = "N/A";
                    }
                }

                return View(car);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении деталей автомобиля с ID {id}", id);
                return View("Error");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
} 