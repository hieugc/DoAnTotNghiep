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

namespace DoAnTotNghiep.Controllers
{
    [Authorize(Roles = Role.Member)]
    public class RequestController: BaseController
    {
        private readonly DoAnTotNghiepContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHubContext<ChatHub> _signalContext;

        public RequestController(DoAnTotNghiepContext context, 
                                IConfiguration configuration, 
                                IHostEnvironment environment,
                                IHubContext<ChatHub> signalContext) : base(environment)
        {
            _context = context;
            _configuration = configuration;
            _signalContext = signalContext;
        }

        private DetailHouseViewModel CreateDetailsHouse(House house)
        {
            byte[] salt = Crypto.Salt(this._configuration);
            string host = this.GetWebsitePath();
            if (house.Users != null)
            {
                this._context.Entry(house.Users).Collection(m => m.Houses).Query().Where(m =>m.Status == (int)StatusHouse.VALID).Load();
            }

            this._context.Entry(house).Reference(m => m.Citys).Query().Load();
            this._context.Entry(house).Reference(m => m.Districts).Query().Load();
            DetailHouseViewModel model = new DetailHouseViewModel(house, salt, house.Users, host);
            DoAnTotNghiepContext Context = this._context;
            if (house.FileOfHouses != null)
            {
                foreach (var f in house.FileOfHouses)
                {
                    Context.Entry(f).Reference(m => m.Files).Load();
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
                var userHouses = this._context.Houses.Where(m => m.IdUser == Id && m.Status == (int) StatusHouse.VALID).ToList();
                foreach (var item in userHouses)
                {
                    this._context.Entry(item).Collection(m => m.FileOfHouses).Query().Load();
                    this._context.Entry(item).Collection(m => m.FeedBacks).Query().Load();

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
                    model.UserHouses.Add(m);
                }

                //list house của mình
                int IdUser = this.GetIdUser();
                var myHouses = this._context.Houses.Where(m => m.IdUser == IdUser && m.Status == (int)StatusHouse.VALID).ToList();
                foreach (var item in myHouses)
                {
                    this._context.Entry(item).Collection(m => m.FileOfHouses).Query().Load();
                    this._context.Entry(item).Collection(m => m.FeedBacks).Query().Load();

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
                    model.MyHouses.Add(m);
                }
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
            var userHouses = this._context.Houses.Where(m => m.Id == IdHouse && m.Status == (int)StatusHouse.VALID).ToList();
            foreach (var item in userHouses)
            {
                this._context.Entry(item).Collection(m => m.FileOfHouses).Query().Load();
                this._context.Entry(item).Collection(m => m.FeedBacks).Query().Load();
                this._context.Entry(item).Reference(m => m.Citys).Query().Load();
                this._context.Entry(item).Reference(m => m.Districts).Query().Load();
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
                model.UserHouses.Add(m);
            }

            //list house của mình
            int IdUser = this.GetIdUser();
            var myHouses = this._context.Houses.Where(m => m.IdUser == IdUser && m.Status == (int)StatusHouse.VALID).ToList();
            foreach (var item in myHouses)
            {
                this._context.Entry(item).Collection(m => m.FileOfHouses).Query().Load();
                this._context.Entry(item).Collection(m => m.FeedBacks).Query().Load();
                this._context.Entry(item).Reference(m => m.Citys).Query().Load();
                this._context.Entry(item).Reference(m => m.Districts).Query().Load();
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
                model.MyHouses.Add(m);
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
                if((user.Point + user.BonusPoint - user.PointUsing) < model.Price)
                {
                    return BadRequest(new
                    {
                        Status = 400,
                        Message = "Yêu cầu không thể thực hiện vì thiếu tiền"
                    });
                }
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
                        ImageUrl = this.GetWebsitePath() + "/Image/house-demo.png"
                    };

                    this._context.Notifications.Add(notification);
                    this._context.SaveChanges();

                    await chatHub.SendNotification(
                        group: Crypto.EncodeKey(user.Id.ToString(), Crypto.Salt(this._configuration)),
                        target: TargetSignalR.Notification(), 
                        model: new NotificationViewModel(notification));
                }

                return request;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
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
            var rq = this._context.Requests.Where(m => m.Id == Id 
                                                    && m.IdUser == IdUser
                                                    && m.Status == (int)StatusRequest.WAIT_FOR_SWAP);
            if (rq.Any())
            {
                try
                {
                    Request request = rq.First();
                    this._context.Requests.Remove(request);
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
                this._context.Entry(house).Collection(m => m.Requests).Query().Where(m => m.Status == (int)StatusRequest.WAIT_FOR_SWAP).Load();
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
                                    this._context.Entry(item.SwapHouses).Reference(m => m.Citys).Query().Load();
                                    this._context.Entry(item.SwapHouses).Reference(m => m.Districts).Query().Load();
                                    swapHouse = new DetailHouseViewModel(item.SwapHouses, salt, item.Users, host);
                                }
                            }
                            DetailRequestViewModel request = new DetailRequestViewModel(item, item.Users, salt, host);
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
        public IActionResult GetListRequestSent()
        {
            return Json(new
            {
                Status = 200,
                Data = this.GetAllRequestsSent()
            });
        }
        [HttpGet("/api/Request/GetRequestSent")]
        public IActionResult ApiGetListRequestSent()
        {
            return Json(new
            {
                Status = 200,
                Data = this.GetAllRequestsSent()
            });
        }
        private List<DetailRequest> GetAllRequestsSent()
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
                    DetailRequest? request = this.CreateDetailRequest(item);
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
            DetailRequest? model = this.GetDetailHouse(Id);
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
        [HttpGet("/api/Request/Detail")]
        public IActionResult ApiGetDetailRequest(int Id)
        {
            DetailRequest? model = this.GetDetailHouse(Id);
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
        private DetailRequest? GetDetailHouse(int IdRequest)
        {
            //int IdUser = this.GetIdUser();
            var item = this._context.Requests
                                        .Where(m => m.Id == IdRequest).FirstOrDefault();
            if(item != null)
            {
                return this.CreateDetailRequest(item);
            }
            return null;
        }
        private DetailRequest? CreateDetailRequest(Request item)
        {
            byte[] salt = Crypto.Salt(this._configuration);
            string host = this.GetWebsitePath();
            int IdUser = this.GetIdUser();
            this._context.Entry(item).Reference(m => m.Houses).Query().Load();
            this._context.Entry(item).Collection(m => m.FeedBacks).Query().Where(m => m.IdUser == IdUser).Load();
            this._context.Entry(item).Collection(m => m.CheckOuts).Query().Where(m => m.IdUser == IdUser).Load();
            if (item.Houses != null)
            {
                this._context.Entry(item.Houses).Reference(m => m.Users).Query().Load();
                this._context.Entry(item.Houses).Collection(m => m.FileOfHouses).Query().Load();
                if (item.Houses.Users != null)
                {
                    DetailHouseViewModel house = this.CreateDetailsHouse(item.Houses);
                    DetailHouseViewModel? swapHouse = null;
                    DetailRatingViewModel? rating = null;
                    if (item.FeedBacks != null)
                    {
                        FeedBack? feedBack = item.FeedBacks.FirstOrDefault();
                        if (feedBack != null)
                        {
                            rating = new DetailRatingViewModel(feedBack);
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
                    DetailRequestViewModel request = new DetailRequestViewModel(item, item.Users, salt, host);
                    return new DetailRequest()
                    {
                        Request = request,
                        SwapHouse = swapHouse,
                        House = house,
                        Rating = rating
                    };
                }
            }
            return null;
        }

        [HttpGet("/api/Request/GetRequestReceived")]
        public IActionResult ApiGetListRequestReceived()
        {
            return Json(new
            {
                Status = 200,
                Data = this.GetAllRequestsReceived()
            });
        }
        private List<DetailRequest> GetAllRequestsReceived()
        {
            int IdUser = this.GetIdUser();
            var houses = this._context.Houses
                                        .Where(m => m.IdUser == IdUser && m.Status == (int)StatusHouse.VALID)
                                        .ToList();
            List<DetailRequest> model = new List<DetailRequest>();
            DoAnTotNghiepContext Context = this._context;

            byte[] salt = Crypto.Salt(this._configuration);
            string host = this.GetWebsitePath();

            foreach(var item in houses)
            {
                this._context.Entry(item).Collection(m => m.Requests).Query().Load();
                if (item.Requests != null)
                {
                    foreach (var itemRequest in item.Requests)
                    {
                        if (itemRequest != null)
                        {
                            DetailRequest? request = this.CreateDetailRequest(itemRequest);
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

        [HttpGet("/Request/GetRequestsByUserAccess")]//lấy danh sách yêu cầu mới
        public IActionResult GetListRequestsByUserAccess(string UserAccess)
        {
            return Json(new
            {
                Status = 200,
                Data = this.GetAllRequestsByUserAccess(UserAccess)
            });
        }
        [HttpGet("/api/Request/GetRequestsByUserAccess")]
        public IActionResult ApiGetListRequestsByUserAccess(string UserAccess)
        {
            return Json(new
            {
                Status = 200,
                Data = this.GetAllRequestsByUserAccess(UserAccess)
            });
        }
        private List<DetailRequest> GetAllRequestsByUserAccess(string UserAccess)
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
                            DetailRequest? request = this.CreateDetailRequest(itemRequest);
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
                    return await this.UpdateStatusRequestAsync((int)StatusRequest.WAIT_FOR_SWAP, Id, Status);
                case (int)StatusRequest.ACCEPT://chấp nhận
                    return await this.UpdateStatusRequestAsync((int)StatusRequest.WAIT_FOR_SWAP, Id, Status);
                case (int)StatusRequest.CHECK_IN://checkIn
                    return await this.UpdateStatusRequestAsync((int)StatusRequest.ACCEPT, Id, Status);
                case (int)StatusRequest.CHECK_OUT://Checkout
                    return await this.UpdateStatusRequestAsync((int)StatusRequest.CHECK_IN, Id, Status);
                //case (int)StatusRequest.WAIT_FOR_RATE:
                //    return this.UpdateStatusRequest((int)StatusRequest.CHECK_OUT, Id, Status);
            }
            return await this.UpdateStatusRequestAsync((int)StatusRequest.CHECK_OUT, Id, Status);
        }

        //chưa xong
        //check In + check Out => 1 chiều
        private async Task<IActionResult> UpdateStatusRequestAsync(int PreStatus, int Id, int Status)
        {
            var rq = this._context.Requests.Where(m => m.Id == Id 
                                                    && m.Status == PreStatus)
                                        .FirstOrDefault();
            if (rq != null)
            {
                int IdUser = this.GetIdUser();
                if (!(Status == (int)StatusRequest.CHECK_OUT
                    && rq.IdSwapHouse != null
                    && rq.IdUser != IdUser))
                {
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
                                ImageUrl = this.GetWebsitePath() + "/Image/house-demo.png"
                            };
                            switch (Status)
                            {
                                case (int)StatusRequest.REJECT://từ chối
                                                               // gửi thông báo từ chối
                                    notification.Content = "Yêu cầu của bạn đến nhà "
                                                            + rq.Houses.Name
                                                            + " bị từ chối";
                                    break;
                                case (int)StatusRequest.ACCEPT://chấp nhận
                                                               //gửi thông báo chấp nhận
                                    notification.Content = "Yêu cầu của bạn đến nhà "
                                                             + rq.Houses.Name
                                                             + " đã được chấp nhận";
                                    //timer nhắc nhở checkIn => qua email
                                    break;
                                case (int)StatusRequest.CHECK_IN://checkIn
                                                                 //trừ tiền user => hết tiền => bắt nạp
                                                                 //gửi thông báo
                                    notification.Content = "Bạn đã check-in "
                                                            + rq.Houses.Name
                                                            + " hệ thống đã gửi thông tin đến email của bạn";
                                    notification.IdUser = IdUser;
                                    //gửi bill qua email
                                    break;
                                case (int)StatusRequest.CHECK_OUT://Checkout
                                    if (rq.IdSwapHouse != null)
                                    {
                                        this._context.Entry(rq).Collection(m => m.CheckOuts).Load();
                                        if (rq.CheckOuts == null)
                                        {
                                            Status = (int)StatusRequest.CHECK_IN;
                                        }
                                        //tạo checkOut
                                        CheckOut checkOut = new CheckOut()
                                        {
                                            IdRequest = rq.Id,
                                            IdUser = IdUser
                                        };
                                        this._context.CheckOuts.Add(checkOut);
                                        this._context.SaveChanges();
                                    }
                                    //kiểm tra time => trừ tiền
                                    //không đủ tiền => bắt nạp

                                    //gửi thông báo
                                    //check lại
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
                                model: new NotificationViewModel(notification));
                            //gửi notification

                            rq.Status = Status;
                            this._context.Requests.Update(rq);
                            this._context.SaveChanges();

                            return Json(new
                            {
                                Status = 200,
                                Message = "Cập nhật thành công"
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            return BadRequest(new
            {
                Status = 400,
                Message = "Yêu cầu không tồn tại"
            });
        }


        //timer trước ngày trao đổi 1 ngày thông báo checkin
        //timer sau ngày trao đổi 1 ngày chưa checkin => hủy yêu cầu
        //timer trước ngày checkout 1 ngày thông báo checkout lố ngày
        //      => tăng phí người chưa check out
        //update endDATE => TĂNG PHÍ NẾU NGƯỜI KIA ACCEPT
    }
}
