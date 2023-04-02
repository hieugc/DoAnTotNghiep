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

namespace DoAnTotNghiep.Controllers
{
    [Authorize(Roles = Role.Admin)]
    public class AdminController: BaseController
    {
        private readonly DoAnTotNghiepContext _context;
        private readonly IConfiguration _configuration;

        public AdminController(DoAnTotNghiepContext context, 
                                    IConfiguration configuration,
                                    IHostEnvironment environment): base(environment)
        {
            _context = context;
            _configuration = configuration;
        }

        //Dash board
        //Danh sách report người dùng 
        //hiển thị số người báo cáo thôi => xem chi tiết load thêm
        [HttpGet("/Admin")]    
        public IActionResult UserReport()
        {
            var houses = this._context.Houses
                                        .Include(m => m.UserReports)
                                        .Where(m => m.UserReports != null 
                                            && m.UserReports.Any(rp => rp.IsResponsed == false))
                                        .ToList();
            List<ReportItem> reportList = new List<ReportItem>();

            string host = this.GetWebsitePath();
            byte[] salt = Crypto.Salt(this._configuration);
            //House house, byte[] salt, User? user = null, string? host = null

            foreach (var item in houses)
            {
                if (item.UserReports != null)
                {
                    this._context.Entry(item).Reference(m => m.Users).Load();
                    //this._context.Entry(item).Collection(m => m.RulesInHouses).Query().Load();
                    //this._context.Entry(item).Collection(m => m.UtilitiesInHouses).Query().Load();
                    //this._context.Entry(item).Reference(m => m.Citys).Query().Load();
                    //this._context.Entry(item).Reference(m => m.Districts).Query().Load();
                    //this._context.Entry(item).Reference(m => m.Wards).Query().Load();
                    //this._context.Entry(item).Collection(m => m.Requests).Query().Load();
                    if(item.Users != null)
                    {
                        this._context.Entry(item.Users).Reference(m => m.Houses).Load();
                    }
                    this._context.Entry(item).Collection(m => m.FileOfHouses).Query().Load();
                    DetailHouseViewModel detailHouse = new DetailHouseViewModel(item, salt, item.Users, host);
                    reportList.Add(new ReportItem() { 
                        House = detailHouse, 
                        NumberReport = item.UserReports.Count() 
                    });
                }
            }

            return View(reportList);
        }


        //Danh sách người dùng
        //Danh sách report đang chờ phản hồi

        //Danh sách nhà chờ duyệt
        //Danh sách tiện ích
        //Danh sách rule
        //Cập nhật danh sách địa chỉ
    }
}
