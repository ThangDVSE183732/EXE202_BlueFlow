import ForgotYourPassword from "../components/ForgotYourPassword";
import LoginForm from "../components/LoginForm";
import PasswordChanged from "../components/PasswordChanged";
import SetPassword from "../components/SetPassword";
import VerifyCode from "../components/VerifyCode";

function Login() {
  return (
    <div className="grid grid-cols-10 w-screen h-screen overflow-hidden">
      <div
        className="col-span-5 h-full grid items-start justify-items-start bg-no-repeat bg-cover bg-center"
        style={{ backgroundImage: "url('/imgs/waveVector.jpg')" }}
      >
        {/* <LoginForm /> */}
        {/* <ForgotYourPassword/> */}
        {/* <VerifyCode/> */}
        {/* <SetPassword /> */}
        <PasswordChanged/>
      </div>
      <div className="col-span-5 grid h-full ">
        <div className="w-full h-screen overflow-hidden">
          <img
            src="/imgs/pexels.jpg"
            alt="login"
            className="block w-full h-full object-cover" // đổi 'object-contain' nếu không muốn ảnh bị cắt
          />
        </div>
      </div>
    </div>
  );
}
export default Login;