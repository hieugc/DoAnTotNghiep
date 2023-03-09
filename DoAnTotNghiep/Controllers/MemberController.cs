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

namespace DoAnTotNghiep.Controllers
{
    [Authorize(Roles = "MEMBER")]
    public class MemberController : BaseController
    {
        private readonly DoAnTotNghiepContext _context;
        private readonly IConfiguration _configuration;
        private IHubContext<ChatHub> _signalContext;

        public MemberController(DoAnTotNghiepContext context, 
            IConfiguration configuration, 
            IHubContext<ChatHub> signalContext)
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
            return View();
        }
        private List<DetailHouseViewModel> GetHouseInMemberPage()
        {
            int IdUser = this.GetIdUser();
            var listHouse = this._context.Houses.Include(m => m.RulesInHouses)
                                                .Include(m => m.UtilitiesInHouses)
                                                .Include(m => m.Citys)
                                                .Include(m => m.Districts)
                                                .Include(m => m.Wards)
                                                .Include(m => m.Requests)
                                                .Include(m => m.FileOfHouses)
                                                .Where(m => m.IdUser == IdUser)
                                                .Take<House>(6)
                                                .ToList();

            byte[] salt = Crypto.Salt(this._configuration);
            string host = this.GetWebsitePath();

            List<DetailHouseViewModel> detailHouseViewModels = new List<DetailHouseViewModel>();
            DoAnTotNghiepContext Context = this._context;
            foreach (var item in listHouse)
            {
                DetailHouseViewModel model = DetailHouseViewModel.GetByHouse(item, salt);
                if(item.FileOfHouses != null)
                {
                    var images = item.Files
                            .Select(m => new ImageBase() { 
                                            Data = string.Empty,
                                            Folder = host + "/" + m.PathFolder + "/" + m.FileName, 
                                            Name = m.FileName, 
                                            Id = m.Id }).ToList();
                    model.Images.AddRange(images);
                }
                detailHouseViewModels.Add(model);
            }

            
            return detailHouseViewModels;
        }

        public IActionResult House()
        {
            ViewData["active"] = 1;//xác định tab active

            var listUtilities = this._context.Utilities.ToList();
            var listRules = this._context.Rules.ToList();
            List<DetailHouseViewModel> detailHouseViewModels = this.GetHouseInMemberPage();
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

        [HttpGet("/api/House/OwnerShip")]
        public JsonResult ApiHouse()
        {
            List<DetailHouseViewModel> model = this.GetHouseInMemberPage();
            return Json(new
                {
                    Status = 200,
                    Data = model
                });
        }
        public IActionResult Requested()
        {
            ViewData["active"] = 2;
            return View();
        }
        public IActionResult WaitingRequest()
        {
            ViewData["active"] = 3;
            return View();
        }
        public IActionResult History()
        {
            ViewData["active"] = 4;
            return View();
        }
        public async Task<IActionResult> Messages(string? connection)
        {
            //chưa sort last send
            int IdUser = this.GetIdUser();
            Dictionary<int, RoomChatViewModel> model = new Dictionary<int, RoomChatViewModel>();
            byte[] salt = Crypto.Salt(this._configuration);

            string? storaged = this.GetCookie(Enum.Cookie.DataChat());
            if (!string.IsNullOrEmpty(storaged))
            {
                Dictionary<int, RoomChatViewModel>? getDataStored = JsonConvert.DeserializeObject<Dictionary<int, RoomChatViewModel>>(storaged);
                if(getDataStored != null)
                {
                    model.AddRange(getDataStored);
                }
            }
            //seftaccess
            using (var Context = this._context)
            {
                if (!string.IsNullOrEmpty(connection))
                {
                    string IdStr = Crypto.DecodeKey(connection, salt);
                    int Id = 0;
                    try
                    {
                        Id = int.Parse(IdStr);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        return this.NotFound();
                    }

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
                            if(room != null)
                            {
                                KeyValuePair<int, RoomChatViewModel> keyValuePair = this.CreateDictionary(room, Context, 10, salt, IdUser);
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
                                if(user.IdFile != null && user.IdFile.Value != 0)
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
                                this.SetCookie(Enum.Cookie.DataChat(), JsonConvert.SerializeObject(model), 24);
                                //connect to users
                                ChatHub chatHub = new ChatHub(this._signalContext);

                                await chatHub.ConnectToGroup((TargetSignalR.Connect() + "-" + Crypto.EncodeKey(user.Id.ToString(), salt)), chatRoom.Id.ToString());

                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                                transaction.Rollback();
                            }
                        }  
                    }
                }
                if(model.Keys.Count() < 10)
                {
                    var chatRooms = Context.UsersInChatRooms
                                             .Include(m => m.ChatRooms).OrderByDescending(m => m.ChatRooms.UpdatedDate)
                                             .Where(m => m.IdUser == IdUser)
                                             .Take(10)
                                             .Select(m => m.ChatRooms).ToList();

                    if (chatRooms != null && chatRooms.Count() > 0)
                    {
                        foreach (var room in chatRooms)
                        {
                            if (room != null)
                            {
                                KeyValuePair<int, RoomChatViewModel> keyValuePair = this.CreateDictionary(room, Context, 10, salt, IdUser);
                                if (model.ContainsKey(room.Id))
                                {
                                    model.Remove(keyValuePair.Key);
                                }
                                model.Add(keyValuePair.Key, keyValuePair.Value);
                            }
                        }
                    }
                }
            }

            var mainUser = this._context.Users.Where(m => m.Id == IdUser).FirstOrDefault();
            if(mainUser == null)
            {
                return NotFound();
            }
            else
            {
                ViewData["userAccess"] = new UserMessageViewModel(mainUser, salt, this.GetWebsitePath());
            }
            ViewData["active"] = 5;
            return View(model);
        }

        private KeyValuePair<int, RoomChatViewModel> CreateDictionary(ChatRoom room, DoAnTotNghiepContext Context, int number, byte[] salt, int IdUser)
        {
            Context.Entry(room).Collection(m => m.Messages).Query().OrderByDescending(m => m.CreatedDate).Take(number).Load();
            var userInChatRoom = Context.UsersInChatRooms
                        .Include(m => m.Users)
                        .Where(m => m.IdChatRoom == room.Id && m.IdUser != IdUser)
                        .ToList();
            List<UserMessageViewModel> users = new List<UserMessageViewModel>();
            foreach (UsersInChatRoom item in userInChatRoom)
            {
                if (item != null)
                {
                    users.Add(new UserMessageViewModel(item, salt));
                }
            }
            RoomChatViewModel roomChat = new RoomChatViewModel()
            {
                IdRoom = room.Id,
                Messages = room.Messages == null ? new List<MessageViewModel>() : room.Messages.Select(m => new MessageViewModel(
                                            m.IdReply == null ? 0 : m.IdReply.Value,
                                            ((m.Status == (int)Status.SEEN) || m.IdUser == IdUser),
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
    }
}
