export const validateRegistrationForm = (formData) => {
    const errors = {};

    // Validate First Name
    if (!formData.firstName.trim()) {
        errors.firstName = "First name is required";
    } else if (formData.firstName.trim().length < 2) {
        errors.firstName = "First name must be at least 2 characters";
    }

    // Validate Last Name
    if (!formData.lastName.trim()) {
        errors.lastName = "Last name is required";
    } else if (formData.lastName.trim().length < 2) {
        errors.lastName = "Last name must be at least 2 characters";
    }

    // Validate Street Address
    if (!formData.streetAddress.trim()) {
        errors.streetAddress = "Street address is required";
    }

    // Validate City
    if (!formData.city.trim()) {
        errors.city = "City is required";
    }

    // Validate State
    if (!formData.state.trim()) {
        errors.state = "State/Province is required";
    }

    // Validate Role
    if (!formData.role) {
        errors.role = "Please select a role";
    }

    // Validate Country
    if (!formData.country.trim()) {
        errors.country = "Country is required";
    }

    // Validate Email
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!formData.email.trim()) {
        errors.email = "Email is required";
    } else if (!emailRegex.test(formData.email)) {
        errors.email = "Please enter a valid email address";
    }

    // Validate Phone Number
    const phoneRegex = /^0\d{9}$/;
    if (!formData.phoneNumber.trim()) {
        errors.phoneNumber = "Phone number is required";
    } else if (!phoneRegex.test(formData.phoneNumber.replace(/[\s\-\(\)]/g, ''))) {
        errors.phoneNumber = "Please enter a valid phone number";
    }

    // Validate Password
    if (!formData.password) {
        errors.password = "Password is required";
    } else if (formData.password.length < 8) {
        errors.password = "Password must be at least 8 characters long";
    } else if (!/(?=.*[a-z])(?=.*[A-Z])(?=.*\d)/.test(formData.password)) {
        errors.password = "Password must contain at least one uppercase letter, one lowercase letter, and one number";
    }

    // Validate Confirm Password
    if (!formData.confirmPassword) {
        errors.confirmPassword = "Please confirm your password";
    } else if (formData.password !== formData.confirmPassword) {
        errors.confirmPassword = "Passwords do not match";
    }

    // Validate Terms Agreement
    if (!formData.agree) {
        errors.agree = "You must agree to the Terms and Privacy Policies";
    }

    return {
        isValid: Object.keys(errors).length === 0,
        errors
    };
};


export const validateLoginForm = (formData) => {
    const errors = {};

    // Validate Email
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!formData.email.trim()) {
        errors.email = "Email is required";
    } else if (!emailRegex.test(formData.email)) {
        errors.email = "Please enter a valid email address";
    }

    // Validate Password
    if (!formData.password) {
        errors.password = "Password is required";
    } else if (formData.password.length < 8) {
        errors.password = "Password must be at least 8 characters long";
    } else if (!/(?=.*[a-z])(?=.*[A-Z])(?=.*\d)/.test(formData.password)) {
        errors.password = "Password must contain at least one uppercase letter, one lowercase letter, and one number";
    }

    return {
        isValid: Object.keys(errors).length === 0,
        errors
    };
};


export const validateForgotPasswordForm = (formData) => {
    const errors = {};

    // Validate Email
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!formData.email.trim()) {
        errors.email = "Email is required";
    } else if (!emailRegex.test(formData.email)) {
        errors.email = "Please enter a valid email address";
    }

    return {
        isValid: Object.keys(errors).length === 0,
        errors
    };
};


export const validateVerifyForm = (formData) => {
    const errors = {};

    // Validate Code
    if (!formData.code.trim()) {
        errors.code = "Code is required";
    }

    return {
        isValid: Object.keys(errors).length === 0,
        errors
    };
};


export const validateSetPasswordForm = (formData) => {
    const errors = {};

    // Validate Password
    if (!formData.password) {
        errors.password = "Password is required";
    } else if (formData.password.length < 8) {
        errors.password = "Password must be at least 8 characters long";
    } else if (!/(?=.*[a-z])(?=.*[A-Z])(?=.*\d)/.test(formData.password)) {
        errors.password = "Password must contain at least one uppercase letter, one lowercase letter, and one number";
    }

    // Validate Confirm Password
    if (!formData.confirmPassword) {
        errors.confirmPassword = "Please confirm your password";
    } else if (formData.password !== formData.confirmPassword) {
        errors.confirmPassword = "Passwords do not match";
    }

    return {
        isValid: Object.keys(errors).length === 0,
        errors
    };
};

// Helper function to check if email already exists (for future API integration)
export const checkEmailExists = async (email) => {
    // This would typically make an API call
    // For now, return false (email doesn't exist)
    return false;
};

// Helper function to format phone number
export const formatPhoneNumber = (phone) => {
    const cleaned = phone.replace(/\D/g, '');
    const match = cleaned.match(/^(\d{3})(\d{3})(\d{4})$/);
    if (match) {
        return `(${match[1]}) ${match[2]}-${match[3]}`;
    }
    return phone;
};