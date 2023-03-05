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

namespace DoAnTotNghiep.Controllers
{
    [Authorize]
    public class HousesController : BaseController
    {
        private readonly DoAnTotNghiepContext _context;
        protected readonly IHostEnvironment environment;
        public HousesController(DoAnTotNghiepContext context, IHostEnvironment environment)
        {
            _context = context;
            this.environment = environment;
        }

        public async Task<IActionResult> Index()
        {
            var doAnTotNghiepContext = _context.Houses.Include(h => h.Citys).Include(h => h.Districts).Include(h => h.Users).Include(h => h.Wards);
            return View(await doAnTotNghiepContext.ToListAsync());
        }

        [AllowAnonymous]
        public IActionResult Details(int? id)
        {
            //kiểm tra người dùng có phải chủ nhân không
            // => có => show kể cả status gì
            // => không thì chỉ cho xem (int) Status.VALID
            // => ! (int) Status.VALID => NotFound()
            return View();
        }

        [HttpPost("House/Create")]
        [ValidateAntiForgeryToken]//test lại
        public async Task<JsonResult> Create([FromBody] CreateHouseViewModel data)
        {
            DetailHouseViewModel returnCode = await this.CreateHouse(data);
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

        private async Task<DetailHouseViewModel> CreateHouse(CreateHouseViewModel data)
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
                                IdCity = 1,
                                IdDistrict = 1,
                                IdWard = 1,
                                Rating = 0,
                                IdUser = this.GetIdUser(),
                                Status = (int)DoAnTotNghiep.Enum.Status.PENDING
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
                            foreach (var item in data.Images)
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
                            DetailHouseViewModel detailHouseViewModel = DetailHouseViewModel.GetByHouse(house);
                            detailHouseViewModel.Rules = data.Rules;
                            detailHouseViewModel.Utilities = data.Utilities;
                            detailHouseViewModel.Images = data.Images;
                            return detailHouseViewModel;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            Console.WriteLine(ex);
                            return new DetailHouseViewModel() { Id = 0};
                        }
                    }
                }
            }
            return new DetailHouseViewModel() { Id = -1};
        }

        [HttpPost("/api/House/Create")]//test lại
        public async Task<IActionResult> apiCreate([FromBody] CreateHouseViewModel data)
        {
            DetailHouseViewModel returnCode = await this.CreateHouse(data);
            switch (returnCode.Id)
            {
                case -1:
                    return BadRequest(new
                    {
                        Message = this.ModelErrors()
                    });
                case 0:
                    return Json(new
                    {
                        Status = HttpStatusCode.InternalServerError,
                        Message = "Ứng dụng tạm thời bảo trì"
                    });
            }
            return Ok(new
            {
                Message = "Đã khởi tạo thành công",
                Data = returnCode
            });
        }

        [HttpPost("House/Update")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromBody] EditHouseViewModel data)
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
                                return Json(new
                                {
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

                            if(model.RulesInHouses == null)
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
                                List<RulesInHouse> ruleUpdate = model.RulesInHouses.Where(m => !data.Rules.Contains(m.IdRules)).ToList();
                                foreach(var item in ruleUpdate)
                                {
                                    item.Status = false;
                                }
                                Context.RulesInHouses.UpdateRange(ruleUpdate);

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
                                List<UtilitiesInHouse> utilitiesUpdate = model.UtilitiesInHouses.Where(m => !data.Utilities.Contains(m.IdUtilities)).ToList();
                                foreach (var item in utilitiesUpdate)
                                {
                                    item.Status = false;
                                }
                                Context.UtilitiesInHouse.UpdateRange(utilitiesUpdate);

                                List<int> id = model.UtilitiesInHouses.Where(m => data.Utilities.Contains(m.IdUtilities)).Select(m => m.IdUtilities).ToList();

                                List<UtilitiesInHouse> utilities = new List<UtilitiesInHouse>();
                                foreach (var item in id)
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
                            List<int> idRemove = new List<int>();
                            foreach (var item in data.Images)
                            {
                                if (item != null)
                                {
                                    if (!string.IsNullOrEmpty(item.Data))
                                    {
                                        Entity.File? file = this.SaveFile(item);
                                        if (file != null)
                                        {
                                            files.Add(file);
                                        }
                                    }
                                    else
                                    {
                                        if(item.Id != null)
                                        {
                                            idRemove.Add(item.Id.Value);
                                        }
                                    }
                                }
                            }
                            if(model.FileOfHouses != null)
                            {
                                var deleteFileOfHouse = model.FileOfHouses
                                                            .Where(m => m.IdHouse == model.Id && !idRemove.Contains(m.IdFile))
                                                            .Select(m => m.IdFile);
                                List<Entity.File> deleteFiles = Context.Files.Where(m => deleteFileOfHouse.Contains(m.Id)).ToList();
                                Context.Files.RemoveRange(deleteFiles);
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
                            return Ok(new
                            {
                                Message = "Đã cập nhật thành công",
                                Data = data
                            });
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            Console.WriteLine(ex);
                            return Json(new
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
                Message = this.ModelErrors()
            });
        }

        [HttpDelete("/House/Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed([FromBody]int id)
        {
            if (_context.Houses == null)
            {
                return Json(new
                {
                    Status = HttpStatusCode.ServiceUnavailable,
                    Message = "Hệ thống đang bảo trì"
                });
            }
            var house = this._context.Houses.Where(m => m.Id == id && m.Status != (int) Status.SWAPPED).ToList();
            if (house == null || house.Count() < 1)
            {
                return Json(new
                {
                    Status = HttpStatusCode.NotModified,
                    Message = "Không tìm thấy nhà của bạn"
                });
            }
            var removeHouse = house.First();
            this._context.Entry(removeHouse).Collection(m => m.RulesInHouses)
                                            .Load();
            this._context.Entry(removeHouse).Collection(m => m.UtilitiesInHouses)
                                            .Load();
            this._context.Entry(removeHouse).Collection(m => m.Requests)
                                            .Load();
            this._context.Entry(removeHouse).Collection(m => m.FileOfHouses)
                                            .Load();

            if (removeHouse.RulesInHouses != null) this._context.RulesInHouses.RemoveRange(removeHouse.RulesInHouses);
            if (removeHouse.UtilitiesInHouses != null) this._context.UtilitiesInHouse.RemoveRange(removeHouse.UtilitiesInHouses);
            if (removeHouse.FileOfHouses != null)
            {
                List<Entity.File> files = new List<Entity.File>();
                foreach(var item in removeHouse.FileOfHouses)
                {
                    if(item.Files != null && this.DeleteFile(item.Files)) files.Add(item.Files);
                }
                this._context.FilesOfHouses.RemoveRange(removeHouse.FileOfHouses);
                this._context.Files.RemoveRange(files);
            }
            if (removeHouse.Requests != null) this._context.Requests.RemoveRange(removeHouse.Requests);

            this._context.Houses.Remove(removeHouse);
            await _context.SaveChangesAsync();
            return Json(new
            {
                Status = 200,
                Message = "Đã xóa nhà thành công"
            });
        }

        [HttpGet]
        public JsonResult GetImagesOfHouse(int IdHouse)
        {
            var model = this._context.FilesOfHouses.Include(m => m.Files).Where(m => m.IdHouse == IdHouse)
                .Select(m =>  new ImageBase()
            {
                    Name = m.Files == null? string.Empty: m.Files.FileName,
                    Data = string.Empty,
                    Folder = m.Files == null? string.Empty: m.Files.PathFolder,
                    Id = m.IdFile
            });
            return Json(new
            {
                Status = 200,
                IdHouse = IdHouse,
                Data = model
            });
        }

        private bool HouseExists(int id)
        {
          return _context.Houses.Any(e => e.Id == id);
        }

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

        private bool DeleteFile(Entity.File file)
        {
            try
            {
                string uploadsFolder = Path.Combine(this.environment.ContentRootPath, "wwwroot", file.PathFolder);

                if (!Directory.Exists(uploadsFolder))
                {
                    return false;
                }
                string filePath = Path.Combine(uploadsFolder, file.FileName);
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
