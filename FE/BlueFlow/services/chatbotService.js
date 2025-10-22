import api from './axios';

export const chatbotService = {

  // POST /api/ChatAI/ask - Send a prompt to GrokAI
  sendPrompt: async (messageData) => {
    try {
      const response = await api.post('/ChatAI/ask', messageData);
      return response.data;
    } catch (error) {
      console.error('Error sending message:', error);
      throw error;
    }
  }
}