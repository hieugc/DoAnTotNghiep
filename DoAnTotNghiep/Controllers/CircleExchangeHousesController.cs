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

namespace DoAnTotNghiep.Controllers
{
    [Authorize(Roles = Role.Member)]
    public class CircleExchangeHousesController : BaseController
    {
        private readonly DoAnTotNghiepContext _context;
        private readonly IConfiguration _configuration;

        public CircleExchangeHousesController(DoAnTotNghiepContext context, 
                                            IHostEnvironment environment,
                                            IConfiguration configuration) : base(environment)
        {
            _context = context;
            _configuration = configuration;
        }

        //Create
        //send Id City + range DATE + Id HOUSE
        private IActionResult Create(int IdCity, int IdHouse, DateTime StartDate, DateTime EndDate)
        {
            //lấy tất cả circle exchange
            var cities = this._context.Cities
                                        .Include(m => m.houses)
                                        .Where(m => m.Id == IdCity && m.houses != null)
                                        .FirstOrDefault();
            if (cities == null) return BadRequest(new
            {
                Status = 400,
                Message = "Không tìm thấy thành phố"
            });
            int IdUser = this.GetIdUser();

            if (cities.houses != null && cities.houses.Any(m => m.Status == (int) StatusHouse.VALID))
            {

                var waitingRequest = this._context.WaitingRequests;
            }
            else
            {
                WaitingRequest request = new WaitingRequest()
                {
                    IdCity = IdCity,
                    IdHouse = IdHouse,
                    IdDistrict = null,
                    IdWard = null,
                    IdUser = IdUser
                };

                this._context.WaitingRequests.Add(request);
                this._context.SaveChanges();
            }

            return Json(new
            {
                Status = 200,
                Message = "ok"
            });
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
            var rq = this._context.RequestsInCircleExchangeHouses
                                    .Include(m => m.CircleExchangeHouse)
                                    .Include(m => m.WaitingRequests)
                                    .Where(m => m.IdWaitingRequest == IdWaitingRequest 
                                                && m.IdCircleExchangeHouse == Circle
                                                && m.WaitingRequests != null && m.WaitingRequests.IdUser == IdUser
                                                && m.CircleExchangeHouse != null && m.CircleExchangeHouse.Status != (int)StatusWaitingRequest.DISABLE)
                                    .FirstOrDefault();//chỉ mình user đó sử dụng

            if(rq == null)
            {
                return BadRequest(new
                {
                    Status = 400,
                    Message = "Không tồn tại"
                });
            }

            //nếu từ chối xóa => circle
            //nếu như accept full thì tiến hành trao đổi => 

            if (rq.CircleExchangeHouse != null && rq.CircleExchangeHouse.Status != (int)StatusWaitingRequest.DISABLE)
            {
                this._context.Entry(rq.CircleExchangeHouse).Collection(m => m.RequestInCircles).Load();

                switch (Status)
                {
                    case (int)StatusWaitingRequest.DISABLE:
                        rq.CircleExchangeHouse.Status = (int)StatusWaitingRequest.DISABLE;
                        this._context.CircleExchangeHouses.Update(rq.CircleExchangeHouse);
                        this._context.SaveChanges();
                        break;
                    case (int)StatusWaitingRequest.ACCEPT:
                        if (rq.CircleExchangeHouse.RequestInCircles != null)
                        {
                            if (!rq.CircleExchangeHouse.RequestInCircles.Any(m => m.Status < (int) StatusWaitingRequest.ACCEPT && m.IdWaitingRequest != rq.IdWaitingRequest))
                            {
                                rq.CircleExchangeHouse.Status = (int)StatusWaitingRequest.ACCEPT;
                                this._context.CircleExchangeHouses.Update(rq.CircleExchangeHouse);
                                this._context.SaveChanges();

                                //gửi thông báo yêu cầu
                            }
                        }
                        rq.Status = (int)StatusWaitingRequest.ACCEPT;
                        break;
                    case (int)StatusWaitingRequest.CHECK_IN:
                        //gửi thông báo ngày sẽ check In
                        rq.Status = (int)StatusWaitingRequest.CHECK_IN;
                        break;
                    case (int)StatusWaitingRequest.CHECK_OUT:
                        if (rq.CircleExchangeHouse.RequestInCircles != null)
                        {
                            if (!rq.CircleExchangeHouse.RequestInCircles.Any(m => m.Status < (int)StatusWaitingRequest.CHECK_OUT && m.IdWaitingRequest != rq.IdWaitingRequest))
                            {
                                rq.CircleExchangeHouse.Status = (int)StatusWaitingRequest.CHECK_OUT;
                                this._context.CircleExchangeHouses.Update(rq.CircleExchangeHouse);
                                this._context.SaveChanges();
                            }
                            else
                            {
                                //gửi thông báo nhắc nhỡ check out
                            }
                        }
                        //tạo checkOut
                        rq.Status = (int)StatusWaitingRequest.CHECK_OUT;
                        break;
                }
                this._context.RequestsInCircleExchangeHouses.Update(rq);
                this._context.SaveChanges();
            }

            return Json(new
            {
                Status = 200,
                Message = "ok"
            });
        }


        [HttpGet("/api/CircleRequest/Get")]
        public IActionResult ApiGetSuggest()
        {
            int IdUser = this.GetIdUser();
            var circle = this._context.CircleExchangeHouseOfUsers
                                        .Include(m => m.CircleExchangeHouse)
                                        .Where(m => m.IdUser == IdUser
                                                        && m.CircleExchangeHouse != null
                                                        && m.CircleExchangeHouse.Status != (int)StatusWaitingRequest.DISABLE)
                                        .Select(m => m.CircleExchangeHouse)
                                        .ToList();

            List<CircleRequestViewModel> model = new List<CircleRequestViewModel>();
            byte[] salt = Crypto.Salt(this._configuration);
            string host = this.GetWebsitePath();
            foreach (var item in circle)
            {
                if (item != null)
                {
                    CircleRequestViewModel? circleRequest = this.CreateDetailRequest(item, host, salt, IdUser);
                    if (circleRequest != null) model.Add(circleRequest);
                }
            }
            return Json(new
            {
                Status = 200,
                Data = model
            });
        }

        private CircleRequestViewModel? CreateDetailRequest(CircleExchangeHouse item, string host, byte[] salt, int IdUser)
        {
            this._context.Entry(item).Collection(m => m.RequestInCircles).Query().Load();
            this._context.Entry(item).Collection(m => m.FeedBacks).Load();
            if (item.RequestInCircles != null)
            {
                var rq = from rqc in item.RequestInCircles
                         join crq in this._context.WaitingRequests on rqc.IdWaitingRequest equals crq.Id
                         select crq.Node(rqc, crq);
                if (rq != null)
                {
                    rq = rq.ToList();
                    foreach (var itemRQ in rq)
                    {
                        this._context.Entry(itemRQ).Reference(m => m.Houses).Load();
                    }
                    WaitingRequest? myNode = rq.Where(m => m.IdUser == IdUser).FirstOrDefault();
                    if (myNode != null)
                    {
                        if (myNode.Houses != null)
                        {
                            myNode.Houses.IncludeLocation(this._context);
                            var inputMyNode = this.CircleRequestDetail(myNode, salt, host);
                            WaitingRequest? prevNode = rq.Where(m => m.IdUser != IdUser && m.IdCity == myNode.Houses.IdCity.Value).FirstOrDefault();
                            if (prevNode != null && inputMyNode != null)
                            {
                                prevNode.Houses.IncludeLocation(this._context);
                                var inputPrevNode = this.CircleRequestDetail(prevNode, salt, host);
                                WaitingRequest? nextNode = rq.Where(m => m.IdUser != IdUser && myNode.IdCity == m.Houses.IdCity.Value).FirstOrDefault();
                                if (nextNode != null && inputPrevNode != null)
                                {
                                    nextNode.Houses.IncludeLocation(this._context);
                                    var inputNextNode = this.CircleRequestDetail(nextNode, salt, host);

                                    if (inputNextNode != null)
                                    {
                                        return new CircleRequestViewModel(inputPrevNode, inputMyNode, inputNextNode, item, IdUser);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }



        //chi tiết yêu cầu
        [HttpGet("/CircleRequest/Detail")]
        public IActionResult GetDetailRequest(int Id)
        {
            CircleRequestViewModel? model = this.DetailRequest(Id);
            if (model == null) return NotFound();
            return PartialView("./Views/Request/_CircleRequestDetail.cshtml", model);
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
            int IdUser = this.GetIdUser();
            var item = this._context.CircleExchangeHouseOfUsers
                                        .Include(m => m.CircleExchangeHouse)
                                        .Where(m => m.IdUser == IdUser
                                                        && m.CircleExchangeHouse != null
                                                        && m.CircleExchangeHouse.Status != (int)StatusWaitingRequest.DISABLE)
                                        .Select(m => m.CircleExchangeHouse)
                                        .FirstOrDefault();

            if (item != null)
            {
                return this.CreateDetailRequest(item, this.GetWebsitePath(), Crypto.Salt(this._configuration), IdUser);
            }
            return null;
        }
        private CircleRequestDetail? CircleRequestDetail(WaitingRequest request, byte[] salt, string host)
        {
            //lấy nhà
            if(request.Houses != null)
            {
                //lấy imagehouse
                this._context.Entry(request.Houses).Collection(m => m.FileOfHouses).Query().Load();
                if(request.Houses.FileOfHouses != null)
                {
                    var fImage = request.Houses.FileOfHouses.FirstOrDefault();
                    if(fImage != null)
                    {
                        this._context.Entry(fImage).Reference(m => m.Files).Load();
                        if(fImage != null)
                        {
                            ImageBase HouseImage = new ImageBase(fImage.Files, host);

                            //lấy người dùng
                            this._context.Entry(request).Reference(m => m.Users).Load();
                            if(request.Users != null)
                            {
                                this._context.Entry(request.Users).Reference(m => m.Files).Load();
                                UserInfo user = new UserInfo(request.Users, salt, host);
                                var numberSwap = from u in this._context.Users
                                                 join h in this._context.Houses on u.Id equals h.IdUser
                                                 join rq in this._context.Requests on h.Id equals rq.IdHouse
                                                 where u.Id == request.Users.Id && rq.Status >= (int)StatusRequest.CHECK_IN
                                                 select rq;
                                user.NumberSwap = (numberSwap == null ? 0: numberSwap.ToList().Count());
                                //this._context.Entry(request.Houses).Collection(m => m.Requests).Query().Where(m => m.Status == (int)StatusRequest.WAIT_FOR_SWAP).Load();
                                //this._context.Entry(request.Houses).Collection(m => m.FeedBacks).Query().Load();
                                //this._context.Entry(request).Reference(m => m.Houses).Load();
                                request.Houses.IncludeLocation(this._context);
                                return new CircleRequestDetail(new DetailHouseViewModel(request.Houses, salt, null, null), request, user, HouseImage);
                            }
                        }
                    }
                }
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
            int IdUser = this.GetIdUser();
            var rq = this._context.CircleExchangeHouses.Where(m => m.Id == Id && (m.Status <= (int)StatusRequest.CHECK_IN)).FirstOrDefault();
            if (rq != null)
            {
                try
                {
                    rq.Status = (int)StatusWaitingRequest.DISABLE;
                    this._context.CircleExchangeHouses.Update(rq);
                    this._context.SaveChanges();
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

        [HttpPost("/api/CircleRequest/GetAllWaiting")]
        [AllowAnonymous]
        public IActionResult AllWaiting()
        {
            var waiting = this._context.WaitingRequests.ToList();
            return Json(new
            {
                Waiting = waiting
            });
        }
        [HttpPost("/api/CircleRequest/GetAllCircle")]
        [AllowAnonymous]
        public IActionResult AllCircle()
        {
            var circle = this._context.CircleExchangeHouses.ToList();
            return Json(new
            {
                Circle = circle
            });
        }

        [HttpPost("/api/CircleRequest/GetAllRequests")]
        [AllowAnonymous]
        public IActionResult AllRequests()
        {
            var waiting = this._context.RequestsInCircleExchangeHouses.ToList();
            return Json(new
            {
                Request = waiting
            });
        }
        [HttpPost("/api/CircleRequest/GetAllUsers")]
        [AllowAnonymous]
        public IActionResult AllUsers()
        {
            var circle = this._context.CircleExchangeHouseOfUsers.ToList();
            return Json(new
            {
                User = circle
            });
        }
    }
}
