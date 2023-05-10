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

namespace DoAnTotNghiep.Controllers
{
    [Authorize(Roles = Role.Member)]
    public class WaitingRatingController: BaseController
    {
        private readonly DoAnTotNghiepContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHubContext<ChatHub> _signalContext;

        public WaitingRatingController(DoAnTotNghiepContext context
                        , IConfiguration configuration
                        , IHostEnvironment environment
                        , IHubContext<ChatHub> signalContext) : base(environment)
        {
            _context = context;
            _configuration = configuration;
            _signalContext = signalContext;
        }

        [HttpGet("/CircleRating/Form")]
        public IActionResult FormCreateRating(int IdRequest, int? IdWaitingRating)
        {
            var request = this._context.CircleExchangeHouses
                                .Where(m => m.Id == IdRequest && m.Status != (int) StatusWaitingRequest.DISABLE)
                                .FirstOrDefault();
            if (request == null) return NotFound();

            if(IdWaitingRating != null)
            {
                var rating = this._context.FeedBackOfCircles
                                            .Where(m => m.Id == IdWaitingRating.Value).FirstOrDefault();
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
                var request = (from cr in this._context.CircleExchangeHouses
                               join cru in this._context.CircleExchangeHouseOfUsers on cr.Id equals cru.IdCircleExchangeHouse
                               join wr in this._context.RequestsInCircleExchangeHouses on cr.Id equals wr.IdCircleExchangeHouse
                               where cru.IdUser == IdUser && cr.Id == feedBack.IdRequest
                               select wr)
                                            .FirstOrDefault();
                if(request != null)
                {
                    if (!this._context.Entry(request).Reference(m => m.WaitingRequests).IsLoaded)
                    {
                        this._context.Entry(request).Reference(m => m.WaitingRequests).Load();
                    }

                    if(request.WaitingRequests != null)
                    {

                        var rq = (from cr in this._context.CircleExchangeHouses
                                  join rcr in this._context.RequestsInCircleExchangeHouses on cr.Id equals rcr.IdCircleExchangeHouse
                                  join wr in this._context.WaitingRequests on rcr.IdWaitingRequest equals wr.Id
                                  join h in this._context.Houses on wr.IdHouse equals h.Id
                                  where cr.Id == feedBack.IdRequest && h.IdCity == request.WaitingRequests.IdCity
                                  select wr).FirstOrDefault();
                        if(rq != null)
                        {

                            if (!this._context.Entry(rq).Reference(m => m.Houses).IsLoaded)
                            {
                                this._context.Entry(rq).Reference(m => m.Houses).Load();
                            }
                            if (rq.Houses != null)
                            {
                                if (!this._context.Entry(rq.Houses).Reference(m => m.Users).IsLoaded)
                                {
                                    this._context.Entry(rq.Houses).Reference(m => m.Users).Load();
                                }
                                if (rq.Houses.Users != null)
                                {
                                    this._context.Entry(request.WaitingRequests).Reference(m => m.Users).Load();


                                    if (request.WaitingRequests.Users != null)
                                    {
                                        using (var transaction = this._context.Database.BeginTransaction())
                                        {
                                            try
                                            {
                                                //nhà người chủ
                                                rq.Houses.Rating = Math.Ceiling(((rq.Houses.Rating * rq.Houses.NumberOfRating) + (feedBack.RatingHouse == null ? 0 : feedBack.RatingHouse.Value)) / (rq.Houses.NumberOfRating + 1) * 10) / 10;
                                                rq.Houses.NumberOfRating += 1;
                                                this._context.Houses.Update(rq.Houses);
                                                this._context.SaveChanges();
                                                //người chủ
                                                rq.Houses.Users.UserRating = Math.Ceiling(((rq.Houses.Users.UserRating * rq.Houses.Users.NumberUserRating) + feedBack.RatingUser) / (rq.Houses.Users.NumberUserRating + 1) * 10) / 10;
                                                rq.Houses.Users.NumberUserRating += 1;
                                                this._context.Users.Update(rq.Houses.Users);
                                                this._context.SaveChanges();
                                                request.Status = (int)StatusWaitingRequest.ENDED;

                                                this._context.RequestsInCircleExchangeHouses.Update(request);
                                                this._context.SaveChanges();

                                                //gửi thông báo
                                                Notification notification = new Notification()
                                                {
                                                    IdType = rq.Id,
                                                    IdUser = rq.Houses.Users.Id,
                                                    Title = NotificationType.RatingTitle,
                                                    Content = rq.Houses.Name + " có đánh giá mới",
                                                    CreatedDate = DateTime.Now,
                                                    IsSeen = false,
                                                    ImageUrl = NotificationImage.Alert,
                                                    Type = NotificationType.CIRCLE_RATING
                                                };
                                                this._context.Notifications.Add(notification);
                                                this._context.SaveChanges();

                                                ChatHub chatHub = new ChatHub(this._signalContext);
                                                await chatHub.SendNotification(
                                                    group: Crypto.EncodeKey(notification.IdUser.ToString(), Crypto.Salt(this._configuration)),
                                                    target: TargetSignalR.Notification(),
                                                    model: new NotificationViewModel(notification, this.GetWebsitePath()));


                                                //thêm vào
                                                FeedBackOfCircle model = new FeedBackOfCircle().Create(feedBack,
                                                                IdUser,
                                                                rq.Houses.Users.Id,
                                                                rq.Houses.Id);
                                                this._context.FeedBackOfCircles.Add(model);
                                                this._context.SaveChanges();

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
                                            Message = "Không lưu được"
                                        });
                                    }
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
                var model = this._context.FeedBackOfCircles.Where(m => m.Id == feedBack.Id && m.IdUserRated == IdUser).FirstOrDefault();
                if(model != null)
                {
                    this._context.Entry(model).Reference(m => m.Houses).Load();
                    this._context.Entry(model).Reference(m => m.UserRated).Load();
                    using (var Context = this._context)
                    {
                        using (var transaction = Context.Database.BeginTransaction())
                        {
                            try
                            {
                                if(model.Houses != null)
                                {
                                    model.Houses.Rating = Math.Ceiling(
                                                    ((model.Houses.Rating * model.Houses.NumberOfRating - model.RateHouse) + (feedBack.RatingHouse == null ? 0 : feedBack.RatingHouse.Value))
                                                                / (model.Houses.NumberOfRating) * 10
                                                    ) / 10;
                                    Context.Houses.Update(model.Houses);
                                    Context.SaveChanges();
                                }

                                if (model.UserRated != null)
                                {
                                    model.UserRated.UserRating = Math.Ceiling(
                                                    ((model.UserRated.UserRating * model.UserRated.NumberUserRating - model.RateUser) + feedBack.RatingUser)
                                                        / (model.UserRated.NumberUserRating) * 10
                                                    ) / 10;
                                    Context.Users.Update(model.UserRated);
                                    Context.SaveChanges();
                                }
                                
                                model.Update(feedBack);
                                Context.FeedBackOfCircles.Update(model);
                                Context.SaveChanges();

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

        /*
        //Detail
        private List<DetailRatingWithUser> GetRatingByHouse(List<FeedBack> feedBacks)
        {
            List<DetailRatingWithUser> model = new List<DetailRatingWithUser>();

            string host = this.GetWebsitePath();
            byte[] salt = Crypto.Salt(this._configuration);
            foreach(var item in feedBacks)
            {
                this._context.Entry(item).Reference(m => m.Users).Load();
                if(item.Users != null)
                {
                    DetailRatingViewModel rating = new DetailRatingViewModel(item);
                    UserInfo user = new UserInfo(item.Users, salt, host);
                    model.Add(new DetailRatingWithUser() { User = user, FeedBack = rating });
                }
            }

            return model;
        }
        [HttpGet("/Rating/GetByHouse")]        
        public IActionResult RatingByHouse(int Id)
        {
            ListRating listRating = new ListRating();
            List<FeedBack> model = this.GetFeedBack(Id);
            listRating.OverView = new FrameRating()
            {
                OneStar = model.Where(m => m.Rating == 1).ToList().Count(),
                TwoStar = model.Where(m => m.Rating == 2).ToList().Count(),
                ThreeStar = model.Where(m => m.Rating == 3).ToList().Count(),
                FourStar = model.Where(m => m.Rating == 4).ToList().Count(),
                FiveStar = model.Where(m => m.Rating == 5).ToList().Count()
            };
            listRating.Rating = this.GetRatingByHouse(model);
            return PartialView("./Views/Rating/_ListRating.cshtml", listRating);
        }
        [HttpGet("/api/Rating/GetByHouse")]
        public IActionResult ApiRatingByHouse(int Id)
        {
            return Json(new
            {
                Status = 200,
                Data = this.GetRatingByHouse(this.GetFeedBack(Id))
            });
        }

        private List<FeedBack> GetFeedBack(int IdHouse)
        {
            List<FeedBack> model = new List<FeedBack>();

            var house = this._context.Houses
                                    .Where(m => m.Id == IdHouse
                                            && m.Status == (int)StatusHouse.VALID)
                                    .FirstOrDefault();

            if (house != null)
            {
                this._context.Entry(house)
                            .Collection(m => m.FeedBacks)
                            .Query()
                            .Load();
                if(house.FeedBacks != null)
                {
                    model.AddRange(house.FeedBacks.ToList());
                }
            }

            return model;
        }
        [HttpGet("/Rating/FrameRate")]
        public IActionResult GetFrameRate(int IdHouse)
        {
            List<FeedBack> model = this.GetFeedBack(IdHouse);
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
        }*/
    }
}
