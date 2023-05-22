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
using Microsoft.Extensions.Hosting;
using System.Text;
using Newtonsoft.Json;
using NuGet.Packaging;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.AspNetCore.Routing;
using System.Security.Cryptography.X509Certificates;
using DoAnTotNghiep.Hubs;
using Microsoft.AspNetCore.SignalR;
using DoAnTotNghiep.Service;

namespace DoAnTotNghiep.Controllers
{
    [Authorize(Roles = Enum.Role.Member)]
    public class NotificationController : BaseController
    {
        private readonly DoAnTotNghiepContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHubContext<ChatHub> _signalContext;
        private readonly IUserService _userService;
        private readonly INotificationService _notificationService;

        public NotificationController(
                                        DoAnTotNghiepContext context, 
                                        IConfiguration configuration,
                                        IHostEnvironment environment,
                                        IHubContext<ChatHub> signalContext,
                                        IUserService userService,
                                        INotificationService notificationService) : base(environment)
        {
            _context = context;
            _configuration = configuration;
            _signalContext = signalContext;
            _userService = userService;
            _notificationService = notificationService;
        }
        
        public IActionResult Index()
        {
            int IdUser = this.GetIdUser();
            string host = this.GetWebsitePath();
            var model = this._notificationService.GetByUser(IdUser, host);
            if (IdUser != 0)
            {
                this.SetViewData(new DoAnTotNghiepContext(this._context.GetConfig()), IdUser, Crypto.Salt(this._configuration));
            }
            return View(model);
        }

        [HttpGet("/Notification/Get")]
        public IActionResult GetNotifications(Pagination pagination)
        {
            return this.Get(pagination);
        }
        [HttpGet("/api/Notification/Get")]
        public IActionResult ApiGetNotifications(Pagination pagination)
        {
            return this.Get(pagination);
        }
        private IActionResult Get(Pagination pagination)
        {
            int IdUser = this.GetIdUser();
            int skip = (pagination.Page - 1 < 0 ? 0 : pagination.Page - 1) * pagination.Limit;
            string host = this.GetWebsitePath();
            var model = this._notificationService.GetByUser(IdUser, host);
            pagination.Page += 1;
            pagination.Total = (int) Math.Ceiling((double)(model.Count() / pagination.Limit));

            return Json(new
            {
                Status = 200,
                Data = new
                {
                    Model = model.Skip(skip).Take(pagination.Limit),
                    Meta = pagination
                }
            });
        }

        [HttpPost("/Notification/Seen")]
        public IActionResult UpdateSeen([FromBody] int Id)
        {
            return this.SeenNotification(Id);
        }

        [HttpPost("/api/Notification/Seen")]
        public IActionResult ApiUpdateSeen([FromBody] int Id)
        {
            return this.SeenNotification(Id);
        }


        [HttpPost("/Notification/SeenAll")]
        public IActionResult WebSeenAll()
        {
            return this.SeenAll();
        }

        [HttpPost("/api/Notification/SeenAll")]
        public IActionResult ApiSeenAll()
        {
            return this.SeenAll();
        }
        private IActionResult SeenAll()
        {
            this._notificationService.SeenAll(this.GetIdUser());
            return Json(new
            {
                Status = 200,
                Message = "Cập nhật thành công"
            });
        }

        private IActionResult SeenNotification(int Id = 0)
        {
            if (ModelState.IsValid)
            {
                this._notificationService.Seen(this.GetIdUser(), Id);
                return Json(new
                {
                    Status = 200,
                    Message = "Cập nhật thành công"
                });
            }
            return BadRequest(new
            {
                Status = 400,
                Message = "Không thể cập nhật"
            });
        }
    }
}
