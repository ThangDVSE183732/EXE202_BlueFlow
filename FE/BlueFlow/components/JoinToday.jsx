function JoinToday() {
    return(
        <div className="grid grid-cols-10 h-full">
              <div className="col-span-4">
                <img  className="object-cover h-full w-full" src="/imgs/StudyBack.png" alt="Placeholder Image" />
              </div>
  <div class="col-span-6 bg-gradient-to-br from-sky-300 to-blue-400 text-left pl-17 pt-13">
    <h3 className="text-white text-lg font-semibold mb-6">Tham gia EventLink ngay hôm nay!</h3>
    <h1 className="text-white text-5xl font-semibold mb-2">Tạo tài khoản</h1>
    <h1 className="text-white text-5xl font-semibold mb-6">Vào hành trình ngay</h1>
    <p className="text-white text-sm">Cho dù bạn đang tổ chức, tài trợ hay khám phá sự kiện</p>
    <p className="text-white text-sm mb-15">hãy đăng ký và mở khóa trải nghiệm cá nhân hóa của bạn.</p>
    <button className="text-xl text-sky-500 bg-white w-40 h-10 rounded-lg">Bắt đầu</button>
  </div>
        </div>
    )
}
export default JoinToday;