import api from './axios';

const partnershipService = {
  // GET /api/Partnerships - Get all partnerships
  getAllPartnerships: async () => {
    try {
      console.log('🚀 partnershipService.getAllPartnerships - START');
      const response = await api.get('/Partnerships');
      console.log('✅ Response received:', response.data);
      return response.data;
    } catch (error) {
      console.error('❌ partnershipService.getAllPartnerships - ERROR:', error);
      throw error;
    }
  },

  // POST /api/Partnerships - Create new partnership (supports FormData for file upload)
  createPartnership: async (partnershipData) => {
    try {
      console.log('🚀 partnershipService.createPartnership - START');
      console.log('📦 Sending data:', partnershipData);
      
      // Axios automatically sets correct Content-Type for FormData
      const response = await api.post('/Partnerships', partnershipData);
      
      console.log('✅ Response received:', response.data);
      return response.data;
    } catch (error) {
      console.error('❌ partnershipService.createPartnership - ERROR:', error);
      if (error.response?.data?.errors) {
        console.error('🔍 VALIDATION ERRORS:', JSON.stringify(error.response.data.errors, null, 2));
      }
      throw error;
    }
  },

  // PUT /api/Partnerships/{eventId}/toggle-status - Toggle partnership status
  updatePartnershipStatus: async (eventId) => {
    try {
      console.log(`🚀 partnershipService.updatePartnershipStatus - START (Event ID: ${eventId})`);
      const response = await api.put(`/Partnerships/${eventId}/toggle-status`);
      console.log('✅ Response received:', response.data);
      return response.data;
    } catch (error) {
      console.error(`❌ partnershipService.updatePartnershipStatus - ERROR (Event ID: ${eventId}):`, error);
      if (error.response?.data?.errors) {
        console.error('🔍 VALIDATION ERRORS:', JSON.stringify(error.response.data.errors, null, 2));
      }
      throw error;
    }
  },

  // PUT /api/Partnerships/partner/{partnerId}/toggle-status - Toggle partnership status
  updatePartnershipStatusByPartner: async (partnerId) => {
    try {
      console.log(`🚀 partnershipService.updatePartnershipStatusByPartner - START (Partner ID: ${partnerId})`);
      const response = await api.put(`/Partnerships/partner/${partnerId}/toggle-status`);
      console.log('✅ Response received:', response.data);
      return response.data;
    } catch (error) {
      console.error(`❌ partnershipService.updatePartnershipStatusByPartner - ERROR (Partner ID: ${partnerId}):`, error);
      if (error.response?.data?.errors) {
        console.error('🔍 VALIDATION ERRORS:', JSON.stringify(error.response.data.errors, null, 2));
      }
      throw error;
    }
  },

  // PUT /api/Partnerships/{id} - Update partnership
  updatePartnership: async (id, partnershipData) => {
    try {
      console.log(`🚀 partnershipService.updatePartnership - START (ID: ${id})`);
      console.log('📦 Sending data:', partnershipData);
      const response = await api.put(`/Partnerships/${id}`, partnershipData);
      console.log('✅ Response received:', response.data);
      return response.data;
    } catch (error) {
      console.error(`❌ partnershipService.updatePartnership - ERROR (ID: ${id}):`, error);
      if (error.response?.data?.errors) {
        console.error('🔍 VALIDATION ERRORS:', JSON.stringify(error.response.data.errors, null, 2));
      }
      throw error;
    }
  },

  // GET /api/Partnerships/{eventId}/partners - Get partners for an event
  getEventPartners: async (eventId) => {
    try {
      console.log(`🚀 partnershipService.getEventPartners - START (Event ID: ${eventId})`);
      const response = await api.get(`/Partnerships/${eventId}/partners`);
      console.log('✅ Response received:', response.data);
      return response.data;
    } catch (error) {
      console.error(`❌ partnershipService.getEventPartners - ERROR (Event ID: ${eventId}):`, error);
      throw error;
    }
  },
};

export default partnershipService;
