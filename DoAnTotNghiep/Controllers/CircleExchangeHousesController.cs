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
                                        .Where(m => m.Id == IdCity).FirstOrDefault();
            if (cities == null) return BadRequest(new
            {
                Status = 400,
                Message = "Không tìm thấy thành phố"
            });
            int IdUser = this.GetIdUser();

            if (cities.houses != null && cities.houses.Any())
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
        private IActionResult Update(int IdWaitingRequest, int Status)
        {
            int IdUser = this.GetIdUser();
            var rq = this._context.WaitingRequests
                                    .Include(m => m.Requests)
                                    .Where(m => m.Id == IdWaitingRequest && m.IdUser == IdUser)
                                    .FirstOrDefault();

            if(rq == null)
            {
                return BadRequest(new
                {
                    Status = 400,
                    Message = "Không tồn tại"
                });
            }

            rq.Status = Status;
            this._context.WaitingRequests.Update(rq);
            this._context.SaveChanges();

            //nếu từ chối xóa => circle
            //nếu như accept full thì tiến hành trao đổi => 
            if(rq.Requests != null)
            {
                foreach (var item in rq.Requests)
                {
                    this._context.Entry(item).Reference(m => m.CircleExchangeHouse).Load();
                    if(item.CircleExchangeHouse != null)
                    {

                    }
                }
            }

            return Json(new
            {
                Status = 200,
                Message = "ok"
            });
        }

        //Delete tự động
        //Get Suggestion


    }
}
