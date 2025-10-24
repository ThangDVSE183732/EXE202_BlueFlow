import api from './axios';

export const authService = {
  // Đăng nhập
  login: async (credentials) => {
    try {
      const response = await api.post('/Auth/login', credentials);
      const { success, message, data, errors } = response.data;
      // Lưu token vào localStorage
      if (success && data?.token) {
        localStorage.setItem('accessToken', data.token);
      }
      if (success && data?.refreshToken) {
        localStorage.setItem('refreshToken', data.refreshToken);
      }
      return { success, message, data, errors };
    } catch (error) {
      throw error.response?.data || error.message;
    }
  },

  // Đăng nhập bằng Google
  loginGoogle: async (googleToken) => {
    try {
      const response = await api.post('/Auth/google', {
        token: googleToken,
        idToken: googleToken
      });
      const { success, message, data, errors } = response.data;
      
      // Lưu token vào localStorage nếu đăng nhập thành công
      if (success && data?.token) {
        localStorage.setItem('accessToken', data.token);
      }
      if (success && data?.refreshToken) {
        localStorage.setItem('refreshToken', data.refreshToken);
      }
      
      return { success, message, data, errors };
    } catch (error) {
      throw error.response?.data || error.message;
    }
  },

  verifyOTP: async (otpData) => {
    try {
    const response = await api.post('/Auth/verify-otp-login', otpData);
    const { success, message, data, errors } = response.data;

    // Lưu token vào localStorage
      if (success && data?.token) {
        localStorage.setItem('accessToken', data.token);
      }
      if (success && data?.refreshToken) {
        localStorage.setItem('refreshToken', data.refreshToken);
      }
      return { success, message, data, errors };
    } catch (error) {
      throw error.response?.data || error.message;
    }

  },

  verifyOTPRegister: async (otpData) => {
    try {
    const response = await api.post('/Auth/verify-otp-register', otpData);
    const { success, message, data, errors } = response.data;

    // Lưu token vào localStorage
      if (success && data?.token) {
        localStorage.setItem('accessToken', data.token);
      }
      if (success && data?.refreshToken) {
        localStorage.setItem('refreshToken', data.refreshToken);
      }
      return { success, message, data, errors };
    } catch (error) {
      throw error.response?.data || error.message;
    }

  },

  // Đăng ký
  register: async (userData) => {
    try {
      const response = await api.post('/Auth/register', userData);
      const { success, message, data, errors } = response.data;
      
      return { success, message, data, errors };
    } catch (error) {
      throw error.response?.data || error.message;
    }
  },

  // Đăng xuất
  logout: async () => {
    try {
      await api.post('/Auth/logout');
    } catch (error) {
      console.error('Logout error:', error);
    } finally {
      localStorage.removeItem('accessToken');
      localStorage.removeItem('refreshToken');
    }
  },

  // Kiểm tra token còn hợp lệ không
  verifyToken: async () => {
    try {
      const response = await api.get('/auth/verify');
      return response.data;
    } catch (error) {
      throw error.response?.data || error.message;
    }
  }
};