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

        public MemberController(DoAnTotNghiepContext context): base(context)
        {
            _context = context;
        }
        public IActionResult Infomation()
        {
            ViewData["active"] = 0;
            return View();
        }
        public IActionResult House()
        {
            ViewData["active"] = 1;
            int IdUser = this.GetIdUser();

            var listHouse = this._context.Houses.Where(m => m.IdUser == IdUser).ToList();
            var listUtilities = this._context.Utilities.ToList();
            var listRules = this._context.Rules.ToList();

            AuthHouseViewModel model = new AuthHouseViewModel()
            {
                Houses = listHouse,
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
