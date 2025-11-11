import { Check } from 'lucide-react';
import { useState } from 'react';
import { usePayment } from '../hooks/usePayment';
import { useAuth } from '../contexts/AuthContext';
import toast from 'react-hot-toast';
import { useNavigate } from 'react-router-dom';

const PricingPlan = () => {
  const [loading, setLoading] = useState(false);
  const { createPremiumPayment } = usePayment();
  const { isAuthenticated } = useAuth();
  const navigate = useNavigate();

  const handleSelectPlan = async (planType) => {
    // Kiểm tra đăng nhập
    if (!isAuthenticated) {
      toast.error('Vui lòng đăng nhập để chọn gói');
      navigate('/login');
      return;
    }

    if (loading) return;
    
    setLoading(true);
    try {
      const paymentData = {
        planType: planType, // "monthly" or "yearly"
        returnUrl: `${window.location.origin}/success`,
        cancelUrl: `${window.location.origin}/failed`
      };

      const response = await createPremiumPayment(paymentData);
      
      if (response.success && response.data?.checkoutUrl) {
        // Redirect to payment gateway
        window.location.href = response.data.checkoutUrl;
      } else {
        toast.error('Không thể tạo link thanh toán');
      }
    } catch (error) {
      console.error('Payment error:', error);
      toast.error(error.message || 'Đã xảy ra lỗi khi tạo thanh toán');
    } finally {
      setLoading(false);
    }
  };
  const plans = [
    {
      name: 'Cơ Bản',
      price: '0',
      currency: '₫',
      period: 'mỗi tháng',
      subPeriod: 'thanh toán hàng tháng',
      planType: null, // Free plan
      features: [
        'Tạo tối đa 3 sự kiện',
        'Quản lý đối tác cơ bản',
        'Báo cáo thống kê',
        'Hỗ trợ qua email',
        'Chức năng cơ bản'
      ],
      buttonText: 'Chọn Gói',
      isPopular: false,
      bgColor: 'bg-gradient-to-br from-blue-50 via-blue-100 to-blue-50',
      textColor: 'text-blue-600',
      buttonColor: 'bg-blue-200 hover:bg-blue-300 text-blue-700',
      borderColor: ''
    },
    {
      name: 'Hàng Tháng',
      price: '50.000',
      currency: '₫',
      period: 'mỗi tháng',
      subPeriod: 'thanh toán hàng tháng',
      planType: 'monthly',
      features: [
        'Tạo không giới hạn sự kiện',
        'Quản lý đối tác nâng cao',
        'Báo cáo chi tiết & phân tích',
        'Chatbot AI hỗ trợ',
        'Hỗ trợ ưu tiên 24/7'
      ],
      buttonText: 'Chọn Gói',
      isPopular: true,
      bgColor: 'bg-gradient-to-br from-blue-500 via-blue-600 to-blue-700',
      textColor: 'text-white',
      buttonColor: 'bg-white hover:bg-gray-100 text-blue-600',
      borderColor: ''
    },
    {
      name: 'Hàng Năm',
      price: '100.000',
      currency: '₫',
      period: 'mỗi năm',
      subPeriod: 'thanh toán hàng năm',
      planType: 'yearly',
      features: [
        'Tất cả tính năng Hàng Tháng',
        'Tiết kiệm 17% chi phí',
        'Tư vấn chiến lược sự kiện',
        'Logo thương hiệu tùy chỉnh',
        'API tích hợp doanh nghiệp'
      ],
      buttonText: 'Chọn Gói',
      isPopular: false,
      bgColor: 'bg-gradient-to-br from-blue-50 via-blue-100 to-blue-50',
      textColor: 'text-blue-600',
      buttonColor: 'bg-blue-200 hover:bg-blue-300 text-blue-700',
      borderColor: ''
    }
  ];

  return (
    <div className="min-h-screen bg-gradient-to-b from-gray-50 to-white py-12 px-4 sm:px-6 lg:px-8">
      <style>{`
        @keyframes liquidWave {
          0% {
            transform: translate(-50%, -75%) rotate(0deg);
          }
          100% {
            transform: translate(-50%, -75%) rotate(360deg);
          }
        }
        
        @keyframes liquidWave2 {
          0% {
            transform: translate(-50%, -25%) rotate(0deg) scale(1);
          }
          50% {
            transform: translate(-30%, -40%) rotate(180deg) scale(1.2);
          }
          100% {
            transform: translate(-50%, -25%) rotate(360deg) scale(1);
          }
        }
        
        @keyframes liquidPulse {
          0%, 100% {
            opacity: 0.4;
            transform: scale(1);
          }
          50% {
            opacity: 0.7;
            transform: scale(1.15);
          }
        }
        
        @keyframes ripple {
          0% {
            transform: scale(0.8);
            opacity: 0.6;
          }
          50% {
            transform: scale(1.1);
            opacity: 0.3;
          }
          100% {
            transform: scale(1.4);
            opacity: 0;
          }
        }
        
        .liquid-effect::before {
          content: '';
          position: absolute;
          width: 250%;
          height: 250%;
          top: -60%;
          left: -60%;
          background: radial-gradient(circle at 30% 40%, rgba(255,255,255,0.5) 0%, rgba(255,255,255,0.3) 30%, transparent 70%);
          animation: liquidWave 6s ease-in-out infinite;
          pointer-events: none;
          z-index: 1;
        }
        
        .liquid-effect::after {
          content: '';
          position: absolute;
          width: 200%;
          height: 200%;
          top: -30%;
          left: -30%;
          background: radial-gradient(circle at 60% 50%, rgba(255,255,255,0.4) 0%, rgba(255,255,255,0.2) 40%, transparent 65%);
          animation: liquidWave2 8s ease-in-out infinite;
          pointer-events: none;
          z-index: 1;
        }
        
        .shimmer {
          position: relative;
          overflow: hidden;
        }
        .shimmer::before {
          content: '';
          position: absolute;
          top: 0;
          left: -100%;
          width: 100%;
          height: 100%;
          background: linear-gradient(90deg, transparent, rgba(255,255,255,0.6), transparent);
          animation: shimmerMove 1.5s infinite;
          pointer-events: none;
          z-index: 1;
        }
        
        @keyframes shimmerMove {
          0% {
            left: -100%;
          }
          100% {
            left: 100%;
          }
        }
        
        .glow-effect {
          position: relative;
        }
        
        .glow-effect::before {
          content: '';
          position: absolute;
          inset: -3px;
          border-radius: inherit;
          padding: 3px;
          background: linear-gradient(45deg, 
            rgba(96, 165, 250, 0.6), 
            rgba(59, 130, 246, 0.8), 
            rgba(37, 99, 235, 0.6),
            rgba(59, 130, 246, 0.8)
          );
          background-size: 300% 300%;
          -webkit-mask: linear-gradient(#fff 0 0) content-box, linear-gradient(#fff 0 0);
          -webkit-mask-composite: xor;
          mask-composite: exclude;
          animation: glowRotate 2s ease-in-out infinite;
          pointer-events: none;
          z-index: 0;
        }
        
        @keyframes glowRotate {
          0%, 100% {
            background-position: 0% 50%;
            opacity: 0.5;
          }
          50% {
            background-position: 100% 50%;
            opacity: 0.8;
          }
        }
      `}</style>
      
      <div className="max-w-6xl mx-auto">
        {/* Header */}
        <div className="text-center mb-12">
          <h1 className="text-4xl font-bold text-gray-900 mb-3">
            Bảng Giá Dịch Vụ
          </h1>
          <p className="text-lg text-gray-600">
            Lựa chọn gói phù hợp với nhu cầu của bạn
          </p>
        </div>

        {/* Pricing Cards */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-5 max-w-4xl mx-auto">
          {plans.map((plan, index) => (
            <div key={index} className="relative pt-6">
              {/* Popular Badge - Outside card */}
              {plan.isPopular && (
                <div className="absolute top-0 left-1/2 transform -translate-x-1/2 bg-gradient-to-r from-yellow-400 to-orange-400 text-white px-3 py-1 rounded-full text-xs font-bold shadow-md z-30">
                  Phổ Biến Nhất
                </div>
              )}
              
              <div
                className={`relative rounded-2xl p-6 shadow-lg transition-all duration-500 hover:shadow-xl hover:scale-105 ${plan.bgColor} ${plan.borderColor} ${
                  plan.isPopular ? 'transform scale-105 shadow-xl glow-effect' : ''
                } liquid-effect shimmer overflow-hidden`}
              >
              {/* Plan Name */}
              <div className="mb-4 mt-1 relative z-10">
                <h3 className={`text-xl font-bold ${plan.textColor}`}>
                  {plan.name}
                </h3>
              </div>

              {/* Price */}
              <div className="mb-4 pb-4 border-b border-opacity-20 relative z-10" style={{borderColor: plan.isPopular ? 'white' : '#3B82F6'}}>
                <div className="flex items-baseline gap-1 justify-center">
                  <span className={`text-xl font-bold ${plan.textColor} tracking-tight`}>
                    {plan.price}
                  </span>
                  <span className={`text-base font-bold ${plan.textColor}`}>
                    ₫
                  </span>
                </div>
                <div className="text-center mt-1 h-9">
                  {plan.price !== '0' && (
                    <>
                      <p className={`text-xs font-medium ${plan.isPopular ? 'text-blue-50' : 'text-blue-600'}`}>
                        {plan.period}
                      </p>
                      <p className={`text-xs mt-0.5 ${plan.isPopular ? 'text-blue-100 opacity-90' : 'text-gray-500'}`}>
                        {plan.subPeriod}
                      </p>
                    </>
                  )}
                </div>
              </div>

              {/* Features */}
              <ul className="space-y-2 mb-6 relative z-10">
                {plan.features.map((feature, featureIndex) => (
                  <li key={featureIndex} className="flex items-start">
                    <Check 
                      size={18} 
                      className={`mr-2.5 flex-shrink-0 mt-0.5 ${plan.isPopular ? 'text-white' : 'text-blue-500'}`}
                    />
                    <span className={`text-xs leading-relaxed ${plan.isPopular ? 'text-white' : 'text-gray-700'}`}>
                      {feature}
                    </span>
                  </li>
                ))}
              </ul>

              {/* Button - Always show to maintain height */}
              <div className="relative z-10 h-10 flex items-center">
                {plan.price !== '0' && isAuthenticated && (
                  <button
                    onClick={() => handleSelectPlan(plan.planType)}
                    disabled={loading}
                    className={`w-full py-2 px-5 rounded-full font-bold text-sm transition-all duration-300 transform hover:scale-105 ${plan.buttonColor} shadow-md hover:shadow-lg disabled:opacity-50 disabled:cursor-not-allowed`}
                  >
                    {loading ? 'Đang xử lý...' : plan.buttonText}
                  </button>
                )}
                {plan.price !== '0' && !isAuthenticated && (
                  <button
                    onClick={() => {
                      toast.error('Vui lòng đăng nhập để chọn gói');
                      navigate('/login');
                    }}
                    className={`w-full py-2 px-5 rounded-full font-bold text-sm transition-all duration-300 transform hover:scale-105 ${plan.buttonColor} shadow-md hover:shadow-lg`}
                  >
                    Đăng nhập để chọn gói
                  </button>
                )}
              </div>
            </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
};

export default PricingPlan;
