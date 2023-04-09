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

        public IActionResult Index(Filter filter)
        {
            ExploreResult model = new ExploreResult();
            int IdCity = filter.IdCity == null ? 0 : filter.IdCity.Value;

            model.Houses = this.GetHouses(filter.Page, filter.Limit, IdCity, filter.DateStart, filter.DateEnd, filter.OptionSort, filter.PriceStart, filter.PriceEnd, filter.Utilities);
            model.Center = this.GetCenter(IdCity);
            return View(model);
        }


        private ListDetailHouses GetHouses(int page = 1,int number = 10, 
                                    int IdCity = 0, 
                                    DateTime? DateStart = null, DateTime? DateEnd = null,
                                    int OptionSort = 0,
                                    int? PriceStart = null, int? PriceEnd = null,
                                    List<int>? Utilities = null)
        {
            
            var listHouse = this.GetContextHouses(IdCity, DateStart, DateEnd, OptionSort, PriceStart, PriceEnd, Utilities);

            int skip = (page - 1 < 0) ? 0 : page - 1;
            Pagination pagination = new Pagination(page, number);
            pagination.Total = (int)Math.Ceiling((double)listHouse.Count() / number);

            byte[] salt = Crypto.Salt(this._configuration);
            string host = this.GetWebsitePath();
            List<DetailHouseViewModel> res = new List<DetailHouseViewModel>();
            foreach (var item in listHouse.Skip(skip).Take(number))
            {
                if (!this._context.Entry(item).Collection(m => m.Requests).IsLoaded)
                {
                    this._context.Entry(item).Collection(m => m.Requests).Query().Load();
                }
                this._context.Entry(item).Collection(m => m.FileOfHouses).Query().Load();
                this._context.Entry(item).Collection(m => m.FeedBacks).Query().Load();
                this._context.Entry(item).Reference(m => m.Citys).Query().Load();
                this._context.Entry(item).Reference(m => m.Districts).Query().Load();

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

            if(res.Count() == 0)
            {
                //chạy code trao đổi xoay vòng
                //lưu lại địa điểm không tìm thấy
            }

            return new ListDetailHouses()
            {
                Houses = res,
                Pagination = pagination
            };
        }
        //chưa chạy thử
        private List<House> GetContextHouses(int IdCity = 0, DateTime? DateStart = null, DateTime? DateEnd = null, int OptionSort = 0, int? PriceStart = null, int? PriceEnd = null, List<int>? Utilities = null)
        {
            List<House> model = new List<House>();
            if (DateStart != null && DateEnd != null)
            {
                var res = this._context.Houses
                                .Include(m => m.Requests)
                                .Include(m => m.UtilitiesInHouses)
                                .Where(m => m.Status == (int)StatusHouse.VALID
                                            && m.IdCity == IdCity
                                            && (m.Requests == null || 
                                                m.Requests != null 
                                                && !m.Requests.Any(r => 
                                                (r.Status == (int) StatusRequest.ACCEPT || r.Status == (int)StatusRequest.CHECK_IN || r.Status == (int)StatusRequest.CHECK_OUT)
                                                && !(r.StartDate >= DateEnd || r.EndDate <= DateStart)
                                        )))
                                .ToList();
                model.AddRange(res);
            }
            else
            {
                var res = this._context.Houses
                                .Include(m => m.Requests)
                                .Include(m => m.UtilitiesInHouses)
                                .Where(m => m.Status == (int)StatusHouse.VALID
                                            && m.IdCity == IdCity)
                                .ToList();

                model.AddRange(res);
            }

            if(PriceStart != null)
            {
                model = model.Where(m => m.Price >= PriceStart.Value).ToList();
            }
            if (PriceEnd != null)
            {
                model = model.Where(m => m.Price <= PriceEnd.Value).ToList();
            }

            if (Utilities != null && Utilities.Count() > 0)
            {
                model = model.Where(m => m.UtilitiesInHouses != null
                                        && m.UtilitiesInHouses
                                                    .Select(m => m.IdUtilities)
                                                    .Intersect(Utilities).Any()).ToList();
            }

            switch (OptionSort)
            {
                case (int)SortResult.RATING:
                    return model.OrderByDescending(m => m.Rating).ToList();
                case (int)SortResult.MIN_PRICE:
                    return model.OrderByDescending(m => m.Price).ToList();
                case (int)SortResult.MAX_PRICE:
                    return model.OrderBy(m => m.Price).ToList();
            }
            //chưa pk closest làm sao?
            return model;
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