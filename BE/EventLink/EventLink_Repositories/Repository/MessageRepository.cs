using EventLink_Repositories.DBContext;
using EventLink_Repositories.Interface;
using EventLink_Repositories.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventLink_Repositories.Repository
{
    public class MessageRepository : GenericRepository<Message>, IMessageRepository
    {
        private readonly EventLinkDBContext _context;
        public MessageRepository(EventLinkDBContext context) : base(context)
        {
            _context = context;
        }
        public async Task<List<Message>> GetMessagesBetweenUsersAsync(Guid userId1, Guid userId2)
        {
            var messages = await _context.Messages
                .Where(m => (m.SenderId == userId1 && m.ReceiverId == userId2) ||
                            (m.SenderId == userId2 && m.ReceiverId == userId1))
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();
            return messages;
        }

        public async Task<List<Message>> GetMessagesByPartnershipIdAsync(Guid partnershipId)
        {
            return await _context.Messages
                .Where(m => m.PartnershipId == partnershipId)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task MarkMessagesAsReadAsync(Guid senderId, Guid receiverId)
        {
            var messages = _context.Messages
                .Where(m => m.SenderId == senderId && m.ReceiverId == receiverId && !m.IsRead)
                .ToList();

            foreach (var message in messages)
            {
                message.IsRead = true;
            }
            await _context.SaveChangesAsync();
        }

        public async Task<Message> SendMessageAsync(Message message)
        {
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();
            return message;
        }
    }
}
