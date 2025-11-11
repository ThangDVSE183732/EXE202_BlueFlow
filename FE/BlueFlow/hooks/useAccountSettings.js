// Custom hook for Account Settings management
import { useState, useCallback, useRef, useEffect } from 'react';
import { accountService } from '../services/accountService';
import { useAuth } from '../contexts/AuthContext';

export const useAccountSettings = (showToast = null) => {
  const { user, updateUser } = useAuth();
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(false);
  
  // Use ref to store showToast callback to avoid dependency issues
  const showToastRef = useRef(showToast);
  
  // Update ref when showToast changes
  useEffect(() => {
    showToastRef.current = showToast;
  }, [showToast]);

  // Fetch account data
  const fetchAccountData = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);
      
      // Get userId from AuthContext
      const userId =  user?.id;
      
      if (!userId) {
        throw new Error('User not logged in');
      }
      
      // Fetch profile by userId
      const response = await accountService.getUserProfileByUserId(userId);
      
      if (!response.success) {
        console.error('❌ API Error:', response.message, response.errors);
        throw new Error(response.message);
      }
      
      // Map API response to form structure
      const profileData = response.data;
      console.log('✅ Profile data received:', profileData);
      
      const formData = {
        companyName: profileData.companyName || '',
        companyLogoUrl: profileData.companyLogoUrl || null,
        industry: profileData.industry || '',
        companySize: profileData.companySize || '',
        foundedYear: profileData.foundedYear || '',
        socialProfile: profileData.socialProfile || '',
        linkedinProfile: profileData.linkedInProfile || '',
        companyDescription: profileData.companyDescription || '',
        officialEmail: profileData.officialEmail || '',
        stateProvince: profileData.stateProvince || '',
        countryRegion: profileData.countryRegion || '',
        city: profileData.city || '',
        streetAddress: profileData.streetAddress || '',
        fullName: profileData.fullName || '',
        jobTitle: profileData.jobTitle || '',
        directEmail: profileData.directEmail || '',
        directPhone: profileData.directPhone || '',
        profileId: profileData.id // Save profile ID for updates
      };
      
      console.log('📋 Form data mapped:', formData);
      return formData;
      
    } catch (err) {
      const errorMessage = err.message || 'Failed to load account data. Please try again.';
      setError(errorMessage);
      
      // Show error toast - use ref to avoid dependency
      if (showToastRef.current) {
        showToastRef.current({
          type: 'error',
          title: 'Lỗi tải dữ liệu!',
          message: errorMessage,
          duration: 4000
        });
      }
      
      throw new Error(errorMessage);
    } finally {
      setLoading(false);
    }
  }, [user]);

  // Save account data
  const saveAccountData = useCallback(async (formData) => {
    try {
      setSaving(true);
      setError(null);
      
      if (!formData.profileId) {
        throw new Error('Profile ID not found');
      }
      
      // Check if there's a file to upload
      const hasFile = formData.companyLogoFile instanceof File;
      
      if (hasFile) {
        // If there's a file, use FormData (multipart/form-data)
        const form = new FormData();
        
        // IMPORTANT: Append Id field first
        form.append('Id', formData.profileId);

        // Append all fields theo đúng thứ tự backend validation errors
        form.append('CompanyName', formData.companyName || '');
        form.append('Industry', formData.industry || '');
        form.append('CompanySize', formData.companySize || '');
        form.append('FoundedYear', formData.foundedYear || '');
        form.append('CompanyDescription', formData.companyDescription || '');
        form.append('SocialProfile', formData.socialProfile || '');
        form.append('LinkedInProfile', formData.linkedinProfile || '');
        form.append('OfficialEmail', formData.officialEmail || '');
        form.append('StateProvince', formData.stateProvince || '');
        form.append('CountryRegion', formData.countryRegion || '');
        form.append('City', formData.city || '');
        form.append('StreetAddress', formData.streetAddress || '');
        form.append('ContactFullName', formData.fullName || '');
        form.append('JobTitle', formData.jobTitle || '');
        form.append('DirectEmail', formData.directEmail || '');
        form.append('DirectPhone', formData.directPhone || '');
        // Append file cuối cùng
        form.append('CompanyLogoUrl', formData.companyLogoFile);


        // Update profile with FormData
        const response = await accountService.updateUserProfile(formData.profileId, form);
        
        if (!response.success) {
          throw new Error(response.message);
        }
        
        // Update user context với companyLogoUrl mới từ backend response
        if (updateUser && response.data?.companyLogoUrl) {
          updateUser({ 
            companyLogoUrl: response.data.companyLogoUrl,
            companyName: formData.companyName,
            fullName: formData.fullName
          });
        }
      } else {
        // No file, send JSON (skip CompanyLogoUrl field)
        const form = new FormData();
          form.append('CompanyName', formData.companyName || '');
        form.append('Industry', formData.industry || '');
        form.append('CompanySize', formData.companySize || '');
        form.append('FoundedYear', formData.foundedYear || '');
        form.append('CompanyDescription', formData.companyDescription || '');
        form.append('SocialProfile', formData.socialProfile || '');
        form.append('LinkedInProfile', formData.linkedinProfile || '');
        form.append('OfficialEmail', formData.officialEmail || '');
        form.append('StateProvince', formData.stateProvince || '');
        form.append('CountryRegion', formData.countryRegion || '');
        form.append('City', formData.city || '');
        form.append('StreetAddress', formData.streetAddress || '');
        form.append('ContactFullName', formData.fullName || '');
        form.append('JobTitle', formData.jobTitle || '');
        form.append('DirectEmail', formData.directEmail || '');
        form.append('DirectPhone', formData.directPhone || '');


        // Update profile with JSON
        const response = await accountService.updateUserProfile(formData.profileId, form);

        if (!response.success) {
          throw new Error(response.message);
        }
        
        // Update user context (no new logo, keep existing)
        if (updateUser) {
          updateUser({ 
            companyName: formData.companyName,
            fullName: formData.fullName
          });
        }
      }

      
      setSuccess(true);
      
      // Show success toast - use ref to avoid dependency
      if (showToastRef.current) {
        showToastRef.current({
          type: 'success',
          title: 'Đã lưu!',
          message: 'Cài đặt tài khoản đã được cập nhật',
          duration: 3000
        });
      }
      
      // Auto-hide success message after 3 seconds
      setTimeout(() => setSuccess(false), 3000);
      
      return { success: true, message: 'Account settings saved successfully' };
      
    } catch (err) {
      const errorMessage = err.message || 'Failed to save account settings. Please try again.';
      setError(errorMessage);
      
      // Show error toast - use ref to avoid dependency
      if (showToastRef.current) {
        showToastRef.current({
          type: 'error',
          title: 'Lỗi lưu cài đặt!',
          message: errorMessage,
          duration: 4000
        });
      }
      
      throw new Error(errorMessage);
    } finally {
      setSaving(false);
    }
  }, [updateUser]);

  // Upload logo
  const uploadLogo = useCallback(async (file) => {
    try {
      setError(null);
      
      // TODO: Implement when backend provides upload endpoint
      // For now, just return success to allow preview
      // The actual file will be handled when form is saved
      console.log('Logo file selected:', file.name);
      
      return { success: true, logoUrl: URL.createObjectURL(file) };
      
    } catch (err) {
      const errorMessage = err.message || 'Failed to upload logo. Please try again.';
      setError(errorMessage);
      throw new Error(errorMessage);
    }
  }, []);

  // Delete logo
  const deleteLogo = useCallback(async () => {
    try {
      setError(null);
      
      // TODO: Implement when backend provides delete logo endpoint
      // For now, just return success
      console.log('Logo removed');
      
      return { success: true };
      
    } catch (err) {
      const errorMessage = err.message || 'Failed to delete logo. Please try again.';
      setError(errorMessage);
      throw new Error(errorMessage);
    }
  }, []);

  // Clear messages
  const clearMessages = useCallback(() => {
    setError(null);
    setSuccess(false);
  }, []);

  return {
    loading,
    saving,
    error,
    success,
    fetchAccountData,
    saveAccountData,
    uploadLogo,
    deleteLogo,
    clearMessages
  };
};