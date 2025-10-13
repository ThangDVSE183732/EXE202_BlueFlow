import { useState } from 'react';

export const useToast = () => {
    const [toasts, setToasts] = useState([]);

    const showToast = ({ type = 'error', title, message, duration = 5000, position = 'top-right' }) => {
        const id = Date.now();
        const newToast = {
            id,
            type,
            title,
            message,
            duration,
            position,
            isVisible: true
        };

        setToasts(prev => [...prev, newToast]);

        // Auto remove after duration
        if (duration > 0) {
            setTimeout(() => {
                removeToast(id);
            }, duration);
        }

        return id;
    };

    const removeToast = (id) => {
        setToasts(prev => prev.filter(toast => toast.id !== id));
    };

    const clearAllToasts = () => {
        setToasts([]);
    };

    return {
        toasts,
        showToast,
        removeToast,
        clearAllToasts
    };
};