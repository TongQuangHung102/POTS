import React from 'react';
import { Navigate, Outlet } from 'react-router-dom';

const Dashboard = () => {
    return (
        <div>
            <h1>Dashboard</h1>
            <p>Welcome to your dashboard!</p>
            <Outlet />
        </div>
    );
};

export default Dashboard;