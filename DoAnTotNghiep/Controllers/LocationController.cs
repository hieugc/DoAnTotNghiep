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
    public class LocationController : BaseController
    {
        private readonly DoAnTotNghiepContext _context;

        public LocationController(DoAnTotNghiepContext context,
                                    IHostEnvironment environment) : base(environment)
        {
            _context = context;
        }


        //Viết thêm api lấy City+district+ward => tên đường số nhà tự nhập
        //từ địa chỉ nhà + idCity+ iddistrict + ward => bing map => lat lng

        //mobile => location: => ghép sẳn [tên đường số nhà tự nhập] City+district+ward

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

        [HttpGet]
        public JsonResult GetCityId(string BingName)
        {
            var city = this._context.Cities.Where(m => m.BingName.Contains(BingName)).FirstOrDefault();
            if (city == null)
            {
                return Json(new
                {
                    Status = 400,
                    Message = "Không tìm thấy " + BingName
                });
            }
            return Json(new
            {
                Status = 200,
                Message = "Dữ liệu đã gửi: " + BingName,
                IdCity = city.Id
            });
        }

        [HttpGet("/api/GetPopularCity")]
        public JsonResult GetPopularCity(int number = 10)
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
                    Status = 200,
                    Message = "Get Successfully",
                    Data = cityList
                }
            );
        }


        [HttpGet("/api/Location/City")]
        public JsonResult GetCity()
        {
            var cityList = this._context.Cities.Select(m => new
            {
                Id = m.Id,
                Name = m.Name,
                BingName = m.BingName
            }).ToList();

            return Json(
                new
                {
                    Data = cityList
                }
            );
        }


        [HttpGet("/api/Location/District")]
        public JsonResult GetDistrict(int IdCity)
        {
            var districtList = this._context.Districts.Where(m => m.IdCity == IdCity).Select(m => new
            {
                Name = m.Name,
                Id = m.Id,
                BingName = m.BingName
            }).ToList();

            return Json(
                new
                {
                    Data = districtList
                }
            );
        }

        [HttpGet("/api/Location/Ward")]
        public JsonResult GetWard(int IdDistrict)
        {
            var wardList = this._context.Wards.Where(m => m.IdDistrict == IdDistrict).Select(m => new
            {
                Name = m.Name,
                Id = m.Id,
                BingName = m.BingName
            }).ToList();

            return Json(
                new
                {
                    Data = wardList
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
