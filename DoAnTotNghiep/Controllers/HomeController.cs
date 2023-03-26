using DoAnTotNghiep.Data;
using DoAnTotNghiep.Entity;
using DoAnTotNghiep.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using DoAnTotNghiep.Enum;
using DoAnTotNghiep.Modules;
using Microsoft.AspNetCore.Authorization;

namespace DoAnTotNghiep.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DoAnTotNghiepContext _context;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, 
                                DoAnTotNghiepContext context, 
                                IConfiguration _configuration,
                                IHostEnvironment environment) : base(environment)
        {
            _logger = logger;
            this._context = context;
            this._configuration = _configuration;
        }

        public IActionResult Index()
        {
            List<DetailHouseViewModel> houses = this.GetPopularHouse(12);
            List<PopularCityViewModel> cities = this.GetPopularCity(4);



            return View(new HomeViewModel()
            {
                PopularCities = cities,
                PopularHouses = houses,
                NumberCities = this.NumberCity(),
                NumberHouses = this.NumberHouse()
            });
        }

        private List<DetailHouseViewModel> GetPopularHouse(int number = 10)
        {
            var listHouse = this._context.Houses
                                                .Take(number)
                                                .OrderByDescending(m => m.Rating)
                                                .Where(m => m.Status == (int)StatusHouse.VALID)
                                                .ToList();
                byte[] salt = Crypto.Salt(this._configuration);
                string host = this.GetWebsitePath();
                List<DetailHouseViewModel> res = new List<DetailHouseViewModel>();
                foreach (var item in listHouse)
                {
                    //this._context.Entry(item).Collection(m => m.RulesInHouses).Query().Load();
                    //this._context.Entry(item).Collection(m => m.UtilitiesInHouses).Query().Load();
                    //this._context.Entry(item).Reference(m => m.Users).Query().Load();
                    //this._context.Entry(item).Reference(m => m.Citys).Query().Load();
                    //this._context.Entry(item).Reference(m => m.Districts).Query().Load();
                    //this._context.Entry(item).Reference(m => m.Wards).Query().Load();
                    this._context.Entry(item).Collection(m => m.Requests).Query().Load();
                    this._context.Entry(item).Collection(m => m.FileOfHouses).Query().Load();
                    this._context.Entry(item).Collection(m => m.FeedBacks).Query().Load();

                    DetailHouseViewModel model = new DetailHouseViewModel(item, salt, item.Users, host);
                    if (item.FileOfHouses != null)
                    {
                        foreach (var f in item.FileOfHouses)
                        {
                            this._context.Entry(f).Reference(m => m.Files).Load();
                            if (f.Files != null)
                            {
                                model.Images.Add(new ImageBase(f.Files, host));
                                break;
                            }
                        }
                    }
                    res.Add(model);
                }
                return res;
        }
        private List<PopularCityViewModel> GetPopularCity(int number = 10)
        {
            string host = this.GetWebsitePath();
            List<PopularCityViewModel> cityList = this._context.Cities
                                                .Include(m => m.houses)
                                                .OrderByDescending(m => m.Count)
                                                .Take(number)
                                                .Where(m => m.houses != null && m.houses.Any())
                                                .Select(m => new PopularCityViewModel()
                                                        {
                                                           Name = m.Name,
                                                           Id = m.Id,
                                                           ImageUrl = host + "/Image/house-demo.png",
                                                           IsDeleted = false,
                                                           Location = new PointViewModel()
                                                           {
                                                               Lat = m.Lat,
                                                               Lng = m.Lng
                                                           }

                                                       }
                                                )
                                                .ToList();
            return cityList;
        }
        private int NumberCity()
        {
            var cities = from c in this._context.Cities
                         join h in this._context.Houses on c.Id equals h.IdCity
                         where h.Status == (int)StatusHouse.VALID
                         select h;

            return cities == null? 0: cities.ToList().Count();
        }
        private int NumberHouse()
        {
            return this._context.Houses
                                .Where(m => m.Status == (int)StatusHouse.VALID)
                                .ToList().Count();
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