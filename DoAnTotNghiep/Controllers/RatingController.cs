﻿using System;
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
    public class RatingController: BaseController
    {
        private readonly DoAnTotNghiepContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHubContext<ChatHub> _signalContext;

        public RatingController(DoAnTotNghiepContext context
                        , IConfiguration configuration
                        , IHostEnvironment environment
                        , IHubContext<ChatHub> signalContext) : base(environment)
        {
            _context = context;
            _configuration = configuration;
            _signalContext = signalContext;
        }
        
        private async Task<IActionResult> CreateAsync(CreateRatingViewModel feedBack)
        {
            if (ModelState.IsValid)
            {
                int IdUser = this.GetIdUser();
                var request = this._context.Requests
                                            .Include(m => m.CheckOuts)
                                            .Where(m => m.Id == feedBack.IdRequest
                                                        && (m.Status == (int)StatusRequest.CHECK_OUT || 
                                                            m.Status == (int)StatusRequest.CHECK_IN
                                                            && m.CheckOuts != null
                                                            && m.CheckOuts.Any(c => c.IdUser == IdUser)))
                                            .FirstOrDefault();
                if(request != null)
                {
                    if (!this._context.Entry(request).Reference(m => m.Houses).IsLoaded)
                    {
                        this._context.Entry(request).Reference(m => m.Houses).Load();
                    }
                    if(request.Houses != null)
                    {
                        if (!this._context.Entry(request.Houses).Reference(m => m.Users).IsLoaded)
                        {
                            this._context.Entry(request.Houses).Reference(m => m.Users).Load();
                        }
                        if(request.Houses.Users != null)
                        {
                            this._context.Entry(request).Reference(m => m.Users).Load();

                            if (request.Users != null)
                            {
                                if (request.IdUser == IdUser)
                                {
                                    using (var transaction = this._context.Database.BeginTransaction())
                                    {
                                        try
                                        {
                                            //nhà người chủ
                                            request.Houses.Rating = Math.Ceiling(((request.Houses.Rating * request.Houses.NumberOfRating) + feedBack.RatingHouse) / (request.Houses.NumberOfRating + 1));
                                            request.Houses.NumberOfRating += 1;
                                            this._context.Houses.Update(request.Houses);
                                            this._context.SaveChanges();

                                            //gửi thông báo
                                            Notification notification = new Notification()
                                            {
                                                IdType = request.Houses.Id,
                                                IdUser = request.Houses.Users.Id,
                                                Title = NotificationType.RatingTitle,
                                                Content = request.Houses.Name + " có đánh giá mới",
                                                CreatedDate = DateTime.Now,
                                                IsSeen = false,
                                                ImageUrl = this.GetWebsitePath() + "/Image/house-demo.png",
                                                Type = NotificationType.RATING
                                            };
                                            this._context.Notifications.Add(notification);
                                            this._context.SaveChanges();

                                            ChatHub chatHub = new ChatHub(this._signalContext);
                                            await chatHub.SendNotification(
                                                group: Crypto.EncodeKey(notification.IdUser.ToString(), Crypto.Salt(this._configuration)),
                                                target: TargetSignalR.Notification(),
                                                model: new NotificationViewModel(notification));

                                            //người chủ
                                            request.Houses.Users.UserRating = Math.Ceiling(((request.Houses.Users.UserRating * request.Houses.Users.NumberUserRating) + feedBack.RatingUser) / (request.Houses.Users.NumberUserRating + 1));
                                            request.Houses.Users.NumberUserRating += 1;
                                            this._context.Users.Update(request.Houses.Users);
                                            this._context.SaveChanges();

                                            //tồn tại hoặc trao đổi = tiền thì đổi status request
                                            if(request.IdSwapHouse != null)
                                            {
                                                this._context.Entry(request).Collection(m => m.FeedBacks).Load();
                                            }
                                            if(request.IdSwapHouse == null || request.FeedBacks != null && request.FeedBacks.Count() > 0)
                                            {
                                                request.Status = (int)StatusRequest.ENDED;
                                                request.UpdatedDate = DateTime.Now;
                                                this._context.Requests.Update(request);
                                                this._context.SaveChanges();
                                            }

                                            //thêm vào
                                            FeedBack model = new FeedBack();
                                            model.Create(feedBack, 
                                                            IdUser, 
                                                            request.Houses.Users.Id, 
                                                            request.Houses.Id);
                                            this._context.FeedBacks.Add(model);
                                            this._context.SaveChanges();

                                            //tăng điểm người dùng
                                            request.Users.BonusPoint += (int)Math.Floor(Math.Abs(request.Point) * 0.1);
                                            this._context.Users.Update(request.Users);
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
                                else if(request.Houses.Users.Id == IdUser)
                                {
                                    if(request.IdSwapHouse != null)
                                    {
                                        this._context.Entry(request).Reference(m => m.SwapHouses).Load();
                                    }
                                    using (var transaction = this._context.Database.BeginTransaction())
                                    {
                                        try
                                        {
                                            if (request.SwapHouses != null)
                                            {
                                                request.SwapHouses.Rating = Math.Ceiling(((request.SwapHouses.Rating * request.SwapHouses.NumberOfRating) + feedBack.RatingHouse) / (request.SwapHouses.NumberOfRating + 1));
                                                request.SwapHouses.NumberOfRating += 1;
                                                this._context.Houses.Update(request.SwapHouses);
                                                this._context.SaveChanges();
                                            }

                                            //gửi thông báo
                                            Notification notification = new Notification()
                                            {
                                                IdType = request.Id,
                                                IdUser = request.IdUser,
                                                Title = NotificationType.RatingTitle,
                                                Content = "Bạn có đánh giá mới",
                                                CreatedDate = DateTime.Now,
                                                IsSeen = false,
                                                ImageUrl = this.GetWebsitePath() + "/Image/house-demo.png",
                                                Type = NotificationType.RATING
                                            };
                                            this._context.Notifications.Add(notification);
                                            this._context.SaveChanges();

                                            ChatHub chatHub = new ChatHub(this._signalContext);
                                            await chatHub.SendNotification(
                                                group: Crypto.EncodeKey(notification.IdUser.ToString(), Crypto.Salt(this._configuration)),
                                                target: TargetSignalR.Notification(),
                                                model: new NotificationViewModel(notification));

                                            request.Users.UserRating = Math.Ceiling(((request.Users.UserRating * request.Users.NumberUserRating) + feedBack.RatingUser) / (request.Users.NumberUserRating + 1));
                                            request.Users.NumberUserRating += 1;
                                            this._context.Users.Update(request.Users);
                                            this._context.SaveChanges();

                                            this._context.Entry(request).Collection(m => m.FeedBacks).Load();
                                            if (request.FeedBacks != null && request.FeedBacks.Count() > 0)
                                            {
                                                request.Status = (int)StatusRequest.ENDED;
                                                request.UpdatedDate = DateTime.Now;
                                                this._context.Requests.Update(request);
                                                this._context.SaveChanges();
                                            }

                                            FeedBack model = new FeedBack();
                                            model.Create(feedBack, request.Houses.Users.Id, request.Users.Id, request.SwapHouses == null ? null : request.SwapHouses.Id);
                                            this._context.FeedBacks.Add(model);
                                            this._context.SaveChanges();

                                            //tăng điểm người dùng
                                            request.Houses.Users.BonusPoint += (int) Math.Floor(Math.Abs(request.Point) * 0.1);
                                            this._context.Users.Update(request.Houses.Users);
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
                var model = this._context.FeedBacks.Where(m => m.Id == feedBack.Id).FirstOrDefault();
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
                                                    ((model.Houses.Rating * model.Houses.NumberOfRating - model.Rating) + feedBack.RatingHouse)
                                                                / (model.Houses.NumberOfRating)
                                                    );
                                    Context.Houses.Update(model.Houses);
                                    Context.SaveChanges();
                                }

                                if (model.UserRated != null)
                                {
                                    model.UserRated.UserRating = Math.Ceiling(
                                                    ((model.UserRated.UserRating * model.UserRated.NumberUserRating - model.RatingUser) + feedBack.RatingUser)
                                                        / (model.UserRated.NumberUserRating)
                                                    );
                                    Context.Users.Update(model.UserRated);
                                    Context.SaveChanges();
                                }
                                
                                model.Update(feedBack);
                                Context.FeedBacks.Update(model);
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



        //Detail
        private List<DetailRatingWithUser> GetRatingByHouse(int IdHouse)
        {
            List<DetailRatingWithUser> model = new List<DetailRatingWithUser>();

            var house = this._context.Houses
                                    .Where(m => m.Id == IdHouse
                                            && m.Status == (int)StatusHouse.VALID)
                                    .FirstOrDefault();

            if(house != null)
            {
                this._context.Entry(house)
                            .Collection(m => m.FeedBacks)
                            .Query()
                            .Load();
                if(house.FeedBacks != null)
                {
                    List<FeedBack> feedBacks = house.FeedBacks.ToList();
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
                }
            }

            return model;
        }
        [HttpGet("/Rating/GetByHouse")]        
        public IActionResult RatingByHouse(int Id)
        {
            return Json(new
            {
                Status = 200,
                Data = this.GetRatingByHouse(Id)
            });
        }
        [HttpGet("/api/Rating/GetByHouse")]
        public IActionResult ApiRatingByHouse(int Id)
        {
            return Json(new
            {
                Status = 200,
                Data = this.GetRatingByHouse(Id)
            });
        }


        //private List<DetailRatingWithHouse> GetRatingByRequest(int IdRequest)
        //{
        //    List<DetailRatingWithHouse> model = new List<DetailRatingWithHouse>();
        //    int IdUser = this.GetIdUser();
        //    var feedBack = this._context.FeedBacks
        //                            .Where(m => m.IdRequest == IdRequest
        //                                    && m.IdUser == IdUser)
        //                            .FirstOrDefault();

        //    if (feedBack != null)
        //    {
        //        this._context.Entry(feedBack)
        //                    .Reference(m => m.Houses)
        //                    .Query()
        //                    .Load();

        //    }

        //    return model;
        //}
        //Delete
        //cho xóa không?
    }
}