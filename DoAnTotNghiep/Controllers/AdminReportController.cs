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
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static System.Net.WebRequestMethods;
using System.Composition;
using Microsoft.Extensions.Hosting;

namespace DoAnTotNghiep.Controllers
{
    [Authorize(Roles = Role.Admin)]
    public class AdminReportController : BaseController
    {
        private readonly DoAnTotNghiepContext _context;
        private readonly IConfiguration _configuration;

        public AdminReportController(DoAnTotNghiepContext context, IConfiguration configuration, IHostEnvironment environment) : base(environment)
        {
            _context = context;
            _configuration = configuration;
        }

        //create
        //HINH ANH + TIEU DE + ID NHA + NOI DUNG
        [HttpPost("/AdminReport/Create")]
        public IActionResult CreateReport([FromBody] AdminReportViewModel model)
        {
            return this.Create(model);
        }
        [HttpPost("/api/AdminReport/Create")]
        public IActionResult ApiCreateReport([FromBody] AdminReportViewModel model)
        {
            return this.Create(model);
        }
        private IActionResult Create(AdminReportViewModel model)
        {
            if (ModelState.IsValid)
            {
                int IdUser = this.GetIdUser();
                using (var context = _context)
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            AdminReport user = new AdminReport()
                            {
                                Content = model.Content,
                                IdHouse = model.IdHouse,
                                IdUser = IdUser,
                                DeadlineDate = model.Deadline,
                                Status = 0
                            };

                            context.AdminReports.Add(user);
                            context.SaveChanges();

                            List<Entity.File> files = new List<Entity.File>();
                            foreach (var item in model.Images)
                            {
                                Entity.File? file = this.SaveFile(item);
                                if (file != null)
                                {
                                    files.Add(file);
                                }
                            }

                            context.Files.AddRange(files);
                            context.SaveChanges();

                            List<FileInAdminReport> reportFiles = new List<FileInAdminReport>();

                            foreach (var item in files)
                            {
                                FileInAdminReport fileInUser = new FileInAdminReport()
                                {
                                    IdFile = item.Id,
                                    IdAdminReport = user.Id
                                };
                            }
                            context.FileInAdminReports.AddRange(reportFiles);
                            context.SaveChanges();
                            transaction.Commit();

                            return Json(new
                            {
                                Status = 200,
                                Message = "ok"
                            });
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                            transaction.Rollback();
                        }
                    }
                }
            }

            return BadRequest(new
            {
                Status = 400,
                Message = "Khởi tạo không thành công"
            });
        }

        //deadline chạy nền
        //update status của nhà không?
        // => valid => spending
        // => swaping => request.status == accept
                            // => kiểm tra tiền người dùng?
                            // => 

        //Update status
        private IActionResult UpdateStatus(int IdAdminReport, int Status)
        {
            var rp = this._context.AdminReports.Where(m => m.Id == IdAdminReport).FirstOrDefault();
            if(rp == null)
            {
                return BadRequest(new
                {
                    Status = 400,
                    Message = "Không tìm thấy báo cáo"
                });
            }
            rp.Status = Status;
            this._context.AdminReports.Update(rp);
            this._context.SaveChanges();
            return Json(new
            {
                Status = 200,
                Message = "ok"
            });
        }


        //get by HouseId
    }
}
