import { useState, useCallback } from 'react';
import toast from 'react-hot-toast';
import eventService from '../services/eventService';

export const useEvent = () => {
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [deleting, setDeleting] = useState(false);
  const [eventData, setEventData] = useState(null);
  const [eventList, setEventList] = useState([]);
  const [error, setError] = useState(null);

  // Create new event
  const createEvent = useCallback(async (formData) => {
    try {
      setSaving(true);
      setError(null);

      const result = await eventService.createEvent(formData);

      if (result.success) {
        setEventData(result.data);
        toast.success('Tạo sự kiện thành công');

        return { success: true, data: result.data };
      } else {
        setError(result.message);
        toast.error(result.message || 'Không thể tạo sự kiện');

        return { success: false, message: result.message };
      }
    } catch (err) {
      const errorMessage = err.message || 'Đã xảy ra lỗi khi tạo sự kiện';
      setError(errorMessage);
      toast.error(errorMessage);

      return { success: false, message: errorMessage };
    } finally {
      setSaving(false);
    }
  }, []);

  // Get event by ID
  const getEventById = useCallback(async (eventId) => {
    try {
      setLoading(true);
      setError(null);

      const result = await eventService.getEventById(eventId);

      if (result.success) {
        setEventData(result.data);
        return { success: true, data: result.data };
      } else {
        setError(result.message);
        toast.error(result.message || 'Không thể tải sự kiện');

        return { success: false, message: result.message };
      }
    } catch (err) {
      const errorMessage = err.message || 'Đã xảy ra lỗi khi tải sự kiện';
      setError(errorMessage);
      toast.error(errorMessage);

      return { success: false, message: errorMessage };
    } finally {
      setLoading(false);
    }
  }, []);

  // Update event
  const updateEvent = useCallback(async (eventId, formData) => {
    try {
      setSaving(true);
      setError(null);

      const result = await eventService.updateEvent(eventId, formData);

      if (result.success) {
        setEventData(result.data);
        toast.success('Cập nhật sự kiện thành công');

        return { success: true, data: result.data };
      } else {
        setError(result.message);
        toast.error(result.message || 'Không thể cập nhật sự kiện');

        return { success: false, message: result.message };
      }
    } catch (err) {
      const errorMessage = err.message || 'Đã xảy ra lỗi khi cập nhật sự kiện';
      setError(errorMessage);
      toast.error(errorMessage);

      return { success: false, message: errorMessage };
    } finally {
      setSaving(false);
    }
  }, []);

  // Delete event
  const deleteEvent = useCallback(async (eventId) => {
    try {
      setDeleting(true);
      setError(null);

      const result = await eventService.deleteEvent(eventId);

      if (result.success) {
        // Remove from local state if in list
        setEventList(prev => prev.filter(event => event.id !== eventId));
        toast.success('Xóa sự kiện thành công');

        return { success: true };
      } else {
        setError(result.message);
        toast.error(result.message || 'Không thể xóa sự kiện');

        return { success: false, message: result.message };
      }
    } catch (err) {
      const errorMessage = err.message || 'Đã xảy ra lỗi khi xóa sự kiện';
      setError(errorMessage);
      toast.error(errorMessage);

      return { success: false, message: errorMessage };
    } finally {
      setDeleting(false);
    }
  }, []);

  // Get event detail
  const getEventDetail = useCallback(async (eventId) => {
    try {
      setLoading(true);
      setError(null);

      const result = await eventService.getEventDetail(eventId);

      if (result.success) {
        setEventData(result.data);
        return { success: true, data: result.data };
      } else {
        setError(result.message);
        toast.error(result.message || 'Không thể tải chi tiết sự kiện');

        return { success: false, message: result.message };
      }
    } catch (err) {
      const errorMessage = err.message || 'Đã xảy ra lỗi khi tải chi tiết sự kiện';
      setError(errorMessage);
      toast.error(errorMessage);

      return { success: false, message: errorMessage };
    } finally {
      setLoading(false);
    }
  }, []);

  // Get all activities for an event
  const getActivities = useCallback(async (eventId) => {
    try {
      setLoading(true);
      setError(null);

      const result = await eventService.getActivities(eventId);

      if (result.success) {
        return { success: true, data: result.data };
      } else {
        setError(result.message);
        toast.error(result.message || 'Không thể tải hoạt động');

        return { success: false, message: result.message };
      }
    } catch (err) {
      const errorMessage = err.message || 'Đã xảy ra lỗi khi tải hoạt động';
      setError(errorMessage);
      toast.error(errorMessage);

      return { success: false, message: errorMessage };
    } finally {
      setLoading(false);
    }
  }, []);

  // Create new activity
  const createActivity = useCallback(async (eventId, activityData) => {
    try {
      setSaving(true);
      setError(null);

      const result = await eventService.createActivity(eventId, activityData);

      if (result.success) {
        toast.success('Tạo hoạt động thành công');

        return { success: true, data: result.data };
      } else {
        setError(result.message);
        toast.error(result.message || 'Không thể tạo hoạt động');

        return { success: false, message: result.message };
      }
    } catch (err) {
      const errorMessage = err.message || 'Đã xảy ra lỗi khi tạo hoạt động';
      setError(errorMessage);
      toast.error(errorMessage);

      return { success: false, message: errorMessage };
    } finally {
      setSaving(false);
    }
  }, []);

  // Update activity
  const updateActivity = useCallback(async (eventId, activityId, activityData) => {
    try {
      setSaving(true);
      setError(null);

      const result = await eventService.updateActivity(eventId, activityId, activityData);

      if (result.success) {
        toast.success('Cập nhật hoạt động thành công');

        return { success: true, data: result.data };
      } else {
        setError(result.message);
        toast.error(result.message || 'Không thể cập nhật hoạt động');

        return { success: false, message: result.message };
      }
    } catch (err) {
      const errorMessage = err.message || 'Đã xảy ra lỗi khi cập nhật hoạt động';
      setError(errorMessage);
      toast.error(errorMessage);

      return { success: false, message: errorMessage };
    } finally {
      setSaving(false);
    }
  }, []);

  // Update event visibility
  const updateEventVisibility = useCallback(async (eventId) => {
    try {
      setSaving(true);
      setError(null);

      const result = await eventService.updateEventVisibility(eventId);

      if (result.success) {
        toast.success('Cập nhật trạng thái hiển thị thành công');

        return { success: true, data: result.data };
      } else {
        setError(result.message);
        toast.error(result.message || 'Không thể cập nhật trạng thái hiển thị');

        return { success: false, message: result.message };
      }
    } catch (err) {
      const errorMessage = err.message || 'Đã xảy ra lỗi khi cập nhật trạng thái hiển thị';
      setError(errorMessage);
      toast.error(errorMessage);

      return { success: false, message: errorMessage };
    } finally {
      setSaving(false);
    }
  }, []);

  // Update event status
  const updateEventStatus = useCallback(async (eventId) => {
    try {
      setSaving(true);
      setError(null);

      const result = await eventService.updateEventStatus(eventId);

      if (result.success) {
        toast.success('Cập nhật trạng thái sự kiện thành công');

        return { success: true, data: result.data };
      } else {
        setError(result.message);
        toast.error(result.message || 'Không thể cập nhật trạng thái sự kiện');

        return { success: false, message: result.message };
      }
    } catch (err) {
      const errorMessage = err.message || 'Đã xảy ra lỗi khi cập nhật trạng thái sự kiện';
      setError(errorMessage);
      toast.error(errorMessage);

      return { success: false, message: errorMessage };
    } finally {
      setSaving(false);
    }
  }, []);

  // Clear error
  const clearError = useCallback(() => {
    setError(null);
  }, []);

  // Reset state
  const resetState = useCallback(() => {
    setEventData(null);
    setEventList([]);
    setError(null);
    setLoading(false);
    setSaving(false);
    setDeleting(false);
  }, []);

  return {
    // State
    loading,
    saving,
    deleting,
    eventData,
    eventList,
    error,
    
    // Event Actions
    createEvent,
    getEventById,
    updateEvent,
    deleteEvent,
    getEventDetail,
    updateEventVisibility,
    updateEventStatus,
    
    // Activity Actions
    getActivities,
    createActivity,
    updateActivity,
    
    // Utilities
    clearError,
    resetState,
    
    // Setters (for manual state updates)
    setEventData,
    setEventList,
  };
};

export default useEvent;
