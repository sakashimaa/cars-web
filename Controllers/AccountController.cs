using Microsoft.AspNetCore.Mvc;
using cars_web.Models;
using cars_web.Services;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using cars_web.Extensions;

namespace cars_web.Controllers
{
    public class AccountController : Controller
    {
        private readonly AuthService _authService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(AuthService authService, ILogger<AccountController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Profile() 
        {
            if (HttpContext.Session.GetString("JWTToken") == null) 
            {
                return RedirectToAction("Login");
            }

            var data = await _authService.GetUserProfile(HttpContext.Session.GetString("JWTToken"));
            
            // Проверяем, получены ли данные
            if (data == null)
            {
                // Создаем модель с минимальной информацией
                return View(new UserProfileViewModel 
                {
                    Email = HttpContext.Session.GetString("UserEmail"),
                });
            }
            
            // Проверка на null перед логированием
            _logger.LogInformation($"email = {data.Email} first name = {data.FirstName ?? "N/A"} last name = {data.LastName ?? "N/A"}");

            return View(data); // Возвращаем данные напрямую, если они совпадают с моделью
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserLoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var token = await _authService.LoginAsync(model);
            if (token != null)
            {
                // Сохраняем токен и ID пользователя в сессии
                HttpContext.Session.SetString("JWTToken", token.AccessToken);
                HttpContext.Session.SetString("UserEmail", model.Email);
                HttpContext.Session.SetInt32("UserId", token.User.Id);
                
                // Проверяем статус подтверждения email через API
                var status = token.User.IsEmailConfirmed;

                HttpContext.Session.SetBoolean("IsEmailConfirmed", status);
                
                if (!status)
                {
                    // Если email не подтвержден, сохраняем данные для повторной отправки
                    HttpContext.Session.SetString("UnverifiedEmail", model.Email);
                    
                    return RedirectToAction("UnverifiedEmail");
                }
                
                _logger.LogInformation("User logged in: {Email}", model.Email);
                
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Неверный логин или пароль");
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserRegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _logger.LogInformation($"Email = {model.Email}, Password = {model.Password}, FirstName = {model.FirstName} LastName = {model.LastName}");

            var result = await _authService.RegisterAsync(model);
            if (result.Success)
            {
                _logger.LogInformation("User registered: {Email}", model.Email);
                
                // Если есть ID пользователя, отправляем подтверждение по email
                if (result.UserId.HasValue)
                {
                    await _authService.SendEmailVerificationAsync(result.UserId.Value);
                    
                    // Сохраняем ID пользователя во временной сессии
                    TempData["RegisteredUserId"] = result.UserId.Value;
                    TempData["RegisteredEmail"] = model.Email;
                    
                    return RedirectToAction("VerificationSent");
                }
                else
                {
                    TempData["SuccessMessage"] = "Регистрация успешна! Теперь вы можете войти.";
                    return RedirectToAction("Login");
                }
            }

            ModelState.AddModelError("", "Ошибка при регистрации. Возможно, этот email уже используется.");
            return View(model);
        }

        [HttpGet]
        public IActionResult VerificationSent()
        {
            // Проверяем наличие данных о зарегистрированном пользователе
            if (TempData["RegisteredEmail"] == null)
            {
                return RedirectToAction("Register");
            }
            
            // Сохраняем во ViewBag для использования в представлении
            ViewBag.Email = TempData["RegisteredEmail"].ToString();
            
            // Сохраняем ID для использования в ResendVerification
            TempData.Keep("RegisteredUserId");
            
            return View();
        }
        
        [HttpGet]
        public async Task<IActionResult> ResendVerification()
        {
            if (TempData["RegisteredUserId"] == null)
            {
                return RedirectToAction("Register");
            }
            
            int userId = (int)TempData["RegisteredUserId"];
            
            bool result = await _authService.SendEmailVerificationAsync(userId);
            
            if (result)
            {
                TempData["SuccessMessage"] = "Письмо с подтверждением отправлено повторно. Пожалуйста, проверьте вашу почту.";
            }
            else
            {
                TempData["ErrorMessage"] = "Не удалось отправить письмо с подтверждением. Пожалуйста, попробуйте позже.";
            }
            
            // Сохраняем ID для возможной повторной отправки
            TempData.Keep("RegisteredUserId");
            TempData.Keep("RegisteredEmail");
            
            return RedirectToAction("VerificationSent");
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Токен подтверждения отсутствует");
            }
            
            var result = await _authService.ConfirmEmailAsync(token);
            
            if (result)
            {
                TempData["SuccessMessage"] = "Email успешно подтвержден! Теперь вы можете войти.";
                return RedirectToAction("Login");
            }
            
            TempData["ErrorMessage"] = "Не удалось подтвердить email. Возможно, токен недействителен или истек.";
            return RedirectToAction("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            // Очищаем сессию
            HttpContext.Session.Clear();
            
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult UnverifiedEmail()
        {
            // Проверяем наличие данных о неподтвержденном email
            var email = HttpContext.Session.GetString("UnverifiedEmail");
            var userId = HttpContext.Session.GetInt32("UserId");
            
            if (string.IsNullOrEmpty(email) || !userId.HasValue)
            {
                return RedirectToAction("Login");
            }
            
            var model = new UnverifiedEmailViewModel
            {
                UserId = userId.Value,
                Email = email
            };
            
            return View(model);
        }
        
        [HttpPost]
        public async Task<IActionResult> SendVerificationEmail()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            
            if (!userId.HasValue)
            {
                return RedirectToAction("Login");
            }
            
            bool result = await _authService.SendEmailVerificationAsync(userId.Value);
            
            if (result)
            {
                TempData["SuccessMessage"] = "Письмо с подтверждением отправлено. Пожалуйста, проверьте вашу почту.";
            }
            else
            {
                TempData["ErrorMessage"] = "Не удалось отправить письмо с подтверждением. Пожалуйста, попробуйте позже.";
            }
            
            return RedirectToAction("UnverifiedEmail");
        }

        [HttpGet]
        [Route("email-verified")]
        public IActionResult EmailVerified(bool success, string email, string error)
        {
            if (success)
            {
                ViewBag.Success = true;
                ViewBag.Email = email;
                TempData["SuccessMessage"] = "Ваш email успешно подтвержден! Теперь вы можете войти в систему.";
            }
            else
            {
                ViewBag.Success = false;
                ViewBag.Error = error ?? "Произошла ошибка при подтверждении email.";
                TempData["ErrorMessage"] = error ?? "Произошла ошибка при подтверждении email.";
            }
            
            return View();
        }
    }
} 