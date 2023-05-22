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
using DoAnTotNghiep.Service;

namespace DoAnTotNghiep.Controllers
{
    public class ExploreController : BaseController
    {
        //xong
        private readonly DoAnTotNghiepContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILocationService _locationService;
        private readonly IFileService _fileService;
        private readonly IHouseService _houseService;
        private readonly IRuleService _ruleService;
        private readonly IUtilitiesService _utilitiesService;

        public ExploreController(DoAnTotNghiepContext context, 
            IConfiguration configuration, 
            IHostEnvironment environment,
            ILocationService locationService,
            IFileService fileService,
            IHouseService houseService,
            IUtilitiesService utilitiesService,
            IRuleService ruleService) : base(environment)
        {
            this._context = context;
            this._configuration = configuration;
            this._locationService = locationService;
            this._fileService = fileService;
            this._houseService = houseService;
            this._utilitiesService = utilitiesService;
            this._ruleService = ruleService;
        }
        public async Task<IActionResult> IndexAsync(Filter filter)
        {
            ExploreResult model = new ExploreResult();
            model.Houses = await this.SearchAsync(filter);
            model.Utilities = this._utilitiesService.All();

            int IdUser = this.GetIdUser();
            if (IdUser != 0)
            {
                this.SetViewData(new DoAnTotNghiepContext(this._context.GetConfig()), IdUser, Crypto.Salt(this._configuration));
            }

            return View(model);
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
                string? key = this._configuration.GetConnectionString(BingMaps.Key);
                if (!string.IsNullOrEmpty(key))
                {
                    string location = await RequestAPI.GetLocationInString(Request.Scheme, key, filter.Location);
                    if (!string.IsNullOrEmpty(location))
                    {
                        var city = this._context.Cities.Where(m => m.BingName == location).FirstOrDefault();
                        if (city != null) IdCity = city.Id;
                    }
                }
            }

            filter.IdCity = IdCity;

            //xử lý district?

            model = await this.GetHousesAsync(filter);
            return model;
        }

        private async Task<ListDetailHouses> GetHousesAsync(Filter filter)
        {
            var listHouse = this._houseService.GetListHouseByFilter(filter);

            int skip = ((filter.Page - 1 <= 0) ? 0 : filter.Page - 1) * filter.Limit;
            Pagination pagination = new Pagination(filter.Page, filter.Limit);
            ViewData["count_item"] = listHouse.Count();
            pagination.Total = (int)Math.Ceiling((double)listHouse.Count() / filter.Limit);

            byte[] salt = Crypto.Salt(this._configuration);
            string host = this.GetWebsitePath();
            List<DetailHouseViewModel> res = this._houseService.GetListDetailHouseWithOneImage(listHouse.Skip(skip).Take(filter.Limit).ToList(), this._fileService, host, salt);
            
            if (res.Count() == 0)
            {
                int IdUser = this.GetIdUser();
                if (IdUser != 0)
                {
                    await CreateWaitingRequestTimerAsync(TargetFunction.ExecuteCreateWaiting, TimeSpan.FromSeconds(10), 1, filter.IdCity.Value, IdUser, filter.DateStart, filter.DateEnd);
                }
            }

            return new ListDetailHouses()
            {
                Houses = res,
                Pagination = pagination
            };
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
        
        [HttpGet("/api/Suggest")]
        public IActionResult GetCityAndDistrict(string location)
        {
            return Json(new
            {
                Status = 200,
                Data = this._locationService.GetSuggestLocation(location)
            });
        }
    }
}