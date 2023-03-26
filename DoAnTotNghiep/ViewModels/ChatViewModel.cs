﻿using DoAnTotNghiep.Entity;
using DoAnTotNghiep.Modules;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnTotNghiep.ViewModels
{
    public class MessageViewModel
    {
        public MessageViewModel() { }

        public MessageViewModel(int IdReply, bool IsSeen, string IdSend, string Message, int Id, DateTime CreatedDate)
        {
            this.IdReply = IdReply;
            this.IsSeen = IsSeen; 
            this.IdSend = IdSend;
            this.Message = Message;
            this.CreatedDate = CreatedDate;
            this.Id = Id;
        }

        public string Message { get; set; } = string.Empty;
        public bool IsSeen { get; set; } = false;
        public int IdReply { get; set; } = 0;
        public string IdSend { get; set; } = string.Empty;
        public int Id { get; set; } = 0;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }

    public class MessageSendViewModel
    {
        public MessageSendViewModel(int IdReply, bool IsSeen, string IdSend, string Message, int Id, DateTime CreatedDate, int IdRoom)
        {
            this.Message = new MessageViewModel(IdReply, IsSeen, IdSend, Message, Id, CreatedDate);
            this.IdRoom = IdRoom;
        }

        public MessageViewModel Message { get; set; } = new MessageViewModel();
        public UserMessageViewModel UserMessage { get; set; } = new UserMessageViewModel();
        public int IdRoom { get; set; } = 0;
    }

    public class UserMessageViewModel
    {
        public UserMessageViewModel() { }
        public UserMessageViewModel(User user, byte[] salt, string host)
        {
            this.UserAccess = Crypto.EncodeKey(user.Id.ToString(), salt);
            this.UserName = user.FirstName + " " + user.LastName;
            this.ImageUrl = (user.IdFile == null ? null :
                                    user.Files == null ? null :
                                    (host + "/" + user.Files.PathFolder + "/" + user.Files.FileName));
        }

        public UserMessageViewModel(UsersInChatRoom user, byte[] salt, string host)
        {
            this.UserAccess = Crypto.EncodeKey(user.IdUser.ToString(), salt);
            this.UserName = user.Users?.FirstName + " " + user.Users?.LastName;
            this.ImageUrl = (user.Users?.IdFile == null ? null :
                                    user.Users?.Files == null ? null :
                                    (host + "/" + user.Users?.Files.PathFolder + "/" + user.Users?.Files.FileName));
        }

        public string UserName { get; set; } = string.Empty;
        public string? ImageUrl { get; set; } = string.Empty;
        public string UserAccess { get; set; } = string.Empty;
    }
    public class RoomChatViewModel
    {
        public int IdRoom { get; set; } = 0;
        public List<UserMessageViewModel> UserMessages { get; set; } = new List<UserMessageViewModel>();
        public List<MessageViewModel> Messages { get; set; } = new List<MessageViewModel>();
    }
    public class CreateMessageViewModel
    {
        public string Message { get; set; } = string.Empty;
        public int IdReply { get; set; } = 0;
        public int IdRoom { get; set; } = 0;
    }
    public class DataGetMessageViewModel
    {
        public int IdRoom { get; set; } = 0;
        public Pagination Pagination { get; set; } = new Pagination();
    }
    public class RangeRoom
    {
        public int Start { get; set; } = 0;
        public int Length { get; set; } = 10;
    }
    public class RoomConnectViewModel
    {
        public int? IdRoom { get; set; } = 0;
        public string ConnectionId { get; set; } = string.Empty;
    }
    public class ConnectRoom
    {
        public ConnectRoom(string connectionId, int idRoom)
        {
            ConnectionId = connectionId;
            IdRoom = idRoom;
        }

        public string ConnectionId { get; set; } = string.Empty;
        public int IdRoom { get; set; } = 0;
    }
    public class ConnectUser
    {
        public ConnectUser(string connectionId, string userAccess)
        {
            ConnectionId = connectionId;
            UserAccess = userAccess;
        }
        public string UserAccess { get; set; } = string.Empty;
        public string ConnectionId { get; set; } = string.Empty;
    }
}
