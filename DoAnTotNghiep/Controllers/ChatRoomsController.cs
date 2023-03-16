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

namespace DoAnTotNghiep.Controllers
{
    [Authorize(Roles = "MEMBER")]
    [Route("Chat")]
    public class ChatRoomsController : BaseController
    {
        private readonly DoAnTotNghiepContext _context;
        private readonly IHubContext<ChatHub> _signalContext;
        private readonly IConfiguration _configuration;
        public ChatRoomsController(DoAnTotNghiepContext context, IHubContext<ChatHub> signalContext, IConfiguration configuration)
        {
            _context = context;
            _signalContext = signalContext;
            _configuration = configuration;
        }


        //Kết nối tất cả phòng
        [HttpPost("/ConnectAllChat")]
        public async Task<IActionResult> ConnectChat([FromBody] string ConnectionId)
        {
            return await this.ConnectAll(ConnectionId, true);
        }
        [HttpPost("/api/ConnectAllRoom")]
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
                    using(var Context = this._context)
                    {
                        var rooms = Context.UsersInChatRooms.Include(m => m.ChatRooms).Where(m => m.IdUser == IdUser && m.ChatRooms != null).ToList();
                        byte[] salt = Crypto.Salt(this._configuration);
                        Dictionary<int, RoomChatViewModel> webModel = new Dictionary<int, RoomChatViewModel>();
                        List<RoomChatViewModel> mobileModel = new List<RoomChatViewModel>();
                        if (isWeb)
                        {
                            foreach (var item in rooms)
                            {
                                if (item.ChatRooms != null)
                                {
                                    RoomChatViewModel model = this.CreateChatRoom(item.ChatRooms, Context, 1, salt, IdUser);
                                    webModel.Add(model.IdRoom, model);
                                }
                            }
                        }
                        else
                        {
                            foreach (var item in rooms)
                            {
                                if (item.ChatRooms != null)
                                {
                                    mobileModel.Add(this.CreateChatRoom(item.ChatRooms, Context, 1, salt, IdUser));
                                }
                            }
                        }

                        foreach (var room in rooms)
                        {
                            await chatHub.AddToGroup(ConnectionId, room.IdChatRoom.ToString());
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
                                Data = mobileModel
                            });
                        }
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
        [HttpPost("/api/Message/ConnectToRoom")]
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

                    var rooms = this._context.UsersInChatRooms
                                            .Include(m => m.ChatRooms)
                                            .Where(m => m.IdUser == IdUser && m.ChatRooms != null && m.IdChatRoom == data.IdRoom).ToList();
                    byte[] salt = Crypto.Salt(this._configuration);

                    Dictionary<int, RoomChatViewModel> webModel = new Dictionary<int, RoomChatViewModel>();
                    List<RoomChatViewModel> mobileModel = new List<RoomChatViewModel>();
                    DoAnTotNghiepContext Context = this._context;

                    if (isWeb)
                    {
                        foreach (var item in rooms)
                        {
                            if (item.ChatRooms != null)
                            {
                                RoomChatViewModel model = this.CreateChatRoom(item.ChatRooms, Context, 1, salt, IdUser);
                                webModel.Add(model.IdRoom, model);
                            }
                        }
                    }
                    else
                    {
                        foreach (var item in rooms)
                        {
                            if (item.ChatRooms != null)
                            {
                                mobileModel.Add(this.CreateChatRoom(item.ChatRooms, Context, 1, salt, IdUser));
                            }
                        }
                    }

                    foreach (var room in rooms)
                    {
                        await chatHub.AddToGroup(data.ConnectionId, room.IdChatRoom.ToString());
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
                            Data = mobileModel
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
        [HttpPost("/api/Message/ContactToUser")]
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
                    List<RoomChatViewModel> model = new List<RoomChatViewModel>();
                    //tạo 1 cái room mới
                    using (DoAnTotNghiepContext Context = _context)
                    {
                        if (Id != IdUser)
                        {
                            var user = Context.Users.FirstOrDefault(m => m.Id == Id);
                            if (user == null)
                            {
                                return BadRequest(new
                                {
                                    Status = 400,
                                    Message = "Không tìm thấy người dùng"
                                });
                            }

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
                                        model.Add(this.CreateChatRoom(room, Context, 20, salt, IdUser));
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

                                        model.Add(new RoomChatViewModel()
                                        {
                                            IdRoom = chatRoom.Id,
                                            UserMessages = new List<UserMessageViewModel>() {
                                                new UserMessageViewModel(user, salt, this.GetWebsitePath())
                                            },
                                            Messages = new List<MessageViewModel>()
                                        });
                                        ChatHub chatHub = new ChatHub(this._signalContext);

                                        await chatHub.ConnectToGroup((TargetSignalR.Connect() + "-" + Crypto.EncodeKey(user.Id.ToString(), salt)), chatRoom.Id.ToString());
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
                                Data = model
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
            var allRooms = this.ChatRoom(pagination);
            return Json(new
            {
                Status = 200,
                Data = allRooms
            });
        }
        [HttpGet("/api/ChatRoom")]
        public JsonResult ApiGetChatRoom(Pagination pagination)
        {
            return Json(new
            {
                Status = 200,
                Data = new
                {
                    rooms = this.ChatRoom(pagination),
                    metaData = pagination
                }
            });
        }
        private List<int> ChatRoom(Pagination pagination)
        {
            int IdUser = this.GetIdUser();
            var allRooms = this._context.UsersInChatRooms
                                        .Where(m => m.IdUser == IdUser)
                                        .Skip((pagination.Page))
                                        .Take((pagination.Limit))
                                        .Select(m => m.IdChatRoom);
            return allRooms == null ? new List<int>() : allRooms.ToList();
        }



        //Gửi tin nhắn
        [HttpPost("/Send")]
        public async Task<IActionResult> SendMessage([FromBody] CreateMessageViewModel data)
        {
            return await this.Send(data);
        }
        [HttpPost("/api/Message/Send")]
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
                    Status = (int)Status.UNSEEN,
                    CreatedDate = DateTime.Now,
                    IdUser = IdUser
                };

                try
                {
                    byte[] salt = Crypto.Salt(this._configuration);
                    string IdSend = Crypto.EncodeKey(IdUser.ToString(), salt);

                    this._context.Add(message);
                    await this._context.SaveChangesAsync();

                    var rooms = this._context.ChatRooms.Where(m => m.Id == data.IdRoom);
                    foreach (var item in rooms)
                    {
                        item.UpdatedDate = DateTime.Now;
                    }
                    this._context.ChatRooms.UpdateRange(rooms);
                    this._context.SaveChanges();

                    var user = this._context.Users.Include(m => m.Files).Where(m => m.Id == IdUser).ToList();
                    List<UserMessageViewModel> users = new List<UserMessageViewModel>();
                    string host = this.GetWebsitePath();
                    foreach (User item in user)
                    {
                        if (item != null)
                        {
                            users.Add(new UserMessageViewModel(item, salt, host));
                        }
                    }

                    RoomChatViewModel model = new RoomChatViewModel()
                    {
                        IdRoom = data.IdRoom,
                        UserMessages = users,
                        Messages = new List<MessageViewModel>() { new MessageViewModel(data.IdReply, false, IdSend,
                                            data.Message, message.Id, message.CreatedDate) }
                    };


                    ChatHub chatHub = new ChatHub(this._signalContext);
                    await chatHub.SendMessage(data.IdRoom.ToString(), TargetSignalR.Receive(), model);
                    return Ok(new
                    {
                        Status = 200,
                        Message = "Gửi thành công",
                        Data = model
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
        [HttpPut("/Message/Seen")]
        public IActionResult UpdateSeenChat([FromBody] int idRoom)
        {
            return this.UpdateSeen(idRoom);
        }
        [HttpPut("/api/Message/Seen")]
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
                    Status = 200,
                    Message = "Cập nhật thành công"
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
            var otherMessage = this._context.Messages
                                          .Where(m => m.IdChatRoom == idRoom
                                                        && m.IdUser != IdUser
                                                        && m.Status == (int)Status.UNSEEN);

            foreach (var item in otherMessage)
            {
                item.Status = (int)Status.SEEN;
            }

            this._context.UpdateRange(otherMessage);
            this._context.SaveChanges();
            return true;
        }


        //Lấy tin nhắn --chưa có mobile
        [HttpPost("/MessagesInChatRoom")]
        public JsonResult GetMessagesInChatRoom([FromBody] DataGetMessageViewModel data)
        {
            if (ModelState.IsValid)
            {
                int IdUser = this.GetIdUser();
                byte[] salt = Crypto.Salt(this._configuration);
                Dictionary<int, RoomChatViewModel> model = new Dictionary<int, RoomChatViewModel>();
                model.Add(data.IdRoom, this.GetRoomChatViewModel(data, salt, IdUser));
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
        private RoomChatViewModel GetRoomChatViewModel(DataGetMessageViewModel data, byte[] salt, int IdUser)
        {
            var messagesInRoom = this._context.Messages
                                            .Where(m => m.IdChatRoom == data.IdRoom)
                                            .OrderByDescending(m => m.CreatedDate)
                                            .Select(
                                                    m => new MessageViewModel(
                                                                m.IdReply == null ? 0 : m.IdReply.Value,
                                                                ((m.Status == (int)Status.SEEN) || m.IdUser == IdUser),
                                                                Crypto.EncodeKey(m.IdUser.ToString(), salt),
                                                                m.Content, m.Id, m.CreatedDate)
                                            )
                                            .Skip(data.RangeRoom.Start)
                                            .Take(data.RangeRoom.Length)
                                            .ToList();

            var userInChatRoom = this._context.UsersInChatRooms
                                                .Include(m => m.Users)
                                                .Where(m => m.IdChatRoom == data.IdRoom && m.IdUser != IdUser)
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

            return new RoomChatViewModel()
            {
                IdRoom = data.IdRoom,
                UserMessages = users,
                Messages = messagesInRoom
            };
        }


        //clear all Chat
        [HttpPost("/ChatRoom/ClearAll")]
        [AllowAnonymous]
        public async Task<IActionResult> ClearAllDataChatRoom([FromBody] string WebsiteKey)
        {
            string? Key = this._configuration.GetConnectionString(Enum.SystemKey.WebsiteKey());
            if (!string.IsNullOrEmpty(WebsiteKey) && !string.IsNullOrEmpty(Key))
            {
                if (!WebsiteKey.Equals(Key))
                {
                    return BadRequest(new
                    {
                        Message = "Key không đúng"
                    });
                }

                //xóa all message
                //xóa all userInRoom
                //xóa all rooms

                var messages = this._context.Messages.ToList();
                var users = this._context.UsersInChatRooms.ToList();
                var rooms = this._context.ChatRooms.ToList();

                this._context.Messages.RemoveRange(messages);
                await this._context.SaveChangesAsync();

                this._context.UsersInChatRooms.RemoveRange(users);
                await this._context.SaveChangesAsync();

                this._context.ChatRooms.RemoveRange(rooms);
                await this._context.SaveChangesAsync();

                return Json(new
                {
                    Message = "Đã xóa xong"
                });
            }
            return BadRequest(new
            {
                Message = "Không có key"
            });
        }

        private KeyValuePair<int, RoomChatViewModel> CreateDictionary(ChatRoom room, DoAnTotNghiepContext Context, int number, byte[] salt, int IdUser)
        {
            RoomChatViewModel model = this.CreateChatRoom(room, Context, number, salt, IdUser);
            return new KeyValuePair<int, RoomChatViewModel>(model.IdRoom, model);
        }
        private RoomChatViewModel CreateChatRoom(ChatRoom room, DoAnTotNghiepContext Context, int number, byte[] salt, int IdUser)
        {
            Context.Entry(room).Collection(m => m.Messages).Query()
                                .OrderByDescending(m => m.CreatedDate)
                                .Take(number).Load();
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
            return new RoomChatViewModel()
            {
                IdRoom = room.Id,
                Messages = (number == 0 || room.Messages == null) ? new List<MessageViewModel>() :
                                room.Messages.OrderByDescending(m => m.Id).Take(number).Select(m => new MessageViewModel(
                                m.IdReply == null ? 0 : m.IdReply.Value,
                                ((m.Status == (int)Status.SEEN) || m.IdUser == IdUser),
                                Crypto.EncodeKey(m.IdUser.ToString(), salt),
                                m.Content, m.Id, m.CreatedDate)).ToList(),
                UserMessages = users
            };
        }
    }
}
