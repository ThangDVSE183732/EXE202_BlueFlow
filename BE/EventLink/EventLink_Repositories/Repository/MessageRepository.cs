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
        public MessageRepository(EventLinkDBContext context) : base(context)
        {
        }

        public async Task<List<Message>> GetConversationMessagesAsync(Guid userId, Guid partnerId, int page, int pageSize)
        {
            return await _context.Messages
                .Where(m => (m.SenderId == userId && m.ReceiverId == partnerId) ||
                           (m.SenderId == partnerId && m.ReceiverId == userId))
                .OrderByDescending(m => m.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .ToListAsync();
        }

        public async Task<List<Message>> GetPartnershipMessagesAsync(Guid partnershipId, int page, int pageSize)
        {
            return await _context.Messages
                .Where(m => m.PartnershipId == partnershipId)
                .OrderByDescending(m => m.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountAsync(Guid userId)
        {
            return await _context.Messages
                .Where(m => m.ReceiverId == userId && m.IsRead == false)
                .CountAsync();
        }

        public async Task<int> GetUnreadCountFromUserAsync(Guid userId, Guid senderId)
        {
            return await _context.Messages
                .Where(m => m.ReceiverId == userId && m.SenderId == senderId && m.IsRead == false)
                .CountAsync();
        }

        public async Task<List<Message>> GetUnreadMessagesAsync(Guid userId)
        {
            return await _context.Messages
                .Where(m => m.ReceiverId == userId && m.IsRead == false)
                .OrderByDescending(m => m.CreatedAt)
                .Include(m => m.Sender)
                .ToListAsync();
        }

        public async Task MarkAsReadAsync(Guid messageId)
        {
            var message = await _context.Messages.FindAsync(messageId);
            if (message != null && message.IsRead == false)
            {
                message.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkConversationAsReadAsync(Guid userId, Guid partnerId)
        {
            var unreadMessages = await _context.Messages
                .Where(m => m.ReceiverId == userId && m.SenderId == partnerId && m.IsRead == false)
                .ToListAsync();

            foreach (var message in unreadMessages)
            {
                message.IsRead = true;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<Guid>> GetUserConversationPartnersAsync(Guid userId)
        {
            var sentTo = await _context.Messages
                .Where(m => m.SenderId == userId)
                .Select(m => m.ReceiverId)
                .Distinct()
                .ToListAsync();

            var receivedFrom = await _context.Messages
                .Where(m => m.ReceiverId == userId)
                .Select(m => m.SenderId)
                .Distinct()
                .ToListAsync();

            return sentTo.Union(receivedFrom).Distinct().ToList();
        }

        public async Task<Message?> GetLastMessageWithUserAsync(Guid userId, Guid partnerId)
        {
            return await _context.Messages
                .Where(m => (m.SenderId == userId && m.ReceiverId == partnerId) ||
                           (m.SenderId == partnerId && m.ReceiverId == userId))
                .OrderByDescending(m => m.CreatedAt)
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> CanUserSendMessageAsync(Guid senderId, Guid receiverId)
        {
            // Check if users exist and are active
            var sender = await _context.Users.FindAsync(senderId);
            var receiver = await _context.Users.FindAsync(receiverId);

            return sender != null && receiver != null &&
                   sender.IsActive == true && receiver.IsActive == true;
        }
    }
}
