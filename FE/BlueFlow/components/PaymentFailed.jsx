import { useEffect, useState, useRef } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { usePayment } from '../hooks/usePayment';
import toast from 'react-hot-toast';

const PaymentFailed = () => {
  const navigate = useNavigate();
  const { userRole } = useAuth();
  const { verifyPayment } = usePayment();
  const [searchParams] = useSearchParams();
  const [isVerifying, setIsVerifying] = useState(true);
  const hasVerified = useRef(false);

  useEffect(() => {
    // Chỉ chạy 1 lần duy nhất
    if (hasVerified.current) return;
    
    const verifyAndRedirect = async () => {
      hasVerified.current = true;
      
      // Lấy orderCode từ URL params
      const orderCode = searchParams.get('orderCode');
      
      if (orderCode) {
        try {
          console.log('Verifying failed payment with orderCode:', orderCode);
          await verifyPayment({ orderCode });
          toast.error('Thanh toán thất bại!');
        } catch (error) {
          console.error('Verify payment error:', error);
          toast.error('Thanh toán không thành công');
        } finally {
          setIsVerifying(false);
        }
      } else {
        console.warn('No orderCode found in URL');
        toast.error('Thanh toán thất bại!');
        setIsVerifying(false);
      }
    };

    verifyAndRedirect();
  }, [searchParams, verifyPayment]);

  useEffect(() => {
    if (isVerifying) return;

    // Auto redirect sau 3 giây
    const timer = setTimeout(() => {
      const role = userRole?.toLowerCase();
      console.log('PaymentFailed - Role from AuthContext:', role);
      if (role) {
        console.log('Navigating to:', `/${role}`);
        navigate(`/${role}`, { state: { activeTab: 'projects' } });
      } else {
        console.log('No role found, navigating to /pricing');
        navigate('/pricing');
      }
    }, 3000);

    return () => clearTimeout(timer);
  }, [navigate, userRole, isVerifying]);

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-b from-gray-50 to-white p-4">
      <div className="max-w-md w-full bg-white rounded-3xl shadow-2xl p-8 text-center relative overflow-hidden">
        {/* Decorative dots animation */}
        <div className="absolute inset-0 overflow-hidden pointer-events-none">
          {[...Array(20)].map((_, i) => (
            <div
              key={i}
              className="absolute bg-red-400 rounded-full animate-float"
              style={{
                width: `${Math.random() * 12 + 4}px`,
                height: `${Math.random() * 12 + 4}px`,
                top: `${Math.random() * 100}%`,
                left: `${Math.random() * 100}%`,
                opacity: Math.random() * 0.5 + 0.2,
                animationDelay: `${Math.random() * 3}s`,
                animationDuration: `${Math.random() * 3 + 2}s`,
              }}
            />
          ))}
        </div>

        {/* Failed Icon */}
        <div className="relative z-10 mb-8">
          <div className="inline-flex items-center justify-center w-32 h-32 bg-gradient-to-br from-red-400 to-red-500 rounded-full shadow-xl animate-bounce-slow">
            <svg
              className="w-16 h-16 text-white"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={3}
                d="M6 18L18 6M6 6l12 12"
              />
            </svg>
          </div>
        </div>

        {/* Failed Message */}
        <div className="relative z-10 mb-8">
          <h1 className="text-3xl font-bold text-gray-900 mb-4">
            {isVerifying ? 'Đang xác minh thanh toán...' : 'Thanh toán thất bại!'}
          </h1>
          <p className="text-gray-500 text-base leading-relaxed">
            {isVerifying
              ? 'Vui lòng đợi trong giây lát...'
              : 'Rất tiếc, giao dịch của bạn không thể hoàn thành. Vui lòng thử lại hoặc liên hệ bộ phận hỗ trợ.'
            }
          </p>
        </div>

        {/* Optional: Loading indicator */}
        <div className="relative z-10">
          <div className="flex justify-center items-center gap-2 text-sm text-gray-400">
            <div className="w-2 h-2 bg-red-400 rounded-full animate-pulse"></div>
            <div className="w-2 h-2 bg-red-400 rounded-full animate-pulse" style={{ animationDelay: '0.2s' }}></div>
            <div className="w-2 h-2 bg-red-400 rounded-full animate-pulse" style={{ animationDelay: '0.4s' }}></div>
          </div>
        </div>
      </div>

      <style>{`
        @keyframes float {
          0%, 100% {
            transform: translateY(0) translateX(0);
          }
          50% {
            transform: translateY(-20px) translateX(10px);
          }
        }
        
        @keyframes bounce-slow {
          0%, 100% {
            transform: scale(1);
          }
          50% {
            transform: scale(1.05);
          }
        }
        
        .animate-float {
          animation: float infinite ease-in-out;
        }
        
        .animate-bounce-slow {
          animation: bounce-slow 2s infinite ease-in-out;
        }
      `}</style>
    </div>
  );
};

export default PaymentFailed;
