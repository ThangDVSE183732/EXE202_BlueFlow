using EventLink_Repositories.Interface;
using EventLink_Repositories.Models;
using Eventlink_Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventlink_Services.Service
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPartnershipRepository _partnershipRepository;
        public MessageService(IMessageRepository messageRepository, IUserRepository userRepository, IPartnershipRepository partnershipRepository)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            _partnershipRepository = partnershipRepository;
        }

        public async Task<List<Message>> GetMessagesBetweenUsersAsync(Guid userId1, Guid userId2)
        {
            var user1 = _userRepository.GetActiveUserByIdAsync(userId1);
            var user2 = _userRepository.GetActiveUserByIdAsync(userId2);

            if(user1 == null || user2 == null)
            {
                throw new ArgumentException("One or both users do not exist or are inactive.");
            }

            return await _messageRepository.GetMessagesBetweenUsersAsync(userId1, userId2);
        }

        public async Task<List<Message>> GetMessagesByPartnershipIdAsync(Guid partnershipId)
        {
            return await _messageRepository.GetMessagesByPartnershipIdAsync(partnershipId);
        }

        public async Task MarkMessagesAsReadAsync(Guid senderId, Guid receiverId)
        {
            await _messageRepository.MarkMessagesAsReadAsync(senderId, receiverId);
        }

        public async Task<Message> SendMessageAsync(Message message)
        {
            return await _messageRepository.SendMessageAsync(message);
        }
    }
}
