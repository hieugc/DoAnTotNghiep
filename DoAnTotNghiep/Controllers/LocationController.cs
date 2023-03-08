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
            List<District> districtList = this._context.Districts.Include(m => m.Citys).ToList();
            List<Ward> wardList = this._context.Wards.Include(m => m.Districts).ToList();

            return Json(
                new LocationViewModel()
                {
                    Cities = cityList,
                    Districts = districtList,
                    Wards = wardList
                }
            );
        }

        [HttpGet("/api/GetPopularCity")]
        public JsonResult GetPopularCity(int number = 6)
        {
            string host = this.HttpContext.Request.Host.Value;
            List<PopularCityViewModel> cityList = this._context.Cities
                                               .OrderBy(m => m.Count)
                                               .Take(number)
                                               .Select(m => new PopularCityViewModel()
                                               {
                                                   Name = m.Name,
                                                   Id = m.Id,
                                                   ImageUrl = host + "/Image/logo.png",
                                                   IsDeleted = false,
                                                   Location = new PointViewModel()
                                                   {
                                                       Lat = m.Lat,
                                                       Lng = m.Lng
                                                   }

                                               })
                                               .ToList();

            return Json(
                new 
                {
                    StatusCode = 200,
                    Message = "Get Successfully",
                    Data = cityList
                }
            );
        }


        [HttpGet]
        public JsonResult GetCity()
        {
            var cityList = this._context.Cities.Select(m => new
            {
                Id = m.Id,
                Name = m.Name,
                IsUpdated = (m.Lat == 0 && string.IsNullOrEmpty(m.BingName))
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
                Id = m.Id,
                IsUpdated = (m.Lat == 0 && string.IsNullOrEmpty(m.BingName))
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
                Id = m.Id,
                IsUpdated = !(m.Lat == 0 && string.IsNullOrEmpty(m.BingName))
            }).ToList();

            return Json(
                new
                {
                    Ward = wardList
                }
            );
        }


        [HttpPut("/City/Update")]
        public JsonResult UpdateCity([FromBody] BingMapViewModel data)
        {
            var city = this._context.Cities.FirstOrDefault(m => m.Id == data.Id);
            if(city == null)
            {
                return Json(new
                {
                    Status = 404,
                    Message = "Không tìm thấy"
                });
            }

            city.BingName = data.BingName;
            city.Lng = data.Lng;
            city.Lat = data.Lat;

            this._context.Cities.Update(city);
            this._context.SaveChanges();

            return Json(
                new
                {
                    Status = 200,
                    Message = "Cập nhật thành công"
                }
            );
        }


        [HttpPut("/District/Update")]
        public JsonResult UpdateDistrict([FromBody] BingMapViewModel data)
        {
            var district = this._context.Districts.FirstOrDefault(m => m.Id == data.Id);
            if (district == null)
            {
                return Json(new
                {
                    Status = 404,
                    Message = "Không tìm thấy"
                });
            }

            district.BingName = data.BingName;
            district.Lng = data.Lng;
            district.Lat = data.Lat;

            this._context.Districts.Update(district);
            this._context.SaveChanges();

            return Json(
                new
                {
                    Status = 200,
                    Message = "Cập nhật thành công"
                }
            );
        }

        [HttpPut("/Ward/Update")]
        public JsonResult UpdateWard([FromBody] BingMapViewModel data)
        {
            var ward = this._context.Wards.FirstOrDefault(m => m.Id == data.Id);
            if (ward == null)
            {
                return Json(new
                {
                    Status = 404,
                    Message = "Không tìm thấy"
                });
            }

            ward.BingName = data.BingName;
            ward.Lng = data.Lng;
            ward.Lat = data.Lat;

            this._context.Wards.Update(ward);
            this._context.SaveChanges();

            return Json(
                new
                {
                    Status = 200,
                    Message = "Cập nhật thành công"
                }
            );
        }
    }
}
