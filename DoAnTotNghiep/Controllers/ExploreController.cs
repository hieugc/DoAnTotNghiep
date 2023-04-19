using DoAnTotNghiep.Data;
using DoAnTotNghiep.Entity;
using DoAnTotNghiep.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using DoAnTotNghiep.Enum;
using DoAnTotNghiep.Modules;
using DoAnTotNghiep.Hubs;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.CodeAnalysis;
using System.Linq;

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

        public async Task<IActionResult> IndexAsync(Filter filter)
        {
            ExploreResult model = new ExploreResult();
            model.Houses =  await this.SearchAsync(filter);
            model.Utilities = this._context.Utilities.ToList();
            return View(model);
        }

        private async Task<string> GetLocation(string query)
        {
            string protocol = this.Request.Scheme;
            string key = "Asf_PRzBpUJVcb9lcAg48BLOuAuaBItg4ZzCqaNQaSIFReqieYA02KBcovVD08Jk";
            Uri localRequest = RequestAPI.GeoCodeRequest(protocol, query, key);
            var localResult = await RequestAPI.Get<string>(localRequest);
            string? res = string.Empty;
            if (!string.IsNullOrEmpty(localResult) && localResult.IndexOf("adminDistrict") != -1)
            {
                res = localResult.Split("adminDistrict")[1].Split("\",")[0].Replace("\"", "").Replace(":", "").Trim();
            }

            return res;
        }

        [HttpGet("/Explore/search")]
        public async Task<IActionResult> Explore(Filter filter)
        {
            return PartialView("~/Views/Explore/_Item.cshtml", await this.SearchAsync(filter));
        }
        [HttpGet("/api/Explore")]
        public async Task<IActionResult> ApiExplore(Filter filter)
        {
            return Json(new
            {
                Status = 200,
                Data = await this.SearchAsync(filter)
            });
        }

        private async Task<ListDetailHouses> SearchAsync(Filter filter)
        {
            ListDetailHouses model = new ListDetailHouses();
            int IdCity = filter.IdCity == null ? 0 : filter.IdCity.Value;
            if (IdCity == 0 && !string.IsNullOrEmpty(filter.Location))
            {
                string location = await this.GetLocation(filter.Location);
                if (!string.IsNullOrEmpty(location))
                {
                    var city = this._context.Cities.Where(m => m.BingName == location).FirstOrDefault();
                    if (city != null) IdCity = city.Id;
                }
            }
            model = await this.GetHousesAsync(filter.Page, filter.Limit, IdCity,
                                            filter.DateStart, filter.DateEnd, filter.OptionSort,
                                            filter.PriceStart, filter.PriceEnd, filter.Utilities,
                                            filter.People, filter.IdDistrict);
            return model;
        }

        private async Task<ListDetailHouses> GetHousesAsync(int page = 1,int number = 10, 
                                    int IdCity = 0, 
                                    DateTime? DateStart = null, DateTime? DateEnd = null,
                                    int OptionSort = 0,
                                    int? PriceStart = null, int? PriceEnd = null,
                                    List<int>? Utilities = null, int? People = null, int? IdDistrict = null)
        {
            
            var listHouse = this.GetContextHouses(IdCity, DateStart, DateEnd, OptionSort, PriceStart, PriceEnd, Utilities, People, IdDistrict);

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

                item.IncludeLocation(this._context);
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
                int IdUser = this.GetIdUser();
                if(IdUser != 0)
                {
                    await CreateWaitingRequestTimerAsync(TargetFunction.ExecuteCreateWaiting, TimeSpan.FromSeconds(10), 1, IdCity, IdUser, DateStart, DateEnd);
                }
            }

            return new ListDetailHouses()
            {
                Houses = res,
                Pagination = pagination
            };
        }
        //chưa chạy thử
        private List<House> GetContextHouses(int IdCity = 0, 
            DateTime? DateStart = null, DateTime? DateEnd = null, 
            int OptionSort = 0, int? PriceStart = null, int? PriceEnd = null, 
            List<int>? Utilities = null, int? People = null, int? IdDistrict = null)
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
                                                (r.Status == (int) StatusRequest.ACCEPT || r.Status == (int)StatusRequest.CHECK_IN)
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
                                        && m.UtilitiesInHouses.Where(u => u.Status == true)
                                                    .Select(m => m.IdUtilities)
                                                    .Intersect(Utilities).Any()).ToList();
            }
            if(People != null)
            {
                model = model.Where(m => m.People >= People.Value).ToList();
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
        private async Task CreateWaitingRequestTimerAsync(string Function, TimeSpan timeStart, int limit, int IdCity, int IdUser, DateTime? DateStart, DateTime? DateEnd)
        {
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;
            DoAnTotNghiepContext inputContext = new DoAnTotNghiepContext(this._context.GetConfig());
            string host = this.GetWebsitePath();
            CreateWaitingRequest Object = new CreateWaitingRequest(IdCity, IdUser, DateStart, DateEnd);

            TimedHostedService timer = new TimedHostedService(
                                            inputContext,
                                            host,
                                            Function,
                                            token,
                                            limit,
                                            timeStart,
                                            Object);

            await timer.StartAsync(token);
        }
        private string RemoveVietnamese(string str)
        {
            str = str.ToLower();
            str = Regex.Replace(str, @"[áàảãạăắằẳẵặâấầẩẫậ]", "a");
            str = Regex.Replace(str, @"[éèẻẽẹêếềểễệ]", "e");
            str = Regex.Replace(str, @"[iíìỉĩị]", "i");
            str = Regex.Replace(str, @"[óòỏõọôốồổỗộơớờởỡợ]", "o");
            str = Regex.Replace(str, @"[úùủũụưứừửữự]", "u");
            str = Regex.Replace(str, @"[ýỳỷỹỵ]", "y");
            return Regex.Replace(str, @"[đ]", "d");
        }

        private string RemoveSyntax(string str)
        {
            string pattern = @"[^a-z0-9\s]+";
            return Regex.Replace(str, pattern, "");
        }

        [HttpGet("/api/Suggest")]
        public IActionResult GetCityAndDistrict(string location)
        {
            location = this.RemoveSyntax(this.RemoveVietnamese(location));
            Console.WriteLine(location);

            var item = from ct in this._context.Cities
                       join dt in this._context.Districts on ct.Id equals dt.IdCity
                       select new CityAndDistrict()
                       {
                           IdCity = ct.Id,
                           IdDistrict = dt.Id,
                           CityName = ct.Name,
                           DistrictName = dt.Name
                       };
            List<CityAndDistrict> model = item.ToList().Where(m => this.RemoveVietnamese((m.DistrictName + " " + m.CityName)).Contains(location)).Take(12).ToList();
            return Json(new
            {
                Status = 200,
                Data = model
            });
        }
    }
}