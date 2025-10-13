// Custom hook for Account Settings management
import { useState, useCallback } from 'react';
// import AccountService from '../services/accountService';

export const useAccountSettings = () => {
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(false);

  // Fetch account data
  const fetchAccountData = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);
      
      // TODO: Uncomment when API is ready
      // const response = await AccountService.getAccountSettings();
      // return response.data;
      
      // Mock data for now
      const mockData = {
        companyName: 'TechCorp Solutions',
        industry: 'Technology & Software',
        companySize: '501-1000 employees',
        foundedYear: '2015',
        website: 'https://www.techcorp.vn',
        linkedinProfile: 'https://linkedin.com/company/yourcompany',
        companyDescription: 'TechCorp Solutions is a leading technology company specializing in innovative software solutions and digital transformation services.',
        officialEmail: 'partnership@techcorp.vn',
        companyPhone: '+84 28 1234 5678',
        country: 'Vietnam',
        city: 'Ho Chi Minh City',
        fullAddress: '123 Nguyen Hue Street, District 1, Ho Chi Minh City',
        contactFullName: 'Nguyen Van An',
        contactJobTitle: 'Partnership Manager',
        contactDirectEmail: 'an.nguyen@techcorp.vn',
        contactDirectPhone: '+84 90 123 4567',
        companyLogo: null
      };
      
      // Simulate API delay
      await new Promise(resolve => setTimeout(resolve, 1000));
      return mockData;
      
    } catch (err) {
      const errorMessage = err.message || 'Failed to load account data. Please try again.';
      setError(errorMessage);
      throw new Error(errorMessage);
    } finally {
      setLoading(false);
    }
  }, []);

  // Save account data
  const saveAccountData = useCallback(async (formData) => {
    try {
      setSaving(true);
      setError(null);
      
      // TODO: Uncomment when API is ready
      // const response = await AccountService.updateAccountSettings(formData);
      
      // Simulate API call
      await new Promise(resolve => setTimeout(resolve, 2000));
      
      setSuccess(true);
      
      // Auto-hide success message after 3 seconds
      setTimeout(() => setSuccess(false), 3000);
      
      return { success: true, message: 'Account settings saved successfully' };
      
    } catch (err) {
      const errorMessage = err.message || 'Failed to save account settings. Please try again.';
      setError(errorMessage);
      throw new Error(errorMessage);
    } finally {
      setSaving(false);
    }
  }, []);

  // Upload logo
  const uploadLogo = useCallback(async (file) => {
    try {
      setError(null);
      
      // TODO: Uncomment when API is ready
      // const response = await AccountService.uploadCompanyLogo(file);
      // return response.data;
      
      // Simulate upload
      await new Promise(resolve => setTimeout(resolve, 1500));
      return { success: true, logoUrl: 'mock-logo-url' };
      
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
      
      // TODO: Uncomment when API is ready
      // await AccountService.deleteCompanyLogo();
      
      // Simulate deletion
      await new Promise(resolve => setTimeout(resolve, 500));
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