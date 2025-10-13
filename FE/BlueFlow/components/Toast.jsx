import { useEffect } from 'react';

const Toast = ({ 
    isVisible, 
    onClose, 
    type = 'error', // 'error', 'success', 'warning', 'info'
    title, 
    message, 
    duration = 2000,
    position = 'top-right' // 'top-right', 'top-left', 'bottom-right', 'bottom-left'
}) => {
    useEffect(() => {
        if (isVisible && duration > 0) {
            const timer = setTimeout(() => {
                onClose();
            }, duration);

            return () => clearTimeout(timer);
        }
    }, [isVisible, duration, onClose]);

    if (!isVisible) return null;

    const getPositionClasses = () => {
        switch (position) {
            case 'top-left':
                return 'fixed top-4 left-4';
            case 'bottom-right':
                return 'fixed bottom-4 right-4';
            case 'bottom-left':
                return 'fixed bottom-4 left-4';
            default:
                return 'fixed top-4 right-4';
        }
    };

    const getTypeStyles = () => {
        switch (type) {
            case 'success':
                return {
                    bg: 'bg-green-500',
                    icon: '✓',
                    iconBg: 'bg-white',
                    iconColor: 'text-green-500'
                };
            case 'warning':
                return {
                    bg: 'bg-yellow-500',
                    icon: '⚠',
                    iconBg: 'bg-white',
                    iconColor: 'text-yellow-500'
                };
            case 'info':
                return {
                    bg: 'bg-blue-500',
                    icon: 'i',
                    iconBg: 'bg-white',
                    iconColor: 'text-blue-500'
                };
            default: // error
                return {
                    bg: 'bg-red-500',
                    icon: '✕',
                    iconBg: 'bg-white',
                    iconColor: 'text-red-500'
                };
        }
    };

    const styles = getTypeStyles();

    return (
        <div className={`${getPositionClasses()} ${styles.bg} text-white px-4 py-3 rounded-lg shadow-lg flex items-center gap-3 z-50 max-w-sm animate-slide-in`}>
            <div className={`flex items-center justify-center w-8 h-8 ${styles.iconBg} rounded-full`}>
                <span className={`${styles.iconColor} font-bold text-lg`}>{styles.icon}</span>
            </div>
            <div className="flex-1">
                {title && <div className="font-semibold text-sm">{title}</div>}
                <div className={`${title ? 'text-xs' : 'font-medium'}`}>
                    {message}
                </div>
            </div>
            <button 
                onClick={onClose}
                className="text-white hover:text-gray-200 text-xl leading-none ml-2"
            >
                ✕
            </button>
        </div>
    );
};

export default Toast;