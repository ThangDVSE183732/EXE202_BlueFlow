// Account Settings Service
// This file contains all API calls related to account settings

import axios from 'axios';

const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:3001/api';

export class AccountService {
  /**
   * Get current user's account settings
   */
  static async getAccountSettings() {
    try {
      const response = await axios.get(`${API_BASE_URL}/account/settings`, {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
          'Content-Type': 'application/json'
        }
      });
      return response.data;
    } catch (error) {
      console.error('Error fetching account settings:', error);
      throw new Error(error.response?.data?.message || 'Failed to fetch account settings');
    }
  }

  /**
   * Update account settings
   * @param {Object} formData - Account settings data
   */
  static async updateAccountSettings(formData) {
    try {
      const response = await axios.put(`${API_BASE_URL}/account/settings`, formData, {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
          'Content-Type': 'application/json'
        }
      });
      return response.data;
    } catch (error) {
      console.error('Error updating account settings:', error);
      throw new Error(error.response?.data?.message || 'Failed to update account settings');
    }
  }

  /**
   * Upload company logo
   * @param {File} file - Logo file
   */
  static async uploadCompanyLogo(file) {
    try {
      const formData = new FormData();
      formData.append('logo', file);

      const response = await axios.post(`${API_BASE_URL}/account/logo`, formData, {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
          'Content-Type': 'multipart/form-data'
        }
      });
      return response.data;
    } catch (error) {
      console.error('Error uploading logo:', error);
      throw new Error(error.response?.data?.message || 'Failed to upload logo');
    }
  }

  /**
   * Delete company logo
   */
  static async deleteCompanyLogo() {
    try {
      const response = await axios.delete(`${API_BASE_URL}/account/logo`, {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
          'Content-Type': 'application/json'
        }
      });
      return response.data;
    } catch (error) {
      console.error('Error deleting logo:', error);
      throw new Error(error.response?.data?.message || 'Failed to delete logo');
    }
  }

  /**
   * Validate email address
   * @param {string} email - Email to validate
   */
  static async validateEmail(email) {
    try {
      const response = await axios.post(`${API_BASE_URL}/account/validate-email`, 
        { email },
        {
          headers: {
            'Authorization': `Bearer ${localStorage.getItem('authToken')}`,
            'Content-Type': 'application/json'
          }
        }
      );
      return response.data;
    } catch (error) {
      console.error('Error validating email:', error);
      throw new Error(error.response?.data?.message || 'Failed to validate email');
    }
  }
}

// Export default for easier imports
export default AccountService;