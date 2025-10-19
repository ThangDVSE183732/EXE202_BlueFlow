using EventLink_Repositories.Models;
using Eventlink_Services.Interface;
using Eventlink_Services.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
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
        public async Task<IActionResult> GetMessagesBetweenUsers([FromBody] MessageRequest request)
        {
            var userId1Claim = User.FindFirst("UserId")?.Value;

            var userId2 = request.ReceiverId;

            if (userId1Claim == null || !Guid.TryParse(userId1Claim, out Guid userId1))
            {
                return Unauthorized("Invalid or missing UserId claim.");
            }

            var messages = await _messageService.GetMessagesBetweenUsersAsync(userId1, userId2);
            return Ok(messages);
        }

        [HttpGet("by-partnership/{partnershipId}")]
        public async Task<IActionResult> GetMessagesByPartnershipId(Guid partnershipId)
        {
            var messages = await _messageService.GetMessagesByPartnershipIdAsync(partnershipId);
            return Ok(messages);
        }

        [HttpPost("mark-as-read")]
        public async Task<IActionResult> MarkMessagesAsRead([FromBody] MessageRequest request)
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out Guid receiverId))
            {
                return Unauthorized("Invalid or missing UserId claim.");
            }
            var senderId = request.ReceiverId;
            await _messageService.MarkMessagesAsReadAsync(senderId, receiverId);
            return NoContent();
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] MessageRequest request)
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out Guid senderId))
            {
                return Unauthorized("Invalid or missing UserId claim.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var messageSend = new Message
            {
                Id = Guid.NewGuid(),
                SenderId = senderId,
                ReceiverId = request.ReceiverId,
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
                await _hubContext.Clients.Group(request.PartnershipId.ToString())
                    .SendAsync("ReceiveMessage", message);
            }
            else
            {
                await _hubContext.Clients.Group(request.ReceiverId.ToString())
                    .SendAsync("ReceiveMessage", senderId, message);
            }

            Console.WriteLine($"📡 SignalR: Sending message from {senderId} to {request.ReceiverId}, partnershipId = {request.PartnershipId}");

            return Ok(message);
        }
    }
}
