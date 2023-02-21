using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DoAnTotNghiep.Data;
using DoAnTotNghiep.Entity;

namespace DoAnTotNghiep.Controllers
{
    public class AuthController : Controller
    {
        private readonly DoAnTotNghiepContext _context;

        public AuthController(DoAnTotNghiepContext context)
        {
            _context = context;
        }

        public IActionResult Infomation()
        {
            return View();
        }
    }
}
