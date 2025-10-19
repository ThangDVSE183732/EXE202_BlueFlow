using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace EventLink
{
    public class ChatHub : Hub
    {
        public async Task JoinRoom(Guid partnershipId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, partnershipId.ToString());
            Console.WriteLine($"✅ {Context.ConnectionId} joined room {partnershipId}");
        }

        public async Task SendMessage(Guid partnershipId, string username, string message)
        {
            await Clients.Group(partnershipId.ToString())
                .SendAsync("ReceiveMessage", username, message, DateTime.UtcNow);
        }

        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"🟢 User connected: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine($"🔴 User disconnected: {Context.ConnectionId}");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
