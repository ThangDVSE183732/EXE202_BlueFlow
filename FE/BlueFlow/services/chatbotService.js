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
  },

  // POST /api/ChatAI/match-partnerships - Match partnerships using AI
  matchPartnerships: async () => {
    try {
      // Backend không cần message nữa, lấy từ JWT token
      // Tăng timeout lên 120 giây vì Claude API có thể mất nhiều thời gian
      const response = await api.post('/ChatAI/match-partnerships', {}, {
        timeout: 120000 // 120 seconds for AI processing
      });
      return response.data;
    } catch (error) {
      console.error('Error matching partnerships:', error);
      
      // Extract detailed error message
      let errorMessage = 'Không thể tìm kiếm bằng AI. Vui lòng thử lại.';
      if (error.response?.data?.message) {
        errorMessage = error.response.data.message;
      } else if (error.response?.data?.error) {
        errorMessage = error.response.data.error;
      } else if (error.message) {
        errorMessage = error.message;
      }
      
      // Create error object with message for better handling
      const customError = new Error(errorMessage);
      customError.response = error.response;
      customError.status = error.response?.status;
      throw customError;
    }
  }
}