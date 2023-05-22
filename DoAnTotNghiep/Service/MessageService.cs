using DoAnTotNghiep.Data;
using DoAnTotNghiep.Entity;
using DoAnTotNghiep.Enum;
using DoAnTotNghiep.Modules;
using DoAnTotNghiep.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.ML;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.WebSockets;
using static QRCoder.PayloadGenerator;

namespace DoAnTotNghiep.Service
{
    public class MessageService : IMessageService
    {
        private DoAnTotNghiepContext _context;
        public MessageService(DoAnTotNghiepContext context)
        {
            this._context = context;
        }

        public void Save(ChatRoom room, List<int> IdUsers)
        {
            this._context.ChatRooms.Add(room);
            this._context.SaveChanges();

            List<UsersInChatRoom> newList = new List<UsersInChatRoom>();
            foreach (var item in IdUsers) newList.Add(new UsersInChatRoom() { IdChatRoom = room.Id, IdUser = item });

            this._context.UsersInChatRooms.AddRange(newList);
            this._context.SaveChanges();
        }
        public void SeenAll(int idRoom, int idUser)
        {
            var otherMessage = this._context.Messages
                                          .Where(m => m.IdChatRoom == idRoom
                                                        && m.IdUser != idUser
                                                        && m.Status == (int)StatusMessage.UNSEEN);

            foreach (var item in otherMessage)
            {
                item.Status = (int)StatusMessage.SEEN;
            }

            this._context.UpdateRange(otherMessage);
            this._context.SaveChanges();
        }
        public ChatRoom Save(Message message, int IdUser)
        {
            this._context.Messages.Add(message);
            this._context.SaveChanges();

            var rooms = this.GetById(IdUser, message.IdChatRoom);
            foreach (var item in rooms)
            {
                item.UpdatedDate = DateTime.Now;
            }
            this._context.ChatRooms.UpdateRange(rooms);
            this._context.SaveChanges();
            return rooms.First();
        }
        public List<ChatRoom> GetByUser(int IdUser, int number, int skip)
        {
            var rooms = from uf in this._context.UsersInChatRooms
                        join r in this._context.ChatRooms on uf.IdChatRoom equals r.Id
                        where uf.IdUser == IdUser
                        select r;
            if (rooms == null) return new List<ChatRoom>();
            return rooms.OrderByDescending(m => m.UpdatedDate).Skip(skip).Take(number).ToList();
        }
        public List<ChatRoom> GetById(int IdUser, int Id)
        {
            var rooms = from uf in this._context.UsersInChatRooms
                        join r in this._context.ChatRooms on uf.IdChatRoom equals r.Id
                        where uf.IdUser == IdUser && r.Id == Id
                        select r;
            if (rooms == null) return new List<ChatRoom>();
            return rooms.ToList();
        }
        public RoomChatViewModel CreateDictionary(ChatRoom room, int number, int skip, byte[] salt, string host, int IdUser)
        {
            return new RoomChatViewModel()
            {
                IdRoom = room.Id,
                Messages = this.GetMessageByRoom(room, number, skip, salt, IdUser),
                UserMessages = this.GetAnotherUserByRoom(room, salt, host, IdUser)
            };
        }
        public List<MessageViewModel> GetMessageByRoom(ChatRoom room, int number, int skip, byte[] salt, int IdUser)
        {
            return this._context.Messages
                                .Where(m => m.IdChatRoom == room.Id)
                                .OrderByDescending(m => m.CreatedDate)
                                .Skip(skip)
                                .Take(number)
                                .Select(m => new MessageViewModel(m, salt, IdUser))
                                .ToList();
        }
        public List<UserMessageViewModel> GetAnotherUserByRoom(ChatRoom room, byte[] salt, string host, int IdUser)
        {
            return this._context.UsersInChatRooms
                                .Include(m => m.Users).ThenInclude(u => u.Files)
                                .Where(m => m.IdChatRoom == room.Id && m.IdUser != IdUser)
                                .Select(m => new UserMessageViewModel(m, salt, host))
                                .ToList();
        }
        public List<ChatRoom> GetRoomWithTwoMembers(int IdUser1, int IdUser2)
        {
            var rooms = from r in this._context.ChatRooms
                        join ur in this._context.UsersInChatRooms on r.Id equals ur.IdChatRoom
                        join ur2 in this._context.UsersInChatRooms on r.Id equals ur2.IdChatRoom
                        where ur.IdUser == IdUser1 && ur2.IdUser == IdUser2
                        select r;
            return rooms.ToList();
        }
    }

    public interface IMessageService
    {
        public void Save(ChatRoom room, List<int> IdUsers);
        public void SeenAll(int idRoom, int idUser);
        public ChatRoom Save(Message message, int IdUser);
        public List<ChatRoom> GetByUser(int IdUser, int number, int skip);
        public List<ChatRoom> GetById(int IdUser, int Id);
        public List<ChatRoom> GetRoomWithTwoMembers(int IdUser1, int IdUser2);
        public RoomChatViewModel CreateDictionary(ChatRoom room, int number, int skip, byte[] salt, string host, int IdUser);
        public List<MessageViewModel> GetMessageByRoom(ChatRoom room, int number, int skip, byte[] salt, int IdUser);
        public List<UserMessageViewModel> GetAnotherUserByRoom(ChatRoom room, byte[] salt, string host, int IdUser);
    }
}
