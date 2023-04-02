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
    [Authorize(Roles = Role.Member)]
    public class ResponseController: BaseController
    {
        private readonly DoAnTotNghiepContext _context;
        private readonly IConfiguration _configuration;

        public ResponseController(DoAnTotNghiepContext context, IConfiguration configuration, IHostEnvironment environment) : base(environment)
        {
            _context = context;
            _configuration = configuration;
        }

        //create
        //HINH ANH + TIEU DE + ID NHA + NOI DUNG
        [HttpPost("/Response/Create")]
        public IActionResult CreateResponse([FromBody] ReportViewModel model)
        {
            return this.Create(model);
        }
        [HttpPost("/api/Response/Create")]
        public IActionResult ApiCreateResponse([FromBody] ReportViewModel model)
        {
            return this.Create(model);
        }
        private IActionResult Create(ReportViewModel model)
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
                            UserReport user = new UserReport()
                            {
                                Content = model.Content,
                                CreatedDate = DateTime.Now,
                                IdHouse = model.IdHouse,
                                IdUser = IdUser
                            };
                            context.UserReports.Add(user);
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

                            List<FileInUserReport> reportFiles = new List<FileInUserReport>();

                            foreach (var item in files)
                            {
                                FileInUserReport fileInUser = new FileInUserReport()
                                {
                                    IdFile = item.Id,
                                    IdUserReport = user.Id
                                };
                            }
                            context.FileInUserReports.AddRange(reportFiles);
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


        //Get report by houseId
        [HttpGet("/Response/GetByAdminReport")]
        [Authorize(Roles = Role.Admin)]
        public IActionResult GetReportByHouse(int houseId)
        {
            var rp = this._context.UserReports
                                    .Include(m => m.Users)
                                    .Include(m => m.Files)
                                    .Where(m => m.IdHouse == houseId).ToList();
            List<DetailReportViewModel> model = new List<DetailReportViewModel>();
            string host = this.GetWebsitePath();
            byte[] salt = Crypto.Salt(this._configuration);
            foreach(var report in rp)
            {
                if(report.Users != null)
                {
                    List<ImageBase> images = new List<ImageBase>();
                    if (report.Files != null)
                    {
                        foreach (var file in report.Files)
                        {
                            this._context.Entry(file).Reference(m => m.Files).Load();
                            if(file.Files != null)
                            {
                                images.Add(new ImageBase(file.Files, host));
                            }
                        }
                    }
                    model.Add(new DetailReportViewModel()
                    {
                        Content = report.Content,
                        User = new UserMessageViewModel(report.Users, salt, host),
                        Images = images
                    });
                }
            }

            return Json(new
            {
                Status = 200,
                Data = model
            });
        }
    }
}
