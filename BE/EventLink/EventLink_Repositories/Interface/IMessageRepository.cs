using EventLink_Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventLink_Repositories.Interface
{
    public interface IMessageRepository : IGenericRepository<Message>
    {
        Task<List<Message>> GetConversationMessagesAsync(Guid userId, Guid partnerId, int page, int pageSize);
        Task<List<Message>> GetPartnershipMessagesAsync(Guid partnershipId, int page, int pageSize);
        Task<int> GetUnreadCountAsync(Guid userId);
        Task<int> GetUnreadCountFromUserAsync(Guid userId, Guid senderId);
        Task<List<Message>> GetUnreadMessagesAsync(Guid userId);
        Task MarkAsReadAsync(Guid messageId);
        Task MarkConversationAsReadAsync(Guid userId, Guid partnerId);
        Task<List<Guid>> GetUserConversationPartnersAsync(Guid userId);
        Task<Message?> GetLastMessageWithUserAsync(Guid userId, Guid partnerId);
        Task<bool> CanUserSendMessageAsync(Guid senderId, Guid receiverId);
    }
}
