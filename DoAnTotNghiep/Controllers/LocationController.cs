using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DoAnTotNghiep.Data;
using DoAnTotNghiep.Entity;
using DoAnTotNghiep.ViewModels;
using Microsoft.AspNetCore.Authorization;
using DoAnTotNghiep.Enum;
using DoAnTotNghiep.Service;

namespace DoAnTotNghiep.Controllers
{
    public class LocationController : BaseController
    {
        private readonly DoAnTotNghiepContext _context;
        private readonly ILocationService _locationService;

        public LocationController(DoAnTotNghiepContext context,
                                    IHostEnvironment environment,
                                    ILocationService locationService) : base(environment)
        {
            _context = context;
            _locationService = locationService;
        }


        [HttpGet("/api/GetPopularCity")]
        public JsonResult GetPopularCity(int number = 10)
        {
            return Json(new {Status = 200,Message = "Get Successfully",Data = this._locationService.GetPopularCity(this.GetWebsitePath(), number)});
        }


        [HttpGet("/api/Location/City")]
        public JsonResult GetCity()
        {
            return Json(new{Status = 200,Data = this._locationService.GetBingViewModel(1, null)});
        }

        [HttpGet("/api/Location/CityWithPoint")]
        public JsonResult GetCityWithPoint()
        {
            return Json(new { Status = 200, Data = this._locationService.GetDistrictViewModel(1, null) });
        }

        [HttpGet("/api/Location/District")]
        public JsonResult GetDistrict(int IdCity)
        {
            return Json(new{Status = 200,Data = this._locationService.GetBingViewModel(2, IdCity)});
        }

        [HttpGet("/api/Location/DistrictWithPoint")]
        public JsonResult GetDistrictPoint(int IdCity)
        {
            return Json(new{ Status = 200, Data = this._locationService.GetDistrictViewModel(2, IdCity)});
        }

        [HttpGet("/api/Location/Ward")]
        public JsonResult GetWard(int IdDistrict)
        {
            return Json(new {Status = 200,Data = this._locationService.GetBingViewModel(3, IdDistrict)});
        }
        [HttpGet("/api/Location/WardWithPoint")]
        public JsonResult GetWardWithPoint(int IdDistrict)
        {
            return Json(new { Status = 200, Data = this._locationService.GetDistrictViewModel(3, IdDistrict) });
        }
    }
}
