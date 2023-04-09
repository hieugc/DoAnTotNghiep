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
using Azure.Core;
using DoAnTotNghiep.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace DoAnTotNghiep.Controllers
{
    [Authorize(Roles = Role.Admin)]
    public class AdminReportController : BaseController
    {
        private readonly DoAnTotNghiepContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHubContext<ChatHub> _signalContext;

        public AdminReportController(DoAnTotNghiepContext context, 
                                IConfiguration configuration, 
                                IHostEnvironment environment,
                                IHubContext<ChatHub> signalContext) : base(environment)
        {
            _context = context;
            _configuration = configuration;
            _signalContext = signalContext;
        }

        //[HttpGet("/Admin/Report")]
        //public IActionResult Index()
        //{
        //    var report = this._context.UserReports
        //                                .OrderBy(m => m.IdHouse)
        //                                .Include(m => m.Houses)
        //                                .Where(m => m.IsResponsed == false)
        //                                .ToList();
        //    return View();
        //}

        //create
        //HINH ANH + TIEU DE + ID NHA + NOI DUNG
        [HttpPost("/AdminReport/Create")]
        public async Task<IActionResult> CreateReportAsync([FromBody] AdminReportViewModel model)
        {
            return await this.CreateAsync(model);
        }
        [HttpPost("/api/AdminReport/Create")]
        public async Task<IActionResult> ApiCreateReportAsync([FromBody] AdminReportViewModel model)
        {
            return await this.CreateAsync(model);
        }
        private async Task<IActionResult> CreateAsync(AdminReportViewModel model)
        {
            if (ModelState.IsValid)
            {
                int IdUser = this.GetIdUser();
                using (var context = _context)
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        var house = context.Houses
                                        .Where(m => m.Id == model.IdHouse).FirstOrDefault();
                        if(house!= null)
                        {
                            try
                            {
                                //nếu nhà không có yêu cầu => pending
                                house.Status = (int)StatusHouse.PENDING;
                                context.Houses.Update(house);
                                context.SaveChanges();
                                //nếu nhà đang đổi => ?

                                AdminReport user = new AdminReport()
                                {
                                    Content = model.Content,
                                    IdHouse = model.IdHouse,
                                    IdUser = house.IdUser,
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


                                //gửi thông báo
                                Notification notification = new Notification()
                                {
                                    IdType = user.Id,
                                    IdUser = user.IdUser,
                                    Title = NotificationType.AdminReportTitle,
                                    Content = "Bạn nhận được phản ánh từ quản trị viên",
                                    CreatedDate = DateTime.Now,
                                    IsSeen = false,
                                    ImageUrl = "/Image/house-demo.png",
                                    Type = NotificationType.ADMIN_REPORT
                                };
                                this._context.Notifications.Add(notification);
                                this._context.SaveChanges();

                                ChatHub chatHub = new ChatHub(this._signalContext);
                                await chatHub.SendNotification(
                                    group: Crypto.EncodeKey(notification.IdUser.ToString(), Crypto.Salt(this._configuration)),
                                    target: TargetSignalR.Notification(),
                                    model: new NotificationViewModel(notification, this.GetWebsitePath()));

                                transaction.Commit();

                                //timer chạy nền

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

        //Update status
        private IActionResult UpdateStatus(int IdAdminReport, int Status)
        {
            var rp = this._context.AdminReports
                                .Include(m => m.Houses)
                                .Where(m => m.Id == IdAdminReport).FirstOrDefault();
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

            //update user report
            return Json(new
            {
                Status = 200,
                Message = "ok"
            });
        }

        //get by HouseId
        private IActionResult GetAdminReport(int id)
        {
            int IdUser = this.GetIdUser();
            var rp = this._context.AdminReports
                                .Include(m => m.Files)
                                .Where(m => m.Id == id)
                                .FirstOrDefault();
            if(rp == null ) return Json(new
            {
                Status = 200,
                Data = new { }
            });
            DetailAdminReportViewModel model = new DetailAdminReportViewModel()
            {
                Content = rp.Content,
                Images = new List<ImageBase>()
            };

            if (rp.Files != null)
            {
                string host = this.GetWebsitePath();
                foreach (var item in rp.Files)
                {
                    this._context.Entry(item).Reference(m => m.Files).Load();
                    if(item.Files != null)
                    {
                        model.Images.Add(new ImageBase(item.Files, host));
                    }
                }
            }


            return Json(new
            {
                Status = 200,
                Message = model
            });
        }
    }
}
