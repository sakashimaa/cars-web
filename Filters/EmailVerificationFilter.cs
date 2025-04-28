using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using cars_web.Extensions;
using cars_web.Services;
using System.Threading.Tasks;

namespace cars_web.Filters
{
    public class EmailVerificationFilter : IAsyncAuthorizationFilter
    {
        private readonly AuthService _authService;

        public EmailVerificationFilter(AuthService authService)
        {
            _authService = authService;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // Проверяем, авторизован ли пользователь
            var token = context.HttpContext.Session.GetString("JWTToken");
            if (token == null)
            {
                // Если пользователь не авторизован, перенаправляем его на страницу входа
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            // Проверяем статус подтверждения email через API
            var status = await _authService.CheckEmailVerificationStatusAsync(token);
            
            if (!status.IsEmailConfirmed)
            {
                // Обновляем информацию в сессии
                context.HttpContext.Session.SetString("UnverifiedEmail", status.Email);
                
                // Если есть ID пользователя в сессии
                if (context.HttpContext.Session.GetInt32("UserId").HasValue)
                {
                    // Перенаправляем на страницу уведомления
                    context.Result = new RedirectToActionResult("UnverifiedEmail", "Account", null);
                }
                else
                {
                    // Если нет ID, перенаправляем на вход
                    context.Result = new RedirectToActionResult("Login", "Account", null);
                }
            }
        }
    }
    
    // Аттрибут для применения к контроллерам или методам
    public class RequireEmailVerificationAttribute : TypeFilterAttribute
    {
        public RequireEmailVerificationAttribute() : base(typeof(EmailVerificationFilter))
        {
        }
    }
} 