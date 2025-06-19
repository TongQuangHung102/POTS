import { Routes, Route, Navigate } from 'react-router-dom';
import { useNavigate } from 'react-router-dom';
import { useEffect } from 'react';

import AdminLayout from '../pages/admin/AdminLayout';
import AdminDashboard from '../pages/admin/AdminDashboard';
import Register from '../pages/RegisterForm';
import Login from '../pages/LoginForm';
import ListChapter from '../pages/content_manager/ListChapter';
import ListLesson from '../pages/content_manager/ListLesson';
import GoogleCallback from '../pages/GoogleCallback';
import { useAuth } from '../hooks/useAuth';
import ListPackage from '../pages/admin/ListPackage';
import PackageDetail from '../pages/admin/PackageDetail';
import AddPackage from '../pages/admin/AddPackage';
import ChooseRole from '../pages/ChooseRole';
import AddUser from '../pages/admin/AddUser';
import StudentDashboard from '../pages/student/StudentDashboard';
import StudentLayout from '../pages/student/StudentLayout';
import Course from '../pages/student/Course';
import ListPackageStudent from '../pages/parent/ListPackageStudent';
import ForgotPasswordForm from '../pages/ForgotPasswordForm';
import Quiz from '../pages/student/Quiz';
import QuestionManage from '../pages/content_manager/QuestionManage';
import ContentManageLayout from '../pages/content_manager/ContentManageLayout';
import UserList from '../pages/admin/UserList';
import ListGrades from '../pages/admin/ListGrades';
import ChooseGrade from '../pages/student/ChooseGrade';
import TestCategory from '../pages/admin/TestCategory';
import ListTest from '../pages/content_manager/ListTest';
import AddQuestionForm from '../pages/content_manager/AddQuestionForm';
import Test from '../pages/student/Test';
import ListGradesAssign from '../pages/content_manager/ListGradesAssign';
import ManageQuestionTest from '../pages/content_manager/ManageQuestionTest';
import ListAIQuestion from '../pages/content_manager/ListAIQuestion';
import ContentManageDashboard from '../pages/content_manager/ContentManageDashboard';
import ListSubjectGrades from '../pages/content_manager/ListSubjectGrade';
import ChooseSubject from '../pages/student/ChooseSubject';
import ListSubject from '../pages/admin/ListSubject';
import ListReport from '../pages/content_manager/ListReport';
import ParentDashboard from '../pages/parent/ParentDashboard';
import ParentLayout from '../pages/parent/ParentLayout';
import UserProfile from '../pages/UserProfile';
import Report from '../pages/content_manager/Report';
import HomePage from '../pages/HomePage';
import PracticeHistory from '../pages/student/PracticeHistory';
import PracticeDetails from '../pages/student/PracticeDetails';
import AccessDenied from '../pages/AccessDenied';
import ProtectedRoute from './ProtectedRoute';
import GuestRoute from './GuestRoute';
import ConfirmPackagePage from '../pages/parent/ConfirmPackagePage';
import Payment from '../pages/admin/Payment';
import { PaymentInfoPage } from '../pages/parent/PaymentInfoPage';
import PaymentHistory from '../pages/parent/PaymentHistory';
import ConfirmEmailPage from '../pages/ConfirmEmailPage';
const AppRoutes = () => {
    const { user, loading } = useAuth();
    const navigate = useNavigate();

    useEffect(() => {
        const checkToken = () => {
            const expiresAt = localStorage.getItem("tokenExpiresAt");
            const nowUtc = new Date();
            const vnTime = new Date(nowUtc.toLocaleString('en-US', { timeZone: 'Asia/Ho_Chi_Minh' }));
        
            if (expiresAt) {
                const expiresAtTime = Number(expiresAt); 
                if (vnTime.getTime() > expiresAtTime) { 
                    localStorage.clear();
                    alert('Phiên của bạn đã hết hạn, vui lòng đăng nhập lại');
                    window.location.href = '/login'; 
                }
            }
        };
        
    
        checkToken();
        const interval = setInterval(checkToken, 60000);
    
        return () => clearInterval(interval);
    }, []);
    

    if (loading) {
        return <div>Loading...</div>;
    }

    return (
        <Routes>
            {/* Route cho login */}
            <Route path='/login' element={
                <GuestRoute>
                    <Login />
                </GuestRoute>
            } />
            <Route path='/register' element={
                <GuestRoute>
                    <Register />
                </GuestRoute>
            } />
            <Route path='/google-callback' element={<GoogleCallback />}></Route>
            <Route path='/choose-role' element={<ChooseRole />}></Route>
            <Route path='/choose-grade' element={<ChooseGrade />}></Route>
            <Route path='/confirm-email' element={<ConfirmEmailPage />}></Route>
            <Route path='/forgot-password' element={
                <GuestRoute>
                    <ForgotPasswordForm />
                </GuestRoute>
            }></Route>
            <Route path='/' element={<HomePage />}></Route>
            <Route path='/access-denied' element={<AccessDenied />}></Route>
            <Route path="/payment" element={<PaymentInfoPage />} />

            <Route path='/admin' element={
                <ProtectedRoute allowedRoles={[3]}>
                    <AdminLayout />
                </ProtectedRoute>
            }>
                <Route index element={<Navigate to="/admin/dashboard" replace />} />
                <Route path="/admin/dashboard" element={<AdminDashboard />} />
                <Route path="/admin/grades" element={<ListGrades />} />
                <Route path="/admin/add-user" element={<AddUser />} />
                <Route path="/admin/grades/:gradeId" element={<ListSubjectGrades />} />
                <Route path="/admin/grades/:gradeId/subject/:subjectId" element={<ListChapter />} />
                <Route path="/admin/grades/:gradeId/subject/:subjectId/chapters/:chapterId" element={<ListLesson />} />
                <Route path='/admin/package' element={<ListPackage />} />
                <Route path='/admin/payment' element={<Payment />} />
                <Route
                    path='/admin/package/:planId'
                    element={<PackageDetail />}
                />
                <Route path='/admin/grades/subject' element={<ListSubject />} />
                <Route path='/admin/package/add' element={<AddPackage />} />
                <Route path='/admin/grades/test_category' element={<TestCategory />}></Route>
                <Route path='/admin/users' element={<UserList />}></Route>
                <Route path='/admin/grades/:gradeId/subject/:subjectId/list_tests' element={<ListTest />}></Route>
                <Route path='/admin/question/:lessonId' element={<QuestionManage />}></Route>
                <Route path='/admin/grades/:gradeId/subject/:subjectId/chapters/:chapterId/lessons/:lessonId/add-question' element={<AddQuestionForm />}></Route>
                <Route path='/admin/grades/:gradeId/subject/:subjectId/test/:testId/questions' element={<ManageQuestionTest />}></Route>
                <Route path='/admin/grades/:gradeId/subject/:subjectId/chapters/:chapterId/lessons/:lessonId/questions' element={<QuestionManage />}></Route>
                <Route path='/admin/grades/:gradeId/subject/:subjectId/chapters/:chapterId/lessons/:lessonId/list-aiquestion' element={<ListAIQuestion />} />
                <Route path='/admin/profile' element={<UserProfile />}></Route>
            </Route>

            <Route path='/student' element={
                <ProtectedRoute allowedRoles={[1]}>
                    <StudentLayout />
                </ProtectedRoute>
            }>
                <Route path="/student/dashboard" element={<StudentDashboard />} />
                <Route index element={<Navigate to="/student/dashboard" replace />} />
                <Route path="/student/grade/:gradeId" element={<ChooseSubject />} />
                <Route path="/student/grade/:gradeId/subject/:subjectId/course" element={<Course />} />
                <Route path="/student/test" element={<Test />} />
                <Route path='/student/grade/:gradeId/subject/:subjectId/lesson/:lessonId/practice' element={<Quiz />}></Route>
                <Route path='/student/grade/:gradeId/subject/:subjectId/lesson/:lessonId/history' element={<PracticeHistory />}></Route>
                <Route path='/student/grade/:gradeId/subject/:subjectId/lesson/:lessonId/history/practice/:practiceId' element={<PracticeDetails />}></Route>
                <Route path='/student/grade/:gradeId/subject/:subjectId/course/test/:testId' element={<Quiz />}></Route>
                <Route path='/student/profile' element={<UserProfile />}></Route>
            </Route>

            <Route path='/content_manage' element={
                <ProtectedRoute allowedRoles={[4]}>
                    <ContentManageLayout />
                </ProtectedRoute>
            }>
                <Route index element={<Navigate to="/content_manage/dashboard" replace />} />
                <Route path="/content_manage/dashboard" element={<ContentManageDashboard />} />
                <Route path="/content_manage/grades" element={<ListGradesAssign />} />
                <Route path="/content_manage/grades/:gradeId" element={<ListSubjectGrades />} />
                <Route path='/content_manage/grades/:gradeId/subject/:subjectId/chapters/:chapterId/lessons/:lessonId/questions' element={<QuestionManage />}></Route>
                <Route path='/content_manage/grades/:gradeId/subject/:subjectId/chapters/:chapterId/lessons/:lessonId/add-question' element={<AddQuestionForm />}></Route>
                <Route path="/content_manage/grades/:gradeId/subject/:subjectId" element={<ListChapter />} />
                <Route path="/content_manage/grades/:gradeId/subject/:subjectId/chapters/:chapterId" element={<ListLesson />} />
                <Route path='/content_manage/grades/:gradeId/subject/:subjectId/list_tests' element={<ListTest />}></Route>
                <Route path='/content_manage/grades/:gradeId/subject/:subjectId/test/:testId/questions' element={<ManageQuestionTest />}></Route>
                <Route path='/content_manage/grades/:gradeId/subject/:subjectId/chapters/:chapterId/lessons/:lessonId/list-aiquestion' element={<ListAIQuestion />} />
                <Route path='/content_manage/report/list' element={<ListReport />} />
                <Route path='/content_manage/report' element={<Report />} />
                <Route path='/content_manage/profile' element={<UserProfile />}></Route>
            </Route>

            <Route path='/parent' element={
                <ProtectedRoute allowedRoles={[2]}>
                    <ParentLayout />
                </ProtectedRoute>
            }>
                <Route index element={<Navigate to="/parent/dashboard" replace />} />
                <Route path='/parent/dashboard' element={<ParentDashboard />}></Route>
                <Route path='/parent/profile' element={<UserProfile />}></Route>
                <Route path="/parent/package" element={<ListPackageStudent />} />
                <Route path="/parent/package/:packageId/confirm" element={<ConfirmPackagePage />} />
                <Route path="/parent/package/payment" element={<PaymentInfoPage />} />
                <Route path="/parent/payment-history" element={<PaymentHistory />} />
            </Route>
            {/* Nếu chưa đăng nhập, điều hướng đến login */}
            {!user && (
                <Route path='/' element={<Navigate to='/login' replace />} />
            )}
        </Routes>
    );
};

export default AppRoutes;
