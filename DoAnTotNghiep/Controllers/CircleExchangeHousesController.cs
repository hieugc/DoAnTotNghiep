using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DoAnTotNghiep.Data;
using DoAnTotNghiep.Entity;
using Microsoft.AspNetCore.Authorization;
using DoAnTotNghiep.Enum;
using DoAnTotNghiep.ViewModels;
using DoAnTotNghiep.Modules;
using Microsoft.Extensions.Hosting;
using DoAnTotNghiep.Service;
using DoAnTotNghiep.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Routing;

namespace DoAnTotNghiep.Controllers
{
    [Authorize(Roles = Role.Member)]
    public class CircleExchangeHousesController : BaseController
    {
        private readonly DoAnTotNghiepContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHubContext<ChatHub> _signalContext;
        private readonly IHouseService _houseService;
        private readonly IUserService _userService;
        private readonly ICircleRequestService _circleRequestService;
        private readonly ICircleFeedBackService _circleFeedBackService;
        private readonly INotificationService _notificationService;


        public CircleExchangeHousesController(
                                        DoAnTotNghiepContext context, 
                                        IHostEnvironment environment,
                                        IConfiguration configuration,
                                        IHouseService houseService,
                                        IUserService userService,
                                        ICircleRequestService circleRequestService,
                                        ICircleFeedBackService circleFeedBackService,
                                        INotificationService notificationService,
                                        IHubContext<ChatHub> signalContext) : base(environment)
        {
            _context = context;
            _configuration = configuration;
            _houseService = houseService;
            _userService = userService;
            _circleRequestService = circleRequestService;
            _circleFeedBackService = circleFeedBackService;
            _notificationService = notificationService;
            _signalContext = signalContext;
        }

        [HttpPost("/CircleRequest/UpdateStatus")]
        public IActionResult UpdateStatus([FromBody] UpdateStatusCircleViewModel model)
        {
            return this.Update(model.Id, model.IdCircle, model.Status);
        }

        [HttpPost("/api/CircleRequest/UpdateStatus")]
        public IActionResult ApiUpdateStatus([FromBody] UpdateStatusCircleViewModel model)
        {
            return this.Update(model.Id, model.IdCircle, model.Status);
        }
        //Update
        private IActionResult Update(int IdWaitingRequest, int Circle, int Status)
        {
            int IdUser = this.GetIdUser();
            var rq = this._circleRequestService.GetWaitingRequestById(Circle, IdUser);
            if(rq == null)
            {
                return BadRequest(new
                {
                    Status = 400,
                    Message = "Không tồn tại"
                });
            }
            rq.InCludeAll(this._context);

            if (rq.CircleExchangeHouse != null && rq.CircleExchangeHouse.Status != (int)StatusWaitingRequest.DISABLE)
            {
                rq.CircleExchangeHouse.IncludeAll(this._context);
                DateTime now = DateTime.Now;
                switch (Status)
                {
                    case (int)StatusWaitingRequest.DISABLE:
                        if(rq.CircleExchangeHouse.Status < (int)StatusWaitingRequest.ACCEPT 
                              || rq.CircleExchangeHouse.Status == (int)StatusWaitingRequest.ACCEPT 
                                    && DateTime.Compare(DateTime.Now, rq.CircleExchangeHouse.StartDate.AddHours(-12)) < 0)
                        {
                            if (rq.CircleExchangeHouse.Status == (int)StatusWaitingRequest.ACCEPT)
                            {
                                /*
                                List<int> ListUsers = new List<int>();
                                Notification notification = new Notification()
                                {
                                    Title = NotificationType.RequestTitle,
                                    CreatedDate = DateTime.Now,
                                    Type = NotificationType.CIRCLE_SWAP,
                                    IdUser = rq.CircleExchangeHouse.Id,
                                    IdType = rq.CircleExchangeHouse.Id,
                                    IsSeen = false,
                                    ImageUrl = NotificationImage.Alert,
                                    Content = "Yêu cầu của bạn đến " + " bị từ chối"
                                };
                                await this.SendNotificationAndMail(notification, salt, host, user, subject);*/
                            }
                            rq.CircleExchangeHouse.Status = (int)StatusWaitingRequest.DISABLE;
                            this._circleRequestService.Update(rq.CircleExchangeHouse);
                            rq.Status = Status;
                            this._circleRequestService.Update(rq); 
                        }
                        else
                        {
                            return BadRequest(new
                            {
                                Status = 401,
                                Message = "Yêu cầu đã được chấp nhận và hết thời gian hủy"
                            });
                        }
                        break;
                    case (int)StatusWaitingRequest.ACCEPT:
                        if (rq.CircleExchangeHouse.RequestInCircles != null)
                        {
                            if (!rq.CircleExchangeHouse.RequestInCircles.Any(m => m.Status < (int) StatusWaitingRequest.ACCEPT && m.IdWaitingRequest != rq.IdWaitingRequest))
                            {
                                rq.CircleExchangeHouse.Status = (int)StatusWaitingRequest.ACCEPT;
                                this._circleRequestService.Update(rq.CircleExchangeHouse);

                                //gửi chi tiết
                                //gửi thông báo ngày sẽ Check In
                            }
                        }
                        rq.Status = (int)StatusWaitingRequest.ACCEPT;
                        this._circleRequestService.Update(rq);
                        break;
                    case (int)StatusWaitingRequest.CHECK_IN:
                        if (rq.CircleExchangeHouse.RequestInCircles != null)
                        {
                            if (!rq.CircleExchangeHouse.RequestInCircles
                                .Any(m => m.Status < (int)StatusWaitingRequest.CHECK_IN &&
                                m.IdWaitingRequest != rq.IdWaitingRequest))
                            {
                                rq.CircleExchangeHouse.Status = (int)StatusWaitingRequest.CHECK_IN;
                                this._circleRequestService.Update(rq.CircleExchangeHouse);
                            }
                        }
                        //gửi thông báo ngày sẽ Check Out
                        rq.Status = (int)StatusWaitingRequest.CHECK_IN;
                        this._circleRequestService.Update(rq);
                        break;
                    case (int)StatusWaitingRequest.CHECK_OUT:
                        if (rq.CircleExchangeHouse.RequestInCircles != null)
                        {
                            if (!rq.CircleExchangeHouse.RequestInCircles
                                .Any(m => m.Status < (int)StatusWaitingRequest.CHECK_OUT 
                                && m.IdWaitingRequest != rq.IdWaitingRequest))
                            {
                                rq.CircleExchangeHouse.Status = (int)StatusWaitingRequest.CHECK_OUT;
                                this._circleRequestService.Update(rq.CircleExchangeHouse);
                            }
                        }
                        rq.Status = (int)StatusWaitingRequest.CHECK_OUT;
                        this._circleRequestService.Update(rq);
                        break;
                }
            }

            return Json(new
            {
                Status = 200,
                Message = "ok"
            });
        }

        private async Task SendNotificationAndMail(Notification notification, byte[] salt, string host, User? user, string subject)
        {
            await this.SendNotification(notification, salt, host);
            if (user != null)
            {
                string? moduleEmail = this._configuration.GetConnectionString(ConfigurationEmail.Email());
                string? modulePassword = this._configuration.GetConnectionString(ConfigurationEmail.Password());
                if (!string.IsNullOrEmpty(moduleEmail) && !string.IsNullOrEmpty(modulePassword))
                {
                    EmailSender sender = new EmailSender(moduleEmail, modulePassword);
                    string body = notification.Content;//bill
                    sender.SendMail(user.Email, subject, body, null, string.Empty);
                }
            }
        }
        private async Task SendNotification(Notification notification, byte[] salt, string host)
        {
            await new ChatHub(this._signalContext).SendNotification(
                            group: Crypto.EncodeKey(notification.IdUser.ToString(), salt),
                            target: TargetSignalR.Notification(),
                            model: new NotificationViewModel(notification, host));
        }


        [HttpGet("/api/CircleRequest/Get")]
        public IActionResult ApiGetSuggest()
        {
            return Json(new
            {
                Status = 200,
                Data = this._circleRequestService.GetSuggest(this.GetIdUser(), this._userService, Crypto.Salt(this._configuration), this.GetWebsitePath())
            });
        }
        //chi tiết yêu cầu
        [HttpGet("/CircleRequest/Detail")]
        public IActionResult GetDetailRequest(int Id)
        {
            CircleRequestViewModel? model = this.DetailRequest(Id);
            if (model == null) return PartialView("~/Views/HistoryTransaction/_item.cshtml", null);
            return PartialView("~/Views/Request/_CircleRequestDetail.cshtml", model);
        }
        [HttpGet("/api/CircleRequest/Detail")]
        public IActionResult ApiGetDetailRequest(int Id)
        {
            CircleRequestViewModel? model = this.DetailRequest(Id);
            if (model == null) return BadRequest(new
            {
                Status = 400,
                Message = "Không tìm thấy chi tiết yêu cầu"
            });
            return Json(new
            {
                Status = 200,
                Data = model
            });
        }
        private CircleRequestViewModel? DetailRequest(int IdRequest)
        {
            var item = this._circleRequestService.GetById(IdRequest);
            if (item != null)
            {
                return this._circleRequestService.GetCircleRequestDetail(item, this.GetIdUser(), this._userService, Crypto.Salt(this._configuration), this.GetWebsitePath());
            }
            return null;
        }

        //xóa yêu cầu
        [HttpPost("/CircleRequest/Delete")]
        public IActionResult DeleteRequest([FromBody] int Id)
        {
            return this.Remove(Id);
        }
        [HttpPost("/api/CircleRequest/Delete")]
        public IActionResult ApiDeleteRequest([FromBody] int Id)
        {
            return this.Remove(Id);
        }
        private IActionResult Remove(int Id)
        {
            if (this._circleRequestService.Remove(Id) == true) return Json(new
            {
                Status = 200,
                Message = "Hủy bỏ thành công"
            });
            return BadRequest(new
            {
                Status = 400,
                Message = "Yêu cầu không tồn tại"
            });
        }
    }
}
