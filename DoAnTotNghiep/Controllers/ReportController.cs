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
using DoAnTotNghiep.Service;
using Microsoft.AspNetCore.Routing;

namespace DoAnTotNghiep.Controllers
{
    public class ReportController: BaseController
    {
        private readonly DoAnTotNghiepContext _context;
        private readonly IConfiguration _configuration;
        private readonly IReportService _reportService;
        private readonly IFileService _fileService;
        private readonly IHouseService _houseService;
        private readonly IUserService _userService;

        public ReportController(DoAnTotNghiepContext context, 
                                IConfiguration configuration, 
                                IHostEnvironment environment,
                                IReportService reportService,
                                IFileService fileService,
                                IHouseService houseService,
                                IUserService userService) : base(environment)
        {
            _context = context;
            _configuration = configuration;
            _reportService = reportService;
            _fileService = fileService;
            _houseService = houseService;
            _userService = userService;
        }

        [HttpGet("/Report/Form")]
        [Authorize(Roles = Role.Member)]
        public IActionResult FormReport(string UserAccess)
        {
            int IdUser = 0;
            if(int.TryParse(Crypto.DecodeKey(UserAccess, Crypto.Salt(this._configuration)), out IdUser)){
                var user = this._context.Users.Where(m => m.Id == IdUser).FirstOrDefault();
                if(user != null)
                {
                    ViewData["name"] = user.FirstName + " " + user.LastName;
                    return PartialView("~/Views/PartialView/_FormReport.cshtml", UserAccess);
                }
            }
            return NotFound();
        }


        //create
        //HINH ANH + TIEU DE + ID NHA + NOI DUNG
        [HttpPost("/Report/Create")]
        [Authorize(Roles = Role.Member)]
        public IActionResult CreateReport([FromBody] ReportViewModel model)
        {
            int IdUser = 0;
            if(model.UserAccess != null)
            {
                try
                {
                    if(int.TryParse(Crypto.DecodeKey(model.UserAccess, Crypto.Salt(this._configuration)), out IdUser))
                    {
                        return this.Create(new CreateReportViewModel()
                        {
                            Content = model.Content,
                            IdHouse = null,
                            IdUser = IdUser,
                            Images = model.Images,
                            Files = null
                        });
                    }
                }
                catch
                {

                }
            }
            return BadRequest(new
            {
                Status = 400,
                Message = "Không tìm thấy User"
            });
            
        }

        [HttpPost("/api/Report/Create")]
        [Authorize(Roles = Role.Member)]
        public IActionResult ApiCreateReport(MobileReportViewModel model)
        {
            int IdUser = 0;
            if (model.UserAccess != null)
            {
                try
                {
                    if(int.TryParse(Crypto.DecodeKey(model.UserAccess, Crypto.Salt(this._configuration)), out IdUser))
                    {
                        return this.Create(new CreateReportViewModel()
                        {
                            Content = model.Content,
                            IdHouse = null,
                            IdUser = IdUser,
                            Images = new List<ImageBase>(),
                            Files = model.Files
                        });
                    }
                }
                catch
                {

                }
            }

            return BadRequest(new
            {
                Status = 400,
                Message = "Không tìm thấy User"
            });
        }
        private IActionResult Create(CreateReportViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.IdHouse != null || model.IdUser != null)
                {
                    if (model.IdUser == null && model.IdHouse != null)
                    {
                        var house = this._houseService.GetHouseByIdStatus(model.IdHouse.Value, (int)StatusHouse.VALID);
                        if (house != null) model.IdUser = house.IdUser;
                    }
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
                                    IdUser = IdUser,
                                    IdUserReport = model.IdUser,
                                    IsResponsed = false
                                };
                                this._reportService.Save(user);

                                List<Entity.File> files = new List<Entity.File>();

                                if (model.Files == null)
                                {
                                    files.AddRange(this._fileService.AddFile(model.Images, this.environment.ContentRootPath));
                                }
                                else
                                {
                                    files.AddRange(this._fileService.AddFile(model.Files, this.environment.ContentRootPath));
                                }

                                this._fileService.SaveRangeFileOfReport(user, this._fileService.SaveRangeFile(files));
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
            }

            return BadRequest(new
            {
                Status = 400,
                Message = "Khởi tạo không thành công"
            });
        }


        //Get report by houseId
        [HttpGet("/Report/GetByHouse")]
        [Authorize(Roles = Role.Admin)]
        public IActionResult GetByHouse(int houseId)
        {
            var rp = this._reportService.GetByHouse(houseId);
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
                    report.Users.InCludeAll(this._context);
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

        [HttpGet("/Report/GetByUser")]
        [Authorize(Roles = Role.Admin)]
        public IActionResult GetByUser(int userId)
        {
            var rp = this._reportService.GetByUser(userId);
            List<DetailReportViewModel> model = new List<DetailReportViewModel>();
            string host = this.GetWebsitePath();
            byte[] salt = Crypto.Salt(this._configuration);
            foreach (var report in rp)
            {
                if (report.Users != null)
                {
                    List<ImageBase> images = new List<ImageBase>();
                    if (report.Files != null)
                    {
                        foreach (var file in report.Files)
                        {
                            this._context.Entry(file).Reference(m => m.Files).Load();
                            if (file.Files != null)
                            {
                                images.Add(new ImageBase(file.Files, host));
                            }
                        }
                    }
                    report.Users.InCludeAll(this._context);
                    model.Add(new DetailReportViewModel()
                    {
                        Content = report.Content,
                        User = new UserMessageViewModel(report.Users, salt, host),
                        Images = images
                    });
                }
            }

            return PartialView("~/Views/Users/_Report.cshtml", model);
        }

        [HttpGet("/Report/GetNumberByUser")]
        [Authorize(Roles = Role.Admin)]
        public IActionResult GetNumberByUser(int userId)
        {
            var rp = this._reportService.GetByUser(userId);
            return Json(new
            {
                Status = 200,
                Data = rp.Count()
            });
        }
    }
}
