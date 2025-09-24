function JoinToday() {
    return(
        <div className="grid grid-cols-10 h-full">
              <div className="col-span-4">
                <img  className="object-cover h-full w-full" src="/imgs/StudyBack.png" alt="Placeholder Image" />
              </div>
  <div class="col-span-6 bg-gradient-to-br from-sky-300 to-blue-400 text-left pl-17 pt-13">
    <h3 className="text-white text-lg font-semibold mb-6">Join EventLink Today</h3>
    <h1 className="text-white text-5xl font-semibold mb-2">Create an Account</h1>
    <h1 className="text-white text-5xl font-semibold mb-6">Start your journey now</h1>
    <p className="text-white text-sm">Whether you're organizing, sponsoring, or exploring events</p>
    <p className="text-white text-sm mb-15">sign up and unlock your personalized experience.</p>
    <button className="text-xl text-sky-500 bg-white w-40 h-10 rounded-lg">Get Started</button>
  </div>
        </div>
    )
}
export default JoinToday;