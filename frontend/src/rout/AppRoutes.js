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
import StudentDashboard from '../pages/StudentDashboard';
import StudentLayout from '../pages/StudentLayout';
import Profile from '../pages/ProfilePage';

import Course from '../pages/student/Course'
import ForgotPasswordForm from '../pages/ForgotPasswordForm';
import Quiz from '../pages/student/Quiz';
import QuestionManage from '../pages/content_manager/QuestionManage';
import ContentManageLayout from '../pages/ContentManageLayout';
import UserList from '../pages/admin/UserList';
const AppRoutes = () => {
    const { user, loading } = useAuth();

    if (loading) {
        return <div>Loading...</div>;
    }

    return (
        <Routes>
            {/* Route cho login */}
            <Route path="/profile" element={<Profile />} />

            <Route path="/pricing" element={<PricingPage />} />
            <Route path='/login' element={<Login />} />
            <Route path='/register' element={<Register />} />
            <Route path='/google-callback' element={<GoogleCallback />}></Route>
            <Route path='/choose-role' element={<ChooseRole />}></Route>
            <Route path='/forgot-password' element={<ForgotPasswordForm />}></Route>
            <Route path='/qiuz' element={<Quiz />}></Route>


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
                <Route path='/admin/package/add' element={<AddPackage />} />
                <Route path='/admin/question' element={<QuestionManage />}></Route>
                <Route path='/admin/users' element={<UserList />}></Route>
            </Route>

            <Route path='/student' element={<StudentLayout />}>
                <Route index element={<Navigate to="/student/dashboard" replace />} />

                <Route path="/student/dashboard" element={<StudentDashboard />} />
                <Route path="/student/course" element={<Course />} />
            </Route>





            {/* Nếu chưa đăng nhập, điều hướng đến login */}
            {!user && (
                <Route path='/' element={<Navigate to='/login' replace />} />
            )}
        </Routes>
    );
};

export default AppRoutes;
