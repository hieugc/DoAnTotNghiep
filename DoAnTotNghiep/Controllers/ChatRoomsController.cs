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

        [HttpPost("/Connect")]
        public async Task<IActionResult> ConnectChat([FromBody] RoomConnectViewModel data)
        {
            if (ModelState.IsValid)
            {
                ChatHub chatHub = new ChatHub(this._signalContext);
                try
                {
                    if (data.IdRoom != null)
                    {
                        int IdUser = this.GetIdUser();
                        //kiểm tra có tồn tại trong DB không? => không => return status == 404
                        var rooms = this._context.UsersInChatRooms.Where(m => m.IdUser == IdUser && m.IdChatRoom == data.IdRoom).FirstOrDefault();
                        if (rooms == null)
                        {
                            return Json(new
                            {
                                Status = 404,
                                Message = "Kết nối không thành công! Phòng chat không tồn tại"
                            });
                        }
                        else
                        {
                            this._context.Entry(rooms).Reference(m => m.ChatRooms).Load();
                            Dictionary<int, RoomChatViewModel> model = new Dictionary<int, RoomChatViewModel>();
                            if (rooms.ChatRooms == null)
                            {
                                model.Add(data.IdRoom.Value, new RoomChatViewModel()
                                {
                                    IdRoom = data.IdRoom.Value,
                                    UserMessages = new List<UserMessageViewModel>(),
                                    Messages = new List<MessageViewModel>()
                                });
                            }
                            else
                            {
                                this._context.Entry(rooms.ChatRooms).Collection(m => m.UsersInChatRooms)
                                        .Query().Where(m => m.IdUser != IdUser).Include(m => m.Users).Load();

                                List<UserMessageViewModel> users = new List<UserMessageViewModel>();
                                byte[] salt = Crypto.Salt(this._configuration);
                                if (rooms.ChatRooms.UsersInChatRooms != null)
                                {
                                    foreach (var item in rooms.ChatRooms.UsersInChatRooms)
                                    {
                                        if (item.Users != null)
                                        {
                                            users.Add(new UserMessageViewModel(item.Users, salt));
                                        }
                                    }
                                }
                                model.Add(data.IdRoom.Value, new RoomChatViewModel()
                                {
                                    IdRoom = data.IdRoom.Value,
                                    UserMessages = users,
                                    Messages = new List<MessageViewModel>()
                                });
                            }

                            //trả dữ liệu về lưu vào client

                            //lưu vào cookie
                            string? cookieRooms = this.GetCookie(Enum.Cookie.ChatRoom());
                            List<string> chatRooms = new List<string>() { data.IdRoom.Value.ToString() };
                            if (!string.IsNullOrEmpty(cookieRooms))
                            {
                                chatRooms.AddRange(cookieRooms.Split(",").ToList());
                            }
                            this.SetCookie(Enum.Cookie.ChatRoom(), string.Join(",", chatRooms), 24);

                            //kết nối với group
                            await chatHub.AddToGroup(data.ConnectionId, data.IdRoom.Value.ToString());

                            return Json(new
                            {
                                Status = 200,
                                Message = "Kết nối thành công",
                                Data = model
                            });
                        }
                    }
                    else
                    {
                        string? rooms = this.GetCookie(Enum.Cookie.ChatRoom());
                        List<string> chatRooms = new List<string>();
                        if (!string.IsNullOrEmpty(rooms))
                        {
                            chatRooms = rooms.Split(",").ToList();
                        }

                        foreach (var room in chatRooms)
                        {
                            await chatHub.AddToGroup(data.ConnectionId, room);
                        }
                    }

                    return Json(new
                    {
                        Status = 200,
                        Message = "Kết nối thành công",
                        Data = data.ConnectionId
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                return Json(new
                {
                    Status = 500,
                    Message = "Lỗi kết nối"
                });
            }
            return Json(new
            {
                Status = 400,
                Message = ModelErrors()
            });
        }

        [HttpGet("/ChatRoom")]
        public JsonResult GetChatRoom(int? start = 0, int? length = 10)
        {
            int IdUser = this.GetIdUser();
            var allRooms = this._context.UsersInChatRooms
                                        .Where(m => m.IdUser == IdUser)
                                        .Skip((start == null? 0: start.Value))
                                        .Take((length == null? 10: length.Value))
                                        .Select(m => m.IdChatRoom).ToList();

            string lIdRoom = string.Join(",", allRooms);
            this.SetCookie(Enum.Cookie.ChatRoom(), lIdRoom, 24);
                
            return Json(new
            {
                Status = 200,
                Data = allRooms
            });
        }

        [HttpPost("/Send")]
        public async Task<JsonResult> SendMessage([FromBody] CreateMessageViewModel data)
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

                    ChatHub chatHub = new ChatHub(this._signalContext);
                    await chatHub.SendMessage(data.IdRoom.ToString(),
                                TargetSignalR.Receive(),
                                new MessageSendViewModel(data.IdReply,
                                                    false, IdSend, data.Message,
                                                    message.Id, message.CreatedDate, data.IdRoom));
                    return Json(new
                    {
                        Status = 200,
                        Message = "Gửi thành công",
                        Data = new MessageSendViewModel(
                                            data.IdReply, false, IdSend,
                                            data.Message, message.Id, message.CreatedDate, data.IdRoom)
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return Json(new
                    {
                        Status = 500,
                        Message = "Gửi thất bại"
                    });
                }
            }
            return Json(new
            {
                Status = 400,
                Message = ModelErrors()
            });
        }

        [HttpPost("/MessagesInChatRoom")]
        public JsonResult GetMessagesInChatRoom([FromBody] DataGetMessageViewModel data)
        {
            if (ModelState.IsValid)
            {
                int IdUser = this.GetIdUser();
                byte[] salt = Crypto.Salt(this._configuration);
                string? lMessageStored = this.GetCookie(Enum.Cookie.DataChat());
                Dictionary<int, RoomChatViewModel> model = new Dictionary<int, RoomChatViewModel>();
                if (!string.IsNullOrEmpty(lMessageStored))
                {
                    Dictionary<int, RoomChatViewModel>? rooms = JsonConvert.DeserializeObject<Dictionary<int, RoomChatViewModel>>(lMessageStored);
                    if (rooms != null && !rooms.ContainsKey(data.IdRoom) || rooms == null)
                    {
                        model.Add(data.IdRoom, this.GetRoomChatViewModel(data, salt, IdUser));
                        string dataChat = JsonConvert.SerializeObject(model);
                        this.SetCookie(Enum.Cookie.DataChat(), dataChat, 24);
                    }
                }
                else
                {
                    model.Add(data.IdRoom, this.GetRoomChatViewModel(data, salt, IdUser));
                    string dataChat = JsonConvert.SerializeObject(model);
                    this.SetCookie(Enum.Cookie.DataChat(), dataChat, 24);
                }

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
                                                                Crypto.EncodeKey(m.Id.ToString(), salt),
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
            foreach (UsersInChatRoom item in userInChatRoom)
            {
                if (item != null)
                {
                    users.Add(new UserMessageViewModel(item, salt));
                }
            }

            return new RoomChatViewModel()
            {
                IdRoom = data.IdRoom,
                UserMessages = users,
                Messages = messagesInRoom
            };
        }

    }
}
