using DoAnTotNghiep.Data;
using DoAnTotNghiep.Entity;
using DoAnTotNghiep.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DoAnTotNghiep.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DoAnTotNghiepContext _context;
        private readonly IConfiguration configurationBuilder;

        public HomeController(ILogger<HomeController> logger, DoAnTotNghiepContext context, IConfiguration configurationBuilder)
        {
            _logger = logger;
            this._context = context;
            this.configurationBuilder = configurationBuilder;
        }

        public IActionResult Index()
        {
            Console.WriteLine(HttpContext.Request.Scheme + "://" + HttpContext.Request.Host + "/");
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