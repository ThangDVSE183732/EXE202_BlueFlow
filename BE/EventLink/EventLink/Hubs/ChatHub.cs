using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventLink.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly ILogger<ChatHub> _logger;

        private static readonly Dictionary<string, string> _connections = new();
        private static readonly object _connectionLock = new();

        public ChatHub(ILogger<ChatHub> logger)
        {
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst("UserId")?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                lock (_connectionLock)
                {
                    _connections[userId] = Context.ConnectionId;
                }

                _logger.LogInformation("User {UserId} connected", userId);
                await Clients.Others.SendAsync("UserOnline", userId);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst("UserId")?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                lock (_connectionLock)
                {
                    _connections.Remove(userId);
                }

                _logger.LogInformation("User {UserId} disconnected", userId);
                await Clients.Others.SendAsync("UserOffline", userId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendTypingIndicator(string receiverId)
        {
            var senderId = Context.User?.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(senderId)) return;

            await Clients.User(receiverId).SendAsync("UserTyping", senderId);
        }

        public async Task StopTypingIndicator(string receiverId)
        {
            var senderId = Context.User?.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(senderId)) return;

            await Clients.User(receiverId).SendAsync("UserStoppedTyping", senderId);
        }
    }
}