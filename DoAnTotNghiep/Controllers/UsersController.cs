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
using Microsoft.AspNetCore.Authorization;
using DoAnTotNghiep.Enum;
using DoAnTotNghiep.Modules;
using DoAnTotNghiep.ViewModels;
using System.Composition.Convention;

namespace DoAnTotNghiep.Controllers
{
    [Authorize(Roles = Role.Admin)]
    public class UsersController : BaseController
    {
        private readonly DoAnTotNghiepContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHouseService _houseService;
        private readonly IUserService _userService;
        private readonly IFileService _fileService;
        private readonly IRequestService _requestService;
        private readonly IFeedBackService _feedBackService;
        public UsersController(DoAnTotNghiepContext context,
                                IRequestService requestService,
                                IFileService fileService,
                                IHouseService houseService,
                                IUserService userService,
                                IConfiguration configuration,
                                IHostEnvironment environment,
                                IFeedBackService feedBackService) : base(environment)
        {
            _context = context;
            _houseService = houseService;
            _userService = userService;
            _fileService = fileService;
            _requestService = requestService;
            _configuration = configuration;
            _feedBackService = feedBackService;
        }
        [HttpGet("/User/GetNumberSwap")]
        public IActionResult GetNumberSwap(int userId)
        {
            return Json(new
            {
                Status = 200,
                Data = this._userService.NumberSwap(userId)
            });
        }
        [HttpPost("/User/BanUser")]
        public IActionResult Ban([FromBody] int idUser)
        {
            var user = this._userService.GetById(idUser);
            if(user == null)
            {
                return BadRequest(new
                {
                    Status = 400,
                    Message = "Không tìm thấy người dùng"
                });
            }
            user.IsBan = true;
            this._userService.UpdateUser(user);
            return Json(new {
                Status = 200,
                Message = "ok"
            });
        }
        [HttpGet("/User/Details")]
        public IActionResult Details(int idUser)
        {
            var user = this.Profile(idUser);
            if (user == null) return NotFound();
            int IdUser = this.GetIdUser();
            if (IdUser != 0)
            {
                this.SetViewData(new DoAnTotNghiepContext(this._context.GetConfig()), IdUser, Crypto.Salt(this._configuration));
            }
            return View("~/Views/Users/_Details.cshtml", user);
        }
        [HttpGet("/Admin/OverViewProfile")]
        public IActionResult OverViewProfile(string? UserAccess)
        {
            int IdAnotherUser = 0;
            if (string.IsNullOrEmpty(UserAccess) || !int.TryParse(Crypto.DecodeKey(UserAccess, Crypto.Salt(this._configuration)), out IdAnotherUser))
            {
                return NotFound();
            }
            var user = this.Profile(IdAnotherUser);
            if (user == null) return NotFound();
            int IdUser = this.GetIdUser();
            if (IdUser != 0)
            {
                this.SetViewData(new DoAnTotNghiepContext(this._context.GetConfig()), IdUser, Crypto.Salt(this._configuration));
            }
            return View("~/Views/Users/_Details.cshtml", user);
        }
        public UserProfile? Profile(int idUser)
        {
            var user = this._userService.GetById(idUser);
            if (user == null) return null;
            byte[] salt = Crypto.Salt(this._configuration);
            string host = this.GetWebsitePath();
            UserInfo.GetEntityRelated(user, this._context);

            return new UserProfile(
                                new UserInfo(user, salt, host),
                                this._houseService.GetListDetailHouseWithOneImage(
                                this._houseService.GetListHouseByUser(idUser),
                                this._fileService, host, salt),
                                this._feedBackService.GetRatingById(idUser, host, salt));
        }
    }
}
