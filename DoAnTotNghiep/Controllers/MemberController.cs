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
using NuGet.Protocol;
using System.Xml.Linq;
using DoAnTotNghiep.Service;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DoAnTotNghiep.Controllers
{
    [Authorize(Roles = Enum.Role.Member)]
    public class MemberController : BaseController
    {
        private readonly DoAnTotNghiepContext _context;
        private readonly IConfiguration _configuration;
        private IHubContext<ChatHub> _signalContext;

        private readonly IFeedBackService _feedBackService;
        private readonly IRequestService _requestService;
        private readonly IFileService _fileService;
        private readonly IHouseService _houseService;
        private readonly IUserService _userService;
        private readonly IRuleService _ruleService;
        private readonly IUtilitiesService _utilitiesService;
        private readonly ICircleRequestService _circleRequestService;
        private readonly IMessageService _messageService;

        public MemberController(DoAnTotNghiepContext context, 
                                IConfiguration configuration, 
                                IHubContext<ChatHub> signalContext, 
                                IHostEnvironment environment,
                                IFeedBackService feedBackService,
                                IRequestService requestService,
                                IFileService fileService,
                                IHouseService houseService,
                                IUserService userService,
                                IMessageService messageService,
                                IRuleService ruleService,
                                IUtilitiesService utilitiesService,
                                ICircleRequestService circleRequestService) : base(environment)
        {
            _context = context;
            _configuration = configuration;
            _signalContext = signalContext;
            _feedBackService = feedBackService;
            _requestService = requestService;
            _fileService = fileService;
            _houseService = houseService;
            _userService = userService;
            _ruleService = ruleService;
            _utilitiesService = utilitiesService;
            _circleRequestService = circleRequestService;
            _messageService = messageService;
        }

        public IActionResult Infomation()
        {
            ViewData["active"] = 0;
            int IdUser = this.GetIdUser();
            var user = this._userService.GetById(IdUser);
            if (user == null)
            {
                return View(new UpdateUserInfo());
            }
            user.InCludeAll(this._context);
            this.SetViewData(new DoAnTotNghiepContext(this._context.GetConfig()), IdUser, Crypto.Salt(this._configuration));
            return View(new UpdateUserInfo(user, this.GetWebsitePath()));
        }

        private ListDetailHouses GetHouseInMemberPage(Pagination pagination, int? IdHouse, byte[] salt, string host, int IdUser)
        {
            List<House> listHouse = new List<House>();
            if(IdHouse != null)
            {
                var all = this._houseService.GetListHouseByUser(IdUser);
                listHouse.AddRange(all.Where(m=>m.Id == IdHouse.Value).ToList());
                listHouse.AddRange(all.Where(m => m.Id != IdHouse.Value).ToList());
            }
            else
            {
                listHouse.AddRange(this._houseService.GetListHouseByUser(IdUser));
            }
            pagination.Total = (int)Math.Ceiling((double)(listHouse.Count() / pagination.Limit));
            int skip = pagination.Page - 1 < 0 ? 0: pagination.Page - 1;
            return new ListDetailHouses()
            {
                Houses  = this._houseService.GetListDetailHouseWithManyImages(listHouse.Skip(skip).Take(pagination.Limit).ToList(), this._fileService, host, salt),
                Pagination = pagination
            };
        }
        public IActionResult House(Pagination pagignation, int? IdHouse, int? IdRequest)
        {
            ViewData["active"] = 1;//xác định tab active

            var listUtilities = this._utilitiesService.All();
            var listRules = this._ruleService.All();
            int IdUser = this.GetIdUser();
            byte[] salt = Crypto.Salt(this._configuration);
            ListDetailHouses detailHouseViewModels = this.GetHouseInMemberPage(pagignation, IdHouse, salt, this.GetWebsitePath(), IdUser);

            ViewData["IdRequest"] = IdRequest;
            ViewData["IdHouse"] = IdHouse;

            AuthHouseViewModel model = new AuthHouseViewModel()
            {
                Houses = detailHouseViewModels,
                OptionHouses = new OptionHouseViewModel()
                {
                    Utilities = listUtilities,
                    Rules = listRules
                }
            };
            if (IdUser != 0)
            {
                this.SetViewData(new DoAnTotNghiepContext(this._context.GetConfig()), IdUser, salt);
            }
            return View(model);
        }
        [HttpGet("/api/House/GetMyHome")]
        public IActionResult ApiHouse(Pagination pagination)
        {
            ListDetailHouses model = this.GetHouseInMemberPage(pagination, null, Crypto.Salt(this._configuration), this.GetWebsitePath(), this.GetIdUser());
            return Json(new
                {
                    Status = 200,
                    Data = model
            });
        }
        [HttpGet("/House/GetMyHome")]
        public IActionResult GetMyHome(Pagination pagination)
        {
            ListDetailHouses model = this.GetHouseInMemberPage(pagination, null, Crypto.Salt(this._configuration), this.GetWebsitePath(), this.GetIdUser());
            return Json(new
            {
                Status = 200,
                Data = model
            });
        }
        [HttpGet("/api/Request/GetRequestReceived")]
        public IActionResult ApiGetListRequestReceived(int? Status)
        {
            int IdUser = this.GetIdUser();
            byte[] salt = Crypto.Salt(this._configuration);
            string host = this.GetWebsitePath();
            var houses = this._houseService.GetListHouseByUser(IdUser);
            return Json(new
            {
                Status = 200,
                Data = this._requestService.GetValidRequestByUser(houses, this._fileService, this._houseService, this._feedBackService, Status, salt, host, IdUser)
            });
        }
        public IActionResult Requested()
        {
            ViewData["active"] = 2;
            int IdUser = this.GetIdUser();
            byte[] salt = Crypto.Salt(this._configuration);
            string host = this.GetWebsitePath();
            List<DetailRequest> model = this._requestService.GetValidRequestByUser(
                this._requestService.GetAllSent(IdUser),
                this._fileService, this._houseService, this._feedBackService, null, salt, host, IdUser
                );
            
            if (IdUser != 0)
            {
                this.SetViewData(new DoAnTotNghiepContext(this._context.GetConfig()), IdUser, salt);
            }
            return View(model);
        }
        public IActionResult RequestValidReceived(int? IdRequest)
        {
            ViewData["active"] = 3;
            List<DetailRequest> model = new List<DetailRequest>();
            int IdUser = this.GetIdUser();
            byte[] salt = Crypto.Salt(this._configuration);
            string host = this.GetWebsitePath();
            List<Request> listRequest = new List<Request>();
            if(IdRequest != null)
            {
                var all = this._requestService.GetValidReceivedRequestByUser(IdUser);
                listRequest.AddRange(all.Where(m => m.Id == IdRequest.Value).ToList());
                listRequest.AddRange(all.Where(m => m.Id != IdRequest.Value).ToList());
            }
            else
            {
                listRequest.AddRange(this._requestService.GetValidReceivedRequestByUser(IdUser));
            }
            ViewData["IdRequest"] = IdRequest;
            model.AddRange(this._requestService.GetValidRequestByUser(listRequest, this._fileService, this._houseService, this._feedBackService, null, salt, host, IdUser));
            if (IdUser != 0)
            {
                this.SetViewData(new DoAnTotNghiepContext(this._context.GetConfig()), IdUser, salt);
            }
            return View(model);
        }
        public IActionResult WaitingRequest()
        {
            ViewData["active"] = 4;

            int IdUser = this.GetIdUser();
            List<CircleRequestViewModel> model = this._circleRequestService.GetSuggest(IdUser, this._userService, Crypto.Salt(this._configuration), GetWebsitePath());
            if (IdUser != 0)
            {
                this.SetViewData(new DoAnTotNghiepContext(this._context.GetConfig()), IdUser, Crypto.Salt(this._configuration));
            }
            return View(model);
        }
        public IActionResult History(string? popup = null)
        {
            ViewData["active"] = 5;
            int IdUser = this.GetIdUser();
            List<HouseSelector> house = this._houseService.GetListHouseByUser(IdUser)
                                            .Select(m => new HouseSelector(m))
                                            .ToList();
            HistoryViewModel model = new HistoryViewModel(house, 0, 0);
            if (!string.IsNullOrEmpty(popup))
            {
                ViewData["action"] = popup;
            }
            if (IdUser != 0)
            {
                this.SetViewData(new DoAnTotNghiepContext(this._context.GetConfig()), IdUser, Crypto.Salt(this._configuration));
            }
            return View(model);
        }
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
            var user = this._userService.GetById(IdUser);
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
            ListDetailHouses model = this.GetHouseInMemberPage(pagination, null, Crypto.Salt(this._configuration), this.GetWebsitePath(), this.GetIdUser());
            return Json(new
            {
                Status = 200,
                Message = "So nha",
                Data = model.Pagination.Total
            });
        }

        public async Task<IActionResult> Messages(string? connection)
        {
            int IdUser = this.GetIdUser();
            Dictionary<int, RoomChatViewModel> model = new Dictionary<int, RoomChatViewModel>();
            byte[] salt = Crypto.Salt(this._configuration);
            string host = this.GetWebsitePath();

            var mainUser = this._userService.GetById(IdUser);
            if (mainUser == null)
            {
                return NotFound();
            }
            else
            {
                mainUser.InCludeAll(this._context);
                ViewData["userAccess"] = new UserMessageViewModel(mainUser, salt, host);
            }

            if (!string.IsNullOrEmpty(connection))
            {
                string IdStr = Crypto.DecodeKey(connection, salt);
                int Id = 0;
                if (!int.TryParse(IdStr, out Id))
                {
                    return this.NotFound();
                }
                if (Id != IdUser)
                {
                    var user = this._userService.GetById(Id);
                    if (user == null) return this.NotFound();
                    var rooms = this._messageService.GetRoomWithTwoMembers(Id, IdUser);

                    if (rooms.Count() > 0)
                    {
                        foreach (var room in rooms)
                        {
                            RoomChatViewModel keyValuePair = this._messageService.CreateDictionary(room, 20, 0, salt, host, IdUser);
                            if (model.ContainsKey(keyValuePair.IdRoom))
                            {
                                model.Remove(keyValuePair.IdRoom);
                            }
                            model.Add(keyValuePair.IdRoom, keyValuePair);
                        }
                    }
                    else
                    {
                        using (var transaction = await this._context.Database.BeginTransactionAsync())
                        {
                            try
                            {
                                ChatRoom chatRoom = new ChatRoom() { UpdatedDate = DateTime.Now };
                                this._messageService.Save(
                                    chatRoom,
                                    new List<int>() { Id, IdUser}
                                );

                                transaction.Commit();
                                if (user.IdFile != null && user.IdFile.Value != 0)
                                {
                                    user.InCludeAll((this._context));
                                }

                                model.Add(chatRoom.Id, new RoomChatViewModel()
                                {
                                    IdRoom = chatRoom.Id,
                                    UserMessages = new List<UserMessageViewModel>() {
                                        new UserMessageViewModel(user, salt, host)
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
            if (model.Keys.Count() < 20)
            {
                var chatRooms = this._messageService.GetByUser(IdUser, 20, 0);
                int number = 20;
                if (model.Count() > 0) number = 0;
                foreach (var room in chatRooms)
                {
                    if (room != null && !model.ContainsKey(room.Id))
                    {
                        RoomChatViewModel keyValuePair = this._messageService.CreateDictionary(room, number, 0, salt, host, IdUser);
                        model.Add(keyValuePair.IdRoom, keyValuePair);
                        if (number == 10) number = 0;
                    }
                }
            }


            ViewData["active"] = 6;
            if (IdUser != 0)
            {
                this.SetViewData(new DoAnTotNghiepContext(this._context.GetConfig()), IdUser, salt);
            }
            return View(model);
        }
        public IActionResult ReportMail()
        {
            ViewData["active"] = 0;
            return View();
        }

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
            if (ModelState.IsValid)
            {
                using (var Transaction = this._context.Database.BeginTransaction())
                {
                    if (ModelState.IsValid)
                    {
                        try
                        {
                            int IdUser = this.GetIdUser();
                            var user = this._userService.GetById(IdUser);
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
                                if (this._userService.IsExistEmail(model.Email, IdUser))
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
                                user = this._fileService.UpdateFileUser(user, model.Image, this.environment.ContentRootPath);
                            }

                            if (model.File != null)
                            {
                                user = this._fileService.UpdateFileUser(user, model.File, this.environment.ContentRootPath);
                            }

                            user.UpdateInfoUser(model);
                            this._userService.UpdateUser(user);

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

        [HttpGet("/User/UpdatePassword")]
        public IActionResult UpdatePassword()
        {
            return PartialView("./Views/Member/_UpdatePassword.cshtml", new UpdatePasswordViewModel());
        }
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
                var user = this._userService.GetById(IdUser);
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
                this._userService.UpdateUser(user);
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
        [HttpGet("/api/User/Info")]
        public IActionResult MyProfile()
        {
            return this.ProfileUser(this.GetIdUser());
        }

        [HttpGet("/api/User/InfoByUserAccess")]
        public IActionResult ProfileUserByAccess(string UserAccess)
        {
            int IdUser = 0;
            if (!int.TryParse(Crypto.DecodeKey(UserAccess, Crypto.Salt(this._configuration)), out IdUser))
            {
                return this.ProfileUser(0);
            }
            return this.ProfileUser(IdUser);
        }
        private IActionResult ProfileUser(int IdUser)
        {
            var user = this._userService.GetById(IdUser);
            if (user == null) return BadRequest(new { Status = 400, Message = "Không tìm thấy người dùng" });
            UserInfo.GetEntityRelated(user, this._context);
            UserInfo userInfo = new UserInfo(user, Crypto.Salt(this._configuration), this.GetWebsitePath());
            return Json(new {Status = 200, Data = userInfo });
        }
        public IActionResult OverViewProfile(string? UserAccess)
        {
            int IdAnotherUser = 0;
            if(!int.TryParse(Crypto.DecodeKey(UserAccess, Crypto.Salt(this._configuration)), out IdAnotherUser))
            {
                return NotFound();
            }
            var user = this._userService.GetById(IdAnotherUser);
            if(user == null) return NotFound();
            byte[] salt = Crypto.Salt(this._configuration);
            string host = this.GetWebsitePath();
            UserInfo.GetEntityRelated(user, this._context);
            int IdUser = this.GetIdUser();
            ViewData["isSelf"] = IdAnotherUser == IdUser ? "true": "false";
            
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
