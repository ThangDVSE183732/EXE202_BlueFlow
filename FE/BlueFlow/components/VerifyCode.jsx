import FloatingInput from "./FloatingInput";
import { Link, useLocation } from "react-router-dom";
import {useToast} from '../hooks/useToast';
import ToastContainer from './ToastContainer';
import { useNavigate } from 'react-router-dom';
import { useState } from "react";
import { validateVerifyForm } from "../utils/validation";
import { authService } from "../services/userService";
import { useAuth } from '../contexts/AuthContext';

function VerifyCode() {

  const [formData, setFormData] = useState({
        code: '',
    });
  const handleInputChange = (name, value) => {
        setFormData(prev => ({
            ...prev,
            [name]: value
        }));
    };

     const { toasts, showToast, removeToast } = useToast();
     const navigate = useNavigate();
     const { login } = useAuth();
     const location = useLocation();

     const email = location.state?.email;
     const fromPage = location.state?.from; // Mặc định là 'login' nếu không có giá trị
     const profileData = location.state?.profileData; // Dữ liệu profile từ SignUpForm


    //  const handleSubmit = (e) => {
    //       e.preventDefault();
    //       const validation = validateVerifyForm(formData);
    //       if(validation.isValid) {

    //         // Navigation will be handled by RoleBasedRedirect component
    //         navigate('/set-password');
    //       } else {
    //         console.log('Validation Errors:', validation.errors);
    //          const errorCount = Object.keys(validation.errors).length;
    //          const firstError = Object.values(validation.errors)[0];

    //         showToast({
    //           type: 'error',
    //           title: 'Validation Error',
    //           message: `${firstError}${errorCount > 1 ? ` (and ${errorCount - 1} more error${errorCount > 2 ? 's' : ''})` : ''}`
    //         });
    //       }
    //     };



    const handleSubmit = async (e) => {
    e.preventDefault();
    const validation = validateVerifyForm(formData);
    
    if (!validation.isValid) {
      const errorCount = Object.keys(validation.errors).length;
      const firstError = Object.values(validation.errors)[0];

      showToast({
        type: 'error',
        title: 'Validation Error',
        message: `${firstError}${errorCount > 1 ? ` (and ${errorCount - 1} more error${errorCount > 2 ? 's' : ''})` : ''}`
      });
      return;
    }

    if (!email) {
      showToast({
        type: 'error',
        title: 'Error',
        message: 'Email not found. Please start from login page.'
      });
      console.log('Email not found in state');
      navigate('/login');
      return;
    }

    try {
      let response;
      // Gọi API verify OTP
      if(fromPage === 'sign-up') {
         response = await authService.verifyOTPRegister({
          otpRequest: {
            email: email,
            otp: formData.code
          },
          profileRequest: {
            companyName: profileData?.companyName || "",
            // companyLogoUrl: profileData?.companyLogoUrl || "",
            industry: profileData?.industry || "",
            companySize: profileData?.companySize || "",
            foundedYear: profileData?.foundedYear || 0,
            aboutUs: profileData?.aboutUs || "",
            mission: profileData?.mission || "",
            companyDescription: profileData?.companyDescription || "",
            socialProfile: profileData?.socialProfile || "",
            linkedInProfile: profileData?.linkedInProfile || "",
            officialEmail: profileData?.officialEmail || "",
            stateProvince: profileData?.stateProvince || "",
            countryRegion: profileData?.countryRegion || "",
            city: profileData?.city || "",
            streetAddress: profileData?.streetAddress || "",
            tags: profileData?.tags || "",
            contactFullName: profileData?.contactFullName || "",
            jobTitle: profileData?.jobTitle || "",
            directEmail: profileData?.directEmail || "",
            directPhone: profileData?.directPhone || ""
          }
      });
      } else {
         response = await authService.verifyOTP({
          email: email,
          otp: formData.code
        });
      }

      if (response.success) {
        showToast({
          type: 'success',
          title: 'Success!',
          message: response.message || 'OTP verified successfully!'
        });

        // Lưu thông tin user vào context
        if (response.data?.user) {
          login(response.data.user);
        }

        // Điều hướng dựa trên từ đâu đến
        if (fromPage === 'login' || fromPage === 'sign-up') {
          // Đăng nhập thành công - chuyển về trang chính
          navigate('/organizer');
        } else {
          // Reset password flow - chuyển đến set password
          navigate('/set-password', { 
            state: { 
              email: email,
              verifiedToken: response.data?.token 
            }
          });
        }
      } else {
        showToast({
          type: 'error',
          title: 'Verification Failed',
          message: response.message || 'Invalid verification code.'
        });
      }
    } catch (error) {
      console.error('Verify OTP error:', error);
      
      showToast({
        type: 'error',
        title: 'Verification Failed',
        message: error.message || 'Verification failed. Please try again.'
      });
    } 
  };


    return (
    <>
      <ToastContainer toasts={toasts} onRemoveToast={removeToast} />
      <div className="text-left w-full max-w-md px-8 pt-8 pl-12 text-black">
        <Link
          to="/forgot-password"
          className="inline-flex items-center gap-2 text-black font-normal hover:underline leading-none h-6 mb-14"
        >
          <svg className="w-5 h-5" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1" strokeLinecap="round" strokeLinejoin="round">
            <path d="M15 18l-6-6 6-6" />
          </svg>
          <span>Back</span>
        </Link>

        <h1 className="mt-6 text-4xl font-bold text-left w-md mb-4 ml-14">Verify code</h1>
        <p className="text-left text-sm w-sm text-gray-400 mb-10 ml-14">
          An authentication code has been sent to your email.
        </p>

        <form onSubmit={handleSubmit} className="ml-14 w-full">
          <div className="mb-2">
            <FloatingInput id="codeVerify" type="text" label="Enter Code" value={formData.code}
              onChange={(value) => handleInputChange('code', value)} />
          </div>
          <p className="text-xs font-medium mb-8 text-left  text-gray-600">Didn't receive the code?
		<Link rel="noopener noreferrer" to="#" className="text-red-600 text-gray-800"> Resend</Link>
        </p>
          <button type="submit" className="block mb-2 w-full p-3 text-center rounded-lg font-medium text-gray-50 bg-blue-400">
            Verify
          </button>
          
        </form>

        
      </div>
    </>
  );
    
}
export default VerifyCode;