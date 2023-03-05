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


namespace DoAnTotNghiep.Controllers
{
    [Authorize(Roles = "MEMBER")]
    public class MemberController : BaseController
    {
        private readonly DoAnTotNghiepContext _context;

        public MemberController(DoAnTotNghiepContext context)
        {
            _context = context;
        }


        [HttpPost("api/GetIdMobile")]
        public IActionResult apiMobile()
        {
            return Ok(new
            {
                Message = this.GetIdUser()
            });
        }

        public IActionResult Infomation()
        {
            ViewData["active"] = 0;
            return View();
        }
        public IActionResult House()
        {
            ViewData["active"] = 1;//xác định tab active
            int IdUser = this.GetIdUser();
            var listHouse = this._context.Houses.Include(m => m.RulesInHouses)
                                                .Include(m => m.UtilitiesInHouses)
                                                .Include(m => m.Citys)
                                                .Include(m => m.Districts)
                                                .Include(m => m.Wards)
                                                .Include(m => m.Requests)
                                                .Include(m => m.FileOfHouses)
                                                .Where(m => m.IdUser == IdUser)
                                                .Take<House>(6)
                                                .ToList();
            var listUtilities = this._context.Utilities.ToList();
            var listRules = this._context.Rules.ToList();

            List<DetailHouseViewModel> detailHouseViewModels = new List<DetailHouseViewModel>();
            foreach(var item in listHouse)
            {
                detailHouseViewModels.Add(DetailHouseViewModel.GetByHouse(item));
            }

            AuthHouseViewModel model = new AuthHouseViewModel()
            {
                Houses = detailHouseViewModels,
                OptionHouses = new OptionHouseViewModel()
                {
                    Utilities = listUtilities,
                    Rules = listRules
                }
            };

            return View(model);
        }
        public IActionResult Requested()
        {
            ViewData["active"] = 2;
            return View();
        }
        public IActionResult WaitingRequest()
        {
            ViewData["active"] = 3;
            return View();
        }
        public IActionResult History()
        {
            ViewData["active"] = 4;
            return View();
        }
        public IActionResult Messages()
        {
            ViewData["active"] = 5;
            return View();
        }
        public IActionResult ReportMail()
        {
            ViewData["active"] = 0;
            return View();
        }
    }
}
