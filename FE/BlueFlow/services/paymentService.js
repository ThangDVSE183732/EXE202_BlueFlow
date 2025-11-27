import apiClient from './axios';

const paymentService = {
  // POST /api/Payment/create-premium-payment
  // Tạo thanh toán premium
  createPremiumPayment: async (paymentData) => {
    try {
      const response = await apiClient.post('/Payment/create-premium-payment', paymentData);
      return response.data;
    } catch (error) {
      throw error.response?.data || error.message;
    }
  },

  // POST /api/Payment/webhook
  // Webhook để nhận thông báo từ payment gateway
  handleWebhook: async (webhookData) => {
    try {
      const response = await apiClient.post('/Payment/webhook', webhookData);
      return response.data;
    } catch (error) {
      throw error.response?.data || error.message;
    }
  },

  // GET /api/Payment/history
  // Lấy lịch sử thanh toán
  getPaymentHistory: async () => {
    try {
      const response = await apiClient.get('/Payment/history');
      return response.data;
    } catch (error) {
      throw error.response?.data || error.message;
    }
  },

  // POST /api/Payment/cancel
  // Hủy thanh toán
  cancelPayment: async (paymentId) => {
    try {
      const response = await apiClient.post('/Payment/cancel', { paymentId });
      return response.data;
    } catch (error) {
      throw error.response?.data || error.message;
    }
  },

  // GET /api/Payment/{id}
  // Lấy thông tin chi tiết thanh toán theo ID
  getPaymentById: async (id) => {
    try {
      const response = await apiClient.get(`/Payment/${id}`);
      return response.data;
    } catch (error) {
      throw error.response?.data || error.message;
    }
  },

  // GET /api/Payment/subscription-plans
  // Lấy danh sách các gói subscription
  getSubscriptionPlans: async () => {
    try {
      const response = await apiClient.get('/Payment/subscription-plans');
      return response.data;
    } catch (error) {
      throw error.response?.data || error.message;
    }
  },

  // GET /api/Payment/verify
  // Xác minh thanh toán
  verifyPayment: async (params) => {
    try {
      const response = await apiClient.get('/Payment/verify', { params });
      return response.data;
    } catch (error) {
      throw error.response?.data || error.message;
    }
  },

  // GET /api/Payment/premium-status
  // Kiểm tra trạng thái premium của user
  getPremiumStatus: async () => {
    try {
      const response = await apiClient.get('/Payment/premium-status');
      return response.data;
    } catch (error) {
      throw error.response?.data || error.message;
    }
  },

  // GET /api/Payment/check-premium
  // Kiểm tra xem user có premium không
  checkPremium: async () => {
    try {
      const response = await apiClient.get('/Payment/check-premium');
      return response.data;
    } catch (error) {
      throw error.response?.data || error.message;
    }
  },
};

export default paymentService;