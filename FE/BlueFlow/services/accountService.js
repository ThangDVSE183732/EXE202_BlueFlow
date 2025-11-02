import api from './axios';

export const accountService = {
  // GET /api/UserProfiles/{id} - Lấy profile theo ID
  // getUserProfile: async (id) => {
  //   try {
  //     const response = await api.get(`/UserProfiles/${id}`);
  //     return {
  //       success: true,
  //       data: response.data,
  //       message: 'Profile retrieved successfully'
  //     };
  //   } catch (error) {
  //     console.error('Get user profile error:', error);
  //     return {
  //       success: false,
  //       message: error.response?.data?.message || 'Failed to get user profile',
  //       errors: error.response?.data?.errors || []
  //     };
  //   }
  // },

  // PUT /api/UserProfiles/{id} - Cập nhật profile theo ID
  updateUserProfile: async (id, profileData) => {
    try {
      // Axios automatically detects FormData and sets correct Content-Type with boundary
      const response = await api.put(`/UserProfiles/${id}`, profileData);
      
      return {
        success: true,
        data: response.data,
        message: 'Profile updated successfully'
      };
    } catch (error) {
      console.error('Update user profile error:', error);
      return {
        success: false,
        message: error.response?.data?.message || 'Failed to update user profile',
        errors: error.response?.data?.errors || []
      };
    }
  },

  // DELETE /api/UserProfiles/{id} - Xóa profile theo ID
  // deleteUserProfile: async (id) => {
  //   try {
  //     const response = await api.delete(`/UserProfiles/${id}`);
  //     return {
  //       success: true,
  //       data: response.data,
  //       message: 'Profile deleted successfully'
  //     };
  //   } catch (error) {
  //     console.error('Delete user profile error:', error);
  //     return {
  //       success: false,
  //       message: error.response?.data?.message || 'Failed to delete user profile',
  //       errors: error.response?.data?.errors || []
  //     };
  //   }
  // },

  // GET /api/UserProfiles/profile_by_userid/{userId} - Lấy profile theo userId
  getUserProfileByUserId: async (userId) => {
    try {
      const response = await api.get(`/UserProfiles/profile_by_userid/${userId}`);
      return {
        success: true,
        data: response.data,
        message: 'Profile retrieved successfully'
      };
    } catch (error) {
      console.error('Get user profile by userId error:', error);
      return {
        success: false,
        message: error.response?.data?.message || 'Failed to get user profile',
        errors: error.response?.data?.errors || []
      };
    }
  }
}
