using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DoAnTotNghiep.Data;
using DoAnTotNghiep.Entity;
using DoAnTotNghiep.Service;
using DoAnTotNghiep.ViewModels;
using Microsoft.AspNetCore.Authorization;
using DoAnTotNghiep.Enum;

namespace DoAnTotNghiep.Controllers
{
    [Authorize(Roles = Role.Admin)]
    public class UtilitiesController : Controller
    {
        private readonly DoAnTotNghiepContext _context;
        private readonly IUtilitiesService _utilitiesService;

        public UtilitiesController(DoAnTotNghiepContext context, IUtilitiesService utilitiesService)
        {
            _context = context;
            _utilitiesService = utilitiesService;
        }

        [HttpGet("/Utilities/Create")]
        public IActionResult Create()
        {
            return PartialView("~/Views/Utilities/_Create.cshtml", new Utilities());
        }

        [HttpPost("/Utilities/Create")]
        public IActionResult Create([FromBody] Utilities model)
        {
            if (ModelState.IsValid)
            {
                this._utilitiesService.SaveUtilities(model);
                return Json(new
                {
                    Status = 200,
                    Data = model
                });
            }
            return BadRequest(new
            {
                Status = 400
            });
        }
        [HttpGet("/Utilities/Get")]
        public IActionResult Get(int Id)
        {
            return PartialView("~/Views/Utilities/_Item.cshtml", this._utilitiesService.GetById(Id));
        }

        [HttpGet("/Utilities/Update")]
        public IActionResult Update(int Id)
        {
            return PartialView("~/Views/Utilities/_Update.cshtml", this._utilitiesService.GetById(Id));
        }

        [HttpPost("/Utilities/Update")]
        public IActionResult Update([FromBody] Utilities model)
        {
            if (ModelState.IsValid)
            {
                this._utilitiesService.UpdateUtilities(model);
                return Json(new
                {
                    Status = 200,
                    Data = model
                });
            }
            return BadRequest(new
            {
                Status = 400
            });
        }
    }
}
