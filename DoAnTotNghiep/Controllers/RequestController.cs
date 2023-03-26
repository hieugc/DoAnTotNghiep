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

        //Status Request
        //Accept
        //Reject
        //WAIT FOR SWAP
        //WAIT FOR RATE
        //ENDED


        private DetailHouseViewModel CreateDetailsHouse(House house)
        {
            byte[] salt = Crypto.Salt(this._configuration);
            string host = this.GetWebsitePath();
            if (house.Users != null)
            {
                this._context.Entry(house.Users).Collection(m => m.Houses).Query().Where(m =>m.Status == (int)StatusHouse.VALID).Load();
            }
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
        public IActionResult CreateRequest([FromBody] RequestViewModel model)
        {
            return this.Create(model);
        }
        [HttpPost("/api/Request/Create")]
        public IActionResult ApiCreateRequest([FromBody] RequestViewModel model)
        {
            return this.Create(model);
        }
        private IActionResult Create(RequestViewModel model)
        {
            if (ModelState.IsValid)
            {
                //kiểm tra nhà tồn tại không?
                var h = this._context.Houses
                                    .Include(m => m.Users)
                                    .Where(m => m.Id == model.IdHouse 
                                                && m.Status == (int) StatusHouse.VALID)
                                    .FirstOrDefault();
                if (h != null)
                {
                    House house = h;
                    if (model.IdSwapHouse != null)
                    {
                        //cần kiểm tra nhà đã chứa request của nhà này ch?
                        var sh = this._context.Houses
                                            .Where(m => m.Id == model.IdSwapHouse.Value 
                                                        && m.Id != house.Id
                                                        && m.Status == (int)StatusHouse.VALID)
                                            .FirstOrDefault();
                        if (sh != null)
                        {
                            //ChatHub chatHub = new ChatHub(this._signalContext);
                            //gui target
                            //this._context.Entry(house).Reference(m => m.Users).Query().Load();
                            //if(house.Users != null)
                            //{
                            //    this._context.Entry(house.Users).Reference(m => m.Files).Query().Load();
                            //}

                            Request? data = this.CreateRequestModel(model);
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
                        //ChatHub chatHub = new ChatHub(this._signalContext);
                        //gui target
                        Request? data = this.CreateRequestModel(model);
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
        private Request? CreateRequestModel(RequestViewModel model)
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
                return request;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return null;
        }


        //chỉnh sửa yêu cầu
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
            DoAnTotNghiepContext Context = this._context;

            byte[] salt = Crypto.Salt(this._configuration);
            string host = this.GetWebsitePath();
            Context.Entry(item).Reference(m => m.Houses).Query().Load();
            if (item.Houses != null)
            {
                Context.Entry(item.Houses).Reference(m => m.Users).Query().Load();
                Context.Entry(item.Houses).Collection(m => m.FileOfHouses).Query().Load();
                if (item.Houses.Users != null)
                {
                    DetailHouseViewModel house = this.CreateDetailsHouse(item.Houses);
                    DetailHouseViewModel? swapHouse = null;
                    Context.Entry(item).Reference(m => m.Users).Query().Load();
                    if (item.IdSwapHouse != null)
                    {
                        Context.Entry(item).Reference(m => m.SwapHouses).Query().Load();
                        if (item.SwapHouses != null)
                        {
                            Context.Entry(item.SwapHouses).Reference(m => m.Users).Query().Load();
                            Context.Entry(item.SwapHouses).Collection(m => m.FileOfHouses).Query().Load();
                            swapHouse = this.CreateDetailsHouse(item.SwapHouses);
                        }
                    }
                    DetailRequestViewModel request = new DetailRequestViewModel(item, item.Users, salt, host);
                    return new DetailRequest()
                    {
                        Request = request,
                        SwapHouse = swapHouse,
                        House = house
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
                this._context.Entry(item).Collection(m => m.Requests).Query().Where(m => m.Status == (int)StatusRequest.WAIT_FOR_SWAP).Load();
                if(item.Requests != null)
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

        [HttpGet("/Request/GetRequestsByUserAccess")]
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
                                        .Where(m => m.IdUser == IdUser && m.Status == (int)StatusHouse.VALID)
                                        .ToList();
            List<DetailRequest> model = new List<DetailRequest>();
            DoAnTotNghiepContext Context = this._context;

            byte[] salt = Crypto.Salt(this._configuration);
            string host = this.GetWebsitePath();

            foreach (var item in houses)
            {
                this._context.Entry(item).Collection(m => m.Requests).Query().Where(m => m.IdUser == Id && m.Status == (int)StatusRequest.WAIT_FOR_SWAP).Load();
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
        [HttpPost("/Request/UpdateStatus")]
        public IActionResult UpdateStatus([FromBody] UpdateStatusViewModel model)
        {
            return this.UpdateStatusRequest(model.Id, model.Status);
        }
        [HttpPost("/api/Request/UpdateStatus")]
        public IActionResult ApiUpdateStatus([FromBody] UpdateStatusViewModel model)
        {
            return this.UpdateStatusRequest(model.Id, model.Status);
        }
        //private IActionResult UpdateRequestStatus(int Id, int Status)
        //{
        //    switch (Status)
        //    {
        //        case (int)StatusRequest.REJECT:
        //            return this.UpdateStatusRequest((int)StatusRequest.WAIT_FOR_SWAP, Id, Status);
        //        case (int) StatusRequest.ACCEPT:
        //            return this.UpdateStatusRequest((int) StatusRequest.WAIT_FOR_SWAP, Id, Status);
        //        case (int)StatusRequest.WAIT_FOR_RATE:
        //            return this.UpdateStatusRequest((int)StatusRequest.ACCEPT, Id, Status);
        //    }
        //    return this.UpdateStatusRequest((int)StatusRequest.WAIT_FOR_RATE, Id, Status);
        //}

        //nếu accept
        //          thì thu tiền
        //          gửi email cho dui
        //nếu end thì chuyển tiền 2 user

        private IActionResult UpdateStatusRequest(int Id, int Status)
        {
            var rq = this._context.Requests.Where(m => m.Id == Id
                                        && m.Status == (int)StatusRequest.WAIT_FOR_SWAP)
                                        .FirstOrDefault();
            if (rq != null)
            {
                try
                {
                    if(Status == (int)StatusRequest.ACCEPT)
                    {
                        //từ chối còn lại
                        this._context.Entry(rq).Reference(m => m.Houses).Query().Load();
                        if(rq.Houses != null)
                        {
                            this._context.Entry(rq.Houses).Collection(m => m.Requests)
                                                            .Query()
                                                            .Where(m => m.Id != rq.Id && m.Status == (int) StatusRequest.WAIT_FOR_SWAP).Load();
                            if(rq.Houses.Requests != null)
                            {
                                List<Request> model = rq.Houses.Requests.ToList();
                                foreach (var item in model)
                                {
                                    item.Status = (int)StatusRequest.REJECT;
                                }
                                this._context.Requests.UpdateRange(model);
                            }
                            //rq.Houses.Status = (int)StatusHouse.SWAPPING;
                            //this._context.Houses.Update(rq.Houses);
                        }
                    }

                    rq.Status = Status;
                    this._context.Requests.Update(rq);
                    this._context.SaveChanges();

                    return Json(new
                    {
                        Status = 200,
                        Message = "Cập nhật thành công"
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
    }
}
