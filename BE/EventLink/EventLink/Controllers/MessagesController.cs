using EventLink_Repositories.Models;
using Eventlink_Services.Interface;
using Eventlink_Services.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EventLink.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IHubContext<ChatHub> _hubContext;
        public MessagesController(IMessageService messageService, IHubContext<ChatHub> hubContext)
        {
            _messageService = messageService;
            _hubContext = hubContext;
        }

        [HttpGet("between-users")]
        public async Task<IActionResult> GetMessagesBetweenUsers(Guid receiverId)
        {
            var userId1Claim = User.FindFirst("UserId")?.Value;

            if (userId1Claim == null || !Guid.TryParse(userId1Claim, out Guid userId1))
            {
                return Unauthorized("Invalid or missing UserId claim.");
            }

            var messages = await _messageService.GetMessagesBetweenUsersAsync(userId1, receiverId);

            // Format ra dạng: "senderName: content"
            var formatted = messages.Select(m =>
            {
                var senderName = m.Sender?.FullName ?? m.SenderId.ToString();
                return $"{senderName}: {m.Content}";
            });

            // Ghép thành đoạn hội thoại
            var chatText = string.Join(Environment.NewLine, formatted);
            return Content(chatText, "text/plain");
        }

        [HttpGet("by-partnership/{partnershipId}")]
        public async Task<IActionResult> GetMessagesByPartnershipId(Guid partnershipId)
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;

            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out Guid userId))
            {
                return Unauthorized("Invalid or missing UserId claim.");
            }

            var messages = await _messageService.GetMessagesByPartnershipIdAsync(partnershipId);
            // Format ra dạng: "senderName: content"

            var formatted = messages.Select(m =>
            {
                var senderName = m.Sender?.FullName ?? m.SenderId.ToString();
                return $"{senderName}: {m.Content}";
            });

            var chatText = string.Join(Environment.NewLine, formatted);

            return Content(chatText, "text/plain");
        }

        [HttpPost("mark-as-read")]
        public async Task<IActionResult> MarkMessagesAsRead([FromBody] MessagesMarkAsReadRequest request)
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out Guid receiverId))
            {
                return Unauthorized("Invalid or missing UserId claim.");
            }

            await _messageService.MarkMessagesAsReadAsync(request.SenderId, receiverId);
            return NoContent();
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] MessageRequest request)
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out Guid senderId))
                return Unauthorized();

            var messageSend = new Message
            {
                Id = Guid.NewGuid(),
                SenderId = senderId,
                ReceiverId = request.ReceiverId ?? Guid.Empty,
                PartnershipId = request.PartnershipId,
                Content = request.Content,
                MessageType = "Text",
                AttachmentUrl = request.AttachmentUrl,
                AttachmentName = request.AttachmentName,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            var message = await _messageService.SendMessageAsync(messageSend);

            if (request.PartnershipId.HasValue)
            {
                var groupName = $"partnership:{request.PartnershipId.Value}";
                await _hubContext.Clients.Group(groupName)
                    .SendAsync("ReceiveMessage", message);
            }
            else if (request.ReceiverId != Guid.Empty)
            {
                var a = senderId.ToString();
                var b = request.ReceiverId.ToString();
                var ordered = string.CompareOrdinal(a, b) <= 0 ? (a, b) : (b, a);
                var groupName = $"private:{ordered.Item1}:{ordered.Item2}";

                await _hubContext.Clients.Group(groupName)
                    .SendAsync("ReceiveMessage", message);
            }

            return Ok(message);
        }
    }
}
