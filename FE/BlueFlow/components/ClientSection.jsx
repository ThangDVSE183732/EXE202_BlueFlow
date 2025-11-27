function ClientSection() {
    return (
        <div>
            <h1 className=" text-3xl font-semibold mb-2">Khách hàng của chúng tôi</h1>
            <h6 className="text-sm text-gray-500 mb-7">Chúng tôi đã và đang hợp tác với hơn 50 khách hàng.</h6>
            <div className="flex justify-center items-center space-x-25">
                <img style={{ transform: "scale(0.8)" }} src="/imgs/Logo (1).png" alt="Client 1" />
                <img style={{ transform: "scale(0.8)" }} src="/imgs/Logo (2).png" alt="Client 2" />
                <img style={{ transform: "scale(0.8)" }} src="/imgs/Logo (3).png" alt="Client 3" />
                <img style={{ transform: "scale(0.8)" }} src="/imgs/Logo (4).png" alt="Client 4" />
                <img style={{ transform: "scale(0.8)" }} src="/imgs/Logo (5).png" alt="Client 5" />
                <img style={{ transform: "scale(0.8)" }} src="/imgs/Logo (6).png" alt="Client 6" />
                <img style={{ transform: "scale(0.8)" }} src="/imgs/Logo (7).png" alt="Client 7" />
            </div>
        </div>
    )
}

export default ClientSection