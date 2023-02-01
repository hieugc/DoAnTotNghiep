using DoAnTotNghiep.Data;
using DoAnTotNghiep.Entity;
using DoAnTotNghiep.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace DoAnTotNghiep.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DoAnTotNghiepContext _context;

        public HomeController(ILogger<HomeController> logger, DoAnTotNghiepContext context)
        {
            _logger = logger;
            this._context = context;
        }

        public IActionResult Index()
        {
            ViewData["Check"] = this._context == null ? "null" : "not null";
            ViewData["Number"] = this._context == null ? "null": this._context.Cities.Count();
            List<City> cities = this._context == null ? new List<City>(): this._context.Cities.ToList();

            ViewData["District"] = this._context == null ? new List<District>(): this._context.Districts.ToList();
            return View(cities);
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