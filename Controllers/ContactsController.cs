using Microsoft.AspNetCore.Mvc;
using cars_web.Models;
using cars_web.Services;
using System.Diagnostics;
using System.Threading.Tasks;
using cars_web.Filters;

namespace cars_web.Controllers
{
    [RequireEmailVerification]
    public class ContactsController : Controller
    {
        private readonly ILogger<ContactsController> _logger;

        public ContactsController(ILogger<ContactsController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
} 