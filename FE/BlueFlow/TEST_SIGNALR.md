# SignalR Connection Issues

## Problem
SignalR connection fails with error: "Could not establish connection. Receiving end does not exist."

## Possible Causes
1. Backend ChatHub not running on `https://localhost:7029/chatHub`
2. CORS not configured properly in backend
3. SSL certificate issues
4. Hub path mismatch (backend might use `/chathub` lowercase)

## Backend Code Analysis
Backend is sending events with **PascalCase**:
- `ReceiveMessage`
- `ConversationUpdated`
- `MessageMarkedAsRead`
- `ConversationMarkedAsRead`

SignalR .NET Core automatically converts to **camelCase** for JavaScript clients:
- `receiveMessage`
- `conversationUpdated`
- `messageMarkedAsRead`
- `conversationMarkedAsRead`

## Temporary Solution
Until SignalR is fixed, we can use **polling** to check for new messages:

```javascript
// Poll every 5 seconds for unread count
useEffect(() => {
  const pollInterval = setInterval(() => {
    messageService.getPartnerListChat().then(response => {
      if (response.success && response.data) {
        const formattedChats = response.data.map((chat) => ({
          // ... format chat
          unreadCount: chat.unreadCount || 0,
        }));
        setAllChats(formattedChats);
      }
    });
  }, 5000); // Poll every 5 seconds
  
  return () => clearInterval(pollInterval);
}, []);
```

## Backend Requirements
Make sure backend has:

1. **Startup.cs / Program.cs** - CORS configuration:
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Important for SignalR
    });
});

app.UseCors("AllowFrontend");
app.MapHub<ChatHub>("/chathub"); // Check exact path
```

2. **ChatHub.cs** - Override OnConnectedAsync:
```csharp
public override async Task OnConnectedAsync()
{
    var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    _logger.LogInformation($"User {userId} connected with connectionId: {Context.ConnectionId}");
    await base.OnConnectedAsync();
}
```
