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
            try
            {
                string[] arr = imageBase.Data.Split("base64,");
                string ext = imageBase.Name.Split(".").Last();
                var bytes = Convert.FromBase64String(arr[1]);

                string folder = Path.Combine("Uploads", DateTime.Now.ToString("yyyy"), DateTime.Now.ToString("MM"));

                string uploadsFolder = Path.Combine(this.environment.ContentRootPath, "wwwroot", folder);
                Console.WriteLine(uploadsFolder);
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                string ImagePath = Guid.NewGuid().ToString() + "." + ext;
                string filePath = Path.Combine(uploadsFolder, ImagePath);
                System.IO.File.WriteAllBytes(filePath, bytes);
                return new Entity.File() { FileName = ImagePath, PathFolder = folder };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }
        protected Entity.File? SaveFile(IFormFile file)
        {
            try
            {
                string folder = Path.Combine("Uploads", DateTime.Now.ToString("yyyy"), DateTime.Now.ToString("MM"));

                string uploadsFolder = Path.Combine(this.environment.ContentRootPath, "wwwroot", folder);
                Console.WriteLine(uploadsFolder);
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                string ImagePath = Guid.NewGuid().ToString() + ".png";
                string filePath = Path.Combine(uploadsFolder, ImagePath);
                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fs);
                }
                return new Entity.File() { FileName = ImagePath, PathFolder = folder };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }
        protected bool DeleteFile(Entity.File file)
        {
            try
            {
                string uploadsFolder = Path.Combine(this.environment.ContentRootPath, "wwwroot", file.PathFolder);
                string filePath = Path.Combine(uploadsFolder, file.FileName);
                if (!Directory.Exists(filePath))
                {
                    return false;
                }
                System.IO.File.Delete(filePath);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

    }
}
