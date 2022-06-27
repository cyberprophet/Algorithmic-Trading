using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using ShareInvest.Interface;
using ShareInvest.Server.Data;
using ShareInvest.Server.Data.Models;

namespace ShareInvest.Server.Hubs
{
    [Authorize]
    public class Message : Hub<IHubs>
    {
        public async Task AddToGroup(string groupName)
        {
            if (context.Users.AsNoTracking().SingleOrDefault(o => o.Id.Equals(Context.UserIdentifier)) is CoreUser cu)
                await Clients.Group(groupName).OnReceiveStringMessage($"{cu.UserName.Split('@')[0]} has joined the group {groupName}.");

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }
        public async Task RemoveFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            if (context.Users.AsNoTracking().SingleOrDefault(o => o.Id.Equals(Context.UserIdentifier)) is CoreUser cu)
                await Clients.Group(groupName).OnReceiveStringMessage($"{cu.UserName.Split('@')[0]} has left the group {groupName}.");
        }
        public override async Task OnConnectedAsync() => await base.OnConnectedAsync();
        public override async Task OnDisconnectedAsync(Exception? exception) => await base.OnDisconnectedAsync(exception);
        public Message(CoreContext context) => this.context = context;
        readonly CoreContext context;
    }
}