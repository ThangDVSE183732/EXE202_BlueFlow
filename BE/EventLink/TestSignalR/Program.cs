using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http.Headers;
using System.Text.Json;

Console.WriteLine("Starting authorized SignalR client...");

var hubUrl = "https://localhost:7029/chatHub";
var apiBase = "https://localhost:7029/api/messages";

Console.Write("Enter JWT: ");
var token = Console.ReadLine();

using var http = new HttpClient();
http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

var connection = new HubConnectionBuilder()
    .WithUrl(hubUrl, o => o.AccessTokenProvider = () => Task.FromResult(token))
    .WithAutomaticReconnect()
    .Build();

connection.On<object>("ReceiveMessage", m => Console.WriteLine("New: " + JsonSerializer.Serialize(m)));
connection.On<object>("LoadHistory", h => {
    Console.WriteLine("History:");
    Console.WriteLine(JsonSerializer.Serialize(h, new JsonSerializerOptions { WriteIndented = true }));
});

await connection.StartAsync();
Console.WriteLine("Connected.");

// Chọn mode
Console.Write("Mode (1 = Partnership, 2 = Private): ");
var mode = Console.ReadLine();

Guid? partnershipId = null;
Guid? receiverId = null;

if (mode == "1")
{
    Console.Write("Enter PartnershipId: ");
    partnershipId = Guid.Parse(Console.ReadLine());

    // Join và load lịch sử partnership
    await connection.InvokeAsync("JoinRoom", partnershipId.Value);

    var resp = await http.GetAsync($"{apiBase}/by-partnership/{partnershipId}");
    Console.WriteLine(await resp.Content.ReadAsStringAsync());
}
else
{
    Console.Write("Enter ReceiverId (user GUID): ");
    receiverId = Guid.Parse(Console.ReadLine());

    // Join và load lịch sử private
    await connection.InvokeAsync("JoinPrivate", receiverId.Value);

    var resp = await http.GetAsync($"{apiBase}/between-users?receiverId={receiverId}");
    Console.WriteLine(await resp.Content.ReadAsStringAsync());
}

// Gửi tin
Console.WriteLine("Type messages (or 'exit' to quit):");
string? input;
while ((input = Console.ReadLine()) != "exit")
{
    if (string.IsNullOrWhiteSpace(input)) continue;

    var sendReq = new
    {
        ReceiverId = receiverId,
        PartnershipId = partnershipId,
        Content = input
    };

    var json = JsonSerializer.Serialize(sendReq);
    var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

    var res = await http.PostAsync($"{apiBase}/send", content);
    Console.WriteLine(res.IsSuccessStatusCode ? "Sent!" : $"Failed: {res.StatusCode}");
}

await connection.StopAsync();
Console.WriteLine("Disconnected.");
