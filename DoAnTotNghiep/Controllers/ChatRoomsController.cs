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
using DoAnTotNghiep.Hubs;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.AspNetCore.SignalR;
using DoAnTotNghiep.ViewModels;
using DoAnTotNghiep.Enum;
using DoAnTotNghiep.Modules;
using System.Text;
using Newtonsoft.Json;
using NuGet.Packaging;
using System.Net.WebSockets;
using System.Net;
using DoAnTotNghiep.Service;
using Microsoft.Extensions.Hosting;

namespace DoAnTotNghiep.Controllers
{
    [Authorize(Roles = Enum.Role.Member)]
    public class ChatRoomsController : BaseController
    {
        private readonly DoAnTotNghiepContext _context;
        private readonly IHubContext<ChatHub> _signalContext;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly IMessageService _messageService;

        public ChatRoomsController(
                                DoAnTotNghiepContext context, 
                                IHubContext<ChatHub> signalContext, 
                                IConfiguration configuration, 
                                IHostEnvironment environment,
                                IUserService userService,
                                IMessageService messageService) : base(environment)
        {
            _context = context;
            _signalContext = signalContext;
            _configuration = configuration;
            _userService = userService;
            _messageService = messageService;
        }

        //Kết nối tất cả phòng
        [HttpPost("/ConnectAllChat")]
        public async Task<IActionResult> ConnectChat([FromBody] string ConnectionId)
        {
            return await this.ConnectAll(ConnectionId, true);
        }
        [HttpPost("/api/ConnectAllRoom")]//kết nối đã có sẳn
        public async Task<IActionResult> ApiConnectChat([FromBody] string ConnectionId)
        {
            return await this.ConnectAll(ConnectionId, false);
        }
        private async Task<IActionResult> ConnectAll(string ConnectionId, bool isWeb)
        {
            if (ModelState.IsValid)
            {
                ChatHub chatHub = new ChatHub(this._signalContext);
                try
                {
                    int IdUser = this.GetIdUser();
                    var rooms = this._messageService.GetByUser(IdUser, 20, 0);
                    byte[] salt = Crypto.Salt(this._configuration);
                    string host = this.GetWebsitePath();
                    Dictionary<int, RoomChatViewModel> webModel = new Dictionary<int, RoomChatViewModel>();

                    if (isWeb)
                    {
                        foreach (var item in rooms)
                        {
                            if (item != null)
                            {
                                RoomChatViewModel model = this._messageService.CreateDictionary(item, 1, 0, salt, host, IdUser);
                                webModel.Add(model.IdRoom, model);
                            }
                        }
                    }
                    foreach (var room in rooms)
                    {
                        await chatHub.AddToGroup(ConnectionId, room.Id.ToString());
                    }
                    await chatHub.AddToGroup(ConnectionId, Crypto.EncodeKey(IdUser.ToString(), salt));

                    if (isWeb)
                    {
                        return Json(new
                        {
                            Status = 200,
                            Message = "Kết nối thành công",
                            Data = webModel
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            Status = 200,
                            Message = "Kết nối thành công"
                        });
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                return BadRequest(new
                {
                    Status = 500,
                    Message = ModelErrors()
                });
            }
            return BadRequest(new
            {
                Status = 400,
                Message = ModelErrors()
            });
        }

        //kết nối từng phòng
        [HttpPost("/Message/ConnectToRoom")]
        public async Task<IActionResult> ConnectChat([FromBody] ConnectRoom data)
        {
            return await this.ConnectToRoom(data, true);
        }
        [HttpPost("/api/Message/ConnectToRoom")]//người thứ 2 bị kết nối
        public async Task<IActionResult> ApiConnectChat([FromBody] ConnectRoom data)
        {
            return await this.ConnectToRoom(data, false);
        }
        private async Task<IActionResult> ConnectToRoom(ConnectRoom data, bool isWeb)
        {
            if (ModelState.IsValid)
            {
                ChatHub chatHub = new ChatHub(this._signalContext);
                try
                {
                    int IdUser = this.GetIdUser();

                    var rooms = this._messageService.GetById(IdUser, data.IdRoom);
                    byte[] salt = Crypto.Salt(this._configuration);
                    string host = this.GetWebsitePath();
                    Dictionary<int, RoomChatViewModel> webModel = new Dictionary<int, RoomChatViewModel>();
                    List<RoomChatViewModel> mobileModel = new List<RoomChatViewModel>();
                    DoAnTotNghiepContext Context = this._context;

                    if (isWeb)
                    {
                        foreach (var item in rooms)
                        {
                            if (item != null)
                            {
                                RoomChatViewModel model = this._messageService.CreateDictionary(item, 1, 0, salt, host, IdUser);
                                webModel.Add(model.IdRoom, model);
                            }
                        }
                    }
                    else
                    {
                        foreach (var item in rooms)
                        {
                            if (item != null)
                            {
                                mobileModel.Add(this._messageService.CreateDictionary(item, 1, 0, salt, host, IdUser));
                            }
                        }
                    }

                    foreach (var room in rooms)
                    {
                        await chatHub.AddToGroup(data.ConnectionId, room.Id.ToString());
                    }

                    if (isWeb)
                    {
                        return Json(new
                        {
                            Status = 200,
                            Message = "Kết nối thành công",
                            Data = webModel
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            Status = 200,
                            Message = "Kết nối thành công",
                            Data = mobileModel.First()
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                return BadRequest(new
                {
                    Status = 500,
                    Message = ModelErrors()
                });
            }
            return BadRequest(new
            {
                Status = 400,
                Message = ModelErrors()
            });
        }

        //Kết nối với người dùng
        [HttpPost("/api/Message/ContactToUser")]//kết nối trang detail (nút liên hệ) => kết nối cho người dùng mới
        public async Task<IActionResult> ApiContactUser([FromBody] ConnectUser data)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    byte[] salt = Crypto.Salt(this._configuration);
                    string IdUserStr = Crypto.DecodeKey(data.UserAccess, salt);
                    int Id = 0;
                    if (!int.TryParse(IdUserStr, out Id))
                    {
                        return BadRequest(new
                        {
                            Status = 400,
                            Message = "Token không hợp lệ"
                        });
                    }
                    int IdUser = this.GetIdUser();
                    string host = this.GetWebsitePath();
                    List<RoomChatViewModel> model = new List<RoomChatViewModel>();

                    if (Id != IdUser)
                    {
                        var user = this._userService.GetById(Id);
                        if (user == null)
                        {
                            return BadRequest(new
                            {
                                Status = 400,
                                Message = "Không tìm thấy người dùng"
                            });
                        }

                        var rooms = this._messageService.GetRoomWithTwoMembers(Id, IdUser);

                        if (rooms.Count() > 0)
                        {
                            foreach (var room in rooms)
                            {
                                if (room != null)
                                    if (rooms.Count() > 0)
                                        model.Add(this._messageService.CreateDictionary(room, 20, 0, salt, host, IdUser));
                            }
                        }
                        else
                        {
                            using (var transaction = await this._context.Database.BeginTransactionAsync())
                            {
                                try
                                {
                                    ChatRoom chatRoom = new ChatRoom() { UpdatedDate = DateTime.Now };
                                    this._messageService.Save(chatRoom, new List<int>() { Id, IdUser });

                                    transaction.Commit();
                                    if (user.IdFile != null && user.IdFile.Value != 0)
                                    {
                                        user.InCludeAll((this._context));
                                    }

                                    model.Add(new RoomChatViewModel()
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
                                    return BadRequest(new
                                    {
                                        Status = 400,
                                        Message = "Khởi tạo không được"
                                    });
                                }
                            }
                        }

                        return Json(new
                        {
                            Status = 200,
                            Message = "Kết nối người dùng thành công",
                            Data = model.First()
                        });
                    }
                    else
                    {
                        return BadRequest(new
                        {
                            Status = 400,
                            Message = "Truyền UserAccess bản thân"
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                return BadRequest(new
                {
                    Status = 500,
                    Message = ModelErrors()
                });
            }
            return BadRequest(new
            {
                Status = 400,
                Message = ModelErrors()
            });
        }

        //Lấy danh sách phòng
        [HttpGet("/ChatRoom")]
        public JsonResult GetChatRoom(Pagination pagination)
        {
            return Json(new
            {
                Status = 200,
                Data = this.ChatRoom(pagination.Limit, ((pagination.Page - 1) <= 0 ? 0 : pagination.Page - 1) * pagination.Limit)
            });
        }
        [HttpGet("/api/ChatRoom")]//lấy danh sách phòng chat (những người chat với nhau)
        public JsonResult ApiGetChatRoom(int page = 0, int limit = 10)
        {
            return Json(new
            {
                Status = 200,
                Data = new
                {
                    rooms = this.ChatRoom(limit, ((page - 1) <= 0 ? 0 : page - 1) * limit),
                    metaData = new Pagination(page, limit)
                }
            });
        }
        private List<RoomChatViewModel> ChatRoom(int number, int skip)
        {
            int IdUser = this.GetIdUser();
            var allRooms = this._messageService.GetByUser(IdUser, number, skip);
            byte[] salt = Crypto.Salt(this._configuration);
            string host = this.GetWebsitePath();
            List<RoomChatViewModel> Chat = new List<RoomChatViewModel>();
            foreach (var item in allRooms)
            {
                if(item != null)
                {
                    Chat.Add(this._messageService.CreateDictionary(item, 20, 0, salt, host, IdUser));
                }
            }
            return Chat;
        }


        //Gửi tin nhắn
        [HttpPost("/Send")]
        public async Task<IActionResult> SendMessage([FromBody] CreateMessageViewModel data)
        {
            return await this.Send(data);
        }
        [HttpPost("/api/Message/Send")]//gửi tin nhắn
        public async Task<IActionResult> ApiSendMessage([FromBody] CreateMessageViewModel data)
        {
            return await this.Send(data);
        }
        private async Task<IActionResult> Send(CreateMessageViewModel data)
        {
            if (ModelState.IsValid)
            {
                int IdUser = this.GetIdUser();
                Message message = new Message()
                {
                    IdChatRoom = data.IdRoom,
                    Content = data.Message,
                    IdReply = (data.IdReply == 0 ? null : data.IdReply),
                    Status = (int)StatusMessage.UNSEEN,
                    CreatedDate = DateTime.Now,
                    IdUser = IdUser
                };

                byte[] salt = Crypto.Salt(this._configuration);
                string host = this.GetWebsitePath();
                try
                {
                    this._messageService.Save(message, IdUser);
                    var user = this._userService.GetById(IdUser);
                    List<UserMessageViewModel> users = new List<UserMessageViewModel>();
                    if(user != null)
                    {
                        user.InCludeAll(this._context);
                        users.Add(new UserMessageViewModel(user, salt, host));
                    }
                    var node = new MessageViewModel(message, salt, IdUser);
                    node.IsSeen = false;
                    await new ChatHub(this._signalContext).SendMessage(data.IdRoom.ToString(), TargetSignalR.Receive(), new RoomChatViewModel()
                    {
                        IdRoom = data.IdRoom,
                        UserMessages = users,
                        Messages = new List<MessageViewModel>() { node }
                    });
                    node.IsSeen = true;
                    return Ok(new
                    {
                        Status = 200,
                        Message = "Gửi thành công",
                        Data = new RoomChatViewModel()
                        {
                            IdRoom = data.IdRoom,
                            UserMessages = users,
                            Messages = new List<MessageViewModel>() { node }
                        }
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return BadRequest(new
                    {
                        Status = 500,
                        Message = "Gửi thất bại"
                    });
                }
            }
            return BadRequest(new
            {
                Status = 400,
                Message = ModelErrors()
            });
        }


        //Cập nhật tin nhắn
        [HttpPost("/Message/Seen")]
        public IActionResult UpdateSeenChat([FromBody] int idRoom)
        {
            return this.UpdateSeen(idRoom);
        }
        [HttpPost("/api/Message/Seen")]//cập nhật đã xem
        public IActionResult ApiUpdateSeenChat([FromBody] int idRoom)
        {
            return this.UpdateSeen(idRoom);
        }
        private IActionResult UpdateSeen(int idRoom)
        {
            if (ModelState.IsValid)
            {
                this.UpdateMessageIsSeen(idRoom);
                return Ok(new
                {
                    Status = 200
                });
            }
            return BadRequest(new
            {
                Status = 400,
                Message = ModelErrors()
            });
        }
        private bool UpdateMessageIsSeen(int idRoom)
        {
            int IdUser = this.GetIdUser();
            try
            {
                this._messageService.SeenAll(idRoom, IdUser);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return false;
        }

        [HttpPost("/MessagesInChatRoom")]
        public JsonResult GetMessagesInChatRoom([FromBody] DataGetMessageViewModel data)
        {
            if (ModelState.IsValid)
            {
                Dictionary<int, RoomChatViewModel> model = new Dictionary<int, RoomChatViewModel>();
                model.Add(data.IdRoom, 
                                        this._messageService.CreateDictionary(
                                        new ChatRoom() { Id = data.IdRoom },
                                        data.Pagination.Limit,
                                        ((data.Pagination.Page - 1) <= 0 ? 0 : (data.Pagination.Page - 1)) * data.Pagination.Limit,
                                        Crypto.Salt(this._configuration),
                                        this.GetWebsitePath(),
                                        this.GetIdUser()));
                return Json(new
                {
                    Status = 200,
                    Data = model
                });
            }
            return Json(new
            {
                Status = 400,
                Message = this.ModelErrors()
            });
        }
        [HttpPost("/api/MessagesInChatRoom")]
        public JsonResult ApiGetMessagesInChatRoom([FromBody] DataGetMessageViewModel data)
        {
            if (ModelState.IsValid)
            {
                return Json(new
                {
                    Status = 200,
                    Data = this._messageService.CreateDictionary(
                        new ChatRoom() { Id = data.IdRoom }, 
                        data.Pagination.Limit, 
                        ((data.Pagination.Page - 1) <= 0 ? 0 : (data.Pagination.Page - 1)) * data.Pagination.Limit, 
                        Crypto.Salt(this._configuration), 
                        this.GetWebsitePath(), 
                        this.GetIdUser())
                });
            }
            return Json(new
            {
                Status = 400,
                Message = this.ModelErrors()
            });
        }
    }
}
