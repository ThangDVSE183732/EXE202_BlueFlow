import api from './axios';

export const messageService = {
  // POST /api/Message/send - Send a new message
  sendMessage: async (messageData) => {
    try {
      const response = await api.post('/Message/send', messageData);
      return response.data;
    } catch (error) {
      console.error('Error sending message:', error);
      throw error;
    }
  },

  // GET /api/Message/conversation/{partnerId} - Get conversation with a specific partner
  getConversation: async (partnerId) => {
    try {
      const response = await api.get(`/Message/conversation/${partnerId}`);
      return response.data;
    } catch (error) {
      console.error('Error getting conversation:', error);
      throw error;
    }
  },

  // GET /api/Message/conversation - Get partner list chats
  getPartnerListChat: async () => {
    try {
      const response = await api.get(`/Message/conversations`);
      return response.data;
    } catch (error) {
      console.error('Error getting partner list chats:', error);
      throw error;
    }
  },


  // PUT /api/Message/conversation/{partnerId}/read - Mark all messages in a conversation as read
  markConversationAsRead: async (partnerId) => {
    try {
      const response = await api.put(`/Message/conversation/${partnerId}/read`);
      return response.data;
    } catch (error) {
      console.error('Error marking conversation as read:', error);
      throw error;
    }
  }
}
