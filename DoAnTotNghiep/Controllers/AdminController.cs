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
using NuGet.Protocol;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using NuGet.Common;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static System.Net.WebRequestMethods;
using System.Composition;
using DoAnTotNghiep.TrainModels;
using DoAnTotNghiep.Service;

namespace DoAnTotNghiep.Controllers
{
    [Authorize(Roles = Role.Admin)]
    public class AdminController: BaseController
    {
        private readonly DoAnTotNghiepContext _context;
        private readonly IConfiguration _configuration;
        private readonly IFeedBackService _feedBackService;
        private readonly IRequestService _requestService;
        private readonly IFileService _fileService;
        private readonly IHouseService _houseService;
        private readonly IUserService _userService;
        private readonly IRuleService _ruleService;
        private readonly IUtilitiesService _utilitiesService;
        private readonly ICircleRequestService _circleRequestService;

        public AdminController(DoAnTotNghiepContext context, 
                                IConfiguration configuration,
                                IHostEnvironment environment,
                                IFeedBackService feedBackService,
                                IRequestService requestService,
                                IFileService fileService,
                                IHouseService houseService,
                                IUserService userService,
                                IRuleService ruleService,
                                IUtilitiesService utilitiesService,
                                ICircleRequestService circleRequestService) : base(environment)
        {
            _context = context;
            _configuration = configuration;
            _feedBackService = feedBackService;
            _requestService = requestService;
            _fileService = fileService;
            _houseService = houseService;
            _userService = userService;
            _ruleService = ruleService;
            _utilitiesService = utilitiesService;
            _circleRequestService = circleRequestService;
        }

        public IActionResult Index()
        {

            return View();
        }

        [HttpGet("/PredictSquare")]
        [AllowAnonymous]
        public IActionResult GetSquared()
        {
            return Json(new PredictHouse().GetSquare());
        }
        public IActionResult OverViewProfile(string? UserAccess)
        {
            int IdAnotherUser = 0;
            if (!int.TryParse(Crypto.DecodeKey(UserAccess, Crypto.Salt(this._configuration)), out IdAnotherUser))
            {
                return NotFound();
            }
            var user = this._userService.GetById(IdAnotherUser);
            if (user == null) return NotFound();
            byte[] salt = Crypto.Salt(this._configuration);
            string host = this.GetWebsitePath();
            UserInfo.GetEntityRelated(user, this._context);
            int IdUser = this.GetIdUser();
            ViewData["isSelf"] = "false";

            if (IdUser != 0)
            {
                this.SetViewData(new DoAnTotNghiepContext(this._context.GetConfig()), IdUser, Crypto.Salt(this._configuration));
            }
            return View(new UserProfile(
                                new UserInfo(user, salt, host),
                                this._houseService.GetListDetailHouseWithOneImage(
                                this._houseService.GetListHouseByUser(IdAnotherUser),
                                this._fileService, host, salt),
                                this._feedBackService.GetRatingById(IdAnotherUser, host, salt)));
        }
    }
}
