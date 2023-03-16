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

namespace DoAnTotNghiep.Controllers
{
    [Authorize(Roles = "MEMBER")]
    public class HousesController : BaseController
    {
        private readonly DoAnTotNghiepContext _context;
        protected readonly IHostEnvironment environment;
        private readonly IConfiguration _configuration;
        public HousesController(DoAnTotNghiepContext context, IHostEnvironment environment, IConfiguration configuration)
        {
            _context = context;
            this.environment = environment;
            _configuration = configuration;
        }


        //create
        private async Task<DetailHouseViewModel> CreateHouse(CreateHouse data)
        {
            if (ModelState.IsValid)
            {
                using (var Context = this._context)
                {
                    using (var transaction = await Context.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            House house = new House()
                            {
                                Name = data.Name,
                                Type = data.Option,
                                Description = data.Description,
                                People = data.People,
                                BedRoom = data.BedRoom,
                                BathRoom = data.BathRoom,
                                Area = data.Square,
                                Lat = data.Lat,
                                Lng = data.Lng,
                                Price = data.Price,
                                IdCity = (data.IdCity == 0? 1: data.IdCity),
                                IdDistrict = data.IdDistrict,
                                IdWard = data.IdWard,
                                Rating = 0,
                                IdUser = this.GetIdUser(),
                                Status = (int)DoAnTotNghiep.Enum.Status.PENDING,
                                StreetAddress = data.Location
                            };

                            Context.Houses.Add(house);
                            Context.SaveChanges();

                            //thêm rule
                            List<RulesInHouse> rules = new List<RulesInHouse>();
                            foreach (var item in data.Rules)
                            {
                                RulesInHouse rulesInHouse = new RulesInHouse()
                                {
                                    IdRules = item,
                                    IdHouse = house.Id,
                                    Status = true
                                };
                                rules.Add(rulesInHouse);
                            }
                            Context.RulesInHouses.AddRange(rules);
                            Context.SaveChanges();

                            //thêm utilities
                            List<UtilitiesInHouse> utilities = new List<UtilitiesInHouse>();
                            foreach (var item in data.Utilities)
                            {
                                UtilitiesInHouse utilitiesInHouse = new UtilitiesInHouse()
                                {
                                    IdUtilities = item,
                                    IdHouse = house.Id,
                                    Status = true
                                };
                                utilities.Add(utilitiesInHouse);
                            }
                            Context.UtilitiesInHouse.AddRange(utilities);
                            Context.SaveChanges();

                            //thêm hình
                            List<Entity.File> files = new List<Entity.File>();

                            if (data.Files == null)
                            {
                                files.AddRange(this.CreateListFile(data.Images));
                            }
                            else
                            {
                                files.AddRange(this.CreateListFile(data.Files));
                            }
                            Context.Files.AddRange(files);
                            Context.SaveChanges();

                            List<FileOfHouse> fileOfHouses = new List<FileOfHouse>();
                            foreach (var item in files)
                            {
                                Entity.FileOfHouse fileOfHouse = new FileOfHouse()
                                {
                                    IdFile = item.Id,
                                    IdHouse = house.Id
                                };
                                fileOfHouses.Add(fileOfHouse);
                            }

                            Context.FilesOfHouses.AddRange(fileOfHouses);
                            Context.SaveChanges();

                            transaction.Commit();

                            byte[] salt = Crypto.Salt(this._configuration);

                            DetailHouseViewModel detailHouseViewModel = new DetailHouseViewModel(house, salt);
                            detailHouseViewModel.Rules = data.Rules;
                            detailHouseViewModel.Utilities = data.Utilities;
                            string host = this.GetWebsitePath();
                            List<ImageBase> images = new List<ImageBase>();
                            foreach (var item in files) images.Add(new ImageBase(item, host));

                            detailHouseViewModel.Images.AddRange(images);
                            return detailHouseViewModel;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            return new DetailHouseViewModel() { Id = 0 };
                        }
                    }
                }
            }
            return new DetailHouseViewModel() { Id = -1 };
        }
        private List<Entity.File> CreateListFile(IFormFileCollection? data)
        {
            List<Entity.File> files = new List<Entity.File>();
            if (data != null)
            {
                foreach (var item in data)
                {
                    if (item != null)
                    {
                        Entity.File? file = this.SaveFile(item);
                        if (file != null)
                        {
                            files.Add(file);
                        }
                    }
                }
            }
            return files;
        }
        private List<Entity.File> CreateListFile(List<ImageBase?> data)
        {
            List<Entity.File> files = new List<Entity.File>();
            foreach (var item in data)
            {
                if (item != null)
                {
                    Entity.File? file = this.SaveFile(item);
                    if (file != null)
                    {
                        files.Add(file);
                    }
                }
            }
            return files;
        }
        [HttpPost("House/Create")]
        [ValidateAntiForgeryToken]//test lại
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
        [HttpPost("/api/House/Create")]//test lại
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


        //update
        private async Task<IActionResult> EditHouse(EditHouse data)
        {
            if (ModelState.IsValid)
            {
                using (var Context = this._context)
                {
                    using (var transaction = await Context.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            int IdUser = this.GetIdUser();
                            var listHouse = Context.Houses.Include(m => m.RulesInHouses)
                                                        .Include(m => m.UtilitiesInHouses)
                                                        .Include(m => m.FileOfHouses)
                                                        .Where(m => m.IdUser == IdUser && m.Id == data.Id)
                                                        .ToList();
                            if (listHouse == null || listHouse.Count < 1)
                            {
                                return BadRequest(new
                                {
                                    Status = 404,
                                    Message = "Không tìm thấy nhà"
                                });
                            }

                            var model = listHouse.First();
                            model.Name = data.Name;
                            model.Type = data.Option;
                            model.Description = data.Description;
                            model.People = data.People;
                            model.BedRoom = data.BedRoom;
                            model.BathRoom = data.BathRoom;
                            model.Area = data.Square;
                            model.Lat = data.Lat;
                            model.Lng = data.Lng;
                            model.StreetAddress = data.Location;
                            model.IdCity = data.IdCity;
                            model.IdDistrict = data.IdDistrict;
                            model.IdWard = data.IdWard;
                            model.Price = data.Price;

                            Context.Houses.Update(model);
                            Context.SaveChanges();

                            if (model.RulesInHouses == null)
                            {
                                List<RulesInHouse> rules = new List<RulesInHouse>();
                                foreach (var item in data.Rules)
                                {
                                    RulesInHouse rulesInHouse = new RulesInHouse()
                                    {
                                        IdRules = item,
                                        IdHouse = model.Id,
                                        Status = true
                                    };
                                    rules.Add(rulesInHouse);
                                }
                                Context.RulesInHouses.AddRange(rules);
                            }
                            else
                            {
                                List<RulesInHouse> ruleUpdateFalse = model.RulesInHouses.Where(m => !data.Rules.Contains(m.IdRules)).ToList();
                                foreach (var item in ruleUpdateFalse)
                                {
                                    item.Status = false;
                                }
                                Context.RulesInHouses.UpdateRange(ruleUpdateFalse);

                                List<RulesInHouse> ruleUpdateTrue = model.RulesInHouses.Where(m => data.Rules.Contains(m.IdRules)).ToList();
                                foreach (var item in ruleUpdateTrue)
                                {
                                    item.Status = true;
                                }
                                Context.RulesInHouses.UpdateRange(ruleUpdateTrue);


                                List<int> id = model.RulesInHouses.Where(m => data.Rules.Contains(m.IdRules)).Select(m => m.IdRules).ToList();

                                List<RulesInHouse> rules = new List<RulesInHouse>();
                                foreach (var item in data.Rules)
                                {
                                    if (!id.Contains(item))
                                    {
                                        RulesInHouse rulesInHouse = new RulesInHouse()
                                        {
                                            IdRules = item,
                                            IdHouse = model.Id,
                                            Status = true
                                        };
                                        rules.Add(rulesInHouse);
                                    }
                                }
                                Context.RulesInHouses.AddRange(rules);
                            }
                            Context.SaveChanges();

                            //thêm utilities

                            if (model.UtilitiesInHouses == null)
                            {
                                List<UtilitiesInHouse> utilities = new List<UtilitiesInHouse>();
                                foreach (var item in data.Utilities)
                                {
                                    UtilitiesInHouse utilitiesInHouse = new UtilitiesInHouse()
                                    {
                                        IdUtilities = item,
                                        IdHouse = model.Id,
                                        Status = true
                                    };
                                    utilities.Add(utilitiesInHouse);
                                }
                                Context.UtilitiesInHouse.AddRange(utilities);
                            }
                            else
                            {
                                List<UtilitiesInHouse> utilitiesUpdateFalse = model.UtilitiesInHouses.Where(m => !data.Utilities.Contains(m.IdUtilities)).ToList();
                                foreach (var item in utilitiesUpdateFalse)
                                {
                                    item.Status = false;
                                }
                                Context.UtilitiesInHouse.UpdateRange(utilitiesUpdateFalse);

                                List<UtilitiesInHouse> utilitiesUpdateTrue = model.UtilitiesInHouses.Where(m => data.Utilities.Contains(m.IdUtilities)).ToList();
                                foreach (var item in utilitiesUpdateTrue)
                                {
                                    item.Status = true;
                                }
                                Context.UtilitiesInHouse.UpdateRange(utilitiesUpdateTrue);

                                List<int> id = model.UtilitiesInHouses.Where(m => data.Utilities.Contains(m.IdUtilities)).Select(m => m.IdUtilities).ToList();

                                List<UtilitiesInHouse> utilities = new List<UtilitiesInHouse>();
                                foreach (var item in data.Utilities)
                                {
                                    if (!id.Contains(item))
                                    {
                                        UtilitiesInHouse utilitiesInHouse = new UtilitiesInHouse()
                                        {
                                            IdUtilities = item,
                                            IdHouse = model.Id,
                                            Status = true
                                        };
                                        utilities.Add(utilitiesInHouse);
                                    }
                                }
                                Context.UtilitiesInHouse.AddRange(utilities);
                            }
                            Context.SaveChanges();

                            //thêm hình
                            List<Entity.File> files = new List<Entity.File>();
                            if (data.Files == null)
                            {
                                List<int> idRemove = new List<int>();
                                foreach (var item in data.Images)
                                {
                                    if (item != null)
                                    {
                                        if (!string.IsNullOrEmpty(item.Data) && (item.Id == null || item.Id == 0))
                                        {
                                            Entity.File? file = this.SaveFile(item);
                                            if (file != null)
                                            {
                                                files.Add(file);
                                            }
                                        }
                                        else
                                        {
                                            if (item.Id != null && item.Id != 0)
                                            {
                                                idRemove.Add(item.Id.Value);
                                            }
                                        }
                                    }
                                }
                                if (model.FileOfHouses != null)
                                {
                                    var deleteFileOfHouse = model.FileOfHouses
                                                                .Where(m => m.IdHouse == model.Id && !idRemove.Contains(m.IdFile))
                                                                .Select(m => m.IdFile);
                                    List<Entity.File> deleteFiles = Context.Files.Where(m => deleteFileOfHouse.Contains(m.Id)).ToList();
                                    Context.Files.RemoveRange(deleteFiles);
                                }
                            }
                            else
                            {
                                foreach (var item in data.Files)
                                {
                                    if (item != null)
                                    {
                                        Entity.File? file = this.SaveFile(item);
                                        if (file != null)
                                        {
                                            files.Add(file);
                                        }
                                    }
                                }
                                if (model.FileOfHouses != null)
                                {
                                    var deleteFileOfHouse = model.FileOfHouses
                                                                .Where(m => m.IdHouse == model.Id && !data.IdRemove.Contains(m.IdFile))
                                                                .Select(m => m.IdFile);
                                    List<Entity.File> deleteFiles = Context.Files.Where(m => deleteFileOfHouse.Contains(m.Id)).ToList();
                                    Context.Files.RemoveRange(deleteFiles);
                                }
                            }

                            Context.Files.AddRange(files);
                            Context.SaveChanges();


                            List<FileOfHouse> fileOfHouses = new List<FileOfHouse>();
                            foreach (var item in files)
                            {
                                Entity.FileOfHouse fileOfHouse = new FileOfHouse()
                                {
                                    IdFile = item.Id,
                                    IdHouse = model.Id
                                };
                                fileOfHouses.Add(fileOfHouse);
                            }

                            Context.FilesOfHouses.AddRange(fileOfHouses);
                            Context.SaveChanges();

                            transaction.Commit();
                            byte[] salt = Crypto.Salt(this._configuration);

                            DetailHouseViewModel detailHouseViewModel = new DetailHouseViewModel(model, salt);
                            detailHouseViewModel.Rules = data.Rules;
                            detailHouseViewModel.Utilities = data.Utilities;
                            string host = this.GetWebsitePath();
                            List<ImageBase> images = new List<ImageBase>();
                            Context.Entry(model).Collection(m => m.FileOfHouses).Query().Load();
                            if (model.FileOfHouses != null)
                            {
                                foreach (var item in model.FileOfHouses)
                                {
                                    Context.Entry(item).Reference(m => m.Files).Query().Load();
                                    if (item.Files != null)
                                    {
                                        images.Add(new ImageBase(item.Files, host));
                                    }
                                }
                            }

                            detailHouseViewModel.Images.AddRange(images);
                            return Json(new
                            {
                                Status = 200,
                                Message = "Đã cập nhật thành công",
                                Data = detailHouseViewModel
                            });
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            Console.WriteLine(ex);
                            return BadRequest(new
                            {
                                Status = HttpStatusCode.InternalServerError,
                                Message = "Ứng dụng tạm thời bảo trì"
                            });
                        }
                    }
                }
            }
            return BadRequest(new
            {
                Status = 400,
                Message = this.ModelErrors()
            });
        }
        [HttpPut("/House/Update")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromBody] EditHouseViewModel data)
        {
            return await this.EditHouse(new EditHouse(null, data));
        }
        [HttpPut("/api/House/Update")]
        public async Task<IActionResult> ApiEdit(MobileEditHouseViewModel data)
        {
            return await this.EditHouse(new EditHouse(data, null));
        }


        //remove
        private async Task<IActionResult> DeleteHouse(int id)
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
            var house = this._context.Houses
                                    .Include(m => m.RulesInHouses)
                                    .Include(m => m.UtilitiesInHouses)
                                    .Include(m => m.Requests)
                                    .Include(m => m.FileOfHouses)
                                    .Where(m => m.Id == id && m.IdUser == IdUser && m.Status != (int)Status.SWAPPED).ToList();
            if (house == null || house.Count() < 1)
            {
                return BadRequest(new
                {
                    Status = 404,
                    Message = "Không tìm thấy nhà của bạn"
                });
            }
            var removeHouse = house.First();
            if (removeHouse.RulesInHouses != null) this._context.RulesInHouses.RemoveRange(removeHouse.RulesInHouses);
            if (removeHouse.UtilitiesInHouses != null) this._context.UtilitiesInHouse.RemoveRange(removeHouse.UtilitiesInHouses);
            if (removeHouse.FileOfHouses != null)
            {
                List<Entity.File> files = new List<Entity.File>();
                foreach (var iitem in removeHouse.FileOfHouses)
                {
                    this._context.Entry(iitem).Reference(m => m.Files).Load();
                    if (iitem.Files != null)
                    {
                        this.DeleteFile(iitem.Files);
                        files.Add(iitem.Files);
                    }
                }
                this._context.FilesOfHouses.RemoveRange(removeHouse.FileOfHouses);
                this._context.Files.RemoveRange(files);
            }
            if (removeHouse.Requests != null) this._context.Requests.RemoveRange(removeHouse.Requests);

            this._context.Houses.Remove(removeHouse);
            await this._context.SaveChangesAsync();
            return Json(new
            {
                Status = 200,
                Message = "Đã xóa nhà thành công"
            });
        }
        [HttpPost("/House/Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed([FromBody] int id)
        {
            return await this.DeleteHouse(id);
        }
        [HttpPost("/api/House/Delete")]
        public async Task<IActionResult> ApiDeleteConfirmed(int id)
        {
            return await this.DeleteHouse(id);
        }
        [HttpGet("/api/House/DeleteAll")]
        public IActionResult ApiDeleteConfirmed()
        {
            int IdUser = this.GetIdUser();
            DoAnTotNghiepContext Context = this._context;
            var house = Context.Houses.Include(m => m.FileOfHouses)
                                            .Include(m => m.RulesInHouses)
                                            .Include(m => m.UtilitiesInHouses)
                                            .Include(m => m.Requests)
                                            .Where(m => m.IdUser == IdUser).ToList();
            foreach (var item in house)
            {
                if (item.RulesInHouses != null) Context.RulesInHouses.RemoveRange(item.RulesInHouses);
                if (item.UtilitiesInHouses != null) Context.UtilitiesInHouse.RemoveRange(item.UtilitiesInHouses);
                if (item.FileOfHouses != null)
                {
                    List<Entity.File> files = new List<Entity.File>();
                    foreach (var iitem in item.FileOfHouses)
                    {
                        Context.Entry(iitem).Reference(m => m.Files).Load();
                        if (iitem.Files != null)
                        {
                            files.Add(iitem.Files);
                        }
                    }
                    Context.FilesOfHouses.RemoveRange(item.FileOfHouses);
                    Context.Files.RemoveRange(files);
                }
                if (item.Requests != null) this._context.Requests.RemoveRange(item.Requests);

                Context.Houses.Remove(item);
                Context.SaveChanges();
            }

            return Json(new
            {
                Message = "ok"
            });
        }


        //detail
        private DetailHouseViewModel? GetDetailsHouse(int Id, int status)
        {
            var house = this.CreateHouse(Id, (int)Status.VALID);
            if (house == null) return null;
            return this.CreateDetailsHouse(house);
        }
        private DetailHouseViewModel CreateDetailsHouse(House house)
        {
            byte[] salt = Crypto.Salt(this._configuration);
            DetailHouseViewModel model = new DetailHouseViewModel(house, salt);
            string host = this.GetWebsitePath();
            DoAnTotNghiepContext Context = this._context;
            if (house.FileOfHouses != null)
            {
                foreach (var f in house.FileOfHouses)
                {
                    Context.Entry(f).Reference(m => m.Files).Load();
                    if (f.Files != null)
                    {
                        model.Images.Add(new ImageBase(f.Files, host));
                    }
                }
            }
            return model;
        }
        private House? CreateHouse(int Id, int status)
        {
            var house = this._context.Houses.Include(m => m.RulesInHouses)
                                            .Include(m => m.UtilitiesInHouses)
                                            .Include(m => m.Citys)
                                            .Include(m => m.Districts)
                                            .Include(m => m.Wards)
                                            .Include(m => m.Requests)
                                            .Include(m => m.FileOfHouses)
                                            .Include(m => m.Users)
                                            .FirstOrDefault(m => m.Id == Id && m.Status == status);
            return house;
        }
        [AllowAnonymous]
        public IActionResult Details(int Id)
        {
            var house = this.CreateHouse(Id, (int)Status.VALID);
            if (house == null) return NotFound();
            PackageDetailHouse model = new PackageDetailHouse(house, Crypto.Salt(this._configuration), house.Users, this.GetWebsitePath());
            ViewData["key"] = "Asf_PRzBpUJVcb9lcAg48BLOuAuaBItg4ZzCqaNQaSIFReqieYA02KBcovVD08Jk";
            model.AllUtilities = this._context.Utilities.ToList();
            model.AllRules = this._context.Rules.ToList();
            return View(model);
        }
        
        [AllowAnonymous]
        [HttpGet("/api/House/Details")]
        public IActionResult ApiDetails(int Id)
        {
            var model = this.GetDetailsHouse(Id, (int)Status.VALID);
            if (model == null) return BadRequest(new
            {
                Status = 404,
                Message = "Không tìm thấy nhà"
            });
            return Json(new
            {
                Status = 200,
                Data = model
            });
        }
        //ownerView
        public IActionResult HouseOverView(int Id)
        {
            int IdUser = this.GetIdUser();
            var house = this._context.Houses.Include(m => m.Users)
                                            .Where(m => m.Id == Id && m.Users != null && m.IdUser == IdUser)
                                            .FirstOrDefault();
            if (house == null) return NotFound();
            PackageDetailHouse model = new PackageDetailHouse(house, Crypto.Salt(this._configuration), house.Users, this.GetWebsitePath());
            ViewData["key"] = "Asf_PRzBpUJVcb9lcAg48BLOuAuaBItg4ZzCqaNQaSIFReqieYA02KBcovVD08Jk";
            model.AllUtilities = this._context.Utilities.ToList();
            model.AllRules = this._context.Rules.ToList();
            return View("./Views/Houses/Details.cshtml", model);
        }

        [HttpGet("/api/House/OverView")]
        public IActionResult OwnerHouseOverView(int Id)
        {
            int IdUser = this.GetIdUser();
            var house = this._context.Houses.Include(m => m.Users)
                                            .Where(m => m.Id == Id && m.Users != null && m.IdUser == IdUser)
                                            .FirstOrDefault();
            if (house == null) return BadRequest(new
            {
                Status = 400,
                Messsage = "Không tìm thấy nhà"
            });
            
            return Json(new
            {
                Status = 200,
                Data = this.CreateDetailsHouse(house)
            });
        }



        //get by userAccess
        [HttpGet("/api/House/GetByUserAccess")]
        [AllowAnonymous]
        public IActionResult ApiGetHomeByUserAccess(string UserAcess)
        {
            return this.GetByUserAccess(UserAcess);
        }
        [HttpGet("/House/GetByUserAccess")]
        [AllowAnonymous]
        public IActionResult GetHomeByUserAccess(string UserAcess)
        {
            return this.GetByUserAccess(UserAcess);
        }
        private IActionResult GetByUserAccess(string UserAcess)
        {
            int UserId = 0;

            byte[] salt = Crypto.Salt(this._configuration);
            if (int.TryParse(Crypto.DecodeKey(UserAcess, salt), out UserId))
            {
                return BadRequest(new
                {
                    Status = 400,
                    Message = "Không thể truy cập người dùng"
                });
            }

            var listHouse = this._context.Houses
                                        .Where(m => m.Status == (int)Status.VALID && m.IdUser == UserId)
                                        .ToList();
            string host = this.GetWebsitePath();
            List<DetailHouseViewModel> res = new List<DetailHouseViewModel>();
            DoAnTotNghiepContext Context = this._context;
            foreach (var item in listHouse)
            {
                DetailHouseViewModel model = new DetailHouseViewModel(item, salt);
                if (item.FileOfHouses != null)
                {
                    foreach (var f in item.FileOfHouses)
                    {
                        Context.Entry(f).Reference(m => m.Files).Load();
                        if (f.Files != null)
                        {
                            model.Images.Add(new ImageBase(f.Files, host));
                        }
                    }
                }
                res.Add(model);
            }
            return Json(new
            {
                Status = 200,
                Message = "Get Successfully",
                Data = res
            });
        }


        //get popularHouse
        [HttpGet("/api/GetPopularHouse")]
        [AllowAnonymous]
        public JsonResult GetPopularHouse(int number = 10)
        {
            var listHouse = this._context.Houses.Take(number)
                                                .OrderByDescending(m => m.Rating)
                                                .Where(m => m.Status == (int) Status.VALID)
                                                .ToList();
            byte[] salt = Crypto.Salt(this._configuration);
            string host = this.GetWebsitePath();
            List<DetailHouseViewModel> res = new List<DetailHouseViewModel>();
            DoAnTotNghiepContext Context = this._context;
            foreach (var item in listHouse)
            {
                DetailHouseViewModel model = new DetailHouseViewModel(item, salt);
                if (item.FileOfHouses != null)
                {
                    foreach (var f in item.FileOfHouses)
                    {
                        Context.Entry(f).Reference(m => m.Files).Load();
                        if (f.Files != null)
                        {
                            model.Images.Add(new ImageBase(f.Files, host));
                        }
                    }
                }
                res.Add(model);
            }
            return Json(new
            {
                Status = 200,
                Message = "Get Successfully",
                Data = res
            });
        }


        //lưu hình ảnh
        private Entity.File? SaveFile(ImageBase imageBase)
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
        private Entity.File? SaveFile(IFormFile file)
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
        private bool DeleteFile(Entity.File file)
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
