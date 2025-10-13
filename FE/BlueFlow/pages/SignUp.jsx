import SignUpForm from "../components/SignUpForm";
import styles from "./SignUp.module.css";
import Logo from "../components/Logo";
function SignUp() {
    return(
        <div className={styles.signUp} >
            <div className="ml-12 mt-5 mb-10">
            <Logo />
            </div>
            <div className="items-center flex justify-center w-full h-[170%]  mt-15 mb-10 ">
                <SignUpForm/>   
            </div>
        </div>
    );
}
export default SignUp;