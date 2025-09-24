import FloatingInput from "./FloatingInput";
import { Link } from "react-router-dom";

function VerifyCode() {
    return (
    <>
      <div className="text-left w-full max-w-md px-8 pt-8 pl-12 text-black">
        <Link
          to="/login"
          className="inline-flex items-center gap-2 text-black font-normal hover:underline leading-none h-6 mb-14"
        >
          <svg className="w-5 h-5" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1" strokeLinecap="round" strokeLinejoin="round">
            <path d="M15 18l-6-6 6-6" />
          </svg>
          <span>Back to Login</span>
        </Link>

        <h1 className="mt-6 text-4xl font-bold text-left w-md mb-4 ml-14">Verify code</h1>
        <p className="text-left text-sm w-sm text-gray-400 mb-10 ml-14">
          An authentication code has been sent to your email.
        </p>

        <form noValidate className="ml-14 w-full">
          <div className="mb-2">
            <FloatingInput id="codeVerify" type="text" label="Enter Code" />
          </div>
          <p className="text-xs font-medium mb-8 text-left  text-gray-600">Didn't receive the code?
		<a rel="noopener noreferrer" href="#" className="text-red-600 text-gray-800"> Resend</a>
        </p>
          <button className="block mb-2 w-full p-3 text-center rounded-lg font-medium text-gray-50 bg-blue-400">
            Verify
          </button>
          
        </form>

        
      </div>
    </>
  );
    
}
export default VerifyCode;