import { useState, useCallback } from 'react';
import toast from 'react-hot-toast';
import partnershipService from '../services/partnershipService';

export const usePartnership = () => {
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [partnerships, setPartnerships] = useState([]);
  const [eventPartners, setEventPartners] = useState([]);
  const [error, setError] = useState(null);

  // Get all partnerships
  const getAllPartnerships = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);

      const result = await partnershipService.getAllPartnerships();

      if (result.success) {
        setPartnerships(result.data);
        return { success: true, data: result.data };
      } else {
        setError(result.message);
        toast.error(result.message || 'Không thể tải danh sách đối tác');
        return { success: false, message: result.message };
      }
    } catch (err) {
      const errorMessage = err.message || 'Đã xảy ra lỗi khi tải danh sách đối tác';
      setError(errorMessage);
      toast.error(errorMessage);
      return { success: false, message: errorMessage };
    } finally {
      setLoading(false);
    }
  }, []);

  // Create new partnership
  const createPartnership = useCallback(async (data) => {
    try {
      setSaving(true);
      setError(null);

      // Create FormData with fields in the exact order specified
      const formData = new FormData();
      
      // Required fields (in order)
      formData.append('EventId', data.eventId);
      formData.append('PartnerId', data.partnerId);
      formData.append('PartnerType', data.partnerType);
      
      // Optional fields (in order)
      if (data.initialMessage) {
        formData.append('InitialMessage', data.initialMessage);
      }
      if (data.proposedBudget !== undefined && data.proposedBudget !== null) {
        formData.append('ProposedBudget', data.proposedBudget.toString());
      }
      if (data.serviceDescription) {
        formData.append('ServiceDescription', data.serviceDescription);
      }
      if (data.preferredContactMethod) {
        formData.append('PreferredContactMethod', data.preferredContactMethod);
      }
      if (data.organizerContactInfo) {
        formData.append('OrganizerContactInfo', data.organizerContactInfo);
      }
      if (data.startDate) {
        formData.append('StartDate', data.startDate);
      } 
      if (data.deadlineDate) {
        formData.append('DeadlineDate', data.deadlineDate);
      }
      
      // File upload (last)
      if (data.partnershipImageFile) {
      console.log('Partnership image:', data.partnershipImageFile);
        formData.append('PartnershipImageFile', data.partnershipImageFile);
      }

      const result = await partnershipService.createPartnership(formData);

      if (result.success) {
        toast.success('Tạo quan hệ đối tác thành công');
        // Refresh partnerships list
        await getAllPartnerships();
        return { success: true, data: result.data };
      } else {
        setError(result.message);
        toast.error(result.message || 'Không thể tạo quan hệ đối tác');
        return { success: false, message: result.message };
      }
    } catch (err) {
      const errorMessage = err.message || 'Đã xảy ra lỗi khi tạo quan hệ đối tác';
      setError(errorMessage);
      toast.error(errorMessage);
      return { success: false, message: errorMessage };
    } finally {
      setSaving(false);
    }
  }, [getAllPartnerships]);

  // Update partnership status (toggle)
  // Only requires eventId now
  const updatePartnershipStatus = useCallback(async (eventId) => {
    try {
      setSaving(true);
      setError(null);

      console.log('🔄 Toggling partnership status for event:', eventId);

      const result = await partnershipService.updatePartnershipStatus(eventId);

      if (result.success) {
        toast.success('Cập nhật trạng thái thành công');
        // Refresh partnerships list
        await getAllPartnerships();
        return { success: true, data: result.data };
      } else {
        setError(result.message);
        toast.error(result.message || 'Không thể cập nhật trạng thái');
        return { success: false, message: result.message };
      }
    } catch (err) {
      const errorMessage = err.message || 'Đã xảy ra lỗi khi cập nhật trạng thái';
      setError(errorMessage);
      toast.error(errorMessage);
      return { success: false, message: errorMessage };
    } finally {
      setSaving(false);
    }
  }, [getAllPartnerships]);

  // Update partnership
  const updatePartnership = useCallback(async (id, partnershipData) => {
    try {
      setSaving(true);
      setError(null);

      const result = await partnershipService.updatePartnership(id, partnershipData);

      if (result.success) {
        toast.success('Cập nhật quan hệ đối tác thành công');
        // Refresh partnerships list
        await getAllPartnerships();
        return { success: true, data: result.data };
      } else {
        setError(result.message);
        toast.error(result.message || 'Không thể cập nhật quan hệ đối tác');
        return { success: false, message: result.message };
      }
    } catch (err) {
      const errorMessage = err.message || 'Đã xảy ra lỗi khi cập nhật quan hệ đối tác';
      setError(errorMessage);
      toast.error(errorMessage);
      return { success: false, message: errorMessage };
    } finally {
      setSaving(false);
    }
  }, [getAllPartnerships]);

  // Get partners for an event
  const getEventPartners = useCallback(async (eventId) => {
    try {
      setLoading(true);
      setError(null);

      const result = await partnershipService.getEventPartners(eventId);

      if (result.success) {
        setEventPartners(result.data);
        return { success: true, data: result.data };
      } else {
        setError(result.message);
        toast.error(result.message || 'Không thể tải danh sách đối tác của sự kiện');
        return { success: false, message: result.message };
      }
    } catch (err) {
      const errorMessage = err.message || 'Đã xảy ra lỗi khi tải danh sách đối tác';
      setError(errorMessage);
      toast.error(errorMessage);
      return { success: false, message: errorMessage };
    } finally {
      setLoading(false);
    }
  }, []);

  // Update partnership status by partner ID
  const updatePartnershipStatusByPartner = useCallback(async (partnerId) => {
    try {
      setSaving(true);
      setError(null);

      const result = await partnershipService.updatePartnershipStatusByPartner(partnerId);

      if (result.success) {
        toast.success('Đã cập nhật trạng thái quan hệ đối tác');
        // Refresh partnerships list if needed
        await getAllPartnerships();
        return { success: true, data: result.data };
      } else {
        setError(result.message);
        toast.error(result.message || 'Không thể cập nhật trạng thái');
        return { success: false, message: result.message };
      }
    } catch (err) {
      const errorMessage = err.message || 'Đã xảy ra lỗi khi cập nhật trạng thái';
      setError(errorMessage);
      toast.error(errorMessage);
      return { success: false, message: errorMessage };
    } finally {
      setSaving(false);
    }
  }, [getAllPartnerships]);

  return {
    // States
    loading,
    saving,
    partnerships,
    eventPartners,
    error,

    // Methods
    getAllPartnerships,
    createPartnership,
    updatePartnershipStatus,
    updatePartnershipStatusByPartner,
    updatePartnership,
    getEventPartners,
  };
};
