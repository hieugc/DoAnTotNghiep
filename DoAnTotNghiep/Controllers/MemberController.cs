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

        public IActionResult Infomation()
        {
            ViewData["active"] = 0;
            int IdUser = this.GetIdUser();
            var user = this._context.Users.Where(m => m.Id == IdUser).FirstOrDefault();
            if (user == null)
            {
                return View(new UpdateUserInfo());
            }
            this.SetViewData(new DoAnTotNghiepContext(this._context.GetConfig()), IdUser, Crypto.Salt(this._configuration));
            return View(new UpdateUserInfo(user, this.GetWebsitePath()));
        }
        private ListDetailHouses GetHouseInMemberPage(Pagination pagination, int? IdHouse)
        {
            int IdUser = this.GetIdUser();
            List<House> listHouse = new List<House>();
            if(IdHouse != null)
            {
                listHouse.AddRange(this._context.Houses.Where(m => m.IdUser == IdUser && m.Id == IdHouse.Value).ToList());
                listHouse.AddRange(this._context.Houses.Where(m => m.IdUser == IdUser && m.Id != IdHouse.Value).ToList());
            }
            else
            {
                listHouse.AddRange(this._context.Houses.Where(m => m.IdUser == IdUser).ToList());
            }
            pagination.Total = (int)Math.Ceiling((double)(listHouse.Count() / pagination.Limit));
            int skip = pagination.Page - 1 < 0 ? 0: pagination.Page - 1;
            byte[] salt = Crypto.Salt(this._configuration);
            string host = this.GetWebsitePath();

            List<DetailHouseViewModel> detailHouseViewModels = new List<DetailHouseViewModel>();
            foreach (var item in listHouse.Skip(skip).Take(pagination.Limit))
            {
                this._context.Entry(item).Reference(m => m.Users).Load();
                this._context.Entry(item).Collection(m => m.RulesInHouses).Query().Load();
                this._context.Entry(item).Collection(m => m.UtilitiesInHouses).Query().Load();
                //this._context.Entry(item).Collection(m => m.Requests).Query().Where(m => m.Status == (int)StatusRequest.WAIT_FOR_SWAP).Load();
                //this._context.Entry(item).Collection(m => m.FileOfHouses).Query().Load();
                //this._context.Entry(item).Collection(m => m.FeedBacks).Query().Load();

                item.IncludeLocation(this._context);
                DetailHouseViewModel model = new DetailHouseViewModel(item, salt, item.Users, host);
                if (item.FileOfHouses != null)
                {
                    foreach (var f in item.FileOfHouses)
                    {
                        this._context.Entry(f).Reference(m => m.Files).Load();
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
        public IActionResult House(Pagination pagignation, int? IdHouse, int? IdRequest)
        {
            ViewData["active"] = 1;//xác định tab active

            var listUtilities = this._context.Utilities.ToList();
            var listRules = this._context.Rules.ToList();
            ListDetailHouses detailHouseViewModels = this.GetHouseInMemberPage(pagignation, IdHouse);
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
            int IdUser = this.GetIdUser();
            if (IdUser != 0)
            {
                this.SetViewData(new DoAnTotNghiepContext(this._context.GetConfig()), IdUser, Crypto.Salt(this._configuration));
            }
            return View(model);
        }
        [HttpGet("/api/House/GetMyHome")]
        public IActionResult ApiHouse(Pagination pagination)
        {
            ListDetailHouses model = this.GetHouseInMemberPage(pagination, null);
            return Json(new
                {
                    Status = 200,
                    Data = model
            });
        }
        [HttpGet("/House/GetMyHome")]
        public IActionResult GetMyHome(Pagination pagination)
        {
            ListDetailHouses model = this.GetHouseInMemberPage(pagination, null);
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
            int IdUser = this.GetIdUser();
            if (IdUser != 0)
            {
                this.SetViewData(new DoAnTotNghiepContext(this._context.GetConfig()), IdUser, Crypto.Salt(this._configuration));
            }
            return View(model);
        }
        [HttpGet("/api/Request/GetRequestReceived")]
        public IActionResult ApiGetListRequestReceived(int? Status)
        {
            return Json(new
            {
                Status = 200,
                Data = this.GetAllRequestsReceived(Status)
            });
        }
        private List<DetailRequest> GetAllRequestsReceived(int? Status)
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
                this._context.Entry(item).Collection(m => m.Requests).Query().Where(m => m.Status != (int)StatusRequest.DISABLE).Load();
                if (item.Requests != null)
                {
                    foreach (var itemRequest in item.Requests)
                    {
                        if (itemRequest != null)
                        {
                            DetailRequest? request = this.CreateDetailRequest(itemRequest, Status);
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
        public IActionResult RequestValidReceived(int? IdRequest)
        {
            ViewData["active"] = 3;
            List<DetailRequest> model = new List<DetailRequest>();
            int IdUser = this.GetIdUser();
            List<Request> listRequest = new List<Request>();
            if(IdRequest != null)
            {
                listRequest.AddRange(this._context.Requests
                                                .Include(m => m.Houses)
                                                .Where(m => m.Id == IdRequest.Value && 
                                                            (m.Status == (int)StatusRequest.ACCEPT
                                                                || m.Status == (int)StatusRequest.CHECK_IN
                                                                || m.Status == (int)StatusRequest.CHECK_OUT
                                                                || m.Status == (int)StatusRequest.ENDED)
                                                            && (m.IdUser == IdUser || m.Houses != null && m.Houses.IdUser == IdUser))
                                                .ToList());

                listRequest.AddRange(this._context.Requests
                                                .Include(m => m.Houses)
                                                .Where(m => m.Id != IdRequest.Value &&
                                                            (m.Status == (int)StatusRequest.ACCEPT
                                                                || m.Status == (int)StatusRequest.CHECK_IN
                                                                || m.Status == (int)StatusRequest.CHECK_OUT
                                                                || m.Status == (int)StatusRequest.ENDED)
                                                            && (m.IdUser == IdUser || m.Houses != null && m.Houses.IdUser == IdUser))
                                                .ToList());
            }
            else
            {
                listRequest.AddRange(this._context.Requests
                                                .Include(m => m.Houses)
                                                .Where(m => (m.Status == (int)StatusRequest.ACCEPT
                                                                || m.Status == (int)StatusRequest.CHECK_IN
                                                                || m.Status == (int)StatusRequest.CHECK_OUT
                                                                || m.Status == (int)StatusRequest.ENDED)
                                                            && (m.IdUser == IdUser || m.Houses != null && m.Houses.IdUser == IdUser))
                                                .ToList());
            }
            ViewData["IdRequest"] = IdRequest;

            byte[] salt = Crypto.Salt(this._configuration);
            string host = this.GetWebsitePath();

            foreach (var itemRequest in listRequest)
            {
                if (itemRequest != null)
                {
                    DetailRequest? request = this.CreateDetailRequest(itemRequest, null);
                    if (request != null)
                    {
                        model.Add(request);
                    }
                }
            }
            if (IdUser != 0)
            {
                this.SetViewData(new DoAnTotNghiepContext(this._context.GetConfig()), IdUser, Crypto.Salt(this._configuration));
            }
            return View(model);
        }
        private List<DetailRequest> GetAllRequestsSent()
        {
            int IdUser = this.GetIdUser();
            var requests = this._context.Requests
                                        .Where(m => m.IdUser == IdUser && m.Status != (int)StatusRequest.DISABLE).ToList();
            List<DetailRequest> model = new List<DetailRequest>();
            DoAnTotNghiepContext Context = this._context;
            foreach (var item in requests)
            {
                if (item != null)
                {
                    DetailRequest? request = this.CreateDetailRequest(item, null);
                    if (request != null)
                    {
                        model.Add(request);
                    }
                }
            }

            return model;
        }
        private DetailRequest? CreateDetailRequest(Request item, int? Status)
        {
            byte[] salt = Crypto.Salt(this._configuration);
            string host = this.GetWebsitePath();
            int IdUser = this.GetIdUser();
            if (item.Houses == null || !this._context.Entry(item).Reference(m => m.Houses).IsLoaded)
            {
                this._context.Entry(item).Reference(m => m.Houses).Query().Load();
            }
            if (item.Houses != null)
            {
                this._context.Entry(item.Houses).Reference(m => m.Users).Query().Load();
                this._context.Entry(item.Houses).Collection(m => m.FileOfHouses).Query().Load();
                this._context.Entry(item).Collection(m => m.FeedBacks).Query().Where(m => m.IdUser == IdUser).Load();
                this._context.Entry(item).Collection(m => m.CheckOuts).Query().Where(m => m.IdUser == IdUser).Load();
                this._context.Entry(item).Collection(m => m.CheckIns).Query().Where(m => m.IdUser == IdUser).Load();

                item.CheckStatus(item);
                if (Status != null && item.Status != Status) return null;

                if (item.Houses.Users != null)
                {
                    this._context.Entry(item.Houses.Users).Reference(m => m.Files).Query().Load();
                    DetailHouseViewModel house = this.CreateDetailsHouse(item.Houses);
                    DetailHouseViewModel? swapHouse = null;
                    DetailRatingViewModel? userRating= null;
                    DetailRatingViewModel? myRating = null;
                    FeedBack? userFeedBack = this._context.FeedBacks
                                                                .Where(m => m.IdUser != IdUser && m.IdRequest == item.Id)
                                                                .FirstOrDefault();
                    if (userFeedBack != null)
                    {
                        userRating = new DetailRatingViewModel(userFeedBack);
                    }
                    if (item.FeedBacks != null)
                    {
                        FeedBack? myFeedBack = item.FeedBacks.Where(m => m.IdUser == IdUser).FirstOrDefault();
                        if (myFeedBack != null)
                        {
                            myRating = new DetailRatingViewModel(myFeedBack);
                            item.Status = (int)StatusRequest.ENDED;
                        }
                    }
                    this._context.Entry(item).Reference(m => m.Users).Query().Load();
                    if (item.IdSwapHouse != null)
                    {
                        this._context.Entry(item).Reference(m => m.SwapHouses).Query().Load();
                        if (item.SwapHouses != null)
                        {
                            this._context.Entry(item.SwapHouses).Reference(m => m.Users).Query().Load();
                            this._context.Entry(item.SwapHouses).Collection(m => m.FileOfHouses).Query().Load();
                            swapHouse = this.CreateDetailsHouse(item.SwapHouses);
                        }
                    }
                    User? inputUser = null;
                    if (item.IdUser != IdUser)
                    {
                        inputUser = item.Houses.Users;
                    }
                    else
                    {
                        inputUser = item.Users;
                    }
                    DetailRequestViewModel request = new DetailRequestViewModel(item, inputUser, salt, host);
                    return new DetailRequest()
                    {
                        Request = request,
                        SwapHouse = swapHouse,
                        House = house,
                        UserRating = userRating,
                        MyRating = myRating
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
            house.IncludeLocation(this._context);
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
            List<CircleRequestViewModel> model = this.GetSuggest();
            int IdUser = this.GetIdUser();
            if (IdUser != 0)
            {
                this.SetViewData(new DoAnTotNghiepContext(this._context.GetConfig()), IdUser, Crypto.Salt(this._configuration));
            }
            return View(model);
        }


        //Get Suggestion
        private List<CircleRequestViewModel> GetSuggest()
        {
            int IdUser = this.GetIdUser();
            var circle = this._context.CircleExchangeHouseOfUsers
                                        .Include(m => m.CircleExchangeHouse)
                                        .Where(m => m.IdUser == IdUser
                                                        && m.CircleExchangeHouse != null
                                                        && m.CircleExchangeHouse.Status != (int)StatusWaitingRequest.DISABLE)
                                        .Select(m => m.CircleExchangeHouse)
                                        .ToList();

            List<CircleRequestViewModel> model = new List<CircleRequestViewModel>();
            byte[] salt = Crypto.Salt(this._configuration);
            string host = this.GetWebsitePath();
            foreach (var item in circle)
            {
                if (item != null)
                {
                    this._context.Entry(item).Collection(m => m.RequestInCircles).Query().Load();
                    this._context.Entry(item).Collection(m => m.FeedBacks).Query().Load();
                    if (item.RequestInCircles != null)
                    {
                        var rq = from rqc in item.RequestInCircles
                                 join crq in this._context.WaitingRequests on rqc.IdWaitingRequest equals crq.Id
                                 select crq.Node(rqc, crq);
                        if (rq != null)
                        {
                            rq = rq.ToList();
                            foreach (var itemRQ in rq)
                            {
                                this._context.Entry(itemRQ).Reference(m => m.Houses).Load();
                            }
                            WaitingRequest? myNode = rq.Where(m => m.IdUser == IdUser).FirstOrDefault();
                            if (myNode != null)
                            {
                                if (myNode.Houses != null)
                                {
                                    myNode.Houses.IncludeLocation(this._context);
                                    var inputMyNode = this.CircleRequestDetail(myNode, salt, host);
                                    WaitingRequest? prevNode = rq.Where(m => m.IdUser != IdUser && m.IdCity == myNode.Houses.IdCity.Value).FirstOrDefault();
                                    if (prevNode != null && inputMyNode != null)
                                    {
                                        prevNode.Houses.IncludeLocation(this._context);
                                        var inputPrevNode = this.CircleRequestDetail(prevNode, salt, host);
                                        WaitingRequest? nextNode = rq.Where(m => m.IdUser != IdUser && myNode.IdCity == m.Houses.IdCity.Value).FirstOrDefault();
                                        if (nextNode != null && inputPrevNode != null)
                                        {
                                            nextNode.Houses.IncludeLocation(this._context);
                                            var inputNextNode = this.CircleRequestDetail(nextNode, salt, host);

                                            if (inputNextNode != null)
                                            {
                                                CircleRequestViewModel node = new CircleRequestViewModel(inputPrevNode, inputMyNode, inputNextNode, item, IdUser);
                                                //var feedBacks = this._context.FeedBacks.Select(m => new DetailRatingViewModel(m)).ToList();
                                                //node.Rating.AddRange(feedBacks);
                                                model.Add(node);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return model;
        }
        private CircleRequestDetail? CircleRequestDetail(WaitingRequest request, byte[] salt, string host)
        {
            //lấy nhà
            if (request.Houses != null)
            {
                //lấy imagehouse
                this._context.Entry(request.Houses).Collection(m => m.FileOfHouses).Query().Load();
                if (request.Houses.FileOfHouses != null)
                {
                    var fImage = request.Houses.FileOfHouses.FirstOrDefault();
                    if (fImage != null)
                    {
                        this._context.Entry(fImage).Reference(m => m.Files).Load();
                        if (fImage != null)
                        {
                            ImageBase HouseImage = new ImageBase(fImage.Files, host);

                            //lấy người dùng
                            this._context.Entry(request).Reference(m => m.Users).Load();
                            if (request.Users != null)
                            {
                                this._context.Entry(request.Users).Reference(m => m.Files).Load();
                                UserInfo user = new UserInfo(request.Users, salt, host);
                                var numberSwap = from u in this._context.Users
                                                 join h in this._context.Houses on u.Id equals h.IdUser
                                                 join rq in this._context.Requests on h.Id equals rq.IdHouse
                                                 where u.Id == request.Users.Id && rq.Status >= (int)StatusRequest.CHECK_IN
                                                 select rq;
                                user.NumberSwap = (numberSwap == null ? 0 : numberSwap.ToList().Count());

                                //this._context.Entry(request.Houses).Collection(m => m.Requests).Query().Where(m => m.Status == (int)StatusRequest.WAIT_FOR_SWAP).Load();
                                //this._context.Entry(request.Houses).Collection(m => m.FeedBacks).Query().Load();
                                //this._context.Entry(request).Reference(m => m.Houses).Load();
                                request.Houses.IncludeLocation(this._context);
                                return new CircleRequestDetail(new DetailHouseViewModel(request.Houses, salt, null, null), request, user, HouseImage);
                            }
                        }
                    }
                }
            }
            return null;
        }

        public IActionResult History()
        {
            ViewData["active"] = 5;
            int IdUser = this.GetIdUser();
            List<HouseSelector> house = this._context.Houses
                                    .Where(m => m.IdUser == IdUser
                                                && m.Status == (int)StatusHouse.VALID)
                                    .Select(m => new HouseSelector(m))
                                    .ToList();
            District? district = this._context.Districts.FirstOrDefault();
            double lat = district == null ? 0 : district.Lat;
            double lng = district == null ? 0 : district.Lng;
            HistoryViewModel model = new HistoryViewModel(house, lat, lng);
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
            ListDetailHouses detailHouseViewModels = this.GetHouseInMemberPage(pagination, null);
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
            if (IdUser != 0)
            {
                this.SetViewData(new DoAnTotNghiepContext(this._context.GetConfig()), IdUser, Crypto.Salt(this._configuration));
            }
            return View(model);
        }
        private KeyValuePair<int, RoomChatViewModel> CreateDictionary(ChatRoom room, DoAnTotNghiepContext Context, int number, byte[] salt, int IdUser)
        {
            Context.Entry(room).Collection(m => m.Messages).Query()
                .OrderByDescending(m => m.CreatedDate).Take(number).Load();
            var userInChatRoom = Context.UsersInChatRooms
                        .Include(m => m.Users).ThenInclude(u => u.Files)
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
                            if (model.File != null)
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
                                    var file = Context.Files.Where(m => m.Id == user.IdFile.Value).FirstOrDefault();
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

        [HttpGet("/Request/All")]
        [AllowAnonymous]
        public IActionResult GetAll()
        {
            var request = this._context.Requests.Where(m => m.Status != (int)StatusRequest.DISABLE).ToList();
            List<DetailRequest> model = new List<DetailRequest>();
            DoAnTotNghiepContext Context = this._context;

            byte[] salt = Crypto.Salt(this._configuration);
            string host = this.GetWebsitePath();

            foreach (var item in request)
            {
                DetailRequest? itemAdd = this.CreateDetailRequest(item, null);
                if (itemAdd != null)
                {
                    model.Add(itemAdd);
                }
            }
            return Json(new
            {
                Status = 200,
                Data = model
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
            var user = this._context.Users.Include(m => m.Files).Include(m => m.Houses).Where(m => m.Id == IdUser).FirstOrDefault();
            if (user == null) return BadRequest(new { Status = 400, Message = "Không tìm thấy người dùng" });
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
            var user = this._context.Users.Include(m => m.Files).Where(m => m.Id == IdAnotherUser).FirstOrDefault();
            if(user == null) return NotFound();

            byte[] salt = Crypto.Salt(this._configuration);
            string host = this.GetWebsitePath();
            UserInfo info = new UserInfo(user, salt, host);
            //lấy danh sách nhà
            var houses = this._context.Houses.Include(m => m.FileOfHouses)
                                            .Where(m => m.IdUser == user.Id).ToList();
            List<DetailHouseViewModel> ListHouse = new List<DetailHouseViewModel>();
            foreach (var house in houses)
            {
                if(house != null)
                {
                    var item = this.CreateDetailsHouse(house);
                    if(item != null) ListHouse.Add(item);
                }
            }
            //lấy danh sách feedbacks
            var feedBacks = this._context.FeedBacks.Include(m => m.Users).ThenInclude(u => u.Files).Where(m => m.IdUserRated == IdAnotherUser).Select(m => new DetailRatingWithUser(m, salt, host)).ToList();
            int IdUser = this.GetIdUser();
            ViewData["isSelf"] = IdAnotherUser == IdUser ? "true": "false";
            
            if (IdUser != 0)
            {
                this.SetViewData(new DoAnTotNghiepContext(this._context.GetConfig()), IdUser, Crypto.Salt(this._configuration));
            }
            return View(new UserProfile(info, ListHouse, feedBacks));
        }
    }
}
