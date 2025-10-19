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
        Task<List<Message>> GetMessagesBetweenUsersAsync(Guid userId1, Guid userId2);
        Task<List<Message>> GetMessagesByPartnershipIdAsync(Guid partnershipId);
        Task MarkMessagesAsReadAsync(Guid senderId, Guid receiverId);
        Task<Message> SendMessageAsync(Message message);
    }
}
