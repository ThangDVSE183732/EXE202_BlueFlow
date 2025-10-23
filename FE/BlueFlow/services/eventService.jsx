import api from './axios';

export const eventService = {
  // GET /api/Events - Lấy danh sách tất cả events
  getAllEvents: async () => {
    try {
      const response = await api.get('/Events');
      return {
        success: true,
        data: response.data,
        message: 'Events retrieved successfully'
      };
    } catch (error) {
      console.error('Get all events error:', error);
      return {
        success: false,
        message: error.response?.data?.message || 'Failed to get events',
        errors: error.response?.data?.errors || []
      };
    }
  },

  // POST /api/Events - Tạo event mới
  createEvent: async (eventData) => {
    try {
      const response = await api.post('/Events', eventData);
      return {
        success: true,
        data: response.data,
        message: 'Event created successfully'
      };
    } catch (error) {
      console.error('Create event error:', error);
      return {
        success: false,
        message: error.response?.data?.message || 'Failed to create event',
        errors: error.response?.data?.errors || []
      };
    }
  },

  // GET /api/Events/{id} - Lấy thông tin event theo ID
  getEventById: async (id) => {
    try {
      const response = await api.get(`/Events/${id}`);
      return {
        success: true,
        data: response.data,
        message: 'Event retrieved successfully'
      };
    } catch (error) {
      console.error('Get event by ID error:', error);
      return {
        success: false,
        message: error.response?.data?.message || 'Failed to get event',
        errors: error.response?.data?.errors || []
      };
    }
  },

  // PUT /api/Events/{id} - Cập nhật event theo ID
  updateEvent: async (id, eventData) => {
    try {
      const response = await api.put(`/Events/${id}`, eventData);
      return {
        success: true,
        data: response.data,
        message: 'Event updated successfully'
      };
    } catch (error) {
      console.error('Update event error:', error);
      return {
        success: false,
        message: error.response?.data?.message || 'Failed to update event',
        errors: error.response?.data?.errors || []
      };
    }
  },

  // DELETE /api/Events/{id} - Xóa event theo ID
  deleteEvent: async (id) => {
    try {
      const response = await api.delete(`/Events/${id}`);
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
        errors: error.response?.data?.errors || []
      };
    }
  },

  // PUT /api/Events/{id}/status - Cập nhật status của event
  updateEventStatus: async (id, status) => {
    try {
      const response = await api.put(`/Events/${id}/status`, { status });
      return {
        success: true,
        data: response.data,
        message: 'Event status updated successfully'
      };
    } catch (error) {
      console.error('Update event status error:', error);
      return {
        success: false,
        message: error.response?.data?.message || 'Failed to update event status',
        errors: error.response?.data?.errors || []
      };
    }
  }
}