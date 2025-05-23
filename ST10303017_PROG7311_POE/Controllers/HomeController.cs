/*
Calwyn Govender
ST10303017
PROG7311
(OpenAI, 2025)
(Troelsen & Japikse, 2022)
*/

using Microsoft.AspNetCore.Authorization; 
using Microsoft.AspNetCore.Mvc;
using ST10303017_PROG7311_POE.Models; 
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

        [AllowAnonymous]
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