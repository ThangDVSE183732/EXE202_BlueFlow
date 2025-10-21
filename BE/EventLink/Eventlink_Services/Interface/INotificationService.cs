using Eventlink_Services.Response;

namespace EventLink_Services.Services.Interfaces
{
    public interface INotificationService
    {
        Task SendMessageNotificationAsync(Guid receiverId, MessageDto message);
        Task SendConversationUpdateAsync(Guid receiverId, Guid senderId);
        Task SendMessageReadNotificationAsync(Guid senderId, Guid messageId);
        Task SendConversationReadNotificationAsync(Guid partnerId, Guid userId);
    }
}