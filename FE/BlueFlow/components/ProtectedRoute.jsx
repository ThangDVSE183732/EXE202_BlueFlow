import { Navigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

// Component bảo vệ route - chỉ cho phép user đã login
export const ProtectedRoute = ({ children }) => {
    const { isAuthenticated, loading } = useAuth();

    if (loading) {
        return <div>Loading...</div>;
    }

    return isAuthenticated ? children : <Navigate to="/login" replace />;
};

// Component bảo vệ route cho trang login/signup - không cho user đã login vào
export const GuestOnlyRoute = ({ children }) => {
    const { isAuthenticated, userRole, loading } = useAuth();

    if (loading) {
        return <div>Loading...</div>;
    }

    // Nếu đã login, redirect về trang role tương ứng
    if (isAuthenticated) {
        const role = userRole?.toLowerCase();
        const redirectPath = `/${role || 'organizer'}`;
        return <Navigate to={redirectPath} replace />;
    }

    return children;
};

// Component bảo vệ route theo role
export const RoleProtectedRoute = ({ children, allowedRoles }) => {
    const { isAuthenticated, userRole, loading } = useAuth();

    if (loading) {
        return <div>Loading...</div>;
    }

    if (!isAuthenticated) {
        return <Navigate to="/login" replace />;
    }

    // So sánh role không phân biệt hoa thường
    if (allowedRoles && !allowedRoles.some(role => role.toUpperCase() === userRole?.toUpperCase())) {
        console.log('Access denied. User role:', userRole, 'Allowed roles:', allowedRoles);
        return <Navigate to="/unauthorized" replace />;
    }

    return children;
};

// Component redirect theo role sau khi login
export const RoleBasedRedirect = () => {
    const { userRole, isAuthenticated } = useAuth();

    if (!isAuthenticated) {
        return <Navigate to="/login" replace />;
    }

    switch (userRole) {
        case 'ORGANIZER':
            return <Navigate to="/organizer" replace />;
        case 'SPONSOR':
            return <Navigate to="/sponsor" replace />;
        case 'SUPPLIER':
            return <Navigate to="/supplier" replace />;
        case 'ADMIN':
            return <Navigate to="/admin" replace />;
        default:
            return <Navigate to="/dashboard" replace />;
    }
};