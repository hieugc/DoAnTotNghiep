﻿using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using DoAnTotNghiep.ViewModels;
using DoAnTotNghiep.Enum;

namespace DoAnTotNghiep.Hubs
{
    [Authorize(Roles = Role.Member)]
    public class ChatHub : Hub
    {
        private IHubContext<ChatHub> _context;
        public ChatHub(IHubContext<ChatHub> context) {
            _context= context;
        }
        
        public async Task SendMessage(string group, string target, RoomChatViewModel message)
        {
            await this._context.Clients.Group(group).SendAsync(target, message);
        }

        public async Task ConnectToGroup(string target, string IdRoom, string userAccess)
        {
            await this._context.Clients.Group(userAccess).SendAsync(target, IdRoom);
        }

        public async Task AddToGroup(string ConnectionId, string IdRoom)
        {
            await this._context.Groups.AddToGroupAsync(ConnectionId, IdRoom);
        }

        public async Task RemoveFromGroup(string ConnectionId, int IdRoom)
        {
            await this._context.Groups.RemoveFromGroupAsync(ConnectionId, IdRoom.ToString());
        }
    }
}