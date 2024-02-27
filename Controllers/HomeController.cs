using LandedMVC.Models;
using LandedMVC.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TestApi.Dtos;

namespace LandedMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApiService<EmailDto> _apiService;

        public HomeController(ILogger<HomeController> logger, ApiService<EmailDto> apiService)
        {
            _logger = logger;
            _apiService = apiService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

		[HttpPost]
		public async Task<IActionResult> EmailAsync(EmailModel email, CancellationToken token) 
        {
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			await _apiService.PostAsync(email.ToDto(), token);
			return Json(email);
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
