// ProtectedRoute.jsx
import React from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';

const ProtectedRoute = ({ allowedRoles, children }) => {
    const { user, loading } = useAuth();

    const role = Number(user?.roleId); 

    if (loading) {
        return <div>Loading...</div>;
    }

    if (!user || !allowedRoles.includes(role)) {
        return <Navigate to="/access-denied" replace />;
    }
    return children;
};

export default ProtectedRoute;