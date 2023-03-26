using DoAnTotNghiep.Data;
using DoAnTotNghiep.Entity;
using DoAnTotNghiep.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using DoAnTotNghiep.Enum;
using DoAnTotNghiep.Modules;

namespace DoAnTotNghiep.Controllers
{
    public class ExploreController : BaseController
    {
        private readonly ILogger<ExploreController> _logger;
        private readonly DoAnTotNghiepContext _context;
        private readonly IConfiguration _configuration;

        public ExploreController(ILogger<ExploreController> logger, DoAnTotNghiepContext context, IConfiguration configuration, IHostEnvironment environment) : base(environment)
        {
            _logger = logger;
            this._context = context;
            this._configuration = configuration;
        }

        /// <summary>
        /// Gửi IdCity hoặc IdDistrict => show House
        /// Filter people, minPrice, maxPrice, utilities, rangeDate
        /// </summary>
        /// => where Status == Valid || Status == Swapping && request.status == accepted && request.endDate < minRangeDATE && request.startDate > maxRangeDate</minRangeDATE>
        /// <returns></returns>

        public IActionResult Index(int IdCity)
        {
            ExploreResult model = new ExploreResult();
            model.Houses = this.GetHouses(12, IdCity);
            model.Center = this.GetCenter(IdCity);

            return View(model);
        }


        private List<DetailHouseViewModel> GetHouses(int number = 10, int IdCity = 0)
        {
            var listHouse = this._context.Houses
                                                .Take(number)
                                                .OrderByDescending(m => m.Rating)
                                                .Where(m => m.Status == (int)StatusHouse.VALID && m.IdCity == IdCity)
                                                .ToList();
            byte[] salt = Crypto.Salt(this._configuration);
            string host = this.GetWebsitePath();
            List<DetailHouseViewModel> res = new List<DetailHouseViewModel>();
            foreach (var item in listHouse)
            {
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
        private Point GetCenter(int IdCity = 0)
        {
            var city = this._context.Cities.Where(m => m.Id == IdCity).FirstOrDefault();
            if(city != null)
            {
                return new Point()
                {
                    Lat = city.Lat,
                    Lng = city.Lng,
                };
            }
            return new Point();
        }
    }
}