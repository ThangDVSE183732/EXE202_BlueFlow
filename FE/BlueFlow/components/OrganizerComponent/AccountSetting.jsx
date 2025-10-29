import React, { useState, useEffect } from 'react';
import { useAccountSettings } from '../../hooks/useAccountSettings';
import { validateForm, validateField, validateFile, DROPDOWN_OPTIONS, FORM_FIELDS } from '../../utils/accountValidation';

// Helper component for form inputs
const FormInput = ({ 
  label, 
  name, 
  type = 'text', 
  value, 
  onChange, 
  placeholder, 
  required = false, 
  error = '',
  className = '' 
}) => (
  <div>
    <label className="block text-sm font-medium mb-2">
      {label} {required && '*'}
    </label>
    <input
      type={type}
      name={name}
      value={value}
      onChange={onChange}
      placeholder={placeholder}
      className={`w-full px-3 py-2 border rounded-lg text-sm focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-none transition-colors ${
        error ? 'border-red-300 bg-red-50' : 'border-gray-300 bg-white'
      } ${className}`}
    />
    {error && <p className="text-red-500 text-xs mt-1">{error}</p>}
  </div>
);

// Helper component for select dropdowns
const FormSelect = ({ 
  label, 
  name, 
  value, 
  onChange, 
  options, 
  required = false, 
  error = '',
  className = 'bg-blue-50' 
}) => (
  <div>
    <label className="block text-sm font-medium mb-2">
      {label} {required && '*'}
    </label>
    <select
      name={name}
      value={value}
      onChange={onChange}
      className={`w-full px-3 py-2 border rounded-lg text-sm focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-none appearance-none transition-colors ${
        error ? 'border-red-300 bg-red-50' : `border-gray-300 ${className}`
      }`}
    >
      {options.map(option => (
        <option key={option.value} value={option.value}>
          {option.label}
        </option>
      ))}
    </select>
    {error && <p className="text-red-500 text-xs mt-1">{error}</p>}
  </div>
);

// Helper component for textarea
const FormTextarea = ({ 
  label, 
  name, 
  value, 
  onChange, 
  placeholder, 
  rows = 4, 
  maxLength = 500,
  error = '' 
}) => (
  <div>
    <label className="block text-sm font-medium mb-2">{label}</label>
    <div className="relative">
      <textarea
        name={name}
        value={value}
        onChange={onChange}
        rows={rows}
        placeholder={placeholder}
        className={`w-full px-3 py-2 border rounded-lg text-sm focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-none resize-none transition-colors ${
          error ? 'border-red-300 bg-red-50' : 'border-gray-300 bg-white'
        }`}
      />
      <div className="absolute bottom-2 right-2 text-xs text-gray-400">
        {value.length}/{maxLength}
      </div>
    </div>
    {error && <p className="text-red-500 text-xs mt-1">{error}</p>}
  </div>
);

const AccountSetting = ({ showToast }) => {
  // Custom hook for account settings management
  const {
    loading,
    saving,
    error,
    success,
    fetchAccountData,
    saveAccountData,
    deleteLogo,
    clearMessages
  } = useAccountSettings(showToast);

  // Form state
  const [formData, setFormData] = useState({
    companyName: '',
    companyLogoUrl: '',
    industry: '',
    companySize: '',
    foundedYear: '',
    socialProfile: '',
    linkedinProfile: '',
    companyDescription: '',
    officialEmail: '',
    stateProvince: '',
    countryRegion: '',
    city: '',
    streetAddress: '',
    fullName: '',
    jobTitle: '',
    directEmail: '',
    directPhone: ''
    });

  // Form validation errors
  const [errors, setErrors] = useState({});

  // Preview URL for logo
  const [logoPreview, setLogoPreview] = useState(null);

  // Load initial data
  useEffect(() => {
    const loadData = async () => {
      try {
        const data = await fetchAccountData();
        setFormData(data);
        // Set logo preview from API if exists (Cloudinary URL)
        if (data.companyLogoUrl) {
          setLogoPreview(data.companyLogoUrl);
        }
      } catch (err) {
        console.error('Failed to load account data:', err);
      }
    };
    loadData();
  }, [fetchAccountData]);

  // Cleanup preview URL on unmount (only for local file previews)
  useEffect(() => {
    return () => {
      if (logoPreview && logoPreview.startsWith('blob:')) {
        URL.revokeObjectURL(logoPreview);
      }
    };
  }, [logoPreview]);

  // Handle input changes with validation
  const handleInputChange = (e) => {
    const { name, value } = e.target;
    
    // Update form data
    setFormData(prev => ({ ...prev, [name]: value }));
    
    // Real-time validation
    const fieldError = validateField(name, value, formData);
    setErrors(prev => ({ 
      ...prev, 
      [name]: fieldError || '' 
    }));
    
    // Clear success message when user makes changes
    if (success) clearMessages();
  };

  // Handle file upload with validation
  const handleFileUpload = (e) => {
    const file = e.target.files[0];
    if (!file) return;

    // Validate file
    const fileError = validateFile(file);
    if (fileError) {
      setErrors(prev => ({ ...prev, companyLogo: fileError }));
      return;
    }

    // Clear any previous errors
    setErrors(prev => ({ ...prev, companyLogo: '' }));
    
    // Create preview URL for immediate display (local only, không gọi API)
    const previewUrl = URL.createObjectURL(file);
    setLogoPreview(previewUrl);
    
    // Save file object to formData (sẽ gửi lên khi nhấn Save)
    setFormData(prev => ({ 
      ...prev, 
      companyLogoFile: file // Lưu file, sẽ upload khi Save
    }));
    
    // Clear success message when user makes changes
    if (success) clearMessages();
  };

  // Handle logo removal
  const handleLogoRemove = async () => {
    try {
      await deleteLogo();
      setFormData(prev => ({ ...prev, companyLogoUrl: null }));
      // Clear preview (only revoke if it's a blob URL)
      if (logoPreview && logoPreview.startsWith('blob:')) {
        URL.revokeObjectURL(logoPreview);
      }
      setLogoPreview(null);
    } catch (err) {
      setErrors(prev => ({ ...prev, companyLogo: err.message }));
    }
  };

  // Handle form save
  const handleSave = async () => {
    try {
      // Validate entire form
      const formErrors = validateForm(formData);
      setErrors(formErrors);

      if (Object.keys(formErrors).length > 0) {
        return;
      }

      // Save data using custom hook
      await saveAccountData(formData);
    } catch (err) {
      console.error('Error saving account settings:', err);
    }
  };

  // Handle cancel
  const handleCancel = async () => {
    try {
      const originalData = await fetchAccountData();
      setFormData(originalData);
      setErrors({});
      clearMessages();
    } catch (err) {
      console.error('Error resetting form:', err);
    }
  };

  // Loading skeleton
  if (loading) {
    return (
      <div className="p-6 pt-3 pl-2 min-h-screen">
        <div className="ml-1 text-left">
          <div className="h-8 bg-gray-200 rounded mb-2 animate-pulse"></div>
          <div className="h-4 bg-gray-200 rounded w-2/3 animate-pulse"></div>
        </div>
        <div className="h-px w-full bg-gray-300 mx-1 mb-5 mt-2" />
        <div className="bg-white rounded-2xl shadow-2xl border border-gray-200 p-8">
          <div className="space-y-4">
            {[...Array(6)].map((_, i) => (
              <div key={i} className="h-16 bg-gray-200 rounded animate-pulse"></div>
            ))}
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="p-6 pt-3 pl-2 min-h-screen">
      <div className="ml-1 text-left">
        <h1 className="text-2xl font-semibold text-blue-500 mb-2">Account setting</h1>
        <p className="text-gray-500 text-sm">Update your company information and contact details</p>
      </div>
      <div className="h-px w-full bg-gray-300 mx-1 mb-5 mt-2" />

      <div className="bg-white rounded-2xl shadow-2xl border border-gray-200 p-8">
        <div className="flex items-start gap-8 pb-8 border-b border-gray-200 mb-8">
          <div className="w-27 h-27 bg-gradient-to-br from-cyan-400 to-cyan-500 rounded-full flex items-center justify-center text-white text-2xl font-bold overflow-hidden">
            {logoPreview ? (
              <img 
                src={logoPreview} 
                alt="Company Logo" 
                className="w-full h-full object-cover"
              />
            ) : (
              formData.companyName ? formData.companyName.substring(0, 2).toUpperCase() : 'TC'
            )}
          </div>
          <div className="flex-1">
            <h3 className="text-lg text-left font-semibold text-gray-900 mb-1">Company Logo</h3>
            <p className="text-gray-500 mb-4 text-sm text-left">Upload your company logo. Recommended size: 400x400px, max 2MB</p>
            <div className="flex gap-3 mb-2">
              <label className="bg-blue-500 hover:bg-blue-600 text-white px-4 py-2 rounded-lg text-sm font-medium cursor-pointer transition-colors">
                Upload New Logo
                <input
                  type="file"
                  accept="image/*"
                  onChange={handleFileUpload}
                  className="hidden"
                />
              </label>
              <button 
                type="button"
                onClick={handleLogoRemove}
                className="border border-red-400 text-red-500 hover:bg-red-50 px-4 py-2 rounded-lg text-sm font-medium transition-colors"
              >
                Remove Logo
              </button>
            </div>
            {errors.companyLogo && (
              <p className="text-red-500 text-xs mt-1">{errors.companyLogo}</p>
            )}
          </div>
        </div>

        <div className="space-y-6 text-left">
          {/* Company Basic Info */}
          <div className="grid grid-cols-2 gap-6">
            <FormInput
              label={FORM_FIELDS.COMPANY_INFO.companyName.label}
              name="companyName"
              value={formData.companyName}
              onChange={handleInputChange}
              placeholder={FORM_FIELDS.COMPANY_INFO.companyName.placeholder}
              required={FORM_FIELDS.COMPANY_INFO.companyName.required}
              error={errors.companyName}
            />
            <FormSelect
              label={FORM_FIELDS.COMPANY_INFO.industry.label}
              name="industry"
              value={formData.industry}
              onChange={handleInputChange}
              options={DROPDOWN_OPTIONS.INDUSTRIES}
              required={FORM_FIELDS.COMPANY_INFO.industry.required}
              error={errors.industry}
            />
          </div>

          {/* Company Details */}
          <div className="grid grid-cols-2 gap-6">
            <FormSelect
              label={FORM_FIELDS.COMPANY_INFO.companySize.label}
              name="companySize"
              value={formData.companySize}
              onChange={handleInputChange}
              options={DROPDOWN_OPTIONS.COMPANY_SIZES}
              error={errors.companySize}
            />
            <FormInput
              label={FORM_FIELDS.COMPANY_INFO.foundedYear.label}
              name="foundedYear"
              value={formData.foundedYear}
              onChange={handleInputChange}
              placeholder={FORM_FIELDS.COMPANY_INFO.foundedYear.placeholder}
              error={errors.foundedYear}
            />
          </div>

          <div className="grid grid-cols-2 gap-6">
            <FormInput
              label="Social Profile"
              name="socialProfile"
              type="url"
              value={formData.socialProfile}
              onChange={handleInputChange}
              error={errors.socialProfile}
            />
            <FormInput
              label="LinkedIn Profile"
              name="linkedinProfile"
              type="url"
              value={formData.linkedinProfile}
              onChange={handleInputChange}
              error={errors.linkedinProfile}
            />
          </div>

          <FormTextarea
            label="Company Description"
            name="companyDescription"
            value={formData.companyDescription}
            onChange={handleInputChange}
            error={errors.companyDescription}
            maxLength={500}
          />
        </div>
      </div>

      <div className="bg-white rounded-2xl shadow-2xl border text-left border-gray-200 p-8 mt-6">
        <h3 className="text-xl font-semibold text-blue-500 mb-6">Contact Information</h3>
        <div className="space-y-6">
          <div className="grid grid-cols-2 gap-6">
            <FormInput
              label="Official Email"
              name="officialEmail"
              type="email"
              value={formData.officialEmail}
              onChange={handleInputChange}
              required
              error={errors.officialEmail}
            />
            <FormInput
              label="State/Province"
              name="province"
              type="tel"
              value={formData.stateProvince}
              onChange={handleInputChange}
              required
              error={errors.province}
            />
          </div>

          <div className="grid grid-cols-2 gap-6">
            <FormSelect
              label="Country/Region"
              name="countryRegion"
              value={formData.countryRegion}
              onChange={handleInputChange}
              options={DROPDOWN_OPTIONS.COUNTRIES}
              required
              error={errors.country}
            />
            <FormInput
              label="City"
              name="city"
              type="text"
              value={formData.city}
              onChange={handleInputChange}
              required
              error={errors.city}
            />
          </div>

          <FormInput
            label="Street Address"
            name="streetAddress"
            type="text"
            value={formData.streetAddress}
            onChange={handleInputChange}
            error={errors.streetAddress}
          />
        </div>
      </div>

      <div className="bg-white rounded-2xl shadow-2xl border text-left border-gray-200 p-8 mt-6">
        <h3 className="text-xl font-semibold text-blue-500 mb-6">Primary Contact Person</h3>
        <div className="space-y-6 ">
          <div className="grid grid-cols-2 gap-6">
            <FormInput
              label="Full Name"
              name="fullName"
              type="text"
              value={formData.fullName}
              onChange={handleInputChange}
              required
              error={errors.fullName}
            />
            <FormInput
              label="Job Title"
              name="jobTitle"
              type="text"
              value={formData.jobTitle}
              onChange={handleInputChange}
              required
              error={errors.jobTitle}
            />
          </div>

          <div className="grid grid-cols-2 gap-6">
            <FormInput
              label="Direct Email"
              name="directEmail"
              type="email"
              value={formData.directEmail}
              onChange={handleInputChange}
              required
              error={errors.directEmail}
            />
            <FormInput
              label="Direct Phone"
              name="directPhone"
              type="tel"
              value={formData.directPhone}
              onChange={handleInputChange}
              required
              error={errors.directPhone}
            />
          </div>
        </div>
      </div>

      {/* Action Buttons - Outside all cards */}
      <div className="flex justify-end gap-4 pt-8">
        <button 
          type="button"
          onClick={handleCancel}
          disabled={saving}
          className="bg-gray-400 hover:bg-gray-500 disabled:bg-gray-300 disabled:cursor-not-allowed text-white px-6 py-2 rounded-lg text-sm font-medium transition-colors"
        >
          Cancel
        </button>
        <button
          type="button"
          onClick={handleSave}
          disabled={saving}
          className="bg-blue-500 hover:bg-blue-600 disabled:bg-blue-300 disabled:cursor-not-allowed text-white px-6 py-2 rounded-lg text-sm font-medium transition-colors flex items-center gap-2"
        >
          {saving && (
            <svg className="animate-spin h-4 w-4 text-white" fill="none" viewBox="0 0 24 24">
              <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
              <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
            </svg>
          )}
          {saving ? 'Saving...' : 'Save Changes'}
        </button>
      </div>
    </div>
  );
};

export default AccountSetting;