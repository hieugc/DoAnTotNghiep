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
using DoAnTotNghiep.TrainModels;
using NuGet.Protocol;
using DoAnTotNghiep.Job;
using DoAnTotNghiep.Service;

namespace DoAnTotNghiep.Controllers
{
    public class HomeController : BaseController
    {
        //xong
        private readonly DoAnTotNghiepContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILocationService _locationService;
        private readonly IFileService _fileService;
        private readonly IHouseService _houseService;
        private readonly IRequestService _requestService;
        private readonly IFeedBackService _feedBackService;

        public HomeController(  DoAnTotNghiepContext context, 
                                IConfiguration _configuration,
                                IHostEnvironment environment,
                                ILocationService locationService,
                                IHouseService houseService,
                                IFileService fileService,
                                IRequestService requestService,
                                IFeedBackService feedBackService) : base(environment)
        {
            this._context = context;
            this._configuration = _configuration;
            this._locationService = locationService;
            this._houseService = houseService;
            this._fileService = fileService;
            this._requestService = requestService;
            this._feedBackService = feedBackService;
        }

        public IActionResult Index()
        {
            int IdUser = this.GetIdUser();
            byte[] salt = Crypto.Salt(this._configuration);
            string host = this.GetWebsitePath();

            List<DetailHouseViewModel> houses = this.GetPopularHouse(salt, host);
            List<PopularCityViewModel> cities = this._locationService.GetPopularCity(host, 4);

            if (IdUser != 0)
            {
                this.SetViewData(new DoAnTotNghiepContext(this._context.GetConfig()), IdUser, salt);
            }
            return View(new HomeViewModel()
            {
                PopularCities = cities,
                PopularHouses = houses,
                NumberCities = this._locationService.NumberCity(),
                NumberHouses = this._houseService.NumberHouse(),
                NewRequests = this._requestService.GetValidRequestByUser(
                                    this._requestService.GetValidRequestByUser(IdUser),
                                    this._fileService, this._houseService, this._feedBackService, null, salt, host, IdUser)
            });
        }
        public IActionResult HomeGetHousePopular()
        {
            return PartialView("~/Views/Home/_Frame_2.cshtml", this.GetPopularHouse(Crypto.Salt(this._configuration), this.GetWebsitePath()));
        }
        public IActionResult HomeGetCityPopular()
        {
            return PartialView("~/Views/Home/_Frame_1.cshtml", this._locationService.GetPopularCity(this.GetWebsitePath(), 4));
        }
        private List<DetailHouseViewModel> GetPopularHouse(byte[] salt, string host, int number = 10)
        {
            var listHouse = this._houseService.GetListPopularHouse(number);
            return this._houseService.GetListDetailHouseWithManyImages(listHouse, this._fileService, host, salt);
        }
    }
}