//using Eventlink_Services.Interface;
//using Microsoft.AspNetCore.SignalR;
//using System;
//using System.Linq;
//using System.Threading.Tasks;

//namespace EventLink.SignalR
//{
//    public class ChatHub : Hub
//    {   
//        private readonly IMessageService _messageService;
//        public ChatHub(IMessageService messageService)
//        {
//            _messageService = messageService;
//        }
//        public async Task JoinRoom(Guid partnershipId)
//        {
//            await Groups.AddToGroupAsync(Context.ConnectionId, partnershipId.ToString());
//            Console.WriteLine($"{Context.ConnectionId} joined room {partnershipId}");
//        }

//        public async Task JoinPrivate(Guid receiverId)
//        {
//            var userId = Context.User?.FindFirst("UserId")?.Value;
//            if (userId == null) return;

//            var ordered = GetOrderedPair(userId, receiverId.ToString());
//            var groupName = $"private:{ordered.Item1}:{ordered.Item2}";

//            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
//            Console.WriteLine($"{Context.ConnectionId} joined private room {groupName}");

//            if (Guid.TryParse(userId, out Guid currentUserId))
//            {
//                var history = await _messageService.GetMessagesBetweenUsersAsync(currentUserId, receiverId);

//                var formattedString = string.Join("\n", history.Select(m =>
//                {
//                    var sender = m.Sender?.FullName ?? m.SenderId.ToString();
//                    return $"{sender}: {m.Content}";
//                }));
//                await Clients.Caller.SendAsync("LoadHistory", formattedString);
//            }
//        }

//        private static (string, string) GetOrderedPair(string a, string b)
//            => string.CompareOrdinal(a, b) <= 0 ? (a, b) : (b, a);

//        public async Task SendMessage(Guid partnershipId, string username, string message)
//        {
//            await Clients.Group(partnershipId.ToString())
//                .SendAsync("ReceiveMessage", username, message, DateTime.UtcNow);
//        }

//        public override async Task OnConnectedAsync()
//        {
//            Console.WriteLine($"User connected: {Context.ConnectionId}");
//            await base.OnConnectedAsync();
//        }

//        public override async Task OnDisconnectedAsync(Exception exception)
//        {
//            Console.WriteLine($"User disconnected: {Context.ConnectionId}");
//            await base.OnDisconnectedAsync(exception);
//        }
//    }
//}
