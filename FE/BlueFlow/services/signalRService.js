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
      
      // Tạo connection với authentication
      this.connection = new signalR.HubConnectionBuilder()
        .withUrl(`${baseUrl}${hubUrl}`, {
          accessTokenFactory: () => token || '',
          transport: signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.LongPolling
        })
        .withAutomaticReconnect({
          nextRetryDelayInMilliseconds: () => {
            // Retry sau 0, 2, 10, 30 giây
            return [0, 2000, 10000, 30000][Math.min(this.connection?.reconnectAttempts || 0, 3)];
          }
        })
        .configureLogging(signalR.LogLevel.Information)
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
      console.log('SignalR Connected successfully');
      
      return this.connection;
    } catch (error) {
      console.error('SignalR Connection Error:', error);
      this.isConnected = false;
      throw error;
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
  onReceiveMessage(callback) {
    if (this.connection) {
      this.connection.on('receiveMessage', (message) => {
        console.log('📨 ReceiveMessage event:', message);
        callback(message);
      });
    }
  }

  // ✅ Backend broadcast ConversationUpdated
  onConversationUpdated(callback) {
    if (this.connection) {
      this.connection.on('conversationUpdated', (senderId) => {
        console.log('🔄 ConversationUpdated event:', senderId);
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
        console.log('👀 User typing:', senderId);
        callback(senderId);
      });
    }
  }

  // Lắng nghe event user stopped typing
  onUserStoppedTyping(callback) {
    if (this.connection) {
      this.connection.on('userStoppedTyping', (senderId) => {
        console.log('✋ User stopped typing:', senderId);
        callback(senderId);
      });
    }
  }

  // Lắng nghe user online
  onUserOnline(callback) {
    if (this.connection) {
      this.connection.on('userOnline', (userId) => {
        console.log('🟢 User online:', userId);
        callback(userId);
      });
    }
  }

  // Lắng nghe user offline
  onUserOffline(callback) {
    if (this.connection) {
      this.connection.on('userOffline', (userId) => {
        console.log('🔴 User offline:', userId);
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
