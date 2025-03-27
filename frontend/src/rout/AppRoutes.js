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
import PackageDetailUser from '../pages/student/PackageDetailUser';
import ChooseRole from '../pages/ChooseRole';
import PricingPage from '../pages/PricingPage';
import AddUser from '../pages/admin/AddUser';
import StudentDashboard from '../pages/StudentDashboard';
import StudentLayout from '../pages/StudentLayout';
import Course from '../pages/student/Course';
import ListPackageStudent from '../pages/student/ListPackageStudent';
import ForgotPasswordForm from '../pages/ForgotPasswordForm';
import Quiz from '../pages/student/Quiz';
import QuestionManage from '../pages/content_manager/QuestionManage';
import ContentManageLayout from '../pages/ContentManageLayout';
import UserList from '../pages/admin/UserList';
import ListGrades from '../pages/admin/ListGrades';
import ChooseGrade from '../pages/student/ChooseGrade';
import TestCategory from '../pages/admin/TestCategory';
import ListTest from '../pages/admin/ListTest';
import AddQuestionForm from '../pages/content_manager/AddQuestionForm';
import Test from '../pages/student/Test';
import ListGradesAssign from '../pages/content_manager/ListGradesAssign';
import ManageQuestionTest from '../pages/content_manager/ManageQuestionTest';
import ListAIQuestion from '../pages/content_manager/ListAIQuestion';
import ContentManageDashboard from '../pages/content_manager/ContentManageDashboard';
import ListSubjectGrades from '../pages/admin/ListSubjectGrade';
import ChooseSubject from '../pages/student/ChooseSubject';
import ListSubject from '../pages/admin/ListSubject';
import ListReport from '../pages/admin/ListReport';
import ParentDashboard from '../pages/parent/ParentDashboard';
import ParentLayout from '../pages/parent/ParentLayout';
import UserProfile from '../pages/UserProfile';
import Report from '../pages/content_manager/Report';
import HomePage from '../pages/HomePage';
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
            <Route path='/choose-grade' element={<ChooseGrade />}></Route>
            <Route path='/forgot-password' element={<ForgotPasswordForm />}></Route>
            <Route path='/profile' element={<UserProfile />}></Route>
            <Route path='/home-page' element={<HomePage />}></Route>




            <Route path='/admin' element={<AdminLayout />}>
                <Route index element={<Navigate to="/admin/dashboard" replace />} />
                <Route path="/admin/dashboard" element={<AdminDashboard />} />
                <Route path="/admin/grades" element={<ListGrades />} />
                <Route path="/admin/add-user" element={<AddUser />} />
                <Route path="/admin/grades/:gradeId" element={<ListSubjectGrades />} />
                <Route path="/admin/grades/:gradeId/subject/:subjectId" element={<ListChapter />} />
                <Route path="/admin/grades/:gradeId/subject/:subjectId/chapters/:chapterId" element={<ListLesson />} />
                <Route path='/admin/package' element={<ListPackage />} />
                <Route
                    path='/admin/package/:planId'
                    element={<PackageDetail />}
                />
                <Route path='/admin/grades/subject' element={<ListSubject />} />
                <Route path='/admin/package/add' element={<AddPackage />} />
                <Route path='/admin/test_category' element={<TestCategory />}></Route>
                <Route path='/admin/users' element={<UserList />}></Route>
                <Route path='/admin/grades/:gradeId/subject/:subjectId/list_tests' element={<ListTest />}></Route>
                <Route path='/admin/question/:lessonId' element={<QuestionManage />}></Route>
                <Route path='/admin/grades/:gradeId/subject/:subjectId/chapters/:chapterId/lessons/:lessonId/add-question' element={<AddQuestionForm />}></Route>
                <Route path='/admin/grades/:gradeId/subject/:subjectId/test/:testId/questions' element={<ManageQuestionTest />}></Route>
                <Route path='/admin/grades/:gradeId/subject/:subjectId/chapters/:chapterId/lessons/:lessonId/questions' element={<QuestionManage />}></Route>
                <Route path='/admin/grades/:gradeId/subject/:subjectId/chapters/:chapterId/lessons/:lessonId/list-aiquestion' element={<ListAIQuestion />} />
            </Route>

            <Route path='/student' element={<StudentLayout />}>
                <Route path="/student/dashboard" element={<StudentDashboard />} />
                <Route
                    path='/student/package/:planId'
                    element={<PackageDetailUser />}
                />
                <Route index element={<Navigate to="/student/dashboard" replace />} />
                <Route path="/student/package" element={<ListPackageStudent />} />
                <Route path="/student/grade/:gradeId" element={<ChooseSubject />} />
                <Route path="/student/grade/:gradeId/subject/:subjectId/course" element={<Course />} />
                <Route path="/student/test" element={<Test />} />
                <Route path='/student/grade/:gradeId/subject/:subjectId/course/practice/:lessonId' element={<Quiz />}></Route>
                <Route path='/student/grade/:gradeId/subject/:subjectId/course/test/:testId' element={<Quiz />}></Route>
                <Route path='/student/profile' element={<UserProfile />}></Route>
            </Route>

            <Route path='/content_manage' element={<ContentManageLayout />}>
                <Route index element={<Navigate to="/content_manage/dashboard" replace />} />
                <Route path="/content_manage/dashboard" element={<ContentManageDashboard />} />
                <Route path="/content_manage/grades" element={<ListGradesAssign />} />
                <Route path="/content_manage/grades/:gradeId" element={<ListSubjectGrades />} />
                <Route path='/content_manage/grades/:gradeId/subject/:subjectId/chapters/:chapterId/lessons/:lessonId/questions' element={<QuestionManage />}></Route>
                <Route path='/content_manage/grades/:gradeId/subject/:subjectId/chapters/:chapterId/lessons/:lessonId/add-question' element={<AddQuestionForm />}></Route>
                <Route path="/content_manage/grades/:gradeId" element={<ListChapter />} />
                <Route path="/content_manage/grades/:gradeId/subject/:subjectId/chapters/:chapterId" element={<ListLesson />} />
                <Route path='/content_manage/grades/:gradeId/subject/:subjectId/list_tests' element={<ListTest />}></Route>
                <Route path='/content_manage/grades/:gradeId/subject/:subjectId/test/:testId/questions' element={<ManageQuestionTest />}></Route>
                <Route path='/content_manage/grades/:gradeId/subject/:subjectId/chapters/:chapterId/lessons/:lessonId/list-aiquestion' element={<ListAIQuestion />} />
                <Route path='/content_manage/report/list' element={<ListReport />} />
                <Route path='/content_manage/report' element={<Report />} />
            </Route>

            <Route path='/parent' element={<ParentLayout />}>
                <Route index element={<Navigate to="/parent/dashboard" replace />} />
                <Route path='/parent/dashboard' element={<ParentDashboard />}></Route>
                <Route path='/parent/profile' element={<UserProfile />}></Route>
            </Route>
            {/* Nếu chưa đăng nhập, điều hướng đến login */}
            {!user && (
                <Route path='/' element={<Navigate to='/login' replace />} />
            )}
        </Routes>
    );
};

export default AppRoutes;
