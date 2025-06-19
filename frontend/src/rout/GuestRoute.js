// GuestRoute.jsx
import React from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';

const GuestRoute = ({ children }) => {
    const { user } = useAuth(); 


    if (user) {

        switch (Number(user.roleId)) {
            case 1:
                return <Navigate to="/student/dashboard" replace />;
            case 2:
                return <Navigate to="/parent/dashboard" replace />;
            case 3:
                return <Navigate to="/admin/dashboard" replace />;
            case 4:
                return <Navigate to="/content_manage/dashboard" replace />;
            default:
                return <Navigate to="/home-page" replace />;
        }
    }


    return children;
};

export default GuestRoute;
