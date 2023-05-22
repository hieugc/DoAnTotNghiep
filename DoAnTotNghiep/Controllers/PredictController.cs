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
using DoAnTotNghiep.Modules;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.Security.Claims;
using DoAnTotNghiep.Enum;
using NuGet.Protocol;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using NuGet.Common;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static System.Net.WebRequestMethods;
using System.Composition;
using DoAnTotNghiep.TrainModels;
using DoAnTotNghiep.Service;

namespace DoAnTotNghiep.Controllers
{
    public class PredictController: BaseController
    {
        private readonly DoAnTotNghiepContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHouseService _houseService;
        private readonly ILocationService _locationService;
        public PredictController(DoAnTotNghiepContext context, 
                                    IConfiguration configuration,
                                    IHostEnvironment environment,
                                    IHouseService houseService,
                                    ILocationService locationService) : base(environment)
        {
            _context = context;
            _configuration = configuration;
            _houseService = houseService;
            _locationService = locationService;
        }
        /*
         * [HttpGet("/Predict")]
        [AllowAnonymous]
        public IActionResult Get(NewModelTrainInput input)
        {
            try
            {
                return Json(new
                {
                    Status = 200,
                    Data = Math.Ceiling(new PredictHouse().GetPredict(input) / 1000)
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return Json(new
            {
                Status = 200,
                Data = 0
            });
        }
         */
        [HttpGet("/Predict")]
        [AllowAnonymous]
        public IActionResult Get(int idCity, double lat, double lng, int capacity, double area, int rating)
        {
            var input = this._locationService.GetPredictByCity(idCity, area, capacity, rating, lat, lng);
            if(input != null)
            {
                try
                {
                    return Json(new
                    {
                        Status = 200,
                        Data = Math.Ceiling(new PredictHouse().GetPredict(input) / 1000)
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            return Json(new
            {
                Status = 200,
                Data = 0
            });
        }
        [HttpGet("/PredictByHouse")]
        [AllowAnonymous]
        public IActionResult GetByHouse(int Id)
        {
            var model = this._houseService.GetPredict(Id);
            if(model != null)
            {
                try
                {
                    return Json(new
                    {
                        Status = 200,
                        Data = Math.Ceiling(new PredictHouse().GetPredict(model) / 1000)
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            return Json(new
            {
                Status = 200,
                Data = 0
            });
        }
    }
}
