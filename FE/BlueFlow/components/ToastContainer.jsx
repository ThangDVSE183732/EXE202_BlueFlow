import Toast from './Toast';

const ToastContainer = ({ toasts, onRemoveToast }) => {
    return (
        <>
            {toasts.map(toast => (
                <Toast
                    key={toast.id}
                    isVisible={toast.isVisible}
                    type={toast.type}
                    title={toast.title}
                    message={toast.message}
                    duration={0} // Handled by useToast hook
                    position={toast.position}
                    onClose={() => onRemoveToast(toast.id)}
                />
            ))}
        </>
    );
};

export default ToastContainer;