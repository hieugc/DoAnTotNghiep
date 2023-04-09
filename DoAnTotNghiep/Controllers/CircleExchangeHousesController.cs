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

namespace DoAnTotNghiep.Controllers
{
    [Authorize(Roles = Role.Member)]
    public class CircleExchangeHousesController : BaseController
    {
        private readonly DoAnTotNghiepContext _context;

        public CircleExchangeHousesController(DoAnTotNghiepContext context, IHostEnvironment environment): base(environment)
        {
            _context = context;
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

        //Update
        private IActionResult Update(int IdWaitingRequest, int Circle, int Status)
        {
            int IdUser = this.GetIdUser();
            var rq = this._context.RequestsInCircleExchangeHouses
                                    .Include(m => m.CircleExchangeHouse)
                                    .Include(m => m.WaitingRequests)
                                    .Where(m => m.IdWaitingRequest == IdWaitingRequest 
                                            && m.IdCircleExchangeHouse == Circle)
                                    .FirstOrDefault();

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

            if (rq.CircleExchangeHouse != null)
            {
                this._context.Entry(rq.CircleExchangeHouse).Collection(m => m.RequestInCircles).Load();
                if (Status == (int)StatusRequest.REJECT)
                {
                    this._context.Entry(rq.CircleExchangeHouse).Collection(m => m.RequestInCircles).Load();
                    this._context.Entry(rq.CircleExchangeHouse).Collection(m => m.UserInCircles).Load();
                    if (rq.CircleExchangeHouse.RequestInCircles != null)
                    {
                        this._context.RequestsInCircleExchangeHouses.RemoveRange(rq.CircleExchangeHouse.RequestInCircles);
                        this._context.SaveChanges();
                    }
                    if (rq.CircleExchangeHouse.UserInCircles != null)
                    {
                        this._context.CircleExchangeHouseOfUsers.RemoveRange(rq.CircleExchangeHouse.UserInCircles);
                        this._context.SaveChanges();
                    }
                    this._context.CircleExchangeHouses.Remove(rq.CircleExchangeHouse);
                    this._context.SaveChanges();

                    //xóa cái vòng người
                }
                else
                {
                    rq.Status = Status;
                    this._context.RequestsInCircleExchangeHouses.Update(rq);
                    this._context.SaveChanges();
                    //nếu như accept full thì tiến hành trao đổi => 

                    if (rq.CircleExchangeHouse.RequestInCircles != null)
                    {
                        if (!rq.CircleExchangeHouse.RequestInCircles.Any(m => m.Status == 0))
                        {
                            rq.CircleExchangeHouse.Status = Status;
                            this._context.CircleExchangeHouses.Update(rq.CircleExchangeHouse);
                            this._context.SaveChanges();

                            //gửi thông báo yêu cầu
                        }
                    }
                }

            }

            return Json(new
            {
                Status = 200,
                Message = "ok"
            });
        }

        //Get Suggestion
        private IActionResult GetSuggest()
        {
            int IdUser = this.GetIdUser();
            //lấy nhà người 1
            //lấy nhà người 2
            //lấy nhà người 3
            return Json(new
            {

            });
        }

    }
}
