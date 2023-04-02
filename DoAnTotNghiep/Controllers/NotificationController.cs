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

namespace DoAnTotNghiep.Controllers
{
    [Authorize(Roles = Enum.Role.Member)]
    public class NotificationController : BaseController
    {
        private readonly DoAnTotNghiepContext _context;
        private readonly IConfiguration _configuration;

        public NotificationController(DoAnTotNghiepContext context, 
            IConfiguration configuration,
            IHostEnvironment environment) : base(environment)
        {
            _context = context;
            _configuration = configuration;
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
            var model = this._context.Notifications
                                        .OrderByDescending(m => m.IsSeen)
                                        .OrderByDescending(m => m.CreatedDate)
                                        .Where(m => m.IdUser == IdUser)
                                        .Select(m => new NotificationViewModel(m))
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

    }
}
