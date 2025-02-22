import { Routes, Route, Navigate } from 'react-router-dom';
import AdminLayout from '../pages/AdminLayout';
import AdminDashboard from '../pages/AdminDashboard';
import Register from '../pages/RegisterForm';
import Login from '../pages/LoginForm';
import ListChapter from '../pages/admin/ListChapter';
import ListLesson from '../pages/admin/ListLesson';
import GoogleCallback from '../pages/GoogleCallback';
import { useAuth } from '../hooks/useAuth';
import ListPackage from '../pages/admin/ListPackage';
import PackageDetail from '../pages/admin/PackageDetail';
import AddPackage from '../pages/admin/AddPackage';
import ChooseRole from '../pages/ChooseRole';
import PricingPage from '../pages/PricingPage';
const AppRoutes = () => {
    const { user, loading } = useAuth();

    if (loading) {
        return <div>Loading...</div>;
    }

    return (
        <Routes>
            {/* Route cho login */}
            <Route path="/pricing" element={<PricingPage />} />
            <Route path='/login' element={<Login />} />
            <Route path='/register' element={<Register />} />
            <Route path='/google-callback' element={<GoogleCallback />}></Route>
            <Route path='/choose-role' element={<ChooseRole />}></Route>
            <Route path='/admin' element={<AdminLayout />}>
                <Route index element={<Navigate to="/admin/dashboard" replace />} />
                <Route path="/admin/dashboard" element={<AdminDashboard />} />
                <Route path="/admin/chapter" element={<ListChapter />} />
                <Route path="/admin/chapter/:chapterId" element={<ListLesson />} />
                <Route path='/admin/package' element={<ListPackage />} />
                <Route
                    path='/admin/package/:planId'
                    element={<PackageDetail />}
                />
                <Route path='/admin/addpackage/' element={<AddPackage />} />
            </Route>
            {/* Nếu chưa đăng nhập, điều hướng đến login */}
            {!user && (
                <Route path='/' element={<Navigate to='/login' replace />} />
            )}
        </Routes>
    );
};

export default AppRoutes;
