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
using Microsoft.AspNetCore.Authorization;
using DoAnTotNghiep.Enum;
using DoAnTotNghiep.ViewModels;

namespace DoAnTotNghiep.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly IHostEnvironment environment;
        public BaseController(IHostEnvironment environment)
        {
            this.environment = environment;
        }
        public IActionResult NotFound() => PartialView("./Views/Base/NotFound.cshtml");
        public IActionResult UnderMaintenance() => PartialView("./Views/Base/UnderMaintenance.cshtml");
        [Authorize]
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
        [Authorize]
        protected string GetRole()
        {
            try
            {
                Claim? claim = this.HttpContext.User.FindFirst(ClaimTypes.Role);
                return claim == null ? Role.UnAuthorize : claim.Value;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Role.UnAuthorize;
            }
        }
        [Authorize]
        protected string GetEmail()
        {
            try
            {
                Claim? claim = this.HttpContext.User.FindFirst(ClaimTypes.Email);
                return claim == null ? Role.UnAuthorize : claim.Value;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Role.UnAuthorize;
            }
        }
        [Authorize]
        protected string GetPassword()
        {
            try
            {
                Claim? claim = this.HttpContext.User.FindFirst(ClaimTypes.Hash);
                return claim == null ? string.Empty : claim.Value;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return string.Empty;
            }
        }
        [Authorize]
        protected string GetExpired()
        {
            try
            {
                Claim? claim = this.HttpContext.User.FindFirst(ClaimTypes.Expired);
                return claim == null ? DateTime.UtcNow.ToString(): claim.Value;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return DateTime.UtcNow.ToString();
            }
        }
        protected void SetCookie(string key, string value, int? expireTime)
        {
            CookieOptions option = new CookieOptions();
            if (expireTime.HasValue)
                option.Expires = DateTime.Now.AddHours(expireTime.Value);
            else
                option.Expires = DateTime.Now.AddHours(8);
            Response.Cookies.Append(key, value, option);
        }
        protected void SetViewData(DoAnTotNghiepContext _context, int IdUser, byte[] salt)
        {
            var user = _context.Users.Include(m => m.Files)
                                    .Include(m => m.Houses)
                                    .Where(m => m.Id == IdUser).FirstOrDefault();
            if (user != null)
            {
                ViewData["UserInfoData"] = new UserInfo(user, salt, this.GetWebsitePath());
                ViewData["userName"] = user.LastName + " " + user.FirstName;
                ViewData["userRole"] = user.Role == 1? DoAnTotNghiep.Enum.Role.Admin : DoAnTotNghiep.Enum.Role.Member;
            }
        }
        protected void RemoveCookie(string key)
        {
            Response.Cookies.Delete(key);
        }
        protected string? GetCookie(string key)
        {
            Request.Cookies.TryGetValue(key, out var cookie);
            return cookie;
        }
        protected string GetWebsitePath() => this.HttpContext.Request.Scheme + "://" + this.HttpContext.Request.Host;
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


        //lưu hình ảnh
        protected Entity.File? SaveFile(ImageBase imageBase)
        {
            return FileSystem.SaveFileWithBase64(imageBase, this.environment.ContentRootPath);
        }
        protected Entity.File? SaveFile(IFormFile file)
        {
            return FileSystem.SaveFileWithFormFile(file, this.environment.ContentRootPath);
        }
        protected bool DeleteFile(Entity.File file)
        {
            return FileSystem.DeleteFile(file, this.environment.ContentRootPath);
        }

    }
}
