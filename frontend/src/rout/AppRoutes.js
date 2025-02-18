import { Routes, Route, Navigate } from 'react-router-dom';
import AdminLayout from '../pages/AdminLayout';
import AdminDashboard from '../pages/AdminDashboard';
import Register from '../pages/RegisterForm';
import Login from '../pages/LoginForm';
import ListChapter from '../pages/admin/ListChapter';
import { useAuth } from '../hooks/useAuth';


const AppRoutes = () => {
    const { user, loading } = useAuth(); // Lấy cả user và loading

    // Nếu đang tải, hiển thị loading
    if (loading) {
        return <div>Loading...</div>;
    }

    return (
        <Routes>
            {/* Route cho login */}
            <Route path="/login" element={<Login />} />
            <Route path="/register" element={<Register />} />
            <Route path="/admin" element={<AdminLayout />}>
                <Route index element={<AdminDashboard />} />
                <Route path="/admin/listchapter" element={<ListChapter />} />
            </Route>
            {/* Nếu chưa đăng nhập, điều hướng đến login */}
            {!user && <Route path="/" element={<Navigate to="/login" replace />} />}




        </Routes>
    );
};

export default AppRoutes;
