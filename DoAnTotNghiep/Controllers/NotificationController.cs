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

namespace DoAnTotNghiep.Controllers
{
    [Authorize(Roles = Enum.Role.Member)]
    public class NotificationController : BaseController
    {
        private readonly DoAnTotNghiepContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHubContext<ChatHub> _signalContext;

        public NotificationController(DoAnTotNghiepContext context, 
            IConfiguration configuration,
            IHostEnvironment environment,
            IHubContext<ChatHub> signalContext) : base(environment)
        {
            _context = context;
            _configuration = configuration;
            _signalContext = signalContext;
        }
        
        public IActionResult Index()
        {
            int IdUser = this.GetIdUser();
            string host = this.GetWebsitePath();
            var model = this._context.Notifications
                                    .OrderByDescending(m => m.CreatedDate)
                                    .Where(m => m.IdUser == IdUser)
                                    .Select(m => new NotificationViewModel(m, host))
                                    .ToList();
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
            var model = this._context.Notifications
                                        .OrderByDescending(m => m.CreatedDate)
                                        .Where(m => m.IdUser == IdUser)
                                        .Select(m => new NotificationViewModel(m, host))
                                        .ToList();
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
            int IdUser = this.GetIdUser();
            var model = this._context.Notifications
                                        .Where(m => m.IdUser == IdUser)
                                        .ToList();
            try
            {
                foreach (var item in model) item.IsSeen = true;
                this._context.Notifications.UpdateRange(model);
                this._context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

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
                int IdUser = this.GetIdUser();
                var model = this._context.Notifications
                                            .Where(m => m.Id == Id && m.IdUser == IdUser)
                                            .FirstOrDefault();
                if(model != null)
                {
                    try
                    {
                        model.IsSeen = true;
                        this._context.Notifications.Update(model);
                        this._context.SaveChanges();

                        return Json(new
                        {
                            Status = 200,
                            Message = "Cập nhật thành công"
                        });
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }
            return BadRequest(new
            {
                Status = 400,
                Message = "Không thể cập nhật"
            });
        }

        [HttpGet("/api/Notification/Demo")]
        public async Task<IActionResult> ApiDemoNotificationsAsync(Pagination pagination)
        {
            int IdUser = this.GetIdUser();
            var user = this._context.Users.Where(m => m.Id != IdUser).ToList();
            List<Notification> list = new List<Notification>();
            string host = this.GetWebsitePath();
            for(int index = 0; index < user.Count(); index++)
            {
                Notification node = new Notification().DemoNotification(user[index].Id);
                node.Content = "api/Notification/Demo";
                list.Add(node);
            }
            this._context.Notifications.AddRange(list);
            this._context.SaveChanges();

            ChatHub chatHub = new ChatHub(this._signalContext);
            foreach(var item in list)
            {
                await chatHub.SendNotification(
                                group: Crypto.EncodeKey(item.IdUser.ToString(), Crypto.Salt(this._configuration)),
                                target: TargetSignalR.Notification(),
                                model: new NotificationViewModel(item, this.GetWebsitePath()));
            }

            return Json(new
            {
                Status = 200,
                Data = list
            });
        }
    }
}
