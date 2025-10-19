using Eventlink_Services.Service;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace EventLink.SignalR
{
    public class AIChatHub : Hub
    {
        private readonly OpenAIService _openAIService;
        public AIChatHub(OpenAIService openAIService)
        {
            _openAIService = openAIService;
        }
        public async Task SendMessage(string message)
        {
            // Gửi tin nhắn user về client (hiển thị ngay)
            await Clients.Caller.SendAsync("ReceiveMessage", "Bạn", message);

            // Tạo prompt cho AI (có thể thêm lịch sử chat nếu muốn context)
            var prompt = $"Người dùng: {message}";

            // Gọi AI service
            var aiResponse = await _openAIService.GetAIResponse(prompt);

            // Gửi phản hồi AI về client
            await Clients.Caller.SendAsync("ReceiveMessage", "AI", aiResponse);
        }
    }
}
