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

namespace DoAnTotNghiep.Controllers
{
    public class LocationController : Controller
    {
        private readonly DoAnTotNghiepContext _context;

        public LocationController(DoAnTotNghiepContext context)
        {
            _context = context;
        }

        [HttpGet]
        public JsonResult Get()
        {
            List<City> cityList = this._context.Cities.ToList();
            List<District> districtList = this._context.Districts.ToList();
            List<Ward> wardList = this._context.Wards.ToList();

            return Json(
                new LocationViewModel()
                {
                    Cities = cityList,
                    Districts = districtList,
                    Wards = wardList
                }
            );
        }


        [HttpGet]
        public JsonResult GetCity()
        {
            var cityList = this._context.Cities.Select(m => new
            {
                Id = m.Id,
                Name = m.Name
            }).ToList();

            return Json(
                new
                {
                    City = cityList
                }
            );
        }


        [HttpGet]
        public JsonResult GetDistrict(int Id)
        {
            var districtList = this._context.Districts.Where(m => m.IdCity == Id).Select(m => new
            {
                Name = m.Name,
                Id = m.Id
            }).ToList();

            return Json(
                new
                {
                    District = districtList
                }
            );
        }

        [HttpGet]
        public JsonResult GetWard(int Id)
        {
            var wardList = this._context.Wards.Where(m => m.IdDistrict == Id).Select(m => new
            {
                Name = m.Name,
                Id = m.Id
            }).ToList();

            return Json(
                new
                {
                    Ward = wardList
                }
            );
        }
    }
}
