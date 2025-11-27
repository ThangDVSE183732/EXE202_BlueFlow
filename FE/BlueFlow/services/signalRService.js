import * as signalR from '@microsoft/signalr';

class SignalRService {
  constructor() {
    this.connection = null;
    this.isConnected = false;
  }

  // Khởi tạo connection
  async startConnection(hubUrl = '/chatHub') {
    try {
      const token = localStorage.getItem('accessToken');
      const baseUrl = 'https://localhost:7029';
      
      console.log('🔌 Starting SignalR connection to:', `${baseUrl}${hubUrl}`);
      console.log('🔑 Using token:', token ? 'Present' : 'Missing');
      
      // Tạo connection với authentication
      this.connection = new signalR.HubConnectionBuilder()
        .withUrl(`${baseUrl}${hubUrl}`, {
          accessTokenFactory: () => {
            const currentToken = localStorage.getItem('accessToken');
            console.log('🔐 Token factory called, token:', currentToken ? 'Present' : 'Missing');
            return currentToken || '';
          },
          skipNegotiation: true, // Skip negotiation and use WebSockets directly
          transport: signalR.HttpTransportType.WebSockets
        })
        .withAutomaticReconnect([0, 2000, 10000, 30000])
        .configureLogging(signalR.LogLevel.Debug) // Changed to Debug for more details
        .build();

      // Event handlers
      this.connection.onreconnecting((error) => {
        console.log('SignalR reconnecting...', error);
        this.isConnected = false;
      });

      this.connection.onreconnected((connectionId) => {
        console.log('SignalR reconnected:', connectionId);
        this.isConnected = true;
      });

      this.connection.onclose((error) => {
        console.log('SignalR connection closed', error);
        this.isConnected = false;
      });

      // Start connection
      await this.connection.start();
      this.isConnected = true;
      console.log('✅ SignalR Connected successfully, connectionId:', this.connection.connectionId);
      
      return this.connection;
    } catch (error) {
      console.error('❌ SignalR Connection Error:', error);
      console.error('Error details:', {
        message: error.message,
        statusCode: error.statusCode,
        innerError: error.innerErrors
      });
      this.isConnected = false;
      
      // Don't throw - allow app to continue without SignalR
      return null;
    }
  }

  // Stop connection
  async stopConnection() {
    if (this.connection) {
      try {
        await this.connection.stop();
        this.isConnected = false;
        console.log('SignalR Disconnected');
      } catch (error) {
        console.error('Error stopping SignalR connection:', error);
      }
    }
  }

  // ✅ Backend broadcast ReceiveMessage qua SignalRNotificationService
  // Backend gửi "ReceiveMessage" nhưng SignalR auto-convert sang "receiveMessage"
  onReceiveMessage(callback) {
    if (this.connection) {
      // SignalR .NET Core auto-converts PascalCase to camelCase
      this.connection.on('receiveMessage', (message) => {
        console.log('📨 receiveMessage event:', message);
        callback(message);
      });
    }
  }

  // ✅ Backend broadcast ConversationUpdated
  // Backend gửi "ConversationUpdated" nhưng SignalR auto-convert sang "conversationUpdated"
  onConversationUpdated(callback) {
    if (this.connection) {
      // SignalR .NET Core auto-converts PascalCase to camelCase
      this.connection.on('conversationUpdated', (senderId) => {
        console.log('🔄 conversationUpdated event:', senderId);
        callback(senderId);
      });
    }
  }

  // ✅ Backend broadcast MessageMarkedAsRead
  onMessageMarkedAsRead(callback) {
    if (this.connection) {
      this.connection.on('messageMarkedAsRead', (messageId) => {
        console.log('✅ MessageMarkedAsRead event:', messageId);
        callback(messageId);
      });
    }
  }

  // ✅ Backend broadcast ConversationMarkedAsRead
  onConversationMarkedAsRead(callback) {
    if (this.connection) {
      this.connection.on('conversationMarkedAsRead', (userId) => {
        console.log('✅ ConversationMarkedAsRead event:', userId);
        callback(userId);
      });
    }
  }
  
  // Lắng nghe event typing - SignalR converts to camelCase
  onUserTyping(callback) {
    if (this.connection) {
      this.connection.on('userTyping', (senderId) => {
        console.log('👀 userTyping:', senderId);
        callback(senderId);
      });
    }
  }

  // Lắng nghe event user stopped typing
  onUserStoppedTyping(callback) {
    if (this.connection) {
      this.connection.on('userStoppedTyping', (senderId) => {
        console.log('✋ userStoppedTyping:', senderId);
        callback(senderId);
      });
    }
  }

  // Lắng nghe user online
  onUserOnline(callback) {
    if (this.connection) {
      this.connection.on('userOnline', (userId) => {
        console.log('🟢 userOnline:', userId);
        callback(userId);
      });
    }
  }

  // Lắng nghe user offline
  onUserOffline(callback) {
    if (this.connection) {
      this.connection.on('userOffline', (userId) => {
        console.log('🔴 userOffline:', userId);
        callback(userId);
      });
    }
  }

  // Gửi typing indicator (✅ Backend có method này)
  async sendTypingIndicator(receiverId) {
    if (this.connection && this.isConnected) {
      try {
        await this.connection.invoke('SendTypingIndicator', receiverId);
        console.log('📤 Typing indicator sent to:', receiverId);
      } catch (error) {
        console.error('❌ Error sending typing indicator:', error);
      }
    }
  }

  // Gửi stop typing indicator (✅ Backend có method này)
  async stopTypingIndicator(receiverId) {
    if (this.connection && this.isConnected) {
      try {
        await this.connection.invoke('StopTypingIndicator', receiverId);
        console.log('🛑 Stop typing indicator sent to:', receiverId);
      } catch (error) {
        console.error('❌ Error sending stop typing indicator:', error);
      }
    }
  }

  // ⚠️ Backend KHÔNG có các method này - chỉ dùng REST API
  // SendMessage → dùng REST API
  // JoinConversation → không cần (backend tự quản lý connection)
  // LeaveConversation → không cần
  // MarkConversationAsRead → dùng REST API

  // Remove event listener
  off(eventName) {
    if (this.connection) {
      this.connection.off(eventName);
    }
  }

  // Get connection state
  getConnectionState() {
    return this.connection?.state || signalR.HubConnectionState.Disconnected;
  }

  // Check if connected
  isConnectionActive() {
    return this.isConnected && this.connection?.state === signalR.HubConnectionState.Connected;
  }
}

// Export singleton instance
export const signalRService = new SignalRService();
export default signalRService;
