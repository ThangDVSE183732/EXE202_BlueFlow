import { Link } from "react-router-dom";
import FloatingInput from "./FloatingInput";
import toast from 'react-hot-toast';
import { useNavigate } from 'react-router-dom';
import { useState } from "react";
import { validateSetPasswordForm } from "../utils/validation";


function SetPassword() {

const [formData, setFormData] = useState({
        password: '',
        confirmPassword: '',
    });
  const handleInputChange = (name, value) => {
        setFormData(prev => ({
            ...prev,
            [name]: value
        }));
    };

     const navigate = useNavigate();


     const handleSubmit = (e) => {
          e.preventDefault();
          const validation = validateSetPasswordForm(formData);
          if(validation.isValid) {

            // Navigation will be handled by RoleBasedRedirect component
            navigate('/password-changed');
          } else {
            console.log('Validation Errors:', validation.errors);
             const errorCount = Object.keys(validation.errors).length;
             const firstError = Object.values(validation.errors)[0];
            
            toast.error(`${firstError}${errorCount > 1 ? ` (and ${errorCount - 1} more error${errorCount > 2 ? 's' : ''})` : ''}`);
          }
        };

      return (
    <>
      <div className="text-left w-full max-w-md px-8 pt-8 pl-12 text-black">
        <Link
          to="/verify-code"
          className="inline-flex items-center gap-2 text-black font-normal hover:underline leading-none h-6 mb-14"
        >
          <svg className="w-5 h-5" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1" strokeLinecap="round" strokeLinejoin="round">
            <path d="M15 18l-6-6 6-6" />
          </svg>
          <span>Back</span>
        </Link>

        <h1 className="mt-6 text-4xl font-bold text-left w-md mb-4 ml-14">Set a password</h1>
        <p className="text-left text-sm text-gray-400 mb-10 ml-14 w-sm">
          Your previous password has been reseted. Please set a new password for your account.
        </p>

        <form onSubmit={handleSubmit} className="ml-14 w-full">
          <div className="mb-6">
            <FloatingInput id="newPassword" type="password" label="New Password" value={formData.password}
              onChange={(value) => handleInputChange('password', value)}/>
          </div>
          <div className="space-y-2 text-sm mb-10">
				<FloatingInput id="rePassword" type="password" label="Re-enter Password" value={formData.confirmPassword}
              onChange={(value) => handleInputChange('confirmPassword', value)}/>
		</div>
          <button type="submit" className="block mb-2 w-full p-3 text-center rounded-lg font-medium text-gray-50 bg-blue-400">
            Submit
          </button>
        </form>

      </div>
    </>
  );
}
export default SetPassword;