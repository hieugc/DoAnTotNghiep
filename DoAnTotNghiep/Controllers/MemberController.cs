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
using Microsoft.Extensions.Hosting;
using System.Text;
using Newtonsoft.Json;
using NuGet.Packaging;
using Microsoft.AspNetCore.SignalR;
using DoAnTotNghiep.Hubs;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.AspNetCore.Routing;
using System.Security.Cryptography.X509Certificates;

namespace DoAnTotNghiep.Controllers
{
    [Authorize(Roles = Enum.Role.Member)]
    public class MemberController : BaseController
    {
        private readonly DoAnTotNghiepContext _context;
        private readonly IConfiguration _configuration;
        private IHubContext<ChatHub> _signalContext;

        public MemberController(DoAnTotNghiepContext context, IConfiguration configuration, IHubContext<ChatHub> signalContext, IHostEnvironment environment) : base(environment)
        {
            _context = context;
            _configuration = configuration;
            _signalContext = signalContext;
        }


        [HttpPost("api/GetIdMobile")]
        public IActionResult apiMobile()
        {
            return Ok(new
            {
                Message = this.GetIdUser()
            });
        }
        public IActionResult Infomation()
        {
            ViewData["active"] = 0;
            int IdUser = this.GetIdUser();
            var user = this._context.Users.Where(m => m.Id == IdUser).FirstOrDefault();
            if(user == null)
            {
                return View(new UpdateUserInfo());
            }
            return View(new UpdateUserInfo(user, this.GetWebsitePath()));
        }
        private ListDetailHouses GetHouseInMemberPage(Pagination pagination)
        {
            int IdUser = this.GetIdUser();
            var listHouse = this._context.Houses.Where(m => m.IdUser == IdUser)
                                                .ToList();

            pagination.Total = (int)Math.Ceiling((double)(listHouse.Count() / pagination.Limit));
            int skip = pagination.Page - 1 < 0 ? 0: pagination.Page - 1;
            byte[] salt = Crypto.Salt(this._configuration);
            string host = this.GetWebsitePath();

            List<DetailHouseViewModel> detailHouseViewModels = new List<DetailHouseViewModel>();
            DoAnTotNghiepContext Context = this._context;
            foreach (var item in listHouse.Skip(skip).Take(pagination.Limit))
            {
                Context.Entry(item).Reference(m => m.Users).Load();
                Context.Entry(item).Collection(m => m.RulesInHouses).Query().Load();
                Context.Entry(item).Collection(m => m.UtilitiesInHouses).Query().Load();
                Context.Entry(item).Reference(m => m.Citys).Query().Load();
                Context.Entry(item).Reference(m => m.Districts).Query().Load();
                Context.Entry(item).Reference(m => m.Wards).Query().Load();
                Context.Entry(item).Collection(m => m.Requests).Query().Where(m => m.Status == (int)StatusRequest.WAIT_FOR_SWAP).Load();
                Context.Entry(item).Collection(m => m.FileOfHouses).Query().Load();

                DetailHouseViewModel model = new DetailHouseViewModel(item, salt, item.Users, host);
                if (item.FileOfHouses != null)
                {
                    foreach (var f in item.FileOfHouses)
                    {
                        Context.Entry(f).Reference(m => m.Files).Load();
                        if(f.Files != null)
                        {
                            model.Images.Add(new ImageBase(f.Files, host));
                        }
                    }
                }
                detailHouseViewModels.Add(model);
            }
            return new ListDetailHouses()
            {
                Houses  = detailHouseViewModels,
                Pagination = pagination
            };
        }
        public IActionResult House(Pagination pagignation)
        {
            ViewData["active"] = 1;//xác định tab active

            var listUtilities = this._context.Utilities.ToList();
            var listRules = this._context.Rules.ToList();
            ListDetailHouses detailHouseViewModels = this.GetHouseInMemberPage(pagignation);
            AuthHouseViewModel model = new AuthHouseViewModel()
            {
                Houses = detailHouseViewModels,
                OptionHouses = new OptionHouseViewModel()
                {
                    Utilities = listUtilities,
                    Rules = listRules
                }
            };

            return View(model);
        }

        [HttpGet("/api/House/GetMyHome")]
        public IActionResult ApiHouse(Pagination pagination)
        {
            ListDetailHouses model = this.GetHouseInMemberPage(pagination);
            return Json(new
                {
                    Status = 200,
                    Data = model
            });
        }
        [HttpGet("/House/GetMyHome")]
        public IActionResult GetMyHome(Pagination pagination)
        {
            ListDetailHouses model = this.GetHouseInMemberPage(pagination);
            return Json(new
            {
                Status = 200,
                Data = model
            });
        }
        public IActionResult Requested()
        {
            ViewData["active"] = 2;
            List<DetailRequest> model = this.GetAllRequestsSent();

            return View(model);
        }

        [HttpGet("/api/Request/GetRequestReceived")]
        public IActionResult ApiGetListRequestReceived()
        {
            return Json(new
            {
                Status = 200,
                Data = this.GetAllRequestsReceived()
            });
        }
        private List<DetailRequest> GetAllRequestsReceived()
        {
            int IdUser = this.GetIdUser();
            var houses = this._context.Houses
                                        .Where(m => m.IdUser == IdUser && m.Status == (int)StatusHouse.VALID)
                                        .ToList();
            List<DetailRequest> model = new List<DetailRequest>();
            DoAnTotNghiepContext Context = this._context;

            byte[] salt = Crypto.Salt(this._configuration);
            string host = this.GetWebsitePath();

            foreach (var item in houses)
            {
                this._context.Entry(item).Collection(m => m.Requests).Query().Load();
                if (item.Requests != null)
                {
                    foreach (var itemRequest in item.Requests)
                    {
                        if (itemRequest != null)
                        {
                            DetailRequest? request = this.CreateDetailRequest(itemRequest);
                            if (request != null)
                            {
                                model.Add(request);
                            }
                        }
                    }
                }
            }
            return model;
        }


        public IActionResult RequestValidReceived()
        {
            ViewData["active"] = 3;
            List<DetailRequest> model = new List<DetailRequest>();
            int IdUser = this.GetIdUser();
            var listRequest = this._context .Requests
                                            .Include(m => m.Houses)
                                            .Where(m => (       m.Status == (int)StatusRequest.ACCEPT
                                                            ||  m.Status == (int)StatusRequest.CHECK_IN
                                                            ||  m.Status == (int)StatusRequest.CHECK_OUT)
                                                        && (m.IdUser == IdUser || m.Houses != null && m.Houses.IdUser == IdUser))
                                            .ToList();

            byte[] salt = Crypto.Salt(this._configuration);
            string host = this.GetWebsitePath();

            foreach (var itemRequest in listRequest)
            {
                if (itemRequest != null)
                {
                    DetailRequest? request = this.CreateDetailRequest(itemRequest);
                    if (request != null)
                    {
                        model.Add(request);
                    }
                }
            }
            return View(model);
        }

        private List<DetailRequest> GetAllRequestsSent()
        {
            int IdUser = this.GetIdUser();
            var requests = this._context.Requests
                                        .Where(m => m.IdUser == IdUser).ToList();
            List<DetailRequest> model = new List<DetailRequest>();
            DoAnTotNghiepContext Context = this._context;
            foreach (var item in requests)
            {
                if (item != null)
                {
                    DetailRequest? request = this.CreateDetailRequest(item);
                    if (request != null)
                    {
                        model.Add(request);
                    }
                }
            }

            return model;
        }
        private DetailRequest? CreateDetailRequest(Request item)
        {
            DoAnTotNghiepContext Context = this._context;
            byte[] salt = Crypto.Salt(this._configuration);
            string host = this.GetWebsitePath();
            if (item.Houses == null || !Context.Entry(item).Reference(m => m.Houses).IsLoaded)
            {
                Context.Entry(item).Reference(m => m.Houses).Query().Load();
            }
            if (item.Houses != null)
            {
                Context.Entry(item.Houses).Reference(m => m.Users).Query().Load();
                Context.Entry(item.Houses).Collection(m => m.FileOfHouses).Query().Load();
                Context.Entry(item).Collection(m => m.FeedBacks).Query().Where(m => m.IdUser == this.GetIdUser()).Load();

                this._context.Entry(item).Collection(m => m.CheckOuts).Query().Where(m => m.IdUser == item.IdUser).Load();
                this._context.Entry(item).Collection(m => m.CheckIns).Query().Where(m => m.IdUser == item.IdUser).Load();
                if (item.Houses.Users != null)
                {
                    DetailHouseViewModel house = this.CreateDetailsHouse(item.Houses);
                    DetailHouseViewModel? swapHouse = null;
                    DetailRatingViewModel? rating= null;
                    if(item.FeedBacks != null)
                    {
                        FeedBack? feedBack = item.FeedBacks.FirstOrDefault();
                        if(feedBack != null)
                        {
                            rating = new DetailRatingViewModel(feedBack);
                            item.Status = (int)StatusRequest.ENDED;
                        }
                    }
                    Context.Entry(item).Reference(m => m.Users).Query().Load();
                    if (item.IdSwapHouse != null)
                    {
                        Context.Entry(item).Reference(m => m.SwapHouses).Query().Load();
                        if (item.SwapHouses != null)
                        {
                            Context.Entry(item.SwapHouses).Reference(m => m.Users).Query().Load();
                            Context.Entry(item.SwapHouses).Collection(m => m.FileOfHouses).Query().Load();
                            swapHouse = this.CreateDetailsHouse(item.SwapHouses);
                        }
                    }

                    DetailRequestViewModel request = new DetailRequestViewModel(item, item.Users, salt, host);
                    return new DetailRequest()
                    {
                        Request = request,
                        SwapHouse = swapHouse,
                        House = house,
                        Rating = rating
                    };
                }
            }
            return null;
        }
        private DetailHouseViewModel CreateDetailsHouse(House house)
        {
            byte[] salt = Crypto.Salt(this._configuration);
            string host = this.GetWebsitePath();
            if (house.Users != null)
            {
                this._context.Entry(house.Users).Collection(m => m.Houses).Query().Where(m => m.Status == (int)StatusHouse.VALID).Load();
            }

            this._context.Entry(house).Reference(m => m.Citys).Query().Load();
            this._context.Entry(house).Reference(m => m.Districts).Query().Load();
            DetailHouseViewModel model = new DetailHouseViewModel(house, salt, house.Users, host);
            DoAnTotNghiepContext Context = this._context;
            if (house.FileOfHouses != null)
            {
                foreach (var f in house.FileOfHouses)
                {
                    Context.Entry(f).Reference(m => m.Files).Load();
                    if (f.Files != null)
                    {
                        model.Images.Add(new ImageBase(f.Files, host));
                        break;
                    }
                }
            }
            return model;
        }

        public IActionResult WaitingRequest()
        {
            ViewData["active"] = 4;
            return View();
        }
        public IActionResult History()
        {
            ViewData["active"] = 5;
            return View();
        }
        //adminReport
        [HttpGet("/api/User/Point")]
        public IActionResult ApiGetPoint()
        {
            return this.Point();
        }

        [HttpGet("/User/Point")]
        public IActionResult GetPoint()
        {
            return this.Point();
        }

        private IActionResult Point()
        {
            int IdUser = this.GetIdUser();
            var user = this._context.Users.Where(m => m.Id == IdUser).FirstOrDefault();
            int Point = 0;
            if (user != null) Point += (user.Point + user.BonusPoint);

            return Json(new
            {
                Status = 200,
                Message = "Điểm người dùng",
                Data = Point
            });
        }

        [HttpGet("/api/GetNumberOfHouse")]
        public IActionResult GetNumberOfHouse(Pagination pagination)
        {
            ListDetailHouses detailHouseViewModels = this.GetHouseInMemberPage(pagination);
            return Json(new
            {
                Status = 200,
                Message = "So nha",
                Data = detailHouseViewModels.Pagination.Total
            });
        }
        public async Task<IActionResult> Messages(string? connection)
        {
            int IdUser = this.GetIdUser();
            Dictionary<int, RoomChatViewModel> model = new Dictionary<int, RoomChatViewModel>();
            byte[] salt = Crypto.Salt(this._configuration);

            //seftaccess
            using (var Context = this._context)
            {
                if (!string.IsNullOrEmpty(connection))
                {
                    string IdStr = Crypto.DecodeKey(connection, salt);
                    int Id = 0;
                    if(!int.TryParse(IdStr, out Id))
                    {
                            return this.NotFound();
                    }
                    if(Id != IdUser)
                    {
                        var user = this._context.Users.FirstOrDefault(m => m.Id == Id);
                        if (user == null) return this.NotFound();
                        var rooms = from r in this._context.ChatRooms
                                    join ur in this._context.UsersInChatRooms on r.Id equals ur.IdChatRoom
                                    join ur2 in this._context.UsersInChatRooms on r.Id equals ur2.IdChatRoom
                                    where ur.IdUser == Id && ur2.IdUser == IdUser
                                    select r;

                        if (rooms.Any())
                        {
                            List<ChatRoom?> chats = rooms.ToList();
                            foreach (var room in chats)
                            {
                                if (room != null)
                                {
                                    KeyValuePair<int, RoomChatViewModel> keyValuePair = this.CreateDictionary(room, Context, 20, salt, IdUser);
                                    if (model.ContainsKey(keyValuePair.Key))
                                    {
                                        model.Remove(keyValuePair.Key);
                                    }
                                    model.Add(keyValuePair.Key, keyValuePair.Value);
                                }
                            }
                        }
                        else
                        {
                            using (var transaction = await Context.Database.BeginTransactionAsync())
                            {
                                try
                                {
                                    ChatRoom chatRoom = new ChatRoom() { UpdatedDate = DateTime.Now };
                                    Context.Add(chatRoom);
                                    Context.SaveChanges();

                                    List<UsersInChatRoom> newList = new List<UsersInChatRoom>()
                                {
                                    new UsersInChatRoom()
                                    {
                                        IdChatRoom = chatRoom.Id,
                                        IdUser = IdUser
                                    },
                                    new UsersInChatRoom()
                                    {
                                        IdChatRoom = chatRoom.Id,
                                        IdUser = Id
                                    }
                                };

                                    Context.AddRange(newList);
                                    Context.SaveChanges();

                                    transaction.Commit();
                                    if (user.IdFile != null && user.IdFile.Value != 0)
                                    {
                                        Context.Entry(user).Reference(m => m.Files).Load();
                                    }

                                    model.Add(chatRoom.Id, new RoomChatViewModel()
                                    {
                                        IdRoom = chatRoom.Id,
                                        UserMessages = new List<UserMessageViewModel>() {
                                        new UserMessageViewModel(user, salt, this.GetWebsitePath())
                                    },
                                        Messages = new List<MessageViewModel>()
                                    });

                                    ChatHub chatHub = new ChatHub(this._signalContext);

                                    string userAccess = Crypto.EncodeKey(user.Id.ToString(), salt);
                                    await chatHub.ConnectToGroup((TargetSignalR.Connect() + "-" + userAccess), chatRoom.Id.ToString(), userAccess);

                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex);
                                    transaction.Rollback();
                                }
                            }
                        }
                    }
                }
                if(model.Keys.Count() < 20)
                {
                    var chatRooms = Context.UsersInChatRooms
                                             .Include(m => m.ChatRooms).OrderByDescending(m => m.ChatRooms.UpdatedDate)
                                             .Where(m => m.IdUser == IdUser)
                                             .Take(20)
                                             .Select(m => m.ChatRooms).ToList();
                    int number = 20;
                    if (model.Count() > 0) number = 0;
                    if (chatRooms != null && chatRooms.Count() > 0)
                    {
                        foreach (var room in chatRooms)
                        {
                            if (room != null && !model.ContainsKey(room.Id))
                            {
                                KeyValuePair<int, RoomChatViewModel> keyValuePair = this.CreateDictionary(room, Context, number, salt, IdUser);
                                model.Add(keyValuePair.Key, keyValuePair.Value);
                                if (number == 10) number = 0;
                            }
                        }
                    }
                }
                var mainUser = Context.Users.Where(m => m.Id == IdUser).FirstOrDefault();
                if (mainUser == null)
                {
                    return NotFound();
                }
                else
                {
                    ViewData["userAccess"] = new UserMessageViewModel(mainUser, salt, this.GetWebsitePath());
                }
            }
            ViewData["active"] = 6;
            return View(model);
        }
        private KeyValuePair<int, RoomChatViewModel> CreateDictionary(ChatRoom room, DoAnTotNghiepContext Context, int number, byte[] salt, int IdUser)
        {
            Context.Entry(room).Collection(m => m.Messages).Query()
                .OrderByDescending(m => m.CreatedDate).Take(number).Load();
            var userInChatRoom = Context.UsersInChatRooms
                        .Include(m => m.Users)
                        .Where(m => m.IdChatRoom == room.Id && m.IdUser != IdUser)
                        .ToList();
            List<UserMessageViewModel> users = new List<UserMessageViewModel>();
            string host = this.GetWebsitePath();
            foreach (UsersInChatRoom item in userInChatRoom)
            {
                if (item != null)
                {
                    users.Add(new UserMessageViewModel(item, salt, host));
                }
            }
            RoomChatViewModel roomChat = new RoomChatViewModel()
            {
                IdRoom = room.Id,
                Messages = (room.Messages == null || number == 0) ? new List<MessageViewModel>() : 
                                        room.Messages.OrderByDescending(m => m.CreatedDate).Take(number).Select(m => new MessageViewModel(
                                                m.IdReply == null ? 0 : m.IdReply.Value,
                                                ((m.Status == (int)StatusMessage.SEEN) || m.IdUser == IdUser),
                                                Crypto.EncodeKey(m.IdUser.ToString(), salt),
                                                m.Content, m.Id, m.CreatedDate)).ToList(),
                UserMessages = users
            };
            return new KeyValuePair<int, RoomChatViewModel>(roomChat.IdRoom, roomChat);
        }
        public IActionResult ReportMail()
        {
            ViewData["active"] = 0;
            return View();
        }

        //view GET ListHouse => userAccess + my house
        //sửa form createRequest

        //Update Info
        [HttpPost("/User/UpdateInfo")]
        public IActionResult UpdateInfo([FromBody] UpdateUserInfo model)
        {
            return this.UpdateInfoMember(model);
        }
        [HttpPost("/api/User/UpdateInfo")]
        public IActionResult ApiUpdateInfo(UpdateUserInfo model)
        {
            return this.UpdateInfoMember(model);
        }
        private IActionResult UpdateInfoMember(UpdateUserInfo model)
        {
            using (var Context = this._context)
            {
                using (var Transaction = Context.Database.BeginTransaction())
                {
                    if (ModelState.IsValid)
                    {
                        try
                        {
                            int IdUser = this.GetIdUser();
                            var user = Context.Users.Where(m => m.Id == IdUser).FirstOrDefault();
                            if (user == null)
                            {
                                return BadRequest(new
                                {
                                    Status = 400,
                                    Message = "Không tìm thấy user"
                                });
                            }
                            if (user.Email != model.Email)
                            {
                                var checkEmail = Context.Users.Where(m => m.Id != IdUser && m.Email.Contains(model.Email)).ToList();
                                if (checkEmail.Any())
                                {
                                    return BadRequest(new
                                    {
                                        Status = 400,
                                        Message = "Email đã tồn tại"
                                    });
                                }
                            }

                            if (model.Image != null)
                            {
                                if (user.IdFile == null)
                                {
                                    Entity.File? file = this.SaveFile(model.Image);
                                    if (file != null)
                                    {
                                        Context.Files.Add(file);
                                        Context.SaveChanges();
                                        user.IdFile = file.Id;
                                    }
                                }
                                else
                                {
                                    var file = Context.Files.Where(m => m.Id == user.IdFile).FirstOrDefault();
                                    if (file != null)
                                    {
                                        Entity.File? saveFile = this.SaveFile(model.Image);
                                        if (saveFile != null)
                                        {
                                            this.DeleteFile(file);
                                            file.PathFolder = saveFile.PathFolder;
                                            file.FileName = saveFile.FileName;
                                            Context.Files.Update(file);
                                            Context.SaveChanges();
                                        }
                                    }
                                }
                            }
                            else if (model.File != null)
                            {
                                if (user.IdFile == null)
                                {
                                    Entity.File? file = this.SaveFile(model.File);
                                    if (file != null)
                                    {
                                        Context.Files.Add(file);
                                        Context.SaveChanges();
                                        user.IdFile = file.Id;
                                    }
                                }
                                else
                                {
                                    var file = Context.Files.Where(m => m.Id == user.IdFile).FirstOrDefault();
                                    if (file != null)
                                    {
                                        Entity.File? saveFile = this.SaveFile(model.File);
                                        if (saveFile != null)
                                        {
                                            this.DeleteFile(file);
                                            file.PathFolder = saveFile.PathFolder;
                                            file.FileName = saveFile.FileName;
                                            Context.Files.Update(file);
                                            Context.SaveChanges();
                                        }
                                    }
                                }
                            }

                            user.UpdateInfoUser(model);
                            Context.Users.Update(user);
                            Context.SaveChanges();

                            Transaction.Commit();

                            return Json(new
                            {
                                Status = 200,
                                Message = "ok"
                            });
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                            Transaction.Rollback();
                        }
                    }
                    else
                    {
                        return BadRequest(new
                        {
                            Status = 400,
                            Message = this.ModelErrors()
                        });
                    }
                }
            }

            return BadRequest(new
            {
                Status = 500,
                Message = "Hệ thống đang bảo trì"
            });
        }

        //Update Password
        [HttpGet("/User/UpdatePassword")]
        public IActionResult UpdatePassword()
        {
            return PartialView("./Views/Member/_UpdatePassword.cshtml", new UpdatePasswordViewModel());
        }

        //Update Password
        [HttpPost("/User/UpdatePassword")]
        public IActionResult UpdatePassword([FromBody] UpdatePasswordViewModel model)
        {
            return this.UpdatePasswordMember(model);
        }
        [HttpPost("/api/User/UpdatePassword")]
        public IActionResult ApiUpdatePassword([FromBody] UpdatePasswordViewModel model)
        {
            return this.UpdatePasswordMember(model);  
        }
        private IActionResult UpdatePasswordMember(UpdatePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                int IdUser = this.GetIdUser();
                var user = this._context.Users.Where(m => m.Id == IdUser).FirstOrDefault();
                if (user == null)
                {
                    return BadRequest(new
                    {
                        Status = 400,
                        Message = "Không tìm thấy người dùng"
                    });
                }
                byte[] salt = Crypto.Salt();
                string password = Crypto.HashPass(model.Password, salt);
                user.Password = password;
                user.Salt = Crypto.SaltStr(salt);
                this._context.Users.Update(user);
                this._context.SaveChanges();
                return Json(new
                {
                    Status = 200,
                    Message = "ok"
                });
            }
            return BadRequest(new
            {
                Status = 400,
                Message = this.ModelErrors()
            });
        }
    }
}
