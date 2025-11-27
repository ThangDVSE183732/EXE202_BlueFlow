import api from './axios';

const eventService = {
  // POST /api/Events - Create new event (supports FormData for file upload)
  createEvent: async (eventData) => {
    try {
      console.log('🚀 eventService.createEvent - START');
      console.log('📍 API URL:', api.defaults.baseURL + '/Events');
      console.log('📦 Sending data:', eventData);
      
      // Axios automatically sets correct Content-Type for FormData
      const response = await api.post('/Events', eventData);
      
      console.log('✅ Response received:', response.data);
      
      // Backend returns: { success: true, message: "...", data: { id, title, ... } }
      // Return it directly without wrapping
      return response.data;
    } catch (error) {
      console.error('❌ eventService.createEvent - ERROR');
      console.error('Error details:', error);
      console.error('Response data:', error.response?.data);
      console.error('Response status:', error.response?.status);
      console.error('Response headers:', error.response?.headers);
      
      // Log validation errors if present
      if (error.response?.data?.errors) {
        console.error('🔍 VALIDATION ERRORS:', JSON.stringify(error.response.data.errors, null, 2));
      }
      
      return {
        success: false,
        message: error.response?.data?.title || error.response?.data?.message || 'Failed to create event',
        errorMessages: error.response?.data?.errors || error.response?.data?.errorMessages || [],
        error: error.response?.data || error.message
      };
    }
  },

  // GET /api/Events/{id} - Get event by ID
  getEventById: async (eventId) => {
    try {
      const response = await api.get(`/Events/${eventId}/UserId`);
      // Backend returns: { success: true, message: "...", data: [...] }
      // Return it directly without wrapping
      return response.data;
    } catch (error) {
      console.error('Get event error:', error);
      return {
        success: false,
        message: error.response?.data?.message || 'Failed to get event',
        errorMessages: error.response?.data?.errorMessages || [],
        error: error.response?.data || error.message
      };
    }
  },

  getEvent: async (id) => {
    try {
      const response = await api.get(`/Events/${id}`);
      // Backend returns: { success: true, message: "...", data: [...] }
      // Return it directly without wrapping
      return response.data;
    } catch (error) {
      console.error('Get event error:', error);
      return {
        success: false,
        message: error.response?.data?.message || 'Failed to get event',
        errorMessages: error.response?.data?.errorMessages || [],
        error: error.response?.data || error.message
      };
    }
  },

  // PUT /api/Events/{id} - Update event (supports FormData for file upload)
  updateEvent: async (eventId, eventData) => {
    try {
      console.log('🔄 eventService.updateEvent - START');
      console.log('📍 API URL:', api.defaults.baseURL + `/Events/${eventId}`);
      console.log('📦 Sending data:', eventData);
      
      // Axios automatically sets correct Content-Type for FormData
      const response = await api.put(`/Events/${eventId}`, eventData);
      
      console.log('✅ Update event response:', response.data);
      
      // Backend returns: { success: true, message: "...", data: {...} }
      return response.data;
    } catch (error) {
      console.error('❌ eventService.updateEvent - ERROR');
      console.error('Error details:', error);
      console.error('Response data:', error.response?.data);
      console.error('Response status:', error.response?.status);
      console.error('Response headers:', error.response?.headers);
      
      return {
        success: false,
        message: error.response?.data?.message || error.response?.data?.title || 'Failed to update event',
        errorMessages: error.response?.data?.errorMessages || error.response?.data?.errors || [],
        error: error.response?.data || error.message
      };
    }
  },

  // DELETE /api/Events/{id} - Delete event
  deleteEvent: async (eventId) => {
    try {
      const response = await api.delete(`/Events/${eventId}`);
      return {
        success: true,
        data: response.data,
        message: 'Event deleted successfully'
      };
    } catch (error) {
      console.error('Delete event error:', error);
      return {
        success: false,
        message: error.response?.data?.message || 'Failed to delete event',
        errorMessages: error.response?.data?.errorMessages || [],
        error: error.response?.data || error.message
      };
    }
  },

  // GET /api/Events/{id}/detail - Get event detail
  getEventDetail: async (eventId) => {
    try {
      const response = await api.get(`/Events/${eventId}/detail`);
      return {
        success: true,
        data: response.data,
        message: 'Event detail retrieved successfully'
      };
    } catch (error) {
      console.error('Get event detail error:', error);
      return {
        success: false,
        message: error.response?.data?.message || 'Failed to get event detail',
        errorMessages: error.response?.data?.errorMessages || [],
        error: error.response?.data || error.message
      };
    }
  },

  // GET /api/events/{eventId}/activities - Get all activities for an event
  getActivities: async (eventId) => {
    try {
      const response = await api.get(`/events/${eventId}/activities`);
      return {
        success: true,
        data: response.data,
        message: 'Activities retrieved successfully'
      };
    } catch (error) {
      console.error('Get activities error:', error);
      return {
        success: false,
        message: error.response?.data?.message || 'Failed to get activities',
        errorMessages: error.response?.data?.errorMessages || [],
        error: error.response?.data || error.message
      };
    }
  },

  // POST /api/events/{eventId}/activities - Create new activity
  createActivity: async (eventId, activityData) => {
    try {
      console.log('🚀 createActivity - START');
      console.log('📍 URL:', `/events/${eventId}/activities`);
      console.log('📦 Activity Data:', JSON.stringify(activityData, null, 2));
      
      const response = await api.post(`/events/${eventId}/activities`, activityData);
      
      console.log('✅ Activity created:', response.data);
      
      return {
        success: true,
        data: response.data,
        message: 'Activity created successfully'
      };
    } catch (error) {
      console.error('❌ createActivity - ERROR');
      console.error('Error details:', error);
      console.error('Response data:', error.response?.data);
      console.error('Response status:', error.response?.status);
      
      // Log validation errors if present
      if (error.response?.data?.errors) {
        console.error('🔍 ACTIVITY VALIDATION ERRORS:', JSON.stringify(error.response.data.errors, null, 2));
      }
      
      return {
        success: false,
        message: error.response?.data?.title || error.response?.data?.message || 'Failed to create activity',
        errorMessages: error.response?.data?.errors || error.response?.data?.errorMessages || [],
        error: error.response?.data || error.message
      };
    }
  },



  // PUT /api/events/{eventId}/activities/{id} - Update activity
  updateActivity: async (eventId, activityId, activityData) => {
    try {
      console.log('🔄 updateActivity - START');
      console.log('📍 URL:', `/events/${eventId}/activities/${activityId}`);
      console.log('📦 Activity Data:', JSON.stringify(activityData, null, 2));
      
      const response = await api.put(`/events/${eventId}/activities/${activityId}`, activityData, {
        headers: {
          'Content-Type': 'application/json'
        }
      });
      
      console.log('✅ Activity updated:', response.data);
      
      return {
        success: true,
        data: response.data,
        message: 'Activity updated successfully'
      };
    } catch (error) {
      console.error('❌ updateActivity - ERROR');
      console.error('Error details:', error);
      console.error('Response data:', error.response?.data);
      console.error('Response status:', error.response?.status);
      
      if (error.response?.data?.errors) {
        console.error('🔍 VALIDATION ERRORS:', JSON.stringify(error.response.data.errors, null, 2));
      }
      
      return {
        success: false,
        message: error.response?.data?.title || error.response?.data?.message || 'Failed to update activity',
        errorMessages: error.response?.data?.errors || error.response?.data?.errorMessages || [],
        error: error.response?.data || error.message
      };
    }
  },

  // DELETE /api/events/{eventId}/activities/{id} - Delete activity
  deleteActivity: async (eventId, activityId) => {
    try {
      console.log('🗑️ deleteActivity - START');
      console.log('📍 URL:', `/events/${eventId}/activities/${activityId}`);
      
      const response = await api.delete(`/events/${eventId}/activities/${activityId}`);
      
      console.log('✅ Activity deleted:', response.data);
      
      return {
        success: true,
        data: response.data,
        message: 'Activity deleted successfully'
      };
    } catch (error) {
      console.error('❌ deleteActivity - ERROR');
      console.error('Error details:', error);
      console.error('Response data:', error.response?.data);
      console.error('Response status:', error.response?.status);
      
      return {
        success: false,
        message: error.response?.data?.title || error.response?.data?.message || 'Failed to delete activity',
        errorMessages: error.response?.data?.errors || error.response?.data?.errorMessages || [],
        error: error.response?.data || error.message
      };
    }
  },

  // PATCH /api/Events/{id}/visibility - Update event visibility
  updateEventVisibility: async (eventId) => {
    try {
      console.log('👁️ updateEventVisibility - START');
      console.log('📍 URL:', `/Events/${eventId}/visibility`);
      
      const response = await api.patch(`/Events/${eventId}/visibility`);
      
      console.log('✅ Event visibility updated:', response.data);
      
      return {
        success: true,
        data: response.data,
        message: 'Event visibility updated successfully'
      };
    } catch (error) {
      console.error('❌ updateEventVisibility - ERROR');
      console.error('Error details:', error);
      console.error('Response data:', error.response?.data);
      console.error('Response status:', error.response?.status);
      
      return {
        success: false,
        message: error.response?.data?.title || error.response?.data?.message || 'Failed to update event visibility',
        errorMessages: error.response?.data?.errors || error.response?.data?.errorMessages || [],
        error: error.response?.data || error.message
      };
    }
  },

  // PATCH /api/Events/{id}/status - Update event status
  updateEventStatus: async (eventId) => {
    try {
      console.log('🔄 updateEventStatus - START');
      console.log('📍 URL:', `/Events/${eventId}/status`);
      
      const response = await api.patch(`/Events/${eventId}/status`);
      
      console.log('✅ Event status updated:', response.data);
      
      return {
        success: true,
        data: response.data,
        message: 'Event status updated successfully'
      };
    } catch (error) {
      console.error('❌ updateEventStatus - ERROR');
      console.error('Error details:', error);
      console.error('Response data:', error.response?.data);
      console.error('Response status:', error.response?.status);
      
      return {
        success: false,
        message: error.response?.data?.title || error.response?.data?.message || 'Failed to update event status',
        errorMessages: error.response?.data?.errors || error.response?.data?.errorMessages || [],
        error: error.response?.data || error.message
      };
    }
  }
};

export default eventService;
