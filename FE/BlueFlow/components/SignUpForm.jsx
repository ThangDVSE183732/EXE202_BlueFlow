import { useState } from 'react';
import {validateRegistrationForm} from '../utils/validation';
import {useToast} from '../hooks/useToast';
import ToastContainer from './ToastContainer';
import { useNavigate } from 'react-router-dom';
import { Link } from 'react-router-dom';
import { authService } from '../services/userService';

function SignUpForm() {
    const [formData, setFormData] = useState({
        firstName: '',
        lastName: '',
        streetAddress: '',
        city: '',
        state: '',
        role: '',
        country: '',
        email: '',
        phoneNumber: '',
        password: '',
        confirmPassword: '',
        agree: false
    });
        const { toasts, showToast, removeToast } = useToast();
        const navigate = useNavigate();



    const handleInputChange = (e) => {
        const { name, value, type, checked } = e.target;
        setFormData(prev => ({
            ...prev,
            [name]: type === 'checkbox' ? checked : value
        }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        const validation = validateRegistrationForm(formData);

        if(!validation.isValid) {
            const errorCount = Object.keys(validation.errors).length;
            const firstError = Object.values(validation.errors)[0];
            
            showToast({
                type: 'error',
                title: 'Validation Error',
                message: `${firstError}${errorCount > 1 ? ` (and ${errorCount - 1} more error${errorCount > 2 ? 's' : ''})` : ''}`
            });
            return;
        }

        try {

            // Bước 1: Gọi API login
                        const response = await authService.register({
                            email: formData.email,
                            password: formData.password,
                            confirmPassword: formData.confirmPassword,
                            fullName: `${formData.firstName} ${formData.lastName}`,
                            role: formData.role,
                            phoneNumber: formData.phoneNumber,
                        });
            
                        if (response.success) {
                            showToast({
                                type: 'success',
                                title: 'Success!',
                                message: response.message || 'OTP has been sent to your email!'
                            });
            
                            // Chuyển đến trang verify OTP với email
                            navigate('/verify-code', { 
                                state: { 
                                    email: formData.email,
                                    from: 'sign-up'
                                }
                            });
                        } else {
                            showToast({
                                type: 'error',
                                title: 'Register Failed',
                                message: response.message || 'Register failed. Please try again.'
                            });
                        }

        }catch (error) {
			console.error('Register error:', error);
            
            showToast({
                type: 'error',
                title: 'Register Failed',
                message: error.message || 'Register failed. Please try again.'
            });
		} 
        
    };

    return(
        <>
        <ToastContainer toasts={toasts} onRemoveToast={removeToast} />
        <div className="bg-white w-[60%] h-full overflow-auto rounded-2xl ">
            <h1 className="text-4xl text-blue-400 font-semibold mt-12 mb-11">Registration</h1>
            <form onSubmit={handleSubmit}>
                <div className="">
                    <h3 className="text-left ml-12 font-semibold mb-4">Name</h3>
                    <div className="grid grid-cols-2 space-x-4 mb-6">
                    <div className="ml-12">
                        <h6 className="text-xs text-left mb-2 text-sky-400">First Name</h6>
                        <input 
                            type="text" 
                            name="firstName"
                            value={formData.firstName}
                            onChange={handleInputChange}
                            className="p-1 border border-sky-300 rounded-lg w-full bg-sky-50" 
                        />
                    </div>
                    <div className="mr-12">
                        <h6 className="text-xs text-left mb-2 text-sky-400">Last Name</h6>
                        <input 
                            type="text" 
                            name="lastName"
                            value={formData.lastName}
                            onChange={handleInputChange}
                            className="p-1 border border-sky-300 rounded-lg w-full bg-sky-50" 
                        />
                    </div>
        
                    </div>
                    <div className="ml-12 mr-12 mb-4">
                        <h3 className=" text-left  font-semibold mb-4 ">Information</h3>
                        <h6 className="text-xs text-left mb-2 text-sky-400">Street Address</h6>
                        <input 
                            type="text" 
                            name="streetAddress"
                            value={formData.streetAddress}
                            onChange={handleInputChange}
                            className="p-1 border border-sky-300 bg-sky-50 rounded-lg w-full " 
                        />
                    </div>

                    <div className="grid grid-cols-2 space-x-4 mb-4">
                    <div className="ml-12">
                        <h6 className="text-xs text-left mb-2 text-sky-400">City</h6>
                        <input 
                            type="text" 
                            name="city"
                            value={formData.city}
                            onChange={handleInputChange}
                            className="p-1 border border-sky-300 rounded-lg w-full bg-sky-50" 
                        />
                    </div>
                    <div className="mr-12">
                        <h6 className="text-xs text-left mb-2 text-sky-400">State/Provide</h6>
                        <input 
                            type="text" 
                            name="state"
                            value={formData.state}
                            onChange={handleInputChange}
                            className="p-1 border border-sky-300 rounded-lg w-full bg-sky-50" 
                        />
                    </div>
        
                    </div>
                    <div className="grid grid-cols-2 space-x-4 mb-6">
                    <div className="ml-12">
                        <h6 className="text-xs text-left mb-2 text-sky-400 ">Role</h6>
                        <select 
                            name="role"
                            value={formData.role}
                            onChange={handleInputChange}
                            className="p-1 py-1.5 border border-sky-300 rounded-lg w-full bg-sky-50 text-gray-400 text-sm"
                        >
                            <option value="">Select a role</option>
                            <option value="Organizer">Organizer</option>
                            <option value="Sponsor">Sponsor</option>
                            <option value="Supplier">Supplier</option>
                        </select>
                    </div>
                    <div className="mr-12">
                        <h6 className="text-xs text-left mb-2 text-sky-400 ">Country</h6>
                        <input 
                            type="text" 
                            name="country"
                            value={formData.country}
                            onChange={handleInputChange}
                            className="p-1 border border-sky-300 rounded-lg w-full bg-sky-50" 
                        />
                    </div>
                    </div>

                    <div className="grid grid-cols-2 space-x-4 mb-8">
                    <div className="ml-12">
                        <h3 className=" text-left mb-4 font-semibold">Email</h3>
                        <input 
                            type="email" 
                            name="email"
                            value={formData.email}
                            onChange={handleInputChange}
                            className="p-1 border border-sky-300 rounded-lg w-full bg-sky-50" 
                        />
                    </div>
                    <div className="mr-12">
                        <h3 className="text-left font-semibold mb-4">Phone Number</h3>
                        <input 
                            type="text" 
                            name="phoneNumber"
                            value={formData.phoneNumber}
                            onChange={handleInputChange}
                            className="p-1 border border-sky-300 rounded-lg w-full bg-sky-50" 
                        />
                    </div>
                    </div>

                    <div className="grid grid-cols-2 space-x-4 mb-6">
                    <div className="ml-12">
                        <h3 className=" text-left mb-4 font-semibold">Password</h3>
                        <input 
                            type="password" 
                            name="password"
                            value={formData.password}
                            onChange={handleInputChange}
                            className="p-1 border border-sky-300 rounded-lg w-full bg-sky-50" 
                        />
                    </div>
                    <div className="mr-12">
                        <h3 className="text-left font-semibold mb-4">Confirm Password</h3>
                        <input 
                            type="password" 
                            name="confirmPassword"
                            value={formData.confirmPassword}
                            onChange={handleInputChange}
                            className="p-1 border border-sky-300 rounded-lg w-full bg-sky-50" 
                        />
                    </div>
                    </div>

                    <div className="text-left ml-12 mb-16">
                      <label htmlFor="agree" className="inline-flex items-center gap-3 text-gray-700">
                        <input
                          id="agree"
                          name="agree"
                          type="checkbox"
                          checked={formData.agree}
                          onChange={handleInputChange}
                          className="h-5 w-5 rounded border-gray-300 accent-blue-500"
                          required
                        />
                        <span className="text-sm">
                          I agree to all the
                          <a href="/terms" className="mx-1 text-red-600 font-semibold hover:underline">Terms</a>
                          and
                          <a href="/privacy" className="ml-1 text-red-600 font-semibold hover:underline">Privacy Policies</a>
                        </span>
                      </label>
                    </div>

                    <div className="ml-44 mr-44 mt-6">

                    <button type="submit" className=" w-full p-3 text-center rounded-lg font-medium text-gray-50 bg-blue-400 mb-2">
            Register
          </button>
                  <p className="text-xs font-medium text-center sm:px-6 text-gray-600">Don't have an account?
        <Link  to="/login" className="text-red-600"> Sign in</Link>
    </p>
                    </div>
                    
                </div>
                
            </form>
        </div>
        </>
    );
}
export default SignUpForm;