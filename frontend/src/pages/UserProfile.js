// UserProfile.jsx
import React, { useState, useEffect } from 'react';
import styles from './UserProfile.module.css';
import { getUserProfileById, changePassword, linkChildAccount, verifyChildAccount, reSentCode, unlinkStudent, createAccount, changeGrade } from '../services/UserService';
import { getStudentsByUserId } from '../services/ParentService';
import { formatDateTime, getTodayFormatted } from '../utils/timeUtils';
import { fetchGrades } from '../services/GradeService';

const UserProfile = () => {
    const userId = sessionStorage.getItem('userId');
    const role = sessionStorage.getItem('roleId');
    const gradeId = sessionStorage.getItem('gradeId');
    const [isChangePassword, setIsChangePassword] = useState(false);
    const [user, setUser] = useState(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(null);
    const [students, setStudents] = useState([]);

    const [oldPassword, setOldPassword] = useState("");
    const [newPassword, setNewPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");

    const [message, setMessage] = useState("");
    const [isAddChildAccount, setIsAddChildAccount] = useState(false);
    const [isCreateChildAccount, setIsCreateChildAccount] = useState(false);
    const [isChangeGrade, setIsChangeGrade] = useState(false);
    const [changeGradeId, setChangeGradeId] = useState();
    const [isVerify, setIsVerify] = useState(false);
    const [emailLink, setEmailLink] = useState("");
    const [code, setCode] = useState("");
    const [loadingVerify, setLoadingVerify] = useState(false);

    const [isDisabled, setIsDisabled] = useState(false);
    const [timeLeft, setTimeLeft] = useState(0);

    const [deletingId, setDeletingId] = useState(null);
    const [grades, setGrades] = useState([]);

    const [cfPassword, setCfPassword] = useState('');

    const [createChildAccount, setCreateChildAccount] = useState({
        parentId: userId,
        username: '',
        password: '',
        email: '',
        gradeId: 0
    })

    const today = getTodayFormatted();

    const fetchUserProfile = async () => {
        try {
            setLoading(true);
            const userData = await getUserProfileById(userId);
            console.log(userData);
            setUser(userData);
        } catch (err) {
            setError("Không thể lấy dữ liệu người dùng.");
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        if (isCreateChildAccount || isChangeGrade) {
            const fetchData = async () => {
                const data = await fetchGrades();
                if (data) setGrades(data);
            };
            fetchData();
        }
    }, [isCreateChildAccount,isChangeGrade ]);

    const fetchStudents = async () => {
        try {
            setLoading(true);
            const studentData = await getStudentsByUserId(userId);
            console.log(studentData);
            setStudents(studentData);
        } catch (err) {
            setError("Không thể lấy danh sách học sinh.");
        } finally {
            setLoading(false);
        }
    };




    useEffect(() => {
        if (userId) {
            fetchUserProfile();
            setChangeGradeId(gradeId);
        }
    }, [userId]);

    useEffect(() => {
        if (role == 2) {
            fetchStudents();
        }
    }, [userId]);

    const handleChangePassword = async () => {
        if (newPassword.length < 6) {
            alert("Mật khẩu phải có ít nhất 6 ký tự.");
            return;
        }
        if (newPassword.length > 20) {
            alert("Mật khẩu không được vượt quá 20 ký tự.");
            return;
        }
        if (newPassword !== confirmPassword) {
            alert("Mật khẩu xác nhận không khớp.");
            return;
        }

        const result = await changePassword(userId, oldPassword, newPassword);
        alert(result.message);

        if (result.success) {
            setIsChangePassword(false);
            setOldPassword("");
            setNewPassword("");
            setConfirmPassword("");
        }
    };

    const handleLinkAccount = async (emailLink) => {
        if (!validateEmail(emailLink)) {
            setMessage("Email không hợp lệ. Vui lòng nhập đúng định dạng!");
            return;
        }
        try {
            setLoadingVerify(true)
            const response = await linkChildAccount(emailLink, userId);
            setMessage(response.message || "Đã gửi mã xác nhận đến email");
            setIsVerify(true);
            setIsAddChildAccount(false);
            setIsDisabled(true);
            setTimeLeft(60);
        } catch (error) {
            setMessage(error.message || "Liên kết thất bại. Vui lòng thử lại!");
        } finally {
            setLoadingVerify(false);
        }
    };

    const handleVerifyAccount = async (code) => {
        try {
            setLoadingVerify(true)
            const response = await verifyChildAccount(emailLink, userId, code);
            setMessage(response.message || "Thành công");
            setIsVerify(false);
            fetchStudents();
        } catch (error) {
            setMessage(error.message || "Liên kết thất bại. Vui lòng thử lại!");
        } finally {
            setLoadingVerify(false);
        }
    }

    const handleResendCode = async (emailLink) => {
        try {
            setLoadingVerify(true)
            const response = await reSentCode(emailLink, userId);
            setMessage(response.message || "Đã gửi mã xác nhận đến email");
            setIsVerify(true);
            setIsAddChildAccount(false);
            setIsDisabled(true);
            setTimeLeft(60);
        } catch (error) {
            setMessage(error.message || "Liên kết thất bại. Vui lòng thử lại!");
        } finally {
            setLoadingVerify(false);
        }
    }

    const handleUnlink = async (studentId) => {
        if (!window.confirm("Bạn có chắc muốn gỡ liên kết học sinh này?")) return;

        setDeletingId(studentId);
        try {
            await unlinkStudent(userId, studentId);
            fetchStudents();
        } catch (error) {
            alert(error.message);
        } finally {
            setDeletingId(null);
        }
    };

    useEffect(() => {
        if (timeLeft > 0) {
            const timer = setInterval(() => {
                setTimeLeft((prev) => {
                    if (prev <= 1) {
                        clearInterval(timer);
                        setIsDisabled(false); // Bật lại nút khi hết thời gian
                        return 0;
                    }
                    return prev - 1;
                });
            }, 1000);
            return () => clearInterval(timer);
        }
    }, [timeLeft]);

    const validateEmail = (email) => {
        return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
    };

    const handleCreateAccount = async () => {
        if (createChildAccount.username.length === 0) {
            alert("Tên không được bỏ trống");
            return;
        }
        if (!validateEmail(createChildAccount.email)) {
            alert("Sai định dạng email");
            return;
        }

        if (createChildAccount.password.length < 6) {
            alert("Mật khẩu phải có ít nhất 6 ký tự.");
            return;
        }

        if (createChildAccount.password != cfPassword) {
            alert("Mật khẩu và xác nhận mật khẩu không giống nhau");
            return;
        }
        console.log(createChildAccount);
        const result = await createAccount(createChildAccount);
        alert(result.message);
    }

    const handleChangeGrade = async () => {
        if(gradeId === changeGradeId){
            alert("Vui lòng chọn lớp khác lớp hiện tại");
            return;
        }

        const isConfirmed = window.confirm("Bạn có chắc chắn muốn đổi sang khối mới?");
        if (!isConfirmed) {
            return;
        }
        try {
            const result = await changeGrade(user.userId, changeGradeId);
            alert(result.message);
            sessionStorage.setItem("gradeId", changeGradeId);
            setIsChangeGrade(false);
            fetchStudents();

        } catch (error) {
            alert(error.message);
        }
    }


    if (loading || !user) return <p>Đang tải...</p>;
    if (error) return <p>{error}</p>;

    return (
        <div className={styles.container}>
            <div className={styles.header}>
                <div className={styles.welcomeSection}>
                    <h1>Xin chào, {user.userName}</h1>
                    <p className={styles.date}>{today}</p>
                </div>
            </div>

            <div className={styles.banner}></div>

            <div className={styles.profileSection}>
                <div className={styles.profileInfo}>
                    <div className={styles.avatar}>
                        <img src="/images/user.png" />
                    </div>
                    <div className={styles.userInfo}>
                        <h2>{user.userName}</h2>
                        <p className={styles.email}>{user.role}</p>
                    </div>
                </div>
                {isChangePassword ? (
                    <div className={styles.groupBtn}>
                        <div>
                            <input
                                type="password"
                                placeholder="Mật khẩu cũ"
                                value={oldPassword}
                                onChange={(e) => setOldPassword(e.target.value)}
                            />
                        </div>
                        <div>
                            <input
                                type="password"
                                placeholder="Mật khẩu mới"
                                value={newPassword}
                                onChange={(e) => setNewPassword(e.target.value)}
                            />
                        </div>
                        <div>
                            <input
                                type="password"
                                placeholder="Xác nhận mật khẩu"
                                value={confirmPassword}
                                onChange={(e) => setConfirmPassword(e.target.value)}
                            />
                        </div>
                        <div>
                            <button className="btn btn-success" onClick={handleChangePassword}>
                                Xác nhận
                            </button>
                            <button className="btn btn-danger m-1" onClick={() => setIsChangePassword(false)}>
                                Hủy
                            </button>
                        </div>
                    </div>
                ) : (
                    <div>
                        <button className="btn btn-primary" onClick={() => setIsChangePassword(true)}>
                            Đổi mật khẩu
                        </button>
                    </div>
                )}

            </div>

            <div className={styles.formContainer}>
                <div className={styles.formRow}>
                    <div className={styles.formGroup}>
                        <label>Tên</label>
                        <input type="text" value={user.userName} placeholder="" />
                    </div>
                    <div className={styles.formGroup}>
                        <label>Email</label>
                        <input type="text" value={user.email} placeholder="Your First Name" />
                    </div>
                </div>
                {role == 1 && (<div className={styles.formRow}>
                    <div className={styles.formGroup}>
                        <label>Khối</label>
                        <input type="text" value={user.grade} />
                    </div>
                    <div className={styles.formGroup}>
                        <label>Ngày tham gia</label>
                        <input type="text" value={formatDateTime(user.createAt)} />
                    </div>
                </div>)}
                {role == 1 && (<div className={styles.formRow}>
                    <div className={styles.formGroup}>
                        <button className='btn btn-primary' onClick={() => setIsChangeGrade(true)}>Thay đổi khối</button>
                    </div>
                </div>)}

                {isChangeGrade && (<div className={styles.formRow}>
                    <div className={styles.formGroup}>
                        <select
                            value={changeGradeId}
                            onChange={(e) =>
                                setChangeGradeId(
                                    e.target.value
                                )
                            }
                        >
                            {grades.filter(g => g.gradeIsVisible).map(cls => (
                                <option key={cls.gradeId} value={cls.gradeId}>
                                    {cls.gradeName}
                                </option>
                            ))}
                        </select>
                    </div>
                    <div className={styles.formGroupBtn}>
                        <button className='btn btn-success m-1' onClick={handleChangeGrade}>Xác nhận</button>
                        <button className='btn btn-danger' onClick={() => setIsChangeGrade(false)}>Hủy</button>
                    </div>

                </div>)}

                {role != 1 && (<div className={styles.formRow}>
                    <div className={styles.formGroup}>
                        <label>Ngày tham gia</label>
                        <input type="text" value={formatDateTime(user.createAt)} />

                    </div>
                </div>)}

                {role == 2 && (<div className={styles.emailSection}>
                    <h3>Tài khoản liên kết</h3>

                    {students.length > 0 ? (
                        <div>
                            {students.map((student) => (
                                <div className={styles.emailItem}>
                                    <div className={styles.emailDetails}>
                                        <p className={styles.emailAddress}>{student.email}</p>
                                        <p className={styles.emailTime}>{student.gradeName}</p>
                                    </div>
                                    <button
                                        className={styles.removeButton}
                                        onClick={() => handleUnlink(student.userId)}
                                    >

                                        Xóa
                                    </button>
                                </div>
                            ))}
                        </div>
                    ) : (
                        <p>Không có học sinh nào.</p>
                    )}
                    {loadingVerify ? "Đang xử lý..." : ''}
                    {(!isAddChildAccount && !isVerify) && (<div>
                        <button className={styles.addEmailButton} onClick={() => setIsAddChildAccount(true)}>+ Thêm</button>
                        <button className={styles.addEmailButton} onClick={() => setIsCreateChildAccount(true)}>+ Tạo tài khoản mới</button>
                    </div>
                    )}

                    {isAddChildAccount && (<div>
                        <input
                            type="email"
                            className="form-control form-control-sm w-50"
                            placeholder="Vui lòng nhập email muốn liên kết"
                            value={emailLink}
                            onChange={(e) => setEmailLink(e.target.value)}
                        />
                        <button className="btn btn-primary btn-sm m-1" onClick={() => handleLinkAccount(emailLink)}>Thêm</button>
                        <button className="btn btn-danger btn-sm m-1" onClick={() => setIsAddChildAccount(false)}>Hủy</button>
                    </div>)}

                    {isVerify && (<div>
                        <input type="text"
                            className="form-control form-control-sm w-50"
                            placeholder="Mã xác nhận"
                            value={code}
                            onChange={(e) => setCode(e.target.value)}
                        />
                        <button className="btn btn-primary btn-sm m-1" onClick={() => handleVerifyAccount(code)}>Xác nhận</button>
                        <button
                            className="btn btn-primary btn-sm m-1"
                            onClick={() => handleResendCode(emailLink)}
                            disabled={isDisabled}
                        >
                            {isDisabled ? `Vui lòng chờ ${timeLeft}s` : "Nhận lại mã"}
                        </button>
                    </div>
                    )}
                    {message && <p className="mt-2">{message}</p>}
                </div>)}
            </div>

            {isCreateChildAccount && (
                <div className={styles.modal}>
                    <div className={styles.modalContent}>
                        <h3>Tạo tài khoản cho con</h3>
                        <label>
                            Họ tên:
                            <input
                                type="text"
                                value={createChildAccount?.username}
                                onChange={(e) =>
                                    setCreateChildAccount({ ...createChildAccount, username: e.target.value })
                                }
                            />
                        </label>
                        <label>
                            Email
                            <input
                                type="text"
                                value={createChildAccount?.email}
                                onChange={(e) => {
                                    const value = e.target.value;
                                    setCreateChildAccount({ ...createChildAccount, email: value });
                                }}
                            />
                        </label>
                        <label>Mật khẩu</label>
                        <input
                            type="password"
                            value={createChildAccount?.order}
                            onChange={(e) =>
                                setCreateChildAccount({ ...createChildAccount, password: e.target.value })
                            }
                        />
                        <label>Xác nhận mật khẩu</label>
                        <input
                            type="password"
                            value={cfPassword}
                            onChange={(e) =>
                                setCfPassword(e.target.value)
                            }
                        />
                        <label>
                            Khối:
                            <select
                                value={createChildAccount?.gradeId}
                                onChange={(e) =>
                                    setCreateChildAccount({
                                        ...createChildAccount,
                                        gradeId: e.target.value
                                    })
                                }
                            >
                                {grades.filter(g => g.gradeIsVisible).map(cls => (
                                    <option key={cls.gradeId} value={cls.gradeId}>
                                        {cls.gradeName}
                                    </option>
                                ))}
                            </select>
                        </label>
                        <div className="button-group">
                            <button onClick={handleCreateAccount} >Tạo</button>
                            <button onClick={() => setIsCreateChildAccount(false)} >Đóng</button>
                        </div>
                    </div>
                </div>

            )}

        </div>
    );
};

export default UserProfile;