import api from './axios';

export const brandService = {
  

  // POST /api/BrandProfiles - Tạo brand profile mới
  createBrandProfile: async (brandData) => {
    try {
      const response = await api.post('/BrandProfiles', brandData);
      return {
        success: true,
        data: response.data,
        message: 'Brand profile created successfully'
      };
    } catch (error) {
      console.error('Create brand profile error:', error);
      return {
        success: false,
        message: error.response?.data?.message || 'Failed to create brand profile',
        errors: error.response?.data?.errors || []
      };
    }
  },

  // GET /api/BrandProfiles/{id} - Lấy brand profile theo ID
  getBrandProfileById: async (id) => {
    try {
      const response = await api.get(`/BrandProfiles/${id}`);
      return {
        success: true,
        data: response.data,
        message: 'Brand profile retrieved successfully'
      };
    } catch (error) {
      console.error('Get brand profile by ID error:', error);
      return {
        success: false,
        message: error.response?.data?.message || 'Failed to get brand profile',
        errors: error.response?.data?.errors || []
      };
    }
  },

  // PUT /api/BrandProfiles/{id} - Cập nhật brand profile theo ID
  updateBrandProfile: async (id, brandData) => {
    try {
      const response = await api.put(`/BrandProfiles/${id}`, brandData);
      return {
        success: true,
        data: response.data,
        message: 'Brand profile updated successfully'
      };
    } catch (error) {
      console.error('Update brand profile error:', error);
      return {
        success: false,
        message: error.response?.data?.message || 'Failed to update brand profile',
        errors: error.response?.data?.errors || []
      };
    }
  },


  // GET /api/BrandProfiles/brand_profile_by_userid/{userId} - Lấy brand profile theo userId
  getBrandProfileByUserId: async (userId) => {
    try {
      const response = await api.get(`/BrandProfiles/brand_profile_by_userid/${userId}`);
      return {
        success: true,
        data: response.data,
        message: 'Brand profile retrieved successfully'
      };
    } catch (error) {
      // Kiểm tra nếu là lỗi 404, 500, hoặc NullReferenceException
      const isNotFoundError =  error.response?.data?.title?.includes('NullReferenceException') ||
                              error.message?.includes('NullReferenceException');
      if (isNotFoundError) {
        // Không log error, chỉ return để trigger tạo brand profile mới
        return {
          success: false,
          data: null,
          message: 'Brand profile not found'
        };
      }
      
      // Các lỗi khác thì vẫn log
      console.error('Get brand profile by userId error:', error);
      return {
        success: false,
        message: error.response?.data?.message || 'Failed to get brand profile',
        errors: error.response?.data?.errors || []
      };
    }
  }
}