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
using DoAnTotNghiep.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Hangfire;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace DoAnTotNghiep.Controllers
{
    [Authorize(Roles = Role.Member)]
    public class RequestController: BaseController
    {
        private readonly DoAnTotNghiepContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHubContext<ChatHub> _signalContext;
        private readonly ILogger<TimedHostedService> _timerLog;

        public RequestController(DoAnTotNghiepContext context, 
                                IConfiguration configuration, 
                                IHostEnvironment environment,
                                IHubContext<ChatHub> signalContext,
                                ILogger<TimedHostedService> _timerLog,
                                IServiceProvider service) : base(environment)
        {
            _context = context;
            _configuration = configuration;
            _signalContext = signalContext;
            this._timerLog = _timerLog;
        }

        private DetailHouseViewModel CreateDetailsHouse(House house)
        {
            byte[] salt = Crypto.Salt(this._configuration);
            string host = this.GetWebsitePath();
            if (house.Users != null)
            {
                this._context.Entry(house.Users).Collection(m => m.Houses).Query().Where(m =>m.Status == (int)StatusHouse.VALID).Load();
            }
            house.IncludeLocation(this._context);
            DetailHouseViewModel model = new DetailHouseViewModel(house, salt, house.Users, host);
            if (house.FileOfHouses != null)
            {
                foreach (var f in house.FileOfHouses)
                {
                    this._context.Entry(f).Reference(m => m.Files).Load();
                    if (f.Files != null)
                    {
                        model.Images.Add(new ImageBase(f.Files, host));
                        break;
                    }
                }
            }
            return model;
        }
        //làm 3 cái tab => show detail như form

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
                model.UserHouses.AddRange(GetListHouse(Id, host, salt));
                //list house của mình
                int IdUser = this.GetIdUser();
                model.MyHouses.AddRange(GetListHouse(IdUser, host, salt));
            }
            return PartialView("./Views/Request/_RequestForm.cshtml", model);
        }

        private List<DetailHouseViewModel> GetListHouse(int IdUser, string host, byte[] salt)
        {
            List<DetailHouseViewModel> model = new List<DetailHouseViewModel>();
            var userHouses = this._context.Houses.Where(m => m.IdUser == IdUser && m.Status == (int)StatusHouse.VALID).ToList();
            foreach (var item in userHouses)
            {
                model.Add(this.CreateDetailHouse(item, host, salt));
            }
            return model;
        }
        private DetailHouseViewModel CreateDetailHouse(House item, string host, byte[] salt)
        {
            this._context.Entry(item).Collection(m => m.FileOfHouses).Query().Load();
            this._context.Entry(item).Collection(m => m.FeedBacks).Query().Load();
            this._context.Entry(item).Collection(m => m.Requests).Query().Load();

            item.IncludeLocation(this._context);
            DetailHouseViewModel m = new DetailHouseViewModel(item, salt, item.Users, host);
            if (item.FileOfHouses != null)
            {
                foreach (var f in item.FileOfHouses)
                {
                    this._context.Entry(f).Reference(m => m.Files).Load();
                    if (f.Files != null)
                    {
                        m.Images.Add(new ImageBase(f.Files, host));
                        break;
                    }
                }
            }
            return m;
        }




        [HttpGet("/Request/FormWithHouseId")]//chưa xong
        public IActionResult RequestForm(int IdHouse)
        {
            ModelRequestForm model = new ModelRequestForm();
            byte[] salt = Crypto.Salt(this._configuration);
            string host = this.GetWebsitePath();
            //list house của người đó
            var userHouses = this._context.Houses.Where(m => m.Id == IdHouse && m.Status == (int)StatusHouse.VALID).ToList();
            foreach (var item in userHouses)
            {
                model.UserHouses.Add(this.CreateDetailHouse(item, host, salt));
            }

            //list house của mình
            int IdUser = this.GetIdUser();
            var myHouses = this._context.Houses.Where(m => m.IdUser == IdUser && m.Status == (int)StatusHouse.VALID).ToList();
            foreach (var item in myHouses)
            {
                model.MyHouses.Add(this.CreateDetailHouse(item, host, salt));
            }
            return PartialView("./Views/Request/_RequestForm.cshtml", model);
        }
        [HttpGet("/Request/EditForm")]//chưa xong
        public IActionResult EditForm(int IdRequest)
        {
            return PartialView("./Views/Request/_RequestEditForm.cshtml");
        }

        //nếu như đã có yêu cầu tới nhà của swapHouse 1 request => chuyển sang accept
        //xóa những yêu cầu đã gửi trc đó k?
        //tạo yêu cầu
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
                var user = this._context.Users.Where(m => m.Id == IdUser).FirstOrDefault();
                if(user == null)
                    return BadRequest(new
                    {
                        Status = 400,
                        Message = "Yêu cầu không thể thực hiện"
                    });
                //kiểm tra nhà tồn tại không?
                var h = this._context.Houses
                                    .Include(m => m.Requests)
                                    .Include(m => m.Users)
                                    .Where(m => m.Id == model.IdHouse 
                                                && m.Status == (int) StatusHouse.VALID
                                                && m.IdUser != IdUser)
                                    .FirstOrDefault();
                if (h != null)
                {
                    House house = h;
                    if(house.Requests != null 
                            && house.Requests.Any(m => StatusRequestStr.IsUnValidHouse(m.Status)
                                                        && (!(m.StartDate >= model.EndDate || m.EndDate <= model.StartDate))))
                    {
                        return BadRequest(new
                        {
                            Status = 400,
                            Message = "Yêu cầu không thể thực hiện vì nhà đang trao đổi"
                        });
                    }

                    if (model.IdSwapHouse != null)
                    {
                        //cần kiểm tra nhà đã chứa request của nhà này ch?
                        var sh = this._context.Houses
                                    .Include(m => m.Requests)
                                    .Include(m => m.Users)
                                    .Where(m => m.Id == model.IdSwapHouse.Value
                                                && m.Id != house.Id
                                                && m.IdUser == IdUser
                                                && m.Status == (int)StatusHouse.VALID)
                                    .FirstOrDefault();
                        if (sh != null)
                        {

                            if (sh.Requests != null
                            && sh.Requests.Any(m => StatusRequestStr.IsUnValidHouse(m.Status)
                                                        && (!(m.StartDate >= model.EndDate || m.EndDate <= model.StartDate))))
                            {
                                return BadRequest(new
                                {
                                    Status = 400,
                                    Message = "Yêu cầu không thể thực hiện vì nhà đang trao đổi"
                                });
                            }

                            Request? data = await this.CreateRequestModelAsync(model, house, user);
                            if (data != null)
                            {
                                return Json(new
                                {
                                    Status = 200,
                                    Message = "Khởi tạo thành công",
                                    Data = new { }
                                });
                            }
                        }
                    }
                    else
                    {
                        if ((user.Point + user.BonusPoint - user.PointUsing) < model.Price)
                        {
                            return BadRequest(new
                            {
                                Status = 400,
                                Message = "Yêu cầu không thể thực hiện vì thiếu tiền"
                            });
                        }
                        Request? data = await this.CreateRequestModelAsync(model, house, user);
                        if (data != null)
                        {
                            return Json(new
                            {
                                Status = 200,
                                Message = "Khởi tạo thành công",
                                Data = new { }
                            });
                        }
                    }
                }
            }
            
            return BadRequest(new
            {
                Status = 400,
                Message = "Yêu cầu không thể thực hiện"
            });
        }
        private async Task<Request?> CreateRequestModelAsync(RequestViewModel model, House house, User user)
        {
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
                Status = (int) StatusRequest.WAIT_FOR_SWAP,
                IdUser = this.GetIdUser()
            };
            var transaction = this._context.Database.BeginTransaction();
            try
            {
                this._context.Requests.Add(request);
                this._context.SaveChanges();

                //gui target
                this._context.Entry(house).Reference(m => m.Users).Query().Load();
                if (house.Users != null)
                {
                    ChatHub chatHub = new ChatHub(this._signalContext);

                    Notification notification = new Notification()
                    {
                        Title = NotificationType.RequestTitle,
                        Content = user.FirstName
                                    + " " + user.LastName
                                    + " vừa yêu cầu đổi nhà "
                                    + house.Name,
                        CreatedDate = DateTime.Now,
                        Type = NotificationType.REQUEST,
                        IdUser = house.Users.Id,
                        IdType = request.Id,
                        IsSeen = false,
                        ImageUrl = "/Image/house-demo.png"
                    };

                    this._context.Notifications.Add(notification);
                    this._context.SaveChanges();

                    await chatHub.SendNotification(
                        group: Crypto.EncodeKey(user.Id.ToString(), Crypto.Salt(this._configuration)),
                        target: TargetSignalR.Notification(), 
                        model: new NotificationViewModel(notification, this.GetWebsitePath()));
                }
                transaction.Commit();
                return request;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                transaction.Rollback();
            }
            return null;
        }


        //chỉnh sửa yêu cầu
        //chưa dùng
        [HttpPost("/Request/Update")]
        public IActionResult UpdateRequest([FromBody] EditRequestViewModel model)
        {
            return this.Edit(model);
        }
        [HttpPost("/api/Request/Update")]
        public IActionResult ApiUpdateRequest([FromBody] EditRequestViewModel model)
        {
            return this.Edit(model);
        }
        private IActionResult Edit(EditRequestViewModel model)
        {
            if (ModelState.IsValid)
            {
                var h = this._context.Houses
                                     .Where(m => m.Id == model.IdHouse 
                                                    && m.Status == (int) StatusHouse.VALID)
                                     .FirstOrDefault();
                if (h != null)
                {
                    House house = h;
                    if (model.IdSwapHouse != null)
                    {
                        var sh = this._context.Houses
                                              .Where(m => m.Id == model.IdSwapHouse.Value 
                                                            && m.Id != house.Id
                                                             && m.Status == (int)StatusHouse.VALID)
                                              .FirstOrDefault();
                        if (sh != null)
                        {
                            Request? data = this.EditRequestModel(model);
                            if (data != null)
                            {
                                return Json(new
                                {
                                    Status = 200,
                                    Message = "Cập nhật thành công",
                                    Data = new {}
                                });
                            }
                        }
                    }
                    else
                    {
                        Request? data = this.EditRequestModel(model);
                        if(data != null)
                        {
                            return Json(new
                            {
                                Status = 200,
                                Message = "Cập nhật thành công",
                                Data = new { }
                            });
                        }
                    }
                }
            }

            return BadRequest(new
            {
                Status = 400,
                Message = "Yêu cầu không thể thực hiện"
            });
        }
        private Request? EditRequestModel(EditRequestViewModel model)
        {
            int IdUser = this.GetIdUser();
            var rq = this._context.Requests
                                    .Where(m => m.Id == model.Id 
                                                && m.Status == (int) StatusRequest.WAIT_FOR_SWAP
                                                && m.IdUser == IdUser);
            if (rq.Any())
            {
                try
                {
                    Request request = rq.First();
                    request.UpdateRequest(model);
                    this._context.Requests.Update(request);
                    this._context.SaveChanges();
                    return request;
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return null;
        }
        

        //xóa yêu cầu
        [HttpPost("/Request/Delete")]
        public IActionResult DeleteRequest([FromBody] int Id)
        {
            return this.Remove(Id);
        }
        [HttpPost("/api/Request/Delete")]
        public IActionResult ApiDeleteRequest([FromBody] int Id)
        {
            return this.Remove(Id);
        }
        private IActionResult Remove(int Id)
        {
            int IdUser = this.GetIdUser();
            var rq = this._context.Requests.Where(m => m.Id == Id && (m.Status < (int) StatusRequest.CHECK_IN)).FirstOrDefault();
            if (rq != null)
            {
                try
                {
                    this._context.Requests.Remove(rq);
                    this._context.SaveChanges();
                    return Json(new
                    {
                        Status = 200,
                        Message = "Xóa thành công"
                    });
                }
                catch(Exception ex ) { 
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
            int IdUser = this.GetIdUser();
            var requests = this._context.Houses.Include(m => m.Requests)
                                     .Where(m => m.Id == IdHouse && m.IdUser == IdUser)
                                     .Select(m => m.Requests)
                                     .ToList();
            return Json(new
            {
                Status = 200,
                Data = requests.Count()
            });
        }

        [HttpGet("/Request/GetByHouse")]
        public IActionResult GetListRequestWithHouse(int IdHouse) {

            return PartialView("./Views/Request/_ListRequestToHouse.cshtml", this.GetNotifyRequests(IdHouse));
        }
        [HttpGet("/api/Request/GetByHouse")]
        public IActionResult ApiGetListRequestWithHouse(int IdHouse)
        {
            return Json(new
            {
                Status = 200,
                Data = this.GetNotifyRequests(IdHouse)
            });
        }
        private List<NotifyRequest> GetNotifyRequests(int IdHouse)
        {
            int IdUser = this.GetIdUser();
            var house = this._context.Houses
                                     .Where(m => m.Id == IdHouse 
                                                && m.IdUser == IdUser)
                                     .FirstOrDefault();
            List<NotifyRequest> model = new List<NotifyRequest>();
            if (house != null)
            {
                this._context.Entry(house).Collection(m => m.Requests)
                    .Query().Where(m => m.Status == (int)StatusRequest.WAIT_FOR_SWAP).Load();
                if(house.Requests != null)
                {
                    DoAnTotNghiepContext Context = this._context;
                    byte[] salt = Crypto.Salt(this._configuration);
                    string host = this.GetWebsitePath();
                    foreach (var item in house.Requests)
                    {
                        Context.Entry(item).Reference(m => m.Users).Query().Load();
                        if (item.Users != null)
                        {
                            DetailHouseViewModel? swapHouse = null;
                            if (item.IdSwapHouse != null)
                            {
                                Context.Entry(item).Reference(m => m.SwapHouses).Query().Load();
                                if (item.SwapHouses != null)
                                {
                                    item.SwapHouses.IncludeLocation(this._context);
                                    swapHouse = new DetailHouseViewModel(item.SwapHouses, salt, item.Users, host);
                                }
                            }
                            User? inputUser = null;
                            if (item.IdUser == IdUser)
                            {
                                inputUser = item.Houses.Users;
                            }
                            else
                            {
                                inputUser = item.Users;
                            }
                            DetailRequestViewModel request = new DetailRequestViewModel(item, inputUser, salt, host);

                            model.Add(new NotifyRequest()
                            {
                                Request = request,
                                SwapHouse = swapHouse
                            });
                        }
                    }
                }
            }
            return model;
        }

        //lấy danh sách yêu cầu bản thân đã gửi
        [HttpGet("/Request/GetRequestSent")]
        public IActionResult GetListRequestSent(int? Status)
        {
            return Json(new
            {
                Status = 200,
                Data = this.GetAllRequestsSent(Status)
            });
        }
        [HttpGet("/api/Request/GetRequestSent")]
        public IActionResult ApiGetListRequestSent(int? Status)
        {
            return Json(new
            {
                Status = 200,
                Data = this.GetAllRequestsSent(Status)
            });
        }
        private List<DetailRequest> GetAllRequestsSent(int? Status)
        {
            int IdUser = this.GetIdUser();
            var requests = this._context.Requests
                                        .Where(m => m.IdUser == IdUser).ToList();
            List<DetailRequest> model = new List<DetailRequest>();
            DoAnTotNghiepContext Context = this._context;

            byte[] salt = Crypto.Salt(this._configuration);
            string host = this.GetWebsitePath();
            foreach (var item in requests)
            {
                if(item != null)
                {
                    DetailRequest? request = this.CreateDetailRequest(item, Status);
                    if (request != null)
                    {
                        model.Add(request);
                    }
                }
            }
            
            return model;
        }

        //chi tiết yêu cầu
        [HttpGet("/Request/Detail")]
        public IActionResult GetDetailRequest(int Id)
        {
            DetailRequest? model = this.DetailRequest(Id);
            if (model == null) return NotFound();
            return PartialView("./Views/Request/_RequestDetail.cshtml", model);
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
            //int IdUser = this.GetIdUser();
            var item = this._context.Requests
                                        .Where(m => m.Id == IdRequest).FirstOrDefault();
            
            if(item != null)
            {
                return this.CreateDetailRequest(item, null);
            }
            return null;
        }
        private DetailRequest? CreateDetailRequest(Request item, int? Status)
        {
            byte[] salt = Crypto.Salt(this._configuration);
            string host = this.GetWebsitePath();
            int IdUser = this.GetIdUser();
            this._context.Entry(item).Reference(m => m.Houses).Query().Load();
            this._context.Entry(item).Collection(m => m.FeedBacks).Query().Where(m => m.IdUser == IdUser).Load();
            this._context.Entry(item).Collection(m => m.CheckOuts).Query().Where(m => m.IdUser == IdUser).Load();
            this._context.Entry(item).Collection(m => m.CheckIns).Query().Where(m => m.IdUser == IdUser).Load();
            item.CheckStatus(item);
            if(Status != null)
            {
                if(item.Status != Status)
                {
                    return null;
                }
            }
            if (item.Houses != null)
            {
                this._context.Entry(item.Houses).Reference(m => m.Users).Query().Load();
                this._context.Entry(item.Houses).Collection(m => m.FileOfHouses).Query().Load();
                if (item.Houses.Users != null)
                {
                    DetailHouseViewModel house = this.CreateDetailsHouse(item.Houses);
                    DetailHouseViewModel? swapHouse = null;
                    DetailRatingViewModel? userRating = null;
                    DetailRatingViewModel? myRating = null;
                    FeedBack? userFeedBack = this._context.FeedBacks
                                                                .Where(m => m.IdUser != IdUser && m.IdRequest == item.Id)
                                                                .FirstOrDefault();
                    if (userFeedBack != null)
                    {
                        userRating = new DetailRatingViewModel(userFeedBack);
                    }
                    if (item.FeedBacks != null)
                    {
                        FeedBack? myFeedBack = item.FeedBacks.Where(m => m.IdUser == IdUser).FirstOrDefault();
                        if (myFeedBack != null)
                        {
                            myRating = new DetailRatingViewModel(myFeedBack);
                            item.Status = (int)StatusRequest.ENDED;
                        }
                    }
                    this._context.Entry(item).Reference(m => m.Users).Query().Load();
                    if (item.IdSwapHouse != null)
                    {
                        this._context.Entry(item).Reference(m => m.SwapHouses).Query().Load();
                        if (item.SwapHouses != null)
                        {
                            this._context.Entry(item.SwapHouses).Reference(m => m.Users).Query().Load();
                            this._context.Entry(item.SwapHouses).Collection(m => m.FileOfHouses).Query().Load();
                            swapHouse = this.CreateDetailsHouse(item.SwapHouses);
                        }
                    }
                    User? inputUser = null;
                    if (item.IdUser == IdUser)
                    {
                        inputUser = item.Houses.Users;
                    }
                    else
                    {
                        inputUser = item.Users;
                    }
                    DetailRequestViewModel request = new DetailRequestViewModel(item, inputUser, salt, host);
                    return new DetailRequest()
                    {
                        Request = request,
                        SwapHouse = swapHouse,
                        House = house,
                        UserRating = userRating,
                        MyRating = myRating
                    };
                }
            }
            return null;
        }

        [HttpGet("/Request/GetRequestsByUserAccess")]//lấy danh sách yêu cầu mới
        public IActionResult GetListRequestsByUserAccess(string UserAccess, int? Status)
        {
            return Json(new
            {
                Status = 200,
                Data = this.GetAllRequestsByUserAccess(UserAccess, Status)
            });
        }
        [HttpGet("/api/Request/GetRequestsByUserAccess")]
        public IActionResult ApiGetListRequestsByUserAccess(string UserAccess, int? Status)
        {
            return Json(new
            {
                Status = 200,
                Data = this.GetAllRequestsByUserAccess(UserAccess, Status)
            });
        }
        private List<DetailRequest> GetAllRequestsByUserAccess(string UserAccess, int? Status)
        {
            int Id = 0;
            if(!int.TryParse(Crypto.DecodeKey(UserAccess, Crypto.Salt(this._configuration)), out Id))
            {
                return new List<DetailRequest>();
            }
            int IdUser = this.GetIdUser();
            var houses = this._context.Houses
                                        .Where(m => m.IdUser == IdUser 
                                        && m.Status == (int)StatusHouse.VALID)
                                        .ToList();
            List<DetailRequest> model = new List<DetailRequest>();
            DoAnTotNghiepContext Context = this._context;

            byte[] salt = Crypto.Salt(this._configuration);
            string host = this.GetWebsitePath();

            foreach (var item in houses)
            {
                this._context.Entry(item).Collection(m => m.Requests)
                                .Query().Where(m => m.IdUser == Id 
                                && m.Status == (int)StatusRequest.WAIT_FOR_SWAP).Load();

                if (item.Requests != null)
                {
                    foreach (var itemRequest in item.Requests)
                    {
                        if (itemRequest != null)
                        {
                            DetailRequest? request = this.CreateDetailRequest(itemRequest, Status);
                            if (request != null)
                            {
                                model.Add(request);
                            }
                        }
                    }
                }
            }
            return model;
        }


        //đổi trạng thái yêu cầu
        //chấp nhận thì chuyển mấy request kia => reject
        //check in
        //check out
        [HttpPost("/Request/UpdateStatus")]
        public async Task<IActionResult> UpdateStatusAsync([FromBody] UpdateStatusViewModel model)
        {
            return await this.UpdateRequestStatusAsync(model.Id, model.Status);
        }

        [HttpPost("/api/Request/NhatUpdateStatus")]
        public async Task<IActionResult> ApiNhatUpdateStatusAsync([FromBody] UpdateStatusViewModel model)
        {
            return await this.NhatUpdateRequestStatusAsync(model.Id, model.Status);
        }
        private async Task<IActionResult> NhatUpdateRequestStatusAsync(int Id, int Status)
        {
            switch (Status)
            {
                case (int)StatusRequest.REJECT://từ chối
                    return await this.NhatUpdateStatusRequestAsync(Id, Status);
                case (int)StatusRequest.ACCEPT://chấp nhận
                    return await this.NhatUpdateStatusRequestAsync(Id, Status);
                case (int)StatusRequest.CHECK_IN://checkIn
                    return await this.NhatUpdateStatusRequestAsync(Id, Status);
                case (int)StatusRequest.CHECK_OUT://Checkout
                    return await this.NhatUpdateStatusRequestAsync(Id, Status);
                    //case (int)StatusRequest.WAIT_FOR_RATE:
                    //    return this.UpdateStatusRequest((int)StatusRequest.CHECK_OUT, Id, Status);
            }
            return await this.NhatUpdateStatusRequestAsync(Id, Status);
        }

        //chưa xong
        //check In + check Out => 1 chiều
        private async Task<IActionResult> NhatUpdateStatusRequestAsync(int Id, int Status)
        {
            var rq = this._context.Requests.Where(m => m.Id == Id)
                                        .FirstOrDefault();
            return await UpdateStatusRequestAsync(rq, Status);
        }

        [HttpPost("/api/Request/UpdateStatus")]
        public async Task<IActionResult> ApiUpdateStatusAsync([FromBody] UpdateStatusViewModel model)
        {
            return await this.UpdateRequestStatusAsync(model.Id, model.Status);
        }
        private async Task<IActionResult> UpdateRequestStatusAsync(int Id, int Status)
        {
            switch (Status)
            {
                case (int)StatusRequest.REJECT://từ chối
                    return await this.UpdateStatusAsync((int)StatusRequest.WAIT_FOR_SWAP, Id, Status);
                case (int)StatusRequest.ACCEPT://chấp nhận
                    return await this.UpdateStatusAsync((int)StatusRequest.WAIT_FOR_SWAP, Id, Status);
                case (int)StatusRequest.CHECK_IN://checkIn
                    return await this.UpdateStatusAsync((int)StatusRequest.ACCEPT, Id, Status);
                case (int)StatusRequest.CHECK_OUT://Checkout
                    return await this.UpdateStatusAsync((int)StatusRequest.CHECK_IN, Id, Status);
                //case (int)StatusRequest.WAIT_FOR_RATE:
                //    return this.UpdateStatusRequest((int)StatusRequest.CHECK_OUT, Id, Status);
            }
            return await this.UpdateStatusAsync((int)StatusRequest.CHECK_OUT, Id, Status);
        }
        private async Task<IActionResult> UpdateStatusAsync(int PreStatus, int Id, int Status)
        {
            var rq = this._context.Requests.Where(m => m.Id == Id
                                                    && m.Status == PreStatus)
                                        .FirstOrDefault();
            return await UpdateStatusRequestAsync(rq, Status);
        }

        private async Task<IActionResult> UpdateStatusRequestAsync(Request? rq, int Status)
        {
            if (rq != null)
            {
                int IdUser = this.GetIdUser();
                var DBtransaction = this._context.Database.BeginTransaction();
                try
                {
                    this._context.Entry(rq).Reference(m => m.Houses).Query().Load();
                    if (rq.Houses != null)
                    {
                        ChatHub chatHub = new ChatHub(this._signalContext);

                        Notification notification = new Notification()
                        {
                            Title = NotificationType.RequestTitle,
                            CreatedDate = DateTime.Now,
                            Type = NotificationType.REQUEST,
                            IdUser = rq.IdUser,
                            IdType = rq.Id,
                            IsSeen = false,
                            ImageUrl = "/Image/house-demo.png"
                        };
                        var user = this._context.Users.Where(m => m.Id == IdUser).FirstOrDefault();
                        switch (Status)
                        {
                            case (int)StatusRequest.REJECT://từ chối
                                                            // gửi thông báo từ chối
                                notification.Content = "Yêu cầu của bạn đến nhà "
                                                        + rq.Houses.Name
                                                        + " bị từ chối";
                                if(rq.Houses.IdCity != null)
                                {
                                    DateTime? DateStart = null, DateEnd = null;
                                    DateTime now = DateTime.Now;
                                    if(DateTime.Compare(now, rq.StartDate) <= 0)
                                    {
                                        DateStart = rq.StartDate;
                                        DateEnd = rq.EndDate;
                                    }
                                    else if (DateTime.Compare(now, rq.EndDate) <= 0)
                                    {
                                        DateStart = now;
                                        DateEnd = rq.EndDate;
                                    }
                                    await CreateWaitingRequestTimerAsync(TargetFunction.ExecuteCreateWaiting,
                                                            TimeSpan.FromSeconds(10), 1, rq.Houses.IdCity.Value, IdUser, DateStart, DateEnd);
                                }
                                //chạy code trao đổi xoay vòng
                                break;
                            case (int)StatusRequest.ACCEPT://chấp nhận
                                                            //gửi thông báo chấp nhận
                                notification.Content = "Yêu cầu của bạn đến nhà "
                                                            + rq.Houses.Name
                                                            + " đã được chấp nhận";
                                if (user != null)
                                {
                                    //gửi bill qua email
                                    string? moduleEmail = this._configuration.GetConnectionString(ConfigurationEmail.Email());
                                    string? modulePassword = this._configuration.GetConnectionString(ConfigurationEmail.Password());
                                    if (!string.IsNullOrEmpty(moduleEmail) && !string.IsNullOrEmpty(modulePassword))
                                    {
                                        Email sender = new Email(moduleEmail, modulePassword);
                                        string body = notification.Content;//bill
                                        sender.SendMail(user.Email, Subject.SendRequestDetail(), body, null, string.Empty);
                                    }
                                }
                                //timer nhắc nhở checkIn => qua email
                                await this.TimerCheckInNotificationAsync(rq);
                                break;
                            case (int)StatusRequest.CHECK_IN://checkIn
                                //mấy cái request còn lại thì sao?
                                //trừ tiền user => hết tiền => bắt nạp
                                if (rq.Type == 1 && rq.Point > 0)
                                {
                                    if (user != null)
                                    {
                                        if((user.BonusPoint + user.Point - user.PointUsing) < rq.Point)
                                        {
                                            return Json(new
                                            {
                                                Status = 200,
                                                Message = "Thiếu tiền"
                                            });
                                        }
                                        else
                                        {
                                            user.PointUsing = rq.Point;
                                            this._context.Users.Update(user);
                                        }
                                    }
                                    else
                                    {
                                        return BadRequest(new
                                        {
                                            Status = 401,
                                            Message = "Không tìm thấy người dùng"
                                        });
                                    }
                                }
                                //gửi thông báo
                                notification.Content = "Bạn đã check-in "
                                                        + rq.Houses.Name
                                                        + " hệ thống đã gửi thông tin đến email của bạn";
                                notification.IdUser = IdUser;

                                //timer thông báo ngày checkOut
                                await this.TimerCheckOutNotificationAsync(rq);

                                //tạo checkIn
                                if (rq.IdSwapHouse != null)
                                {
                                    this._context.Entry(rq).Collection(m => m.CheckIns).Load();
                                    if (rq.CheckIns == null || rq.CheckIns.Count() == 0)
                                    {
                                        Status = (int)StatusRequest.ACCEPT;
                                    }
                                }
                                //tạo checkIn
                                CheckIn checkIn = new CheckIn()
                                {
                                    IdRequest = rq.Id,
                                    IdUser = IdUser
                                };
                                this._context.CheckIns.Add(checkIn);
                                this._context.SaveChanges();

                                break;
                            case (int)StatusRequest.CHECK_OUT://Checkout
                                //kiểm tra time => trừ tiền
                                //không đủ tiền => bắt nạp
                                if (rq.Type == 1 && rq.Point > 0)
                                {
                                    if (user != null)
                                    {
                                        if ((user.BonusPoint + user.Point - user.PointUsing) < rq.Point)
                                        {
                                            return Json(new
                                            {
                                                Status = 200,
                                                Message = "Thiếu tiền"
                                            });
                                        }
                                        else
                                        {
                                            user.BonusPoint -= rq.Point;
                                            if (user.BonusPoint < 0)
                                            {
                                                user.Point += user.BonusPoint;
                                                user.BonusPoint = 0;
                                            }
                                            user.PointUsing = 0;
                                            this._context.Users.Update(user);

                                            HistoryTransaction transaction = new HistoryTransaction()
                                            {
                                                Amount = rq.Point,
                                                IdUser = IdUser,
                                                CreatedDate = DateTime.Now,
                                                Status = (int)StatusTransaction.USED,
                                                Content = "Bạn thanh toán "
                                                            + rq.Point + ""
                                                            + " điểm trao đổi nhà " 
                                                            + rq.Houses.Name + " của " 
                                                            + user.FirstName + " " + user.LastName
                                                            + " vào lúc" + DateTime.Now.ToString("hh:mm:ss dd/MM/yyyy")
                                            };
                                            this._context.HistoryTransactions.Add(transaction);
                                            this._context.SaveChanges();
                                        }
                                    }
                                    else
                                    {
                                        return BadRequest(new
                                        {
                                            Status = 401,
                                            Message = "Không tìm thấy người dùng"
                                        });
                                    }
                                }

                                if (rq.IdSwapHouse != null)
                                {
                                    this._context.Entry(rq).Collection(m => m.CheckOuts).Load();
                                    if (rq.CheckOuts == null || rq.CheckOuts.Count() == 0)
                                    {
                                        Status = (int)StatusRequest.CHECK_IN;
                                    }
                                }
                                //tạo checkOut
                                CheckOut checkOut = new CheckOut()
                                {
                                    IdRequest = rq.Id,
                                    IdUser = IdUser
                                };
                                this._context.CheckOuts.Add(checkOut);
                                this._context.SaveChanges();

                                //gửi thông báo
                                notification.Content = "Bạn đã check-out "
                                                        + rq.Houses.Name
                                                        + " hãy viết cảm nhận của mình sau chuyển đi và nhận thưởng từ hệ thống";
                                notification.IdUser = IdUser;
                                break;
                        }

                        //gửi notification
                        this._context.Notifications.Add(notification);
                        this._context.SaveChanges();

                        await chatHub.SendNotification(
                            group: Crypto.EncodeKey(notification.IdUser.ToString(), Crypto.Salt(this._configuration)),
                            target: TargetSignalR.Notification(),
                            model: new NotificationViewModel(notification, this.GetWebsitePath()));
                        //gửi notification

                        rq.Status = Status;
                        this._context.Requests.Update(rq);
                        this._context.SaveChanges();

                        DBtransaction.Commit();
                        return Json(new
                        {
                            Status = 200,
                            Message = "Cập nhật thành công"
                        });
                    }

                    DBtransaction.Commit();
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
        private async Task TimerCheckInNotificationAsync(Request request)
        {
            //chỉnh time start => request.startDate - 1
            await InitTimerAsync(request, TargetFunction.ExecuteCheckIn, TimeSpan.FromMinutes(1), 2);
        }
        private async Task TimerCheckOutNotificationAsync(Request request)
        {
            //chỉnh time start => request.endDate - 1
            await InitTimerAsync(request, TargetFunction.ExecuteCheckOut, TimeSpan.FromMinutes(1), 1);
        }
        private async Task InitTimerAsync(Request request, string Function, TimeSpan timeStart, int limit)
        {
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;
            DoAnTotNghiepContext inputContext = new DoAnTotNghiepContext(this._context.GetConfig());
            string host = this.GetWebsitePath();
            RequestBackground Object = new RequestBackground(new ChatHub(this._signalContext), request, this._configuration);
            TimedHostedService timer = new TimedHostedService(
                                            inputContext,
                                            host,
                                            Function,
                                            token,
                                            limit,
                                            timeStart,
                                            Object);

            await timer.StartAsync(token);
        }
        private async Task CreateWaitingRequestTimerAsync(string Function, TimeSpan timeStart, int limit, int IdCity, int IdUser, DateTime? DateStart, DateTime? DateEnd)
        {
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;
            DoAnTotNghiepContext inputContext = new DoAnTotNghiepContext(this._context.GetConfig());
            string host = this.GetWebsitePath();
            CreateWaitingRequest Object = new CreateWaitingRequest(IdCity, IdUser, DateStart, DateEnd);

            TimedHostedService timer = new TimedHostedService(
                                            inputContext,
                                            host,
                                            Function,
                                            token,
                                            limit,
                                            timeStart,
                                            Object);

            await timer.StartAsync(token);
        }
        //timer trước ngày trao đổi 1 ngày thông báo checkin//
        //timer sau ngày trao đổi 1 ngày chưa checkin => hủy yêu cầu//
        //timer trước ngày checkout 1 ngày thông báo checkout
        //      => tăng phí người chưa check out
        //update endDATE => TĂNG PHÍ NẾU NGƯỜI KIA ACCEPT

        [HttpGet("/Statistics/House")]
        public IActionResult StatisticsHouse(InputRequestStatistic input)
        {
            int IdUser = this.GetIdUser();
            var request = this._context.Requests
                                        .Include(m => m.Houses)
                                        .Where(m => m.IdHouse == input.IdHouse 
                                                    && m.Houses != null && m.Houses.IdUser == IdUser
                                                    && (m.Status == (int)StatusRequest.ENDED || m.Status == (int)StatusRequest.CHECK_OUT)
                                                    && (m.StartDate.Year == input.Year || m.EndDate.Year == input.Year)
                                        )
                                        .Select(m => new RequestStatistics(m))
                                        .ToList();

            var houseUseForSwap = this._context.Requests
                                        .Where(m => m.IdSwapHouse != null 
                                                    &&  m.IdSwapHouse == input.IdHouse
                                                    && m.IdUser == IdUser
                                                    && (m.StartDate.Year == input.Year || m.EndDate.Year == input.Year)
                                        )
                                        .Select(m => new RequestStatistics(m))
                                        .ToList();

            return Json(new {
                Status = 200,
                Data = new HouseStatistics()
                {
                    Requests = request,
                    UseForSwap = houseUseForSwap
                }
            });
        }
    }
}
