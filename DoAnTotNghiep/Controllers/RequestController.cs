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
using static System.ComponentModel.BackgroundWorker;
using System.ComponentModel;
using System.Xml;
using DoAnTotNghiep.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Hangfire;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.DependencyInjection;
using DoAnTotNghiep.Service;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DoAnTotNghiep.Controllers
{
    [Authorize(Roles = Role.Member)]
    public class RequestController : BaseController
    {
        //xong
        private readonly DoAnTotNghiepContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHubContext<ChatHub> _signalContext;
        private readonly ILogger<TimedHostedService> _timerLog;
        private readonly IFeedBackService _feedBackService;
        private readonly IRequestService _requestService;
        private readonly IFileService _fileService;
        private readonly IHouseService _houseService;
        private readonly IUserService _userService;
        private readonly INotificationService _notificationService;
        private readonly ITransactionService _transactionService;
        private readonly ICheckInService _checkInService;
        private readonly ICheckOutService _checkOutService;

        public RequestController(DoAnTotNghiepContext context,
                                IConfiguration configuration,
                                IHostEnvironment environment,
                                IHubContext<ChatHub> signalContext,
                                ILogger<TimedHostedService> timerLog,
                                IServiceProvider service,
                                IFeedBackService feedBackService,
                                IRequestService requestService,
                                IFileService fileService,
                                IHouseService houseService,
                                IUserService userService,
                                INotificationService notificationService,
                                ITransactionService transactionService,
                                ICheckInService checkInService,
                                ICheckOutService checkOutService) : base(environment)
        {
            _context = context;
            _configuration = configuration;
            _signalContext = signalContext;
            _timerLog = timerLog;
            _feedBackService = feedBackService;
            _requestService = requestService;
            _fileService = fileService;
            _houseService = houseService;
            _userService = userService;
            _notificationService = notificationService;
            _transactionService = transactionService;
            _checkInService = checkInService;
            _checkOutService = checkOutService;
        }

        [HttpGet("/Request/FormWithUserAccess")]
        public IActionResult RequestForm(string UserAccess)
        {
            int Id = 0;
            ModelRequestForm model = new ModelRequestForm();
            if (int.TryParse(Crypto.DecodeKey(UserAccess, Crypto.Salt(this._configuration)), out Id))
            {
                byte[] salt = Crypto.Salt(this._configuration);
                string host = this.GetWebsitePath();
                //list house của người đó
                model.UserHouses.AddRange(
                    this._houseService.GetListDetailHouseWithOneImage(
                        this._houseService.GetListHouseByUser(Id), this._fileService, host, salt));
                //list house của mình
                model.MyHouses.AddRange(
                    this._houseService.GetListDetailHouseWithOneImage(
                        this._houseService.GetListHouseByUser(this.GetIdUser()), this._fileService, host, salt));
            }
            return PartialView("./Views/Request/_RequestForm.cshtml", model);
        }
        [HttpGet("/Request/FormWithHouseId")]//chưa xong
        public IActionResult RequestForm(int IdHouse)
        {
            ModelRequestForm model = new ModelRequestForm();
            byte[] salt = Crypto.Salt(this._configuration);
            string host = this.GetWebsitePath();
            //list house của người đó
            model.UserHouses.AddRange(this._houseService.GetListDetailHouseWithOneImage(
                        this._houseService.GetHouseByIdStatus(IdHouse, (int)StatusHouse.VALID),
                        this._fileService, host, salt));
            //list house của mình
            model.MyHouses.AddRange(
                this._houseService.GetListDetailHouseWithOneImage(
                    this._houseService.GetListHouseByUser(this.GetIdUser()), this._fileService, host, salt));
            return PartialView("./Views/Request/_RequestForm.cshtml", model);
        }
        [HttpGet("/Request/EditForm")]//chưa xong
        public IActionResult EditForm(int IdRequest)
        {
            return PartialView("./Views/Request/_RequestEditForm.cshtml");
        }


        [HttpPost("/Request/Create")]
        public async Task<IActionResult> CreateRequestAsync([FromBody] RequestViewModel model)
        {
            return await this.CreateAsync(model);
        }
        [HttpPost("/api/Request/Create")]
        public async Task<IActionResult> ApiCreateRequestAsync([FromBody] RequestViewModel model)
        {
            return await this.CreateAsync(model);
        }
        private async Task<IActionResult> CreateAsync(RequestViewModel model)
        {
            if (ModelState.IsValid)
            {
                int IdUser = this.GetIdUser();
                //kiểm tra đủ tiền không?
                var user = this._userService.GetById(IdUser);
                if (user == null)
                    return BadRequest(new
                    {
                        Status = 400,
                        Message = "Yêu cầu không thể thực hiện"
                    });

                //kiểm tra nhà tồn tại không?
                var house = this._houseService.GetHouseByIdStatus(model.IdHouse, (int)StatusHouse.VALID);
                if (house != null && house.IdUser != IdUser)
                {
                    house.IncludeAll(this._context);
                    if (this._houseService.CheckAnyValidRequest(house, model.StartDate, model.EndDate))
                    {
                        return BadRequest(new
                        {
                            Status = 401,
                            Message = "Yêu cầu không thể thực hiện vì nhà đang trao đổi"
                        });
                    }

                    if (model.IdSwapHouse != null)
                    {
                        var sh = this._houseService.GetHouseByIdWithUser(model.IdSwapHouse.Value, IdUser);
                        if (sh != null && sh.Id != house.Id)
                        {
                            if (this._houseService.CheckAnyValidRequest(sh, model.StartDate, model.EndDate))
                            {
                                return BadRequest(new
                                {
                                    Status = 401,
                                    Message = "Yêu cầu không thể thực hiện vì nhà bạn đang trao đổi"
                                });
                            }
                        }
                    }
                    if ((user.Point + user.BonusPoint - user.PointUsing) < model.Price)
                    {
                        return BadRequest(new
                        {
                            Status = 402,
                            Message = "Yêu cầu không thể thực hiện vì thiếu tiền"
                        });
                    }

                    Request request = new Request()
                    {
                        CreatedDate = DateTime.Now,
                        UpdatedDate = DateTime.Now,
                        Type = model.Type,
                        IdHouse = model.IdHouse,
                        IdSwapHouse = model.IdSwapHouse,
                        Point = model.Price,
                        StartDate = model.StartDate,
                        EndDate = model.EndDate,
                        Status = (int)StatusRequest.WAIT_FOR_SWAP,
                        IdUser = IdUser
                    };
                    var transaction = this._context.Database.BeginTransaction();
                    try
                    {
                        this._requestService.SaveRequest(request);

                        if (house.Users != null)
                        {
                            Notification notification = new Notification().WaitingRequestNotification(house, request, house.Users);
                            this._notificationService.SaveNotification(notification);

                            await new ChatHub(this._signalContext).SendNotification(
                                group: Crypto.EncodeKey(notification.IdUser.ToString(), Crypto.Salt(this._configuration)),
                                target: TargetSignalR.Notification(),
                                model: new NotificationViewModel(notification, this.GetWebsitePath()));
                        }
                        transaction.Commit();
                        return Json(new
                        {
                            Status = 200,
                            Message = "Khởi tạo thành công",
                            Data = new { }
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        transaction.Rollback();
                    }
                }
            }

            return BadRequest(new
            {
                Status = 400,
                Message = "Yêu cầu không thể thực hiện"
            });
        }

        //xóa yêu cầu
        [HttpPost("/Request/Delete")]
        public async Task<IActionResult> DeleteRequestAsync([FromBody] int Id)
        {
            return await this.RemoveAsync(Id);
        }
        [HttpPost("/api/Request/Delete")]
        public async Task<IActionResult> ApiDeleteRequestAsync([FromBody] int Id)
        {
            return await this.RemoveAsync(Id);
        }
        private async Task<IActionResult> RemoveAsync(int Id)
        {
            int IdUser = this.GetIdUser();
            var rq = this._requestService.GetRequestByIdWithLessStatus(Id, (int)StatusRequest.CHECK_IN);
            if (rq != null)
            {
                try
                {
                    if (rq.Status >= (int)StatusRequest.ACCEPT)
                    {
                        rq.IncludeAll(this._context);
                        int IdSend = 0;
                        if (IdUser != rq.IdUser)
                        {
                            IdSend = rq.IdUser;
                        }
                        else
                        {
                            if (rq.Houses != null)
                            {
                                IdSend = rq.Houses.IdUser;
                            }
                        }
                        if (IdSend != 0)
                        {
                            string host = this.GetWebsitePath();
                            if (IdSend == rq.IdUser)
                            {
                                if (rq.Houses.IdCity != null)
                                {
                                    await CreateWaitingRequestTimerAsync(TargetFunction.ExecuteCreateWaiting,
                                        TimeSpan.FromSeconds(10), 1, rq.Houses.IdCity.Value, IdSend, rq.StartDate, rq.EndDate, host);
                                }
                            }
                            else if (rq.Type == 2 && IdSend != rq.IdUser)
                            {
                                rq.SwapHouses.IncludeAll(this._context);
                                if (rq.SwapHouses.IdCity != null)
                                {
                                    await CreateWaitingRequestTimerAsync(TargetFunction.ExecuteCreateWaiting,
                                        TimeSpan.FromSeconds(10), 1, rq.SwapHouses.IdCity.Value, IdSend, rq.StartDate, rq.EndDate, host);
                                }
                            }

                            Notification notification = new Notification().RejectRequestNotification(rq.Houses, rq, IdSend);
                            this._notificationService.SaveNotification(notification);
                            await this.SendNotificationAndMail(notification, Crypto.Salt(this._configuration), host, this._userService.GetById(IdSend), Subject.SendReject());
                        }
                    }

                    rq.Status = (int)StatusRequest.REJECT;
                    this._requestService.UpdateRequest(rq);
                    return Json(new
                    {
                        Status = 200,
                        Message = "Xóa thành công"
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return BadRequest(new
            {
                Status = 400,
                Message = "Yêu cầu không tồn tại"
            });
        }

        //lấy danh sách yêu cầu theo nhà
        [HttpGet("/Request/NumberRequestInHouse")]
        public IActionResult GetNumberRequestInHouse(int IdHouse)
        {
            return Json(new
            {
                Status = 200,
                Data = this._requestService.GetWaitingRequestByHouse(IdHouse, this.GetIdUser()).Count()
            });
        }
        [HttpGet("/Request/GetByHouse")]
        public IActionResult GetListRequestWithHouse(int IdHouse)
        {
            return PartialView("./Views/Request/_ListRequestToHouse.cshtml", this._requestService.GetNotifyRequests(
                this._requestService.GetWaitingRequestByHouse(IdHouse, this.GetIdUser()),
                this._fileService, this._houseService, Crypto.Salt(this._configuration), this.GetWebsitePath()));
        }
        [HttpGet("/api/Request/GetByHouse")]
        public IActionResult ApiGetListRequestWithHouse(int IdHouse)
        {
            return Json(new
            {
                Status = 200,
                Data = this._requestService.GetNotifyRequests(this._requestService.GetWaitingRequestByHouse(IdHouse, this.GetIdUser()),
                this._fileService, this._houseService, Crypto.Salt(this._configuration), this.GetWebsitePath())
            });
        }
        [HttpGet("/Request/GetRequestSent")]
        public IActionResult GetListRequestSent(int? Status)
        {
            int IdUser = this.GetIdUser();
            return Json(new
            {
                Status = 200,
                Data = this._requestService.GetValidRequestByUser(
                    this._requestService.GetAllSent(IdUser),
                    this._fileService, this._houseService, this._feedBackService, Status, Crypto.Salt(this._configuration), this.GetWebsitePath(), IdUser)
            });
        }
        [HttpGet("/api/Request/GetRequestSent")]
        public IActionResult ApiGetListRequestSent(int? Status)
        {
            int IdUser = this.GetIdUser();
            return Json(new
            {
                Status = 200,
                Data = this._requestService.GetValidRequestByUser(
                    this._requestService.GetAllSent(IdUser),
                    this._fileService, this._houseService, this._feedBackService, Status, Crypto.Salt(this._configuration), this.GetWebsitePath(), IdUser)
            });
        }
        [HttpGet("/Request/Detail")]
        public IActionResult GetDetailRequest(int Id)
        {
            DetailRequest? model = this.DetailRequest(Id);
            if (model == null) return PartialView("~/Views/HistoryTransaction/_item.cshtml", null);
            if(model.Request.Status == (int)StatusRequest.WAIT_FOR_SWAP)
            {
                return PartialView("./Views/Request/_WaitRequest.cshtml", new NotifyRequest(){Request = model.Request,SwapHouse = model.SwapHouse});
            }
            return PartialView("~/Views/Request/_RequestDetail.cshtml", model);
        }
        [HttpGet("/api/Request/Detail")]
        public IActionResult ApiGetDetailRequest(int Id)
        {
            DetailRequest? model = this.DetailRequest(Id);
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
        private DetailRequest? DetailRequest(int IdRequest)
        {
            var item = this._requestService.GetRequestById(IdRequest);
            if (item != null)
            {
                int IdUser = this.GetIdUser();
                byte[] salt = Crypto.Salt(this._configuration);
                string host = this.GetWebsitePath();
                return this._requestService.CreateDetailRequest(item, this._fileService, this._houseService, this._feedBackService, null, salt, host, IdUser);
            }
            return null;
        }
        [HttpPost("/Request/UpdateStatus")]
        public async Task<IActionResult> UpdateStatusAsync([FromBody] UpdateStatusViewModel model)
        {
            return await this.UpdateRequestStatusAsync(model.Id, model.Status);
        }

        [HttpPost("/api/Request/UpdateStatus")]
        public async Task<IActionResult> ApiUpdateStatusAsync([FromBody] UpdateStatusViewModel model)
        {
            return await this.UpdateRequestStatusAsync(model.Id, model.Status);
        }
        private async Task<IActionResult> UpdateRequestStatusAsync(int Id, int Status)
        {
            int IdUser = this.GetIdUser();
            string host = this.GetWebsitePath();
            byte[] salt = Crypto.Salt(this._configuration);
            switch (Status)
            {
                case (int)StatusRequest.REJECT:
                    var request1 = this._requestService.GetRequestById(Id, (int)StatusRequest.WAIT_FOR_SWAP);
                    if (request1 != null && request1.IdUser == IdUser) request1 = null;
                    return await this.UpdateRequestStatusAsync(request1
                        , IdUser, host, salt, Subject.SendReject(), (int)StatusRequest.REJECT);
                case (int)StatusRequest.ACCEPT://chấp nhận
                    var request2 = this._requestService.GetRequestById(Id, (int)StatusRequest.WAIT_FOR_SWAP);
                    if (request2 != null && request2.IdUser == IdUser) request1 = null;
                    return await this.UpdateRequestStatusAsync(request2
                        , IdUser, host, salt, Subject.SendRequestDetail(), (int)StatusRequest.ACCEPT);
                case (int)StatusRequest.CHECK_IN://checkIn
                    return await this.UpdateRequestStatusAsync(this._requestService.GetRequestById(Id, (int)StatusRequest.ACCEPT)
                        , IdUser, host, salt, string.Empty, (int)StatusRequest.CHECK_IN);
            }
            return await this.UpdateRequestStatusAsync(this._requestService.GetRequestByIdWithEndTime(Id, (int)StatusRequest.CHECK_IN)
                        , IdUser, host, salt, string.Empty, (int)StatusRequest.CHECK_OUT);
        }
        private async Task<IActionResult> UpdateRequestStatusAsync(Request? request, int IdUser, string host, byte[] salt, string subject, int status)
        {
            if (request != null)
            {
                var DBtransaction = this._context.Database.BeginTransaction();
                try
                {
                    request.IncludeAll(this._context);
                    if (request.Houses != null)
                    {
                        request.Status = status;
                        switch (status)
                        {
                            case (int)StatusRequest.CHECK_IN:
                                if (request.IdSwapHouse != null)
                                {
                                    if (request.CheckIns == null || request.CheckIns.Count() == 0)
                                    {
                                        request.Status = (int)StatusRequest.ACCEPT;
                                    }
                                }
                                this._checkInService.Save(new CheckIn()
                                {
                                    IdRequest = request.Id,
                                    IdUser = IdUser
                                });
                                break;
                            case (int)StatusRequest.CHECK_OUT:
                                if (request.IdSwapHouse != null)
                                {
                                    if (request.CheckOuts == null || request.CheckOuts.Count() == 0)
                                    {
                                        request.Status = (int)StatusRequest.CHECK_IN;
                                    }
                                }

                                this._checkOutService.Save(new CheckOut()
                                {
                                    IdRequest = request.Id,
                                    IdUser = IdUser
                                });
                                break;
                        }

                        this._requestService.UpdateRequest(request);
                        var user = this._userService.GetById(IdUser);

                        switch (status)
                        {
                            case (int)StatusRequest.REJECT:
                                Notification notification1 = new Notification().RejectRequestNotification(request.Houses, request, request.IdUser);
                                if (request.Houses.IdCity != null)
                                {
                                    await CreateWaitingRequestTimerAsync(TargetFunction.ExecuteCreateWaiting,
                                        TimeSpan.FromSeconds(10), 1, request.Houses.IdCity.Value, IdUser, request.StartDate, request.EndDate, host);
                                }
                                this._notificationService.SaveNotification(notification1);
                                DBtransaction.Commit();
                                await this.SendNotificationAndMail(notification1, salt, host, user, subject);
                                break;
                            case (int)StatusRequest.ACCEPT://chấp nhận
                                Notification notification2 = new Notification().AcceptRequestNotification(request.Houses, request, request.IdUser);
                                this._notificationService.SaveNotification(notification2);
                                DBtransaction.Commit();
                                await this.SendNotificationAndMail(notification2, salt, host, this._userService.GetById(request.IdUser), subject);
                                await this.TimerCheckInNotificationAsync(request, host);
                                break;
                            case (int)StatusRequest.CHECK_IN://checkIn
                                Notification notification3 = new Notification().CheckInRequestNotification(request.Houses, request, IdUser);

                                if (request.Type == 1 && request.Point > 0)
                                {
                                    if (user != null && request.IdUser == user.Id)
                                    {
                                        if ((user.BonusPoint + user.Point - user.PointUsing) < request.Point)
                                        {
                                            DBtransaction.Rollback();
                                            return Json(new
                                            {
                                                Status = 200,
                                                Message = "Thiếu tiền"
                                            });
                                        }
                                        else
                                        {
                                            user.PointUsing = request.Point;
                                            this._userService.UpdateUser(user);
                                        }
                                    }
                                }
                                this._notificationService.SaveNotification(notification3);
                                DBtransaction.Commit();

                                await this.TimerCheckOutNotificationAsync(request, host);
                                await this.SendNotification(notification3, salt, host);
                                break;
                            case (int)StatusRequest.CHECK_OUT:
                                Notification notification4 = new Notification().CheckOutRequestNotification(request.Houses, request, IdUser);
                                if (request.Type == 1 && request.Point > 0)
                                {
                                    if (user != null && request.IdUser == user.Id)
                                    {
                                        if ((user.BonusPoint + user.Point - user.PointUsing) < request.Point)
                                        {
                                            DBtransaction.Rollback();
                                            return BadRequest(new
                                            {
                                                Status = 200,
                                                Message = "Thiếu tiền"
                                            });
                                        }
                                        else
                                        {
                                            user.BonusPoint -= request.Point;
                                            if (user.BonusPoint < 0)
                                            {
                                                user.Point += user.BonusPoint;
                                                user.BonusPoint = 0;
                                            }
                                            user.PointUsing = 0;
                                            this._userService.UpdateUser(user);
                                            this._transactionService.Save(new HistoryTransaction().RequestTransaction(request, user, IdUser));
                                        }
                                    }
                                }
                                this._notificationService.SaveNotification(notification4);
                                DBtransaction.Commit();
                                await this.SendNotification(notification4, salt, host);
                                break;
                        }
                        return Json(new
                        {
                            Status = 200,
                            Message = "Cập nhật thành công"
                        });
                    }
                    else
                    {
                        DBtransaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    DBtransaction.Rollback();
                }
            }

            return BadRequest(new
            {
                Status = 400,
                Message = "Yêu cầu không tồn tại"
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
        private async Task TimerCheckInNotificationAsync(Request request, string host)
        {
            TimeSpan timeToGo = request.StartDate.AddDays(-1) - DateTime.Now;
            if (timeToGo <= TimeSpan.Zero) timeToGo = TimeSpan.Zero;
            await InitTimerAsync(request, TargetFunction.ExecuteCheckIn, timeToGo, 2, host);
        }
        private async Task TimerCheckOutNotificationAsync(Request request, string host)
        {
            TimeSpan timeToGo = request.EndDate.AddDays(-1) - DateTime.Now;
            if (timeToGo <= TimeSpan.Zero) timeToGo = TimeSpan.Zero;
            await InitTimerAsync(request, TargetFunction.ExecuteCheckOut, timeToGo, 1, host);
        }
        private async Task InitTimerAsync(Request request, string Function, TimeSpan timeStart, int limit, string host)
        {
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;
            DoAnTotNghiepContext inputContext = new DoAnTotNghiepContext(this._context.GetConfig());
            RequestBackground Object = new RequestBackground(new ChatHub(this._signalContext), request, this._configuration);
            TimedHostedService timer = new TimedHostedService(inputContext, host, Function, token, limit, timeStart, Object);
            await timer.StartAsync(token);
        }
        private async Task CreateWaitingRequestTimerAsync(string Function, TimeSpan timeStart, int limit, int IdCity, int IdUser, DateTime startDate, DateTime endDate, string host)
        {
            DateTime? DateStart = null, DateEnd = null;
            DateTime now = DateTime.Now;
            if (DateTime.Compare(now, startDate) <= 0) { DateStart = startDate; DateEnd = endDate; }
            else if (DateTime.Compare(now, endDate) <= 0) { DateStart = now; DateEnd = endDate; }
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;
            DoAnTotNghiepContext inputContext = new DoAnTotNghiepContext(this._context.GetConfig());
            CreateWaitingRequest Object = new CreateWaitingRequest(IdCity, IdUser, DateStart, DateEnd);
            TimedHostedService timer = new TimedHostedService(inputContext, host, Function, token, limit, timeStart, Object);
            await timer.StartAsync(token);
        }

        [HttpGet("/Statistics/House")]
        public IActionResult StatisticsHouse(InputRequestStatistic input)
        {
            int IdUser = this.GetIdUser();

            return Json(new
            {
                Status = 200,
                Data = new HouseStatistics()
                {
                    Requests = this._requestService.StatisticRequestToHouse(input, IdUser),
                    UseForSwap = this._requestService.StatisticHouseUseForSwap(input, IdUser)
                }
            });
        }
    }
}