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

namespace DoAnTotNghiep.Controllers
{
    [Authorize(Roles = Enum.Role.Member)]
    public class HousesController : BaseController
    {
        private readonly DoAnTotNghiepContext _context;
        private readonly IConfiguration _configuration;
        public HousesController(DoAnTotNghiepContext context,
                                IHostEnvironment environment,
                                IConfiguration configuration) : base(environment)
        {
            _context = context;
            _configuration = configuration;
        }

        private async Task<List<double>> GetLocation(string query)
        {
            string protocol = this.Request.Scheme;
            string key = "Asf_PRzBpUJVcb9lcAg48BLOuAuaBItg4ZzCqaNQaSIFReqieYA02KBcovVD08Jk";
            Uri localRequest = RequestAPI.GeoCodeRequest(protocol, query, key);
            var localResult = await RequestAPI.Get<string>(localRequest);
            double lat = 0;
            double lng = 0;
            if (!string.IsNullOrEmpty(localResult) && localResult.IndexOf("coordinates") != -1)
            {
                var split = localResult.Split("\"coordinates\":[")[1].Split("]")[0].Trim();
                double.TryParse(split.Split(",")[0], out lat);
                double.TryParse(split.Split(",")[1], out lng);
            }

            return new List<double>() { lat, lng};
        }

        private async Task<List<double>> GetLocation(string city, string district, string ward, string address)
        {
            string protocol = this.Request.Scheme;
            string key = "Asf_PRzBpUJVcb9lcAg48BLOuAuaBItg4ZzCqaNQaSIFReqieYA02KBcovVD08Jk";
            Uri localRequest = RequestAPI.GeoCodeRequest(protocol, city, district, ward, address, key);
            var localResult = await RequestAPI.Get<string>(localRequest);
            double lat = 0;
            double lng = 0;
            if (!string.IsNullOrEmpty(localResult) && localResult.IndexOf("coordinates") != -1)
            {
                var split = localResult.Split("\"coordinates\":[")[1].Split("]")[0].Trim();
                double.TryParse(split.Split(",")[0], out lat);
                double.TryParse(split.Split(",")[1], out lng);
            }

            return new List<double>() { lat, lng };
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
                            if(data.Lat == 0 || data.Lng == 0)
                            {
                                List<double> doubles = await this.GetLocation(data.Location);
                                data.Lat = doubles[0];
                                data.Lng = doubles[1];
                            }

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
                                IdDistrict = (data.IdDistrict == 0? null: data.IdDistrict),
                                IdWard = (data.IdWard == 0? null: data.IdWard),
                                Rating = 0,
                                IdUser = this.GetIdUser(),
                                Status = (int) Enum.StatusHouse.VALID,
                                StreetAddress = data.Location,
                                Bed = data.Bed
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
                                                        .Where(m => m.IdUser == IdUser 
                                                                    && m.Id == data.Id
                                                                    && m.Status != (int) StatusHouse.SWAPPING)
                                                        .ToList();
                            if (listHouse == null || listHouse.Count < 1)
                            {
                                return BadRequest(new
                                {
                                    Status = 404,
                                    Message = "Không tìm thấy nhà"
                                });
                            }

                            //Lat == 0
                            //Lng == 0
                            if (data.Lat == 0 || data.Lng == 0)
                            {
                                List<double> doubles = await this.GetLocation(data.Location);
                                data.Lat = doubles[0];
                                data.Lng = doubles[1];
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
                            model.IdDistrict = (data.IdDistrict == 0? null: data.IdDistrict);
                            model.IdWard = (data.IdWard == 0 ? null : data.IdWard);
                            model.Price = data.Price;
                            model.Status = data.Status;

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
                                                                .Where(m => m.IdHouse == model.Id 
                                                                            && !idRemove.Contains(m.IdFile))
                                                                .ToList();
                                    List<Entity.File> deleteFiles = Context.Files.Where(m => idRemove.Contains(m.Id)).ToList();
                                    Context.Files.RemoveRange(deleteFiles);
                                    Context.FilesOfHouses.RemoveRange(deleteFileOfHouse);
                                    Context.SaveChanges();
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
                                                                .Where(m => m.IdHouse == model.Id 
                                                                            && data.IdRemove.Contains(m.IdFile))
                                                                .ToList();
                                    List<Entity.File> deleteFiles = Context.Files.Where(m => data.IdRemove.Contains(m.Id)).ToList();
                                    Context.Files.RemoveRange(deleteFiles);
                                    Context.FilesOfHouses.RemoveRange(deleteFileOfHouse);
                                    Context.SaveChanges();
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
                                    .Include(m => m.FeedBacks)
                                    .Where(m => m.Id == id && m.IdUser == IdUser && m.Status != (int)StatusHouse.SWAPPING).ToList();
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
            if (removeHouse.FeedBacks != null) this._context.FeedBacks.RemoveRange(removeHouse.FeedBacks);

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
                                            .Include(m => m.FeedBacks)
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
                if (item.FeedBacks != null) this._context.FeedBacks.RemoveRange(item.FeedBacks);

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
            var house = this.CreateHouse(Id, (int)StatusHouse.VALID);
            if (house == null) return null;
            return this.CreateDetailsHouse(house);
        }
        private DetailHouseViewModel CreateDetailsHouse(House house)
        {
            byte[] salt = Crypto.Salt(this._configuration);
            string host = this.GetWebsitePath();
            if(house.Users != null)
            {
                this._context.Entry(house.Users).Collection(m => m.Houses).Query().Load();
            }
            DetailHouseViewModel model = new DetailHouseViewModel(house, salt, house.Users, host);
            model.Ratings = this.GetRatingByHouse(house, host, salt);
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
        //Detail
        private List<DetailRatingWithUser> GetRatingByHouse(House house, string host, byte[] salt)
        {
            List<DetailRatingWithUser> model = new List<DetailRatingWithUser>();
            this._context.Entry(house)
                        .Collection(m => m.FeedBacks)
                        .Query()
                        .Load();
            if (house.FeedBacks != null)
            {
                List<FeedBack> feedBacks = house.FeedBacks.ToList();
                foreach (var item in feedBacks)
                {
                    this._context.Entry(item).Reference(m => m.Users).Load();
                    if (item.Users != null)
                    {
                        DetailRatingViewModel rating = new DetailRatingViewModel(item);
                        UserInfo user = new UserInfo(item.Users, salt, host);
                        model.Add(new DetailRatingWithUser() { User = user, FeedBack = rating });
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
            var house = this.CreateHouse(Id, (int)StatusHouse.VALID);
            if (house == null) return NotFound();
            int IdUser = this.GetIdUser();
            ViewData["isAuthorize"] = IdUser == 0? "false": "true";
            ViewData["isOwner"] = IdUser == house.Users.Id? "true": "false";
            byte[] salt = Crypto.Salt(this._configuration);
            string host = this.GetWebsitePath();
            PackageDetailHouse model = new PackageDetailHouse(house, salt, house.Users, host);
            ViewData["key"] = "Asf_PRzBpUJVcb9lcAg48BLOuAuaBItg4ZzCqaNQaSIFReqieYA02KBcovVD08Jk";

            model.AllUtilities = this._context.Utilities.ToList();
            model.AllRules = this._context.Rules.ToList();
            model.Ratings = this.GetRatingByHouse(house, host, salt);
            return View(model);
        }
        [AllowAnonymous]
        [HttpGet("/api/House/Details")]
        public IActionResult ApiDetails(int Id)
        {
            var model = this.GetDetailsHouse(Id, (int)StatusHouse.VALID);
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

        [Authorize(Roles = Role.Member)]
        public IActionResult HouseOverView(int Id)
        {
            int IdUser = this.GetIdUser();
            var house = this._context.Houses.Include(m => m.Users)
                                            .Where(m => m.Id == Id && m.Users != null && m.IdUser == IdUser)
                                            .FirstOrDefault();
            if (house == null) return NotFound();
            ViewData["isAuthorize"] = "true";
            ViewData["isOwner"] = "true";
            byte[] salt = Crypto.Salt(this._configuration);
            string host = this.GetWebsitePath();
            PackageDetailHouse model = new PackageDetailHouse(house, salt, house.Users, host);
            ViewData["key"] = "Asf_PRzBpUJVcb9lcAg48BLOuAuaBItg4ZzCqaNQaSIFReqieYA02KBcovVD08Jk";

            model.AllUtilities = this._context.Utilities.ToList();
            model.AllRules = this._context.Rules.ToList();
            model.Ratings = this.GetRatingByHouse(house, host, salt);
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

            var listHouse = this._context.Houses
                                        .Where(m => m.Status == (int)StatusHouse.VALID 
                                                    && m.IdUser == UserId)
                                        .ToList();
            string host = this.GetWebsitePath();
            List<DetailHouseViewModel> res = new List<DetailHouseViewModel>();
            DoAnTotNghiepContext Context = this._context;
            foreach (var item in listHouse)
            {
                Context.Entry(item).Reference(m => m.Citys).Query().Load();
                Context.Entry(item).Reference(m => m.Districts).Query().Load();
                Context.Entry(item).Collection(m => m.FileOfHouses).Query().Load();
                Context.Entry(item).Collection(m => m.RulesInHouses).Query().Load();
                Context.Entry(item).Collection(m => m.UtilitiesInHouses).Query().Load();
                Context.Entry(item).Reference(m => m.Users).Query().Load();
                DetailHouseViewModel model = new DetailHouseViewModel(item, salt, item.Users, host);
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
            var listHouse = this._context.Houses
                                                .Take(number)
                                                .OrderByDescending(m => m.Rating)
                                                .Where(m => m.Status == (int) StatusHouse.VALID)
                                                .ToList();
            byte[] salt = Crypto.Salt(this._configuration);
            string host = this.GetWebsitePath();
            List<DetailHouseViewModel> res = new List<DetailHouseViewModel>();
            DoAnTotNghiepContext Context = this._context;
            foreach (var item in listHouse)
            {
                Context.Entry(item).Reference(m => m.Users).Load();
                Context.Entry(item).Collection(m => m.RulesInHouses).Query().Load();
                Context.Entry(item).Collection(m => m.UtilitiesInHouses).Query().Load();
                Context.Entry(item).Reference(m => m.Citys).Query().Load();
                Context.Entry(item).Reference(m => m.Districts).Query().Load();
                Context.Entry(item).Reference(m => m.Wards).Query().Load();
                Context.Entry(item).Collection(m => m.Requests).Query().Load();
                Context.Entry(item).Collection(m => m.FileOfHouses).Query().Load();
                Context.Entry(item).Collection(m => m.FeedBacks).Query().Load();

                DetailHouseViewModel model = new DetailHouseViewModel(item, salt, item.Users, host);
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

        [Authorize]
        [HttpGet("/api/File/Delelet")]
        public IActionResult DeleteEmptyFile()
        {
            string path = Path.Combine(this.environment.ContentRootPath, "wwwroot");
            var existFile = this._context.Files.Select(m => Path.Combine(path, m.PathFolder, m.FileName)).ToList();
            List<bool> bools = new List<bool>();

            if(System.IO.Directory.Exists(Path.Combine(path, "Uploads", "2023", "03")))
            {
                string[] fileEntries = Directory.GetFiles(Path.Combine(path, "Uploads", "2023", "03"));
                foreach(var item in fileEntries)
                {
                    if (!existFile.Contains(item))
                    {
                        System.IO.File.Delete(item);
                    }
                }
                string[] afileEntries = Directory.GetFiles(Path.Combine(path, "Uploads", "2023", "03"));

                return Json(new
                {
                    Data = new {
                        Db = existFile,
                        BeforeFiles = fileEntries,
                        AfterFiles = afileEntries
                    }
                });
            }
            return Json(new
            {
                Message = "Không có folder"
            });
        }
    }
}
