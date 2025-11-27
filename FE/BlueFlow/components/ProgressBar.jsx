import { useEffect } from 'react';
import { useLocation } from 'react-router-dom';
import NProgress from 'nprogress';

// Configure NProgress
NProgress.configure({ 
  showSpinner: false, // Ẩn spinner tròn, chỉ giữ progress bar
  trickle: false, // Tắt auto increment để animation mượt hơn
  minimum: 0.08,
  easing: 'linear', // Chuyển động đều, không giật
  speed: 500 // Tăng tốc độ lên để mượt hơn
});

function ProgressBar() {
  const location = useLocation();

  useEffect(() => {
    // Bắt đầu progress bar khi route thay đổi
    NProgress.start();
    
    // Kết thúc progress bar sau một thời gian ngắn
    const timer = setTimeout(() => {
      NProgress.done();
    }, 300);

    return () => {
      clearTimeout(timer);
      NProgress.done();
    };
  }, [location.pathname]);

  return null;
}

export default ProgressBar;
