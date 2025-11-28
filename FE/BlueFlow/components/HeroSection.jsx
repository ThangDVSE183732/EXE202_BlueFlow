import { useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

function HeroSection() {
    const navigate = useNavigate();
    const { isAuthenticated, userRole } = useAuth();

    const handleStartNow = () => {
        if (!isAuthenticated) {
            // Nếu chưa đăng nhập, chuyển đến trang login
            navigate('/login');
            return;
        }

        // Chuyển đến trang tương ứng với role của user
        const role = userRole?.toLowerCase();
        if (role === 'organizer') {
            navigate('/organizer');
        } else if (role === 'sponsor') {
            navigate('/sponsor');
        } else if (role === 'supplier') {
            navigate('/supplier');
        } else {
            // Default: chuyển đến trang login nếu role không xác định
            navigate('/login');
        }
    };

    return (
        <div className="text-left" >
            <h1 className="text-7xl font-bold bg-gradient-to-b from-sky-300 to-blue-400 bg-clip-text text-transparent">EVENTLINK</h1>
            <h3 className="text-4xl font-semibold text-white mt-4 mb-3">Connecting</h3>
            <h4 className="text-gray-300 font-normal mb-16 text-lg">ORGANIZERS - SPONSORS - SERVICE PROVIDERS</h4>
            <button 
                onClick={handleStartNow}
                className="bg-blue-400 w-44 h-10 rounded-lg text-white text-lg font-medium hover:bg-blue-500 transition-colors cursor-pointer"
            >
                Bắt đầu ngay
            </button>
        </div>
    )
}
export default HeroSection;