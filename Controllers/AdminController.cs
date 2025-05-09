using Microsoft.AspNetCore.Mvc;
using cars_web.Models;
using cars_web.Services;
using cars_web.Extensions;
using Microsoft.AspNetCore.Http;

namespace cars_web.Controllers
{
    [Route("admin")]
    public class AdminController : Controller
    {
        private readonly AuthService _authService;
        private readonly AdminService _adminService;
        private readonly ILogger<AdminController> _logger;
        private readonly FileService _fileService;

        public AdminController(AuthService authService, AdminService adminService, ILogger<AdminController> logger, FileService fileService)
        {
            _authService = authService;
            _adminService = adminService;
            _logger = logger;
            _fileService = fileService;
        }

        #region Аутентификация и управление аккаунтом

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AdminLoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var tokenResponse = await _authService.AdminLoginAsync(model);
            if (tokenResponse != null)
            {
                // Сохраняем токен и данные администратора в сессии
                HttpContext.Session.SetString("AdminJWTToken", tokenResponse.AccessToken);
                HttpContext.Session.SetString("AdminUsername", model.Username);
                HttpContext.Session.SetInt32("AdminId", tokenResponse.Admin.Id);
                HttpContext.Session.SetString("AdminFullName", tokenResponse.Admin.FullName);
                HttpContext.Session.SetBoolean("IsSuperAdmin", tokenResponse.Admin.IsSuperAdmin);
                
                // Сохраняем разрешения администратора
                HttpContext.Session.SetString("AdminPermissions", string.Join(",", tokenResponse.Admin.Permissions));
                
                _logger.LogInformation("Admin logged in: {Username}", model.Username);
                _logger.LogInformation("Admin token: {Token}", tokenResponse.AccessToken);
                
                return RedirectToAction("Dashboard");
            }

            ModelState.AddModelError("", "Неверное имя пользователя или пароль");
            return View(model);
        }

        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            // Проверяем наличие админского токена
            if (HttpContext.Session.GetString("AdminJWTToken") == null)
            {
                return RedirectToAction("Login");
            }
            
            // В будущем здесь будет логика загрузки данных для админской панели
            
            return View();
        }

        [HttpPost("logout")]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            // Очищаем только данные админской сессии
            HttpContext.Session.Remove("AdminJWTToken");
            HttpContext.Session.Remove("AdminUsername");
            HttpContext.Session.Remove("AdminId");
            HttpContext.Session.Remove("AdminFullName");
            HttpContext.Session.Remove("IsSuperAdmin");
            HttpContext.Session.Remove("AdminPermissions");
            
            return RedirectToAction("Login");
        }

        #endregion

        #region Управление пользователями

        [HttpGet("users")]
        public async Task<IActionResult> Users()
        {
            if (HttpContext.Session.GetString("AdminJWTToken") == null)
            {
                return RedirectToAction("Login");
            }

            var adminToken = HttpContext.Session.GetString("AdminJWTToken");
            var users = await _adminService.GetUsersAsync(adminToken);

            return View(users);
        }

        [HttpGet("users/{id}")]
        public async Task<IActionResult> UserDetails(int id)
        {
            if (HttpContext.Session.GetString("AdminJWTToken") == null)
            {
                return RedirectToAction("Login");
            }

            var adminToken = HttpContext.Session.GetString("AdminJWTToken");
            var user = await _adminService.GetUserByIdAsync(id, adminToken);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpGet("users/edit/{id}")]
        public async Task<IActionResult> EditUser(int id)
        {
            if (HttpContext.Session.GetString("AdminJWTToken") == null)
            {
                return RedirectToAction("Login");
            }

            var adminToken = HttpContext.Session.GetString("AdminJWTToken");
            var user = await _adminService.GetUserByIdAsync(id, adminToken);

            if (user == null)
            {
                return NotFound();
            }

            var model = new UserUpdateModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName
            };

            ViewBag.UserId = id;
            return View(model);
        }

        [HttpPost("users/edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(int id, UserUpdateModel model)
        {
            if (HttpContext.Session.GetString("AdminJWTToken") == null)
            {
                return RedirectToAction("Login");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.UserId = id;
                return View(model);
            }

            var adminToken = HttpContext.Session.GetString("AdminJWTToken");
            var result = await _adminService.UpdateUserAsync(id, model, adminToken);

            if (result)
            {
                TempData["SuccessMessage"] = "Пользователь успешно обновлен";
                return RedirectToAction("Users");
            }
            else
            {
                ModelState.AddModelError("", "Ошибка при обновлении пользователя");
                ViewBag.UserId = id;
                return View(model);
            }
        }

        [HttpPost]
        [Route("users/delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (HttpContext.Session.GetString("AdminJWTToken") == null)
            {
                return RedirectToAction("Login");
            }

            var adminToken = HttpContext.Session.GetString("AdminJWTToken");
            var result = await _adminService.DeleteUserAsync(id, adminToken);

            if (result)
            {
                TempData["SuccessMessage"] = "Пользователь успешно удален";
            }
            else
            {
                TempData["ErrorMessage"] = "Ошибка при удалении пользователя";
            }

            return RedirectToAction("Users");
        }

        #endregion

        #region Управление автомобилями

        [HttpGet("cars")]
        public async Task<IActionResult> Cars()
        {
            if (HttpContext.Session.GetString("AdminJWTToken") == null)
            {
                return RedirectToAction("Login");
            }

            var adminToken = HttpContext.Session.GetString("AdminJWTToken");
            var cars = await _adminService.GetCarsAsync(adminToken);

            return View(cars);
        }

        [HttpGet("cars/create")]
        public IActionResult CreateCar()
        {
            if (HttpContext.Session.GetString("AdminJWTToken") == null)
            {
                return RedirectToAction("Login");
            }

            return View(new CarCreateModel());
        }

        [HttpPost("cars/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCar(CarCreateModel model)
        {
            _logger.LogInformation("Начало обработки запроса на создание автомобиля: {ModelName}", model.Name);
            
            if (HttpContext.Session.GetString("AdminJWTToken") == null)
            {
                _logger.LogWarning("Отсутствует токен авторизации при создании автомобиля");
                TempData["ErrorMessage"] = "Необходима авторизация. Пожалуйста, войдите снова.";
                return RedirectToAction("Login");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Модель невалидна при создании автомобиля: {Errors}", 
                    string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                return View(model);
            }

            // Проверяем, что путь к изображению не пустой
            if (string.IsNullOrEmpty(model.ImagePath))
            {
                _logger.LogWarning("Путь к изображению пуст при создании автомобиля");
                ModelState.AddModelError("ImagePath", "Необходимо добавить изображение");
                return View(model);
            }
            
            _logger.LogInformation("Создание автомобиля с изображением: {ImagePath}", model.ImagePath);

            var adminToken = HttpContext.Session.GetString("AdminJWTToken");
            _logger.LogInformation("Получен токен авторизации длиной {TokenLength} символов", adminToken?.Length ?? 0);
            
            var car = await _adminService.CreateCarAsync(model, adminToken);

            if (car != null)
            {
                _logger.LogInformation("Автомобиль успешно создан с ID: {CarId}", car.Id);
                TempData["SuccessMessage"] = "Автомобиль успешно создан";
                return RedirectToAction("Cars");
            }
            else
            {
                _logger.LogError("Ошибка при создании автомобиля через API");
                ModelState.AddModelError("", "Ошибка при создании автомобиля. Проверьте соединение с API или перезайдите в систему.");
                return View(model);
            }
        }

        [HttpGet("cars/{id}")]
        public async Task<IActionResult> CarDetails(int id)
        {
            if (HttpContext.Session.GetString("AdminJWTToken") == null)
            {
                return RedirectToAction("Login");
            }

            var adminToken = HttpContext.Session.GetString("AdminJWTToken");
            var car = await _adminService.GetCarByIdAsync(id, adminToken);

            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        [HttpGet("cars/edit/{id}")]
        public async Task<IActionResult> EditCar(int id)
        {
            if (HttpContext.Session.GetString("AdminJWTToken") == null)
            {
                return RedirectToAction("Login");
            }

            var adminToken = HttpContext.Session.GetString("AdminJWTToken");
            var car = await _adminService.GetCarByIdAsync(id, adminToken);

            if (car == null)
            {
                return NotFound();
            }

            var model = new CarUpdateModel
            {
                Name = car.Name,
                ShortDescription = car.ShortDescription,
                FullDescription = car.FullDescription,
                Price = car.Price,
                Quantity = car.Quantity,
                ImagePath = car.ImagePath
            };

            ViewBag.CarId = id;
            return View(model);
        }

        [HttpPost("cars/edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCar(int id, CarUpdateModel model)
        {
            if (HttpContext.Session.GetString("AdminJWTToken") == null)
            {
                return RedirectToAction("Login");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.CarId = id;
                return View(model);
            }

            var adminToken = HttpContext.Session.GetString("AdminJWTToken");
            var result = await _adminService.UpdateCarAsync(id, model, adminToken);

            if (result)
            {
                TempData["SuccessMessage"] = "Автомобиль успешно обновлен";
                return RedirectToAction("Cars");
            }
            else
            {
                ModelState.AddModelError("", "Ошибка при обновлении автомобиля");
                ViewBag.CarId = id;
                return View(model);
            }
        }

        [HttpPost]
        [Route("cars/delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCar(int id)
        {
            _logger.LogInformation("Начало обработки запроса на удаление автомобиля с ID: {Id}", id);
            if (HttpContext.Session.GetString("AdminJWTToken") == null)
            {
                return RedirectToAction("Login");
            }

            var adminToken = HttpContext.Session.GetString("AdminJWTToken");
            var result = await _adminService.DeleteCarAsync(id, adminToken);

            if (result)
            {
                TempData["SuccessMessage"] = "Автомобиль успешно удален";
            }
            else
            {
                TempData["ErrorMessage"] = "Ошибка при удалении автомобиля";
            }

            return RedirectToAction("Cars");
        }

        #endregion

        #region Управление работниками

        [HttpGet("workers")]
        public async Task<IActionResult> Workers()
        {
            if (HttpContext.Session.GetString("AdminJWTToken") == null)
            {
                return RedirectToAction("Login");
            }

            var adminToken = HttpContext.Session.GetString("AdminJWTToken");
            var workers = await _adminService.GetWorkersAsync(adminToken);

            return View(workers);
        }

        [HttpGet("workers/create")]
        public IActionResult CreateWorker()
        {
            if (HttpContext.Session.GetString("AdminJWTToken") == null)
            {
                return RedirectToAction("Login");
            }

            return View(new WorkerCreateModel());
        }

        [HttpPost("workers/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateWorker(WorkerCreateModel model)
        {
            if (HttpContext.Session.GetString("AdminJWTToken") == null)
            {
                return RedirectToAction("Login");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var adminToken = HttpContext.Session.GetString("AdminJWTToken");
            var worker = await _adminService.CreateWorkerAsync(model, adminToken);

            if (worker != null)
            {
                TempData["SuccessMessage"] = "Работник успешно создан";
                return RedirectToAction("Workers");
            }
            else
            {
                ModelState.AddModelError("", "Ошибка при создании работника");
                return View(model);
            }
        }

        [HttpGet("workers/{id}")]
        public async Task<IActionResult> WorkerDetails(int id)
        {
            if (HttpContext.Session.GetString("AdminJWTToken") == null)
            {
                return RedirectToAction("Login");
            }

            var adminToken = HttpContext.Session.GetString("AdminJWTToken");
            var worker = await _adminService.GetWorkerByIdAsync(id, adminToken);

            if (worker == null)
            {
                return NotFound();
            }

            return View(worker);
        }

        [HttpGet("workers/edit/{id}")]
        public async Task<IActionResult> EditWorker(int id)
        {
            if (HttpContext.Session.GetString("AdminJWTToken") == null)
            {
                return RedirectToAction("Login");
            }

            var adminToken = HttpContext.Session.GetString("AdminJWTToken");
            var worker = await _adminService.GetWorkerByIdAsync(id, adminToken);

            if (worker == null)
            {
                return NotFound();
            }

            var model = new WorkerUpdateModel
            {
                Name = worker.Name,
                ShortDescription = worker.ShortDescription,
                FullDescription = worker.FullDescription,
                Position = worker.Position,
                WorkTime = worker.WorkTime,
                ImagePath = worker.ImagePath
            };

            ViewBag.WorkerId = id;
            return View(model);
        }

        [HttpPost("workers/edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditWorker(int id, WorkerUpdateModel model)
        {
            if (HttpContext.Session.GetString("AdminJWTToken") == null)
            {
                return RedirectToAction("Login");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.WorkerId = id;
                return View(model);
            }

            var adminToken = HttpContext.Session.GetString("AdminJWTToken");
            var result = await _adminService.UpdateWorkerAsync(id, model, adminToken);

            if (result)
            {
                TempData["SuccessMessage"] = "Работник успешно обновлен";
                return RedirectToAction("Workers");
            }
            else
            {
                ModelState.AddModelError("", "Ошибка при обновлении работника");
                ViewBag.WorkerId = id;
                return View(model);
            }
        }

        [HttpPost("workers/delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteWorker(int id)
        {
            if (HttpContext.Session.GetString("AdminJWTToken") == null)
            {
                return RedirectToAction("Login");
            }

            var adminToken = HttpContext.Session.GetString("AdminJWTToken");
            var result = await _adminService.DeleteWorkerAsync(id, adminToken);

            if (result)
            {
                TempData["SuccessMessage"] = "Работник успешно удален";
            }
            else
            {
                TempData["ErrorMessage"] = "Ошибка при удалении работника";
            }

            return RedirectToAction("Workers");
        }

        #endregion

        #region Управление услугами

        [HttpGet("services")]
        public async Task<IActionResult> Services()
        {
            if (HttpContext.Session.GetString("AdminJWTToken") == null)
            {
                return RedirectToAction("Login");
            }

            var adminToken = HttpContext.Session.GetString("AdminJWTToken");
            var services = await _adminService.GetServicesAsync(adminToken);

            return View(services);
        }

        [HttpGet("services/create")]
        public IActionResult CreateService()
        {
            if (HttpContext.Session.GetString("AdminJWTToken") == null)
            {
                return RedirectToAction("Login");
            }

            return View(new ServiceCreateModel());
        }

        [HttpPost("services/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateService(ServiceCreateModel model)
        {
            if (HttpContext.Session.GetString("AdminJWTToken") == null)
            {
                return RedirectToAction("Login");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var adminToken = HttpContext.Session.GetString("AdminJWTToken");
            var service = await _adminService.CreateServiceAsync(model, adminToken);

            if (service != null)
            {
                TempData["SuccessMessage"] = "Услуга успешно создана";
                return RedirectToAction("Services");
            }
            else
            {
                ModelState.AddModelError("", "Ошибка при создании услуги");
                return View(model);
            }
        }

        [HttpGet("services/{id}")]
        public async Task<IActionResult> ServiceDetails(int id)
        {
            if (HttpContext.Session.GetString("AdminJWTToken") == null)
            {
                return RedirectToAction("Login");
            }

            var adminToken = HttpContext.Session.GetString("AdminJWTToken");
            var service = await _adminService.GetServiceByIdAsync(id, adminToken);

            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }

        [HttpGet("services/edit/{id}")]
        public async Task<IActionResult> EditService(int id)
        {
            if (HttpContext.Session.GetString("AdminJWTToken") == null)
            {
                return RedirectToAction("Login");
            }

            var adminToken = HttpContext.Session.GetString("AdminJWTToken");
            var service = await _adminService.GetServiceByIdAsync(id, adminToken);

            if (service == null)
            {
                return NotFound();
            }

            var model = new ServiceUpdateModel
            {
                Name = service.Name,
                ShortDescription = service.ShortDescription,
                LongDescription = service.LongDescription,
                Price = service.Price,
                ImagePath = service.ImagePath
            };

            ViewBag.ServiceId = id;
            return View(model);
        }

        [HttpPost("services/edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditService(int id, ServiceUpdateModel model)
        {
            if (HttpContext.Session.GetString("AdminJWTToken") == null)
            {
                return RedirectToAction("Login");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ServiceId = id;
                return View(model);
            }

            var adminToken = HttpContext.Session.GetString("AdminJWTToken");
            var result = await _adminService.UpdateServiceAsync(id, model, adminToken);

            if (result)
            {
                TempData["SuccessMessage"] = "Услуга успешно обновлена";
                return RedirectToAction("Services");
            }
            else
            {
                ModelState.AddModelError("", "Ошибка при обновлении услуги");
                ViewBag.ServiceId = id;
                return View(model);
            }
        }

        [HttpPost("services/delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteService(int id)
        {
            if (HttpContext.Session.GetString("AdminJWTToken") == null)
            {
                return RedirectToAction("Login");
            }

            var adminToken = HttpContext.Session.GetString("AdminJWTToken");
            var result = await _adminService.DeleteServiceAsync(id, adminToken);

            if (result)
            {
                TempData["SuccessMessage"] = "Услуга успешно удалена";
            }
            else
            {
                TempData["ErrorMessage"] = "Ошибка при удалении услуги";
            }

            return RedirectToAction("Services");
        }

        #endregion

        [HttpPost("upload-image")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (HttpContext.Session.GetString("AdminJWTToken") == null)
            {
                _logger.LogWarning("Попытка загрузки изображения без авторизации");
                return Unauthorized(new { success = false, message = "Необходима авторизация" });
            }

            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("Попытка загрузки пустого файла");
                return BadRequest(new { success = false, message = "Файл не выбран или пуст" });
            }

            // Проверка размера файла (максимум 5 МБ)
            if (file.Length > 5 * 1024 * 1024)
            {
                _logger.LogWarning("Попытка загрузки слишком большого файла: {Size} байт", file.Length);
                return BadRequest(new { success = false, message = "Размер файла не должен превышать 5 МБ" });
            }

            // Проверка типа файла
            var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
            if (!allowedTypes.Contains(file.ContentType.ToLower()))
            {
                _logger.LogWarning("Попытка загрузки файла неподдерживаемого типа: {ContentType}", file.ContentType);
                return BadRequest(new { success = false, message = "Поддерживаются только изображения в форматах JPEG, PNG, GIF и WebP" });
            }

            try
            {
                _logger.LogInformation("Начало загрузки изображения: {FileName}, {Size} байт", file.FileName, file.Length);
                var imagePath = await _fileService.SaveImageAsync(file);
                
                if (string.IsNullOrEmpty(imagePath))
                {
                    _logger.LogError("Сервис сохранения изображений вернул пустой путь");
                    return BadRequest(new { success = false, message = "Не удалось сохранить изображение" });
                }

                _logger.LogInformation("Изображение успешно загружено: {Path}", imagePath);
                return Ok(new { success = true, imagePath });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Необработанная ошибка при загрузке изображения: {FileName}", file.FileName);
                return StatusCode(500, new { success = false, message = "Ошибка сервера при загрузке изображения" });
            }
        }
    }
} 