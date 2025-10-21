using EventLink.Hubs;
using Eventlink_Services.Interface;
using Eventlink_Services.Response;
using EventLink_Services.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace EventLink.Hubs
{
    public class SignalRNotificationService : INotificationService
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly ILogger<SignalRNotificationService> _logger;

        public SignalRNotificationService(
            IHubContext<ChatHub> hubContext,
            ILogger<SignalRNotificationService> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task SendMessageNotificationAsync(Guid receiverId, MessageDto message)
        {
            try
            {
                _logger.LogInformation("🔥 Sending real-time message to {ReceiverId}", receiverId);

                await _hubContext.Clients.User(receiverId.ToString())
                    .SendAsync("ReceiveMessage", message);

                _logger.LogInformation("✅ Message sent to {ReceiverId}", receiverId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send message notification");
            }
        }

        public async Task SendConversationUpdateAsync(Guid receiverId, Guid senderId)
        {
            try
            {
                await _hubContext.Clients.User(receiverId.ToString())
                    .SendAsync("ConversationUpdated", senderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send conversation update");
            }
        }

        public async Task SendMessageReadNotificationAsync(Guid senderId, Guid messageId)
        {
            try
            {
                await _hubContext.Clients.User(senderId.ToString())
                    .SendAsync("MessageMarkedAsRead", messageId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send read notification");
            }
        }

        public async Task SendConversationReadNotificationAsync(Guid partnerId, Guid userId)
        {
            try
            {
                await _hubContext.Clients.User(partnerId.ToString())
                    .SendAsync("ConversationMarkedAsRead", userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send conversation read notification");
            }
        }
    }
}