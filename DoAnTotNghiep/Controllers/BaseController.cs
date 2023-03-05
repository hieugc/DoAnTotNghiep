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
using Microsoft.AspNetCore.Mvc.ModelBinding;
using DoAnTotNghiep.Modules;

namespace DoAnTotNghiep.Controllers
{
    public abstract class BaseController : Controller
    {
        public BaseController()
        {
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

        protected string ModelErrors()
        {
            List<string> errors = new List<string>();
            foreach (ModelStateEntry modelState in ModelState.Values)
            {
                if (modelState != null)
                {
                    errors.AddRange(modelState.Errors.Select(s => s.ErrorMessage));
                }
            }
            return string.Join(". \n", errors);
        }
    }
}
