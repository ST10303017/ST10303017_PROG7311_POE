// File: Controllers/HomeController.cs
using Microsoft.AspNetCore.Authorization; // Required if you globally authorize and want to allow anonymous here
using Microsoft.AspNetCore.Mvc;
using ST10303017_PROG7311_POE.Models; // For ErrorViewModel if used
using System.Diagnostics;

namespace ST10303017_PROG7311_POE.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [AllowAnonymous] // Allow access to home page even if global authorization is set
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}