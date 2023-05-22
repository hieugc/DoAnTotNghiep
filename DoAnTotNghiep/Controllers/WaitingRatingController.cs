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
using Microsoft.Extensions.Hosting;
using static System.ComponentModel.BackgroundWorker;
using System.ComponentModel;
using System.Xml;
using Azure.Core;
using DoAnTotNghiep.Hubs;
using Microsoft.AspNetCore.SignalR;
using DoAnTotNghiep.Service;

namespace DoAnTotNghiep.Controllers
{
    [Authorize(Roles = Role.Member)]
    public class WaitingRatingController: BaseController
    {
        private readonly DoAnTotNghiepContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHubContext<ChatHub> _signalContext;
        private readonly IHouseService _houseService;
        private readonly IUserService _userService;
        private readonly ICircleRequestService _circleRequestService;
        private readonly ICircleFeedBackService _circleFeedBackService;
        private readonly INotificationService _notificationService;

        public WaitingRatingController(
                                        DoAnTotNghiepContext context, 
                                        IConfiguration configuration, 
                                        IHostEnvironment environment, 
                                        IHubContext<ChatHub> signalContext,
                                        IHouseService houseService,
                                        IUserService userService,
                                        ICircleRequestService circleRequestService,
                                        ICircleFeedBackService circleFeedBackService,
                                        INotificationService notificationService) : base(environment)
        {
            _context = context;
            _configuration = configuration;
            _signalContext = signalContext;
            _houseService = houseService;
            _userService = userService;
            _circleRequestService = circleRequestService;
            _circleFeedBackService = circleFeedBackService;
            _notificationService = notificationService;
        }

        [HttpGet("/CircleRating/Form")]
        public IActionResult FormCreateRating(int IdRequest, int? IdWaitingRating)
        {
            var request = this._circleRequestService.GetById(IdRequest);
            if (request == null) return NotFound();

            if(IdWaitingRating != null)
            {
                var rating = this._circleFeedBackService.GetById(IdWaitingRating.Value);
                if(rating != null)
                {
                    EditRatingViewModel editRating = new EditRatingViewModel(rating);
                    return PartialView("~/Views/WaitingRating/_EditFormRating.cshtml", editRating);
                }
            }
            return PartialView("~/Views/WaitingRating/_FormRating.cshtml", IdRequest);
        }
        private async Task<IActionResult> CreateAsync(CreateRatingViewModel feedBack)
        {
            if (ModelState.IsValid)
            {
                int IdUser = this.GetIdUser();
                var request = this._circleRequestService.GetWaitingRequestById(feedBack.IdRequest, IdUser);
                if(request != null)
                {
                    request.InCludeAll(this._context);
                    if(request.WaitingRequests != null)
                    {
                        request.WaitingRequests.IncludeAll(this._context);
                        if (request.WaitingRequests.Houses != null)
                        {
                            request.WaitingRequests.Houses.IncludeAll(this._context);
                            if (request.WaitingRequests.Houses.Users != null)
                            {
                                if (request.WaitingRequests.Users != null)
                                {
                                    using (var transaction = this._context.Database.BeginTransaction())
                                    {
                                        try
                                        {
                                            this._circleFeedBackService.Save(new FeedBackOfCircle().Create(feedBack, IdUser, request.WaitingRequests.Houses.Users.Id, request.WaitingRequests.Houses.Id));

                                            request.Status = (int)StatusWaitingRequest.ENDED;
                                            this._circleRequestService.Update(request);

                                            //nhà người chủ
                                            List<int> intsHouse = this._houseService.CalRating(request.WaitingRequests.Houses.Id);
                                            request.WaitingRequests.Houses.Rating = Math.Ceiling((double)intsHouse.First() / intsHouse.Last()) ;
                                            request.WaitingRequests.Houses.NumberOfRating += 1;
                                            this._houseService.UpdateHouse(request.WaitingRequests.Houses);
                                            //người chủ
                                            List<int> ints = this._userService.CalRate(request.WaitingRequests.Houses.Users.Id);
                                            request.WaitingRequests.Houses.Users.UserRating = Math.Ceiling((double)ints.First() / ints.Last()) ;
                                            request.WaitingRequests.Houses.Users.NumberUserRating += 1;
                                            this._userService.UpdateUser(request.WaitingRequests.Houses.Users);

                                            Notification notification = new Notification()
                                            {
                                                IdType = request.WaitingRequests.Id,
                                                IdUser = request.WaitingRequests.Houses.Users.Id,
                                                Title = NotificationType.RatingTitle,
                                                Content = request.WaitingRequests.Houses.Name + " có đánh giá mới",
                                                CreatedDate = DateTime.Now,
                                                IsSeen = false,
                                                ImageUrl = NotificationImage.Alert,
                                                Type = NotificationType.CIRCLE_RATING
                                            };
                                            this._notificationService.SaveNotification(notification);
                                            transaction.Commit();
                                            await new ChatHub(this._signalContext).SendNotification(
                                                group: Crypto.EncodeKey(notification.IdUser.ToString(), Crypto.Salt(this._configuration)),
                                                target: TargetSignalR.Notification(),
                                                model: new NotificationViewModel(notification, this.GetWebsitePath()));
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
                                    return BadRequest(new
                                    {
                                        Status = 500,
                                        Message = "Không lưu được"
                                    });
                                }
                            }
                        }
                    }
                }
                return BadRequest(new
                {
                    Status = 400,
                    Message = "Không tồn tại yêu cầu"
                });
            }
            return BadRequest(new
            {
                Status = 400,
                Message = this.ModelErrors()
            });
        }

        [HttpPost("/CircleRating/Create")]
        public async Task<IActionResult> CreateRatingAsync([FromBody] CreateRatingViewModel feedBack)
        {
            return await this.CreateAsync(feedBack);
        }

        [HttpPost("/api/CircleRating/Create")]
        public async Task<IActionResult> ApiCreateRatingAsync([FromBody] CreateRatingViewModel feedBack)
        {
            return await this.CreateAsync(feedBack);
        }

        //Update -> status + message
        private IActionResult Edit(EditRatingViewModel feedBack)
        {
            if (ModelState.IsValid)
            {
                int IdUser = this.GetIdUser();
                var model = this._circleFeedBackService.GetByCircleRequest(IdUser, feedBack.Id);
                if(model != null)
                {
                    model.IncludeAll(this._context);
                    using (var transaction = this._context.Database.BeginTransaction())
                    {
                        try
                        {
                            model.Update(feedBack);
                            this._circleFeedBackService.Update(model);

                            if (model.Houses != null)
                            {
                                List<int> ints = this._houseService.CalRating(model.Houses.Id);
                                model.Houses.Rating = Math.Ceiling((double)ints.First() / ints.Last()) ;
                                this._houseService.UpdateHouse(model.Houses);
                            }

                            if (model.UserRated != null)
                            {
                                List<int> ints = this._userService.CalRate(model.UserRated.Id);
                                model.UserRated.UserRating = Math.Ceiling((double)ints.First() / ints.Last()) ;
                                this._userService.UpdateUser(model.UserRated);
                            }

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
                    return BadRequest(new
                    {
                        Status = 500,
                        Message = "Không chỉnh sửa được"
                    });
                }
                return BadRequest(new
                {
                    Status = 400,
                    Message = "Không tồn tại đánh giá"
                });
            }
            return BadRequest(new
            {
                Status = 400,
                Message = this.ModelErrors()
            });
        }
        [HttpPost("/CircleRating/Update")]
        public IActionResult UpdateRating([FromBody] EditRatingViewModel feedBack)
        {
            return this.Edit(feedBack);
        }

        [HttpPost("/api/CircleRating/Update")]
        public IActionResult ApiUpdateRating([FromBody] EditRatingViewModel feedBack)
        {
            return this.Edit(feedBack);
        }
    }
}
