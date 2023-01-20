using DoAnTotNghiep.Data;
using DoAnTotNghiep.Entity;
using DoAnTotNghiep.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DoAnTotNghiep.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DoAnTotNghiepContext context;

        public HomeController(ILogger<HomeController> logger, DoAnTotNghiepContext context)
        {
            _logger = logger;
            this.context = context;
        }

        public IActionResult Index()
        {
            Console.Write(this.context.Cities.Any());
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}