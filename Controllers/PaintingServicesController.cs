using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using cars_web.Services;
using cars_web.Models;
using System.Diagnostics;

namespace cars_web.Controllers
{
    public class PaintingServicesController : Controller
    {
        private readonly PaintingServiceClient _paintingServiceClient;

        public PaintingServicesController(PaintingServiceClient paintingServiceClient)
        {
            _paintingServiceClient = paintingServiceClient;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var services = await _paintingServiceClient.GetAllServices();
                return View(services);
            }
            catch (Exception ex)
            {
                // Log error
                ViewBag.ErrorMessage = "Не удалось загрузить услуги покраски. Пожалуйста, попробуйте позже.";
                ViewBag.DetailedError = ex.Message;
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var service = await _paintingServiceClient.GetServiceById(id);
                
                if (service == null)
                {
                    return NotFound();
                }
                
                return View(service);
            }
            catch (Exception ex)
            {
                // Log error
                ViewBag.ErrorMessage = "Не удалось загрузить информацию об услуге. Пожалуйста, попробуйте позже.";
                ViewBag.DetailedError = ex.Message;
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}