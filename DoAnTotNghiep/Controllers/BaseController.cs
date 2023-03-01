using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DoAnTotNghiep.Data;
using DoAnTotNghiep.Entity;
using System.Security.Claims;

namespace DoAnTotNghiep.Controllers
{
    public abstract class BaseController : Controller
    {
        private readonly DoAnTotNghiepContext _context;

        public BaseController(DoAnTotNghiepContext context)
        {
            _context = context;
        }

        public IActionResult NotFound() => PartialView("./Views/Base/NotFound.cshtml");

        protected int GetIdUser()
        {
            try
            {
                Claim? claim = this.HttpContext.User.FindFirst(ClaimTypes.Name);
                return claim == null? 0: int.Parse(claim.Value);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return 0;
            }
        }
    }
}
