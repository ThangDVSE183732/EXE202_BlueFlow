using EventLink_Repositories.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Net.Http.Json;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        var connection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7029/chathub")
            .Build();

        connection.On<string, string, DateTime>("ReceiveMessage", (username, message, time) =>
        {
            Console.WriteLine($"[{time:HH:mm:ss}] {username}: {message}");
        });

        await connection.StartAsync();
        Console.WriteLine("✅ Connected to SignalR Hub.");

        Console.Write("Nhập partnershipId để tham gia: ");
        var partnershipId = Guid.Parse(Console.ReadLine());

        // 🔹 Gọi API để lấy tin nhắn cũ
        using var http = new HttpClient();
        var oldMessages = await http.GetFromJsonAsync<List<Message>>(
            $"https://localhost:7029/api/messages/by-partnership/{partnershipId}"
        );

        Console.WriteLine("💬 Tin nhắn trước đó:");
        foreach (var m in oldMessages)
        {
            Console.WriteLine($"[{m.CreatedAt:HH:mm}] {m.Sender.FullName}: {m.Content}");
        }

        await connection.InvokeAsync("JoinRoom", partnershipId);
        Console.WriteLine($"📦 Đã vào phòng chat {partnershipId}");

        Console.Write("Tên của bạn: ");
        var username = Console.ReadLine();

        while (true)
        {
            var msg = Console.ReadLine();

            // 🔹 Gọi API để lưu vào DB
            var saveMessage = new
            {
                PartnershipId = partnershipId,
                Content = msg
            };
            var json = JsonContent.Create(saveMessage);
            await http.PostAsync("https://localhost:7029/api/messages/send", json);

            // 🔹 Gửi realtime
            await connection.InvokeAsync("SendMessage", partnershipId, username, msg);
        }

    }
}
