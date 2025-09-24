function PasswordChanged() {
    return (
      <div className="text-center w-full max-w-md px-8 pt-35 ml-20 text-black">
        <img src="/imgs/success.png" alt="success" className="mx-auto mb-6 w-20 h-20"/>
        <h1 className="mt-6 text-3xl font-bold w-md mb-2 text-left ml-14">Password Changed</h1>
        <p className=" text-base w-sm text-gray-500 mb-10 ">
          Your password has been changed successfully.
        </p>

        <form noValidate className="w-2xs ml-12">
          
          <button className="block mb-2 w-full p-3 text-center rounded-lg font-medium text-gray-50 bg-blue-400">
            Back to Login
          </button>
          
        </form>

        
      </div>
  );
}

export default PasswordChanged;