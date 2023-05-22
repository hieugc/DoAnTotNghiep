using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DoAnTotNghiep.Data;
using DoAnTotNghiep.Entity;
using Microsoft.AspNetCore.Authorization;
using DoAnTotNghiep.ViewModels;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Hosting;
using System.Buffers.Text;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Net;
using DoAnTotNghiep.Enum;
using Microsoft.AspNetCore.Routing;
using DoAnTotNghiep.Modules;
using NuGet.Packaging;
using System.Text;
using Microsoft.EntityFrameworkCore.Internal;
using DoAnTotNghiep.Service;
using System.Net.WebSockets;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DoAnTotNghiep.Controllers
{
    [Authorize(Roles = Enum.Role.Member)]
    public class HousesController : BaseController
    {
        //xong
        private readonly DoAnTotNghiepContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHouseService _houseService;
        private readonly IFeedBackService _feedBackService;
        private readonly IFileService _fileService;
        private readonly IRuleService _ruleService;
        private readonly IUtilitiesService _utilitiesService;
        public HousesController(DoAnTotNghiepContext context,
                                IHostEnvironment environment,
                                IConfiguration configuration,
                                IHouseService houseService,
                                IFeedBackService feedBackService,
                                IFileService fileService,
                                IRuleService ruleService,
                                IUtilitiesService utilitiesService) : base(environment)
        {
            _context = context;
            _configuration = configuration;
            _houseService = houseService;
            _feedBackService = feedBackService;
            _fileService = fileService;
            _ruleService = ruleService;
            _utilitiesService = utilitiesService;
        }

        private async Task<DetailHouseViewModel> CreateHouse(CreateHouse data)
        {
            if (ModelState.IsValid)
            {
                int IdUser = this.GetIdUser();
                string? key = this._configuration.GetConnectionString(BingMaps.Key);
                if (!string.IsNullOrEmpty(key) && (data.Lat == 0 || data.Lng == 0))
                {
                    string query = data.Location + ", " + data.WardName + ", " + data.DistrictName + ", " + data.CityName;
                    List<double> doubles = await RequestAPI.GetLocation(this.Request.Scheme, key, query);
                    data.Lat = doubles[0];
                    data.Lng = doubles[1];
                }
                House house = new House().CreateHouse(data, IdUser, (int)StatusHouse.VALID);
                using (var transaction = await this._context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        this._houseService.SaveHouse(house);
                        this._ruleService.AddRuleForHouse(house, data.Rules);
                        this._utilitiesService.AddUtilitiesForHouse(house, data.Utilities);

                        //thêm hình
                        List<Entity.File> files = new List<Entity.File>();

                        if (data.Files == null)
                        {
                            files.AddRange(this._fileService.SaveRangeFile(this._fileService.AddFileHouse(house, data.Images, this.environment.ContentRootPath)));
                        }
                        else
                        {
                            files.AddRange(this._fileService.SaveRangeFile(this._fileService.AddFileHouse(house, data.Files, this.environment.ContentRootPath)));
                        }
                        this._fileService.SaveRangeFileOfHouse(house, files);
                        transaction.Commit();

                        byte[] salt = Crypto.Salt(this._configuration);

                        DetailHouseViewModel detailHouseViewModel = new DetailHouseViewModel(house, salt);
                        detailHouseViewModel.Rules = data.Rules;
                        detailHouseViewModel.Utilities = data.Utilities;
                        detailHouseViewModel.CityName = data.CityName;
                        detailHouseViewModel.DistrictName = data.DistrictName;
                        detailHouseViewModel.WardName = data.WardName;

                        string host = this.GetWebsitePath();
                        List<ImageBase> images = new List<ImageBase>();
                        foreach (var item in files) images.Add(new ImageBase(item, host));
                        detailHouseViewModel.Images.AddRange(images);
                        return detailHouseViewModel;
                    }
                    catch (Exception ex)
                    {
                        FileSystem.WriteExceptionFile(ex.ToString(), "SaveHouse_" + IdUser);
                        transaction.Rollback();
                        return new DetailHouseViewModel() { Id = 0 };
                    }
                }
            }
            return new DetailHouseViewModel() { Id = -1 };
        }
        [HttpPost("House/Create")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Create([FromBody] CreateHouseViewModel data)
        {
            DetailHouseViewModel returnCode = await this.CreateHouse(new CreateHouse(null, data));
            switch (returnCode.Id)
            {
                case -1:
                    return Json(new
                    {
                        Status = 400,
                        Message = ModelErrors()
                    });
                case 0:
                    return Json(new
                    {
                        Status = HttpStatusCode.InternalServerError,
                        Message = "Ứng dụng tạm thời bảo trì"
                    });
            }
            return Json(new
            {
                Status = 200,
                Message = "Đã khởi tạo thành công!",
                Data = returnCode
            });
        }
        [HttpPost("/api/House/Create")]
        public async Task<IActionResult> ApiCreate(MobileCreateHouseViewModel data)
        {
            DetailHouseViewModel returnCode = await this.CreateHouse(new CreateHouse(data, null));
            switch (returnCode.Id)
            {
                case -1:
                    return BadRequest(new
                    {
                        Status = 400,
                        Message = this.ModelErrors()
                    });
                case 0:
                    return BadRequest(new
                    {
                        Status = 500,
                        Message = "Ứng dụng tạm thời bảo trì"
                    });
            }
            return Json(new
            {
                Status = 200,
                Message = "Đã khởi tạo thành công",
                Data = new { }
            });
        }

        private async Task<IActionResult> EditHouse(EditHouse data)
        {
            if (ModelState.IsValid)
            {
                int IdUser = this.GetIdUser();
                var model = this._houseService.GetHouseByIdWithUser(data.Id, IdUser);
                if (model == null)
                {
                    return BadRequest(new
                    {
                        Status = 404,
                        Message = "Không tìm thấy nhà"
                    });
                }
                string? key = this._configuration.GetConnectionString(BingMaps.Key);
                if (!string.IsNullOrEmpty(key) && (data.Lat == 0 || data.Lng == 0))
                {
                    string query = data.Location + ", " + data.WardName + ", " + data.DistrictName + ", " + data.CityName;
                    List<double> doubles = await RequestAPI.GetLocation(this.Request.Scheme, key, query);
                    data.Lat = doubles[0];
                    data.Lng = doubles[1];
                }
                using (var transaction = await this._context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        model.IncludeAll(this._context);
                        model.EditHouse(data);
                        this._houseService.UpdateHouse(model);
                        this._ruleService.UpdateRuleForHouse(model, data.Rules);
                        this._utilitiesService.UpdateUtilitiesForHouse(model, data.Utilities);

                        List<Entity.File> files = new List<Entity.File>();
                        if (data.Files == null)
                        {
                            files.AddRange(this._fileService.SaveRangeFile(this._fileService.UpdateFileHouse(model, data.Images, this.environment.ContentRootPath)));
                        }
                        else
                        {
                            files.AddRange(this._fileService.SaveRangeFile(this._fileService.UpdateFileHouse(model, data.Files, data.IdRemove, this.environment.ContentRootPath)));
                        }
                        this._fileService.SaveRangeFileOfHouse(model, files);

                        transaction.Commit();

                        byte[] salt = Crypto.Salt(this._configuration);
                        DetailHouseViewModel detailHouseViewModel = new DetailHouseViewModel(model, salt);
                        detailHouseViewModel.Rules = data.Rules;
                        detailHouseViewModel.Utilities = data.Utilities;
                        detailHouseViewModel.CityName = data.CityName;
                        detailHouseViewModel.DistrictName = data.DistrictName;
                        detailHouseViewModel.WardName = data.WardName;
                        string host = this.GetWebsitePath();
                        detailHouseViewModel.Images.AddRange(this._fileService.GetImageBases(model, host));

                        return Json(new
                        {
                            Status = 200,
                            Message = "Đã cập nhật thành công",
                            Data = detailHouseViewModel
                        });
                    }
                    catch (Exception ex)
                    {
                        FileSystem.WriteExceptionFile(ex.ToString(), "UpdateHouse_" + model.Id);
                        transaction.Rollback();
                        return BadRequest(new
                        {
                            Status = HttpStatusCode.InternalServerError,
                            Message = "Ứng dụng tạm thời bảo trì"
                        });
                    }
                }
            }
            return BadRequest(new
            {
                Status = 400,
                Message = this.ModelErrors()
            });
        }
        [HttpPost("/House/Update")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromBody] EditHouseViewModel data)
        {
            return await this.EditHouse(new EditHouse(null, data));
        }
        [HttpPost("/api/House/Update")]
        public async Task<IActionResult> ApiEdit(MobileEditHouseViewModel data)
        {
            return await this.EditHouse(new EditHouse(data, null));
        }

        //remove
        private IActionResult DeleteHouse(int id)
        {
            if (_context.Houses == null)
            {
                return BadRequest(new
                {
                    Status = HttpStatusCode.ServiceUnavailable,
                    Message = "Hệ thống đang bảo trì"
                });
            }

            int IdUser = this.GetIdUser();
            var house = this._houseService.GetHouseByIdWithUser(id, IdUser);


            if (house == null)
            {
                return BadRequest(new
                {
                    Status = 404,
                    Message = "Không tìm thấy nhà của bạn"
                });
            }

            if (this._houseService.CheckAnyValidRequest(house))
            {
                return BadRequest(new
                {
                    Status = 404,
                    Message = "Nhà đang thực hiện yêu cầu"
                });
            }
            else
            {
                this._houseService.DeleteHouse(house);
            }

            return Json(new
            {
                Status = 200,
                Message = "Đã xóa nhà thành công"
            });
        }
        [HttpPost("/House/Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed([FromBody] int id)
        {
            return this.DeleteHouse(id);
        }
        [HttpPost("/api/House/Delete")]
        public IActionResult ApiDeleteConfirmed(int id)
        {
            return this.DeleteHouse(id);
        }

        [AllowAnonymous]
        [HttpGet("/api/House/Details")]
        public IActionResult ApiDetails(int Id)
        {
            var house = this._houseService.GetHouseByIdStatus(Id, (int)StatusHouse.VALID);
            if (house == null) return BadRequest(new
            {
                Status = 404,
                Message = "Không tìm thấy nhà"
            }); ;

            byte[] salt = Crypto.Salt(this._configuration);
            string host = this.GetWebsitePath();

            return Json(new
            {
                Status = 200,
                Data = this._houseService.UpdateDetailHouse(
                        this._houseService.GetDetailHouse(house, host, salt),
                        this._fileService.GetImageBases(house, host),
                        this._feedBackService.GetRatingByHouse(house, host, salt)
                )
            });
        }
        [AllowAnonymous]
        public IActionResult Details(int Id)
        {
            return this.DetailHouse(Id, false);
        }
        [Authorize(Roles = Role.Member)]
        public IActionResult HouseOverView(int Id)
        {
            ViewData["isAuthorize"] = "true";
            ViewData["isOwner"] = "true";
            return this.DetailHouse(Id, true);
        }
        private IActionResult DetailHouse(int Id, bool isOverView)
        {
            var house = this._houseService.GetHouseByIdStatus(Id, (int)StatusHouse.VALID);
            if (house == null) return NotFound();

            int IdUser = this.GetIdUser();
            house.IncludeAll(this._context);
            if (!isOverView)
            {
                ViewData["isAuthorize"] = IdUser == 0 ? "false" : "true";
                ViewData["isOwner"] = IdUser == house.Users.Id ? "true" : "false";
            }
            
            byte[] salt = Crypto.Salt(this._configuration);
            string host = this.GetWebsitePath();
            PackageDetailHouse model = new PackageDetailHouse(house, salt, house.Users, host);
            model.AllUtilities = this._utilitiesService.All();
            model.AllRules = this._ruleService.All();
            model.Ratings.AddRange(this._feedBackService.GetRatingByHouse(house, host, salt));
            model.Images.AddRange(this._fileService.GetImageBases(house, host));

            if (IdUser != 0)
            {
                this.SetViewData(new DoAnTotNghiepContext(this._context.GetConfig()), IdUser, Crypto.Salt(this._configuration));
            }

            return View("~/Views/Houses/Details.cshtml", model);
        }

        [HttpGet("/api/House/GetByUserAccess")]
        [AllowAnonymous]
        public IActionResult ApiGetHomeByUserAccess(string UserAccess)
        {
            return this.GetByUserAccess(UserAccess);
        }
        [HttpGet("/House/GetByUserAccess")]
        [AllowAnonymous]
        public IActionResult GetHomeByUserAccess(string UserAccess)
        {
            return this.GetByUserAccess(UserAccess);
        }

        private IActionResult GetByUserAccess(string UserAccess)
        {
            int UserId = 0;

            byte[] salt = Crypto.Salt(this._configuration);
            if (!int.TryParse(Crypto.DecodeKey(UserAccess, salt), out UserId))
            {
                return BadRequest(new
                {
                    Status = 400,
                    Message = "Không thể truy cập người dùng"
                });
            }

            var listHouse = this._houseService.GetListHouseByUser(UserId);
            string host = this.GetWebsitePath();
            return Json(new
            {
                Status = 200,
                Message = "Get Successfully",
                Data = this._houseService.GetListDetailHouseWithManyImages(listHouse, this._fileService, host, salt)
            });
        }

        [HttpGet("/api/GetPopularHouse")]
        [AllowAnonymous]
        public JsonResult GetPopularHouse(int number = 10)
        {
            var listHouse = this._houseService.GetListPopularHouse(number);
            byte[] salt = Crypto.Salt(this._configuration);
            string host = this.GetWebsitePath();
            return Json(new
            {
                Status = 200,
                Message = "Get Successfully",
                Data = this._houseService.GetListDetailHouseWithManyImages(listHouse, this._fileService, host, salt)
            });
        }
    }
}
