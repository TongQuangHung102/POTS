import { Routes, Route, Navigate } from 'react-router-dom';
import AdminLayout from '../pages/AdminLayout';
import AdminDashboard from '../pages/AdminDashboard';
import Register from '../pages/RegisterForm';
import Login from '../pages/LoginForm';
import ListChapter from '../pages/admin/ListChapter';
import ListLesson from '../pages/admin/ListLesson';
import GoogleCallback from '../pages/GoogleCallback';
import { useAuth } from '../hooks/useAuth';


const AppRoutes = () => {
    const { user, loading } = useAuth(); 

    if (loading) {
        return <div>Loading...</div>;
    }

    return (
        <Routes>
            {/* Route cho login */}
            <Route path="/login" element={<Login />} />
            <Route path="/register" element={<Register />} />
            <Route path="/google-callback" element={<GoogleCallback />} />
            <Route path="/admin" element={<AdminLayout />}>
                <Route index element={<AdminDashboard />} />
                <Route path="/admin/listchapter" element={<ListChapter />} />
                <Route path="/admin/listchapter/:chapterId" element={<ListLesson />} />
            </Route>
            {/* Nếu chưa đăng nhập, điều hướng đến login */}
            {!user && <Route path="/" element={<Navigate to="/login" replace />} />}
        </Routes>
    );
};

export default AppRoutes;
