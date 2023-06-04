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
using Org.BouncyCastle.Asn1.Ocsp;

namespace DoAnTotNghiep.Controllers
{
    [Authorize(Roles = Role.Member)]
    public class RatingController: BaseController
    {
        private readonly DoAnTotNghiepContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHubContext<ChatHub> _signalContext;
        private readonly IFeedBackService _feedBackService;
        private readonly IRequestService _requestService;
        private readonly IFileService _fileService;
        private readonly IHouseService _houseService;
        private readonly IUserService _userService;
        private readonly INotificationService _notificationService;
        private readonly ITransactionService _transactionService;

        public RatingController(
                                DoAnTotNghiepContext context, 
                                IConfiguration configuration, 
                                IHostEnvironment environment, 
                                IHubContext<ChatHub> signalContext,
                                IFeedBackService feedBackService,
                                IRequestService requestService,
                                IFileService fileService,
                                IHouseService houseService,
                                IUserService userService,
                                INotificationService notificationService,
                                ITransactionService transactionService) : base(environment)
        {
            _context = context;
            _configuration = configuration;
            _signalContext = signalContext;
            _feedBackService = feedBackService;
            _requestService = requestService;
            _fileService = fileService;
            _houseService = houseService;
            _userService = userService;
            _notificationService = notificationService;
            _transactionService = transactionService;
    }

        [HttpGet("/Rating/Form")]
        public IActionResult FormCreateRating(int IdRequest, int? IdRating)
        {
            var request = this._requestService.GetRequestById(IdRequest);
            if (request == null) return NotFound();
            int idUser = this.GetIdUser();
            bool isCan = ((request.IdUser == idUser) || (request.IdUser != idUser) && request.Type == 2);

            ViewData["isCan"] = isCan ? "true" : "false";
            if(IdRating != null)
            {
                var rating = this._feedBackService.GetRatingById(IdRating.Value);
                if(rating != null)
                {
                    EditRatingViewModel editRating = new EditRatingViewModel(rating);
                    return PartialView("~/Views/Rating/_EditFormRating.cshtml", editRating);
                }
            }
            return PartialView("~/Views/Rating/_FormRating.cshtml", IdRequest);
        }

        private async Task<IActionResult> CreateAsync(CreateRatingViewModel feedBack)
        {
            if (ModelState.IsValid)
            {
                int IdUser = this.GetIdUser();
                var request = this._requestService.GetRequestByFeedBack(feedBack.IdRequest, IdUser);
                if(request != null)
                {
                    request.IncludeAll(this._context);
                    if(request.Houses != null)
                    {
                        request.Houses.IncludeAll(this._context);
                        if(request.Houses.Users != null)
                        {
                            if (request.Users != null)
                            {
                                string host = this.GetWebsitePath();
                                if (request.IdUser == IdUser)
                                {
                                    using (var transaction = this._context.Database.BeginTransaction())
                                    {
                                        try
                                        {
                                            FeedBack model = new FeedBack();
                                            model.Create(feedBack, IdUser, request.Houses.Users.Id, request.Houses.Id);
                                            this._feedBackService.Save(model);

                                            List<int> intsHouse = this._houseService.CalRating(request.Houses.Id);
                                            request.Houses.Rating = Math.Ceiling((double)intsHouse.First()/ intsHouse.Last());
                                            request.Houses.NumberOfRating += 1;
                                            this._houseService.UpdateHouse(request.Houses);

                                            List<int> ints = this._userService.CalRate(request.Houses.Users.Id);
                                            request.Houses.Users.UserRating = Math.Ceiling((double)ints.First() / ints.Last());
                                            request.Houses.Users.NumberUserRating += 1;
                                            this._userService.UpdateUser(request.Houses.Users);

                                            if(request.IdSwapHouse == null || request.IdSwapHouse != null && request.FeedBacks != null && request.FeedBacks.Count() > 1)
                                            {
                                                request.Status = (int)StatusRequest.ENDED;
                                                request.UpdatedDate = DateTime.Now;
                                                this._requestService.UpdateRequest(request);
                                            }
                                            //tăng điểm người dùng
                                            request.Users.BonusPoint += (int)Math.Floor(Math.Abs(request.Point) * 0.1);
                                            this._userService.UpdateUser(request.Users);

                                            //gửi thông báo
                                            Notification notification = new Notification().FeedBackNotification(request, request.Houses.Users.Id);
                                            this._notificationService.SaveNotification(notification);
                                            transaction.Commit();

                                            await new ChatHub(this._signalContext).SendNotification(
                                                group: Crypto.EncodeKey(notification.IdUser.ToString(),
                                                Crypto.Salt(this._configuration)),
                                                target: TargetSignalR.Notification(),
                                                model: new NotificationViewModel(notification, host));
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
                                else if(request.Houses.Users.Id == IdUser)
                                {
                                    using (var transaction = this._context.Database.BeginTransaction())
                                    {
                                        try
                                        {
                                            FeedBack model = new FeedBack();
                                            model.Create(feedBack, request.Houses.Users.Id, request.Users.Id, request.SwapHouses == null ? null : request.SwapHouses.Id);
                                            this._feedBackService.Save(model);

                                            if (request.SwapHouses != null)
                                            {
                                                List<int> intsHouse = this._houseService.CalRating(request.SwapHouses.Id);
                                                request.SwapHouses.Rating = Math.Ceiling((double)intsHouse.First() / intsHouse.Last());
                                                request.Houses.Users.NumberUserRating += 1;
                                                this._houseService.UpdateHouse(request.SwapHouses);
                                            }
                                            List<int> ints = this._userService.CalRate(request.Users.Id);
                                            request.Users.UserRating = Math.Ceiling((double)ints.First() / ints.Last());
                                            request.Houses.Users.NumberUserRating += 1;
                                            this._userService.UpdateUser(request.Users);

                                            if (request.FeedBacks != null && request.FeedBacks.Count() > 0)
                                            {
                                                request.Status = (int)StatusRequest.ENDED;
                                                request.UpdatedDate = DateTime.Now;
                                                this._requestService.UpdateRequest(request);
                                            }
                                            //gửi thông báo
                                            Notification notification = new Notification().FeedBackNotification(request, request.IdUser);
                                            this._notificationService.SaveNotification(notification);

                                            //tăng điểm người dùng
                                            request.Houses.Users.BonusPoint += (int) Math.Floor(Math.Abs(request.Point) * 0.1);
                                            this._userService.UpdateUser(request.Houses.Users);
                                            transaction.Commit();

                                            await new ChatHub(this._signalContext).SendNotification(
                                                group: Crypto.EncodeKey(notification.IdUser.ToString(), Crypto.Salt(this._configuration)),
                                                target: TargetSignalR.Notification(),
                                                model: new NotificationViewModel(notification, host));
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

        [HttpPost("/Rating/Create")]
        public async Task<IActionResult> CreateRatingAsync([FromBody] CreateRatingViewModel feedBack)
        {
            return await this.CreateAsync(feedBack);
        }

        [HttpPost("/api/Rating/Create")]
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
                var model = this._feedBackService.GetRatingById(IdUser, feedBack.Id);
                if(model != null)
                {
                    model.IncludeAll(this._context);
                    using (var transaction = this._context.Database.BeginTransaction())
                    {
                        try
                        {
                            model.Update(feedBack);
                            this._feedBackService.Update(model);
                            if (model.Houses != null)
                            {
                                List<int> ints = this._houseService.CalRating(model.Houses.Id);
                                model.Houses.Rating = Math.Ceiling((double)ints.First() / ints.Last());
                                this._houseService.UpdateHouse(model.Houses);
                            }

                            if (model.UserRated != null)
                            {
                                List<int> ints = this._userService.CalRate(model.UserRated.Id);
                                model.UserRated.UserRating = Math.Ceiling((double)ints.First() / ints.Last());
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
        [HttpPost("/Rating/Update")]
        public IActionResult UpdateRating([FromBody] EditRatingViewModel feedBack)
        {
            return this.Edit(feedBack);
        }

        [HttpPost("/api/Rating/Update")]
        public IActionResult ApiUpdateRating([FromBody] EditRatingViewModel feedBack)
        {
            return this.Edit(feedBack);
        }

        [HttpGet("/Rating/GetByHouse")]        
        public IActionResult RatingByHouse(int Id)
        {
            ListRating listRating = new ListRating();
            List<FeedBack> model = this._feedBackService.GetRatingByHouse(Id);
            listRating.OverView = new FrameRating()
            {
                OneStar = model.Where(m => m.Rating == 1).ToList().Count(),
                TwoStar = model.Where(m => m.Rating == 2).ToList().Count(),
                ThreeStar = model.Where(m => m.Rating == 3).ToList().Count(),
                FourStar = model.Where(m => m.Rating == 4).ToList().Count(),
                FiveStar = model.Where(m => m.Rating == 5).ToList().Count()
            };
            listRating.Rating = this._feedBackService.GetRatingByHouse(model, this.GetWebsitePath(), Crypto.Salt(this._configuration));
            return PartialView("./Views/Rating/_ListRating.cshtml", listRating);
        }
        [HttpGet("/api/Rating/GetByHouse")]
        public IActionResult ApiRatingByHouse(int Id)
        {
            return Json(new
            {
                Status = 200,
                Data = this._feedBackService.GetRatingByHouse(this._feedBackService.GetRatingByHouse(Id), this.GetWebsitePath(), Crypto.Salt(this._configuration))
            });
        }
        [HttpGet("/Rating/FrameRate")]
        public IActionResult GetFrameRate(int IdHouse)
        {
            List<FeedBack> model = this._feedBackService.GetRatingByHouse(IdHouse);
            return Json(new
            {
                Status = 200,
                Data = new FrameRating()
                {
                    OneStar = model.Where(m => m.Rating == 1).ToList().Count(),
                    TwoStar = model.Where(m => m.Rating == 2).ToList().Count(),
                    ThreeStar = model.Where(m => m.Rating == 3).ToList().Count(),
                    FourStar = model.Where(m => m.Rating == 4).ToList().Count(),
                    FiveStar = model.Where(m => m.Rating == 5).ToList().Count()
                }
            });
        }
    }
}
