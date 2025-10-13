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

// Component bảo vệ route theo role
export const RoleProtectedRoute = ({ children, allowedRoles }) => {
    const { isAuthenticated, userRole, loading } = useAuth();

    if (loading) {
        return <div>Loading...</div>;
    }

    if (!isAuthenticated) {
        return <Navigate to="/login" replace />;
    }

    if (allowedRoles && !allowedRoles.includes(userRole)) {
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