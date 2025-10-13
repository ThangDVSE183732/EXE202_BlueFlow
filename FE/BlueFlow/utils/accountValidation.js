// Form validation constants and utilities for Account Settings

export const VALIDATION_RULES = {
  REQUIRED_FIELDS: [
    'companyName',
    'industry', 
    'officialEmail',
    'companyPhone',
    'country',
    'city',
    'contactFullName',
    'contactJobTitle',
    'contactDirectEmail',
    'contactDirectPhone'
  ],

  EMAIL_REGEX: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
  URL_REGEX: /^https?:\/\/.+/,
  PHONE_REGEX: /^[\+]?[1-9][\d]{0,15}$/,

  MAX_FILE_SIZE: 2 * 1024 * 1024, // 2MB
  ALLOWED_FILE_TYPES: ['image/jpeg', 'image/jpg', 'image/png', 'image/gif'],

  MAX_DESCRIPTION_LENGTH: 500,
  MIN_COMPANY_NAME_LENGTH: 2,
  MAX_COMPANY_NAME_LENGTH: 100
};

export const FORM_FIELDS = {
  COMPANY_INFO: {
    companyName: {
      label: 'Company Name',
      required: true,
      placeholder: 'Enter company name'
    },
    industry: {
      label: 'Industry',
      required: true,
      placeholder: 'Select industry'
    },
    companySize: {
      label: 'Company Size',
      required: false,
      placeholder: 'Select company size'
    },
    foundedYear: {
      label: 'Founded Year',
      required: false,
      placeholder: 'e.g., 2015'
    },
    website: {
      label: 'Website',
      required: false,
      placeholder: 'https://www.company.com'
    },
    linkedinProfile: {
      label: 'LinkedIn Profile',
      required: false,
      placeholder: 'https://linkedin.com/company/yourcompany'
    },
    companyDescription: {
      label: 'Company Description',
      required: false,
      placeholder: 'Describe your company...'
    }
  },

  CONTACT_INFO: {
    officialEmail: {
      label: 'Official Email',
      required: true,
      placeholder: 'company@example.com'
    },
    companyPhone: {
      label: 'Company Phone',
      required: true,
      placeholder: '+84 28 1234 5678'
    },
    country: {
      label: 'Country/Region',
      required: true,
      placeholder: 'Select country'
    },
    city: {
      label: 'City',
      required: true,
      placeholder: 'Enter city'
    },
    fullAddress: {
      label: 'Full Address',
      required: false,
      placeholder: 'Enter full address'
    }
  },

  CONTACT_PERSON: {
    contactFullName: {
      label: 'Full Name',
      required: true,
      placeholder: 'Enter full name'
    },
    contactJobTitle: {
      label: 'Job Title',
      required: true,
      placeholder: 'Enter job title'
    },
    contactDirectEmail: {
      label: 'Direct Email',
      required: true,
      placeholder: 'person@example.com'
    },
    contactDirectPhone: {
      label: 'Direct Phone',
      required: true,
      placeholder: '+84 90 123 4567'
    }
  }
};

export const DROPDOWN_OPTIONS = {
  INDUSTRIES: [
    { value: '', label: 'Select industry' },
    { value: 'Technology & Software', label: 'Technology & Software' },
    { value: 'Finance & Banking', label: 'Finance & Banking' },
    { value: 'Healthcare', label: 'Healthcare' },
    { value: 'Education', label: 'Education' },
    { value: 'Manufacturing', label: 'Manufacturing' },
    { value: 'Retail & E-commerce', label: 'Retail & E-commerce' },
    { value: 'Real Estate', label: 'Real Estate' },
    { value: 'Automotive', label: 'Automotive' },
    { value: 'Food & Beverage', label: 'Food & Beverage' },
    { value: 'Other', label: 'Other' }
  ],

  COMPANY_SIZES: [
    { value: '', label: 'Select company size' },
    { value: '1-10 employees', label: '1-10 employees' },
    { value: '11-50 employees', label: '11-50 employees' },
    { value: '51-200 employees', label: '51-200 employees' },
    { value: '201-500 employees', label: '201-500 employees' },
    { value: '501-1000 employees', label: '501-1000 employees' },
    { value: '1000+ employees', label: '1000+ employees' }
  ],

  COUNTRIES: [
    { value: '', label: 'Select country' },
    { value: 'Vietnam', label: 'Vietnam' },
    { value: 'United States', label: 'United States' },
    { value: 'Singapore', label: 'Singapore' },
    { value: 'Thailand', label: 'Thailand' },
    { value: 'Malaysia', label: 'Malaysia' },
    { value: 'Philippines', label: 'Philippines' },
    { value: 'Indonesia', label: 'Indonesia' },
    { value: 'Japan', label: 'Japan' },
    { value: 'South Korea', label: 'South Korea' },
    { value: 'Australia', label: 'Australia' }
  ]
};

// Validation utility functions
export const validateField = (fieldName, value, formData = {}) => {
  const rules = VALIDATION_RULES;
  
  // Check required fields
  if (rules.REQUIRED_FIELDS.includes(fieldName) && !value?.trim()) {
    return `${FORM_FIELDS.COMPANY_INFO[fieldName]?.label || 
             FORM_FIELDS.CONTACT_INFO[fieldName]?.label || 
             FORM_FIELDS.CONTACT_PERSON[fieldName]?.label || 
             fieldName} is required`;
  }

  // Email validation
  if ((fieldName === 'officialEmail' || fieldName === 'contactDirectEmail') && value) {
    if (!rules.EMAIL_REGEX.test(value)) {
      return 'Please enter a valid email address';
    }
  }

  // URL validation
  if ((fieldName === 'website' || fieldName === 'linkedinProfile') && value) {
    if (!rules.URL_REGEX.test(value)) {
      return 'Please enter a valid URL starting with http:// or https://';
    }
  }

  // Phone validation
  if ((fieldName === 'companyPhone' || fieldName === 'contactDirectPhone') && value) {
    if (!rules.PHONE_REGEX.test(value.replace(/\s/g, ''))) {
      return 'Please enter a valid phone number';
    }
  }

  // Company name length validation
  if (fieldName === 'companyName' && value) {
    if (value.length < rules.MIN_COMPANY_NAME_LENGTH) {
      return `Company name must be at least ${rules.MIN_COMPANY_NAME_LENGTH} characters`;
    }
    if (value.length > rules.MAX_COMPANY_NAME_LENGTH) {
      return `Company name must not exceed ${rules.MAX_COMPANY_NAME_LENGTH} characters`;
    }
  }

  // Description length validation
  if (fieldName === 'companyDescription' && value) {
    if (value.length > rules.MAX_DESCRIPTION_LENGTH) {
      return `Description must not exceed ${rules.MAX_DESCRIPTION_LENGTH} characters`;
    }
  }

  return null;
};

// Validate entire form
export const validateForm = (formData) => {
  const errors = {};
  
  // Validate all fields
  Object.keys(formData).forEach(fieldName => {
    const error = validateField(fieldName, formData[fieldName], formData);
    if (error) {
      errors[fieldName] = error;
    }
  });

  return errors;
};

// File validation
export const validateFile = (file) => {
  const rules = VALIDATION_RULES;
  
  if (!file) return null;

  // Check file size
  if (file.size > rules.MAX_FILE_SIZE) {
    return 'File size must be less than 2MB';
  }

  // Check file type
  if (!rules.ALLOWED_FILE_TYPES.includes(file.type)) {
    return 'Please select an image file (JPEG, PNG, or GIF)';
  }

  return null;
};