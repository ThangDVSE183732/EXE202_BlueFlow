import { Link } from "react-router-dom";
import FloatingInput from "./FloatingInput";


function SetPassword() {
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

        <h1 className="mt-6 text-4xl font-bold text-left w-md mb-4 ml-14">Set a password</h1>
        <p className="text-left text-sm text-gray-400 mb-10 ml-14 w-sm">
          Your previous password has been reseted. Please set a new password for your account.
        </p>

        <form noValidate className="ml-14 w-full">
          <div className="mb-6">
            <FloatingInput id="newPassword" type="password" label="New Password" />
          </div>
          <div className="space-y-2 text-sm mb-10">
				<FloatingInput id="rePassword" type="password" label="Re-enter Password" />
		</div>
          <button className="block mb-2 w-full p-3 text-center rounded-lg font-medium text-gray-50 bg-blue-400">
            Submit
          </button>
        </form>

      </div>
    </>
  );
}
export default SetPassword;