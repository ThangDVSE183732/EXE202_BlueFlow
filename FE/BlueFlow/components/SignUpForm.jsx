
function SignUpForm() {
    return(
        <div className="bg-white w-[60%] h-full overflow-auto rounded-2xl">
            <h1 className="text-4xl text-blue-400 font-semibold mt-12 mb-11">Registration</h1>
            <form>
                <div className="">
                    <h3 className="text-left ml-12 font-semibold mb-4">Name</h3>
                    <div className="grid grid-cols-2 space-x-4 mb-6">
                    <div className="ml-12">
                        <h6 className="text-xs text-left mb-2 text-sky-400">First Name</h6>
                        <input type="text"  className="p-1 border border-sky-300 rounded-lg w-full bg-sky-50" />
                    </div>
                    <div className="mr-12">
                        <h6 className="text-xs text-left mb-2 text-sky-400">Last Name</h6>
                        <input type="text"  className="p-1 border border-sky-300 rounded-lg w-full bg-sky-50" />
                    </div>
        
                    </div>
                    <div className="ml-12 mr-12 mb-4">
                        <h3 className=" text-left  font-semibold mb-4 ">Information</h3>
                        <h6 className="text-xs text-left mb-2 text-sky-400">Street Address</h6>
                        <input type="text"  className="p-1 border border-sky-300 bg-sky-50 rounded-lg w-full " />
                    </div>

                    <div className="grid grid-cols-2 space-x-4 mb-4">
                    <div className="ml-12">
                        <h6 className="text-xs text-left mb-2 text-sky-400">City</h6>
                        <input type="text"  className="p-1 border border-sky-300 rounded-lg w-full bg-sky-50" />
                    </div>
                    <div className="mr-12">
                        <h6 className="text-xs text-left mb-2 text-sky-400">State/Provide</h6>
                        <input type="text"  className="p-1 border border-sky-300 rounded-lg w-full bg-sky-50" />
                    </div>
        
                    </div>
                    <div className="grid grid-cols-2 space-x-4 mb-6">
                    <div className="ml-12">
                        <h6 className="text-xs text-left mb-2 text-sky-400 ">Role</h6>
                        <input type="text"  className="p-1 border border-sky-300 rounded-lg w-full bg-sky-50" />
                    </div>
                    <div className="mr-12">
                        <h6 className="text-xs text-left mb-2 text-sky-400 ">Country</h6>
                        <input type="text"  className="p-1 border border-sky-300 rounded-lg w-full bg-sky-50" />
                    </div>
                    </div>

                    <div className="grid grid-cols-2 space-x-4 mb-8">
                    <div className="ml-12">
                        <h3 className=" text-left mb-4 font-semibold">Email</h3>
                        <input type="text"  className="p-1 border border-sky-300 rounded-lg w-full bg-sky-50" />
                    </div>
                    <div className="mr-12">
                        <h3 className="text-left font-semibold mb-4">Phone Number</h3>
                        <input type="text"  className="p-1 border border-sky-300 rounded-lg w-full bg-sky-50" />
                    </div>
                    </div>

                    <div className="grid grid-cols-2 space-x-4 mb-6">
                    <div className="ml-12">
                        <h3 className=" text-left mb-4 font-semibold">Password</h3>
                        <input type="password"  className="p-1 border border-sky-300 rounded-lg w-full bg-sky-50" />
                    </div>
                    <div className="mr-12">
                        <h3 className="text-left font-semibold mb-4">Confirm Password</h3>
                        <input type="password"  className="p-1 border border-sky-300 rounded-lg w-full bg-sky-50" />
                    </div>
                    </div>

                    <div className="text-left ml-12 mb-16">
                      <label htmlFor="agree" className="inline-flex items-center gap-3 text-gray-700">
                        <input
                          id="agree"
                          name="agree"
                          type="checkbox"
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

                    <button className=" w-full p-3 text-center rounded-lg font-medium text-gray-50 bg-blue-400 mb-2">
            Register
          </button>
          		<p className="text-xs font-medium text-center sm:px-6 text-gray-600">Don't have an account?
		<a rel="noopener noreferrer" href="#" className="text-red-600 text-gray-800"> Sign up</a>
	</p>
                    </div>
                    
                </div>
                
            </form>
        </div>
    );
}
export default SignUpForm;