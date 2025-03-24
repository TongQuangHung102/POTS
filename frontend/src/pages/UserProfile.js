// UserProfile.jsx
import React, { useState } from 'react';
import styles from './UserProfile.module.css';

const UserProfile = () => {
    const role = 4;
    const [isChangePassword, setIsChangePassword] = useState(false);
    return (
        <div className={styles.container}>
            <div className={styles.header}>
                <div className={styles.welcomeSection}>
                    <h1>Xin chào, DanhHuy</h1>
                    <p className={styles.date}>Thứ 7, 11/02/2025</p>
                </div>
            </div>

            <div className={styles.banner}></div>

            <div className={styles.profileSection}>
                <div className={styles.profileInfo}>
                    <div className={styles.avatar}>
                        <img src="/images/user.png" />
                    </div>
                    <div className={styles.userInfo}>
                        <h2>Danh Huy</h2>
                        <p className={styles.email}>Học sinh</p>
                    </div>
                </div>
                {isChangePassword && (
                    <div className={styles.groupBtn}>
                        <div>
                            <input type="text" placeholder="Mật khẩu cũ" />
                        </div>
                        <div>
                            <input type="text" placeholder="Mật khẩu mới" />
                        </div>
                        <div>
                            <input type="text" placeholder="Xác nhận mật khẩu" />
                        </div>
                        <div>
                            <button className='btn btn-success' onClick={() => setIsChangePassword(false)}>Xác nhận</button>
                        </div>
                    </div>)}
                {!isChangePassword && (<div>
                    <button className={styles.editButton} onClick={() => setIsChangePassword(true)}>Đổi mật khẩu</button>
                </div>
                )}

            </div>

            <div className={styles.formContainer}>
                <div className={styles.formRow}>
                    <div className={styles.formGroup}>
                        <label>Tên</label>
                        <input type="text" value='Vũ Danh Huy' placeholder="" />
                    </div>
                    <div className={styles.formGroup}>
                        <label>Email</label>
                        <input type="text" value='danhhuy@gmail.com' placeholder="Your First Name" />
                    </div>
                </div>
                {role === 1 && (<div className={styles.formRow}>
                    <div className={styles.formGroup}>
                        <label>Khối</label>
                        <div className={styles.selectField}>
                            <select>
                                <option>Khối 5</option>
                            </select>
                            <span className={styles.dropdownIcon}>▼</span>
                        </div>
                    </div>
                    <div className={styles.formGroup}>
                        <label>Ngày tham gia</label>
                        <div className={styles.selectField}>
                            <select>
                                <option>Your First Name</option>
                            </select>
                            <span className={styles.dropdownIcon}>▼</span>
                        </div>
                    </div>
                </div>)}

                {role === 4 && (<div className={styles.formRow}>
                    <div className={styles.formGroup}>
                        <label>Khối</label>
                        <div className={styles.selectField}>
                            <select>
                                <option>Khối 5</option>
                            </select>
                            <span className={styles.dropdownIcon}>▼</span>
                        </div>
                    </div>
                    <div className={styles.formGroup}>
                        <label>Ngày tham gia</label>
                        <div className={styles.selectField}>
                            <select>
                                <option>Your First Name</option>
                            </select>
                            <span className={styles.dropdownIcon}>▼</span>
                        </div>

                    </div>
                </div>)}

                {role === 4 && (<div className={styles.emailSection}>
                    <h3>Tài khoản liên kết</h3>
                    <div className={styles.emailItem}>
                        <div className={styles.emailIcon}>
                            <span>✉</span>
                        </div>
                        <div className={styles.emailDetails}>
                            <p className={styles.emailAddress}>danhhuy1@gmail.com</p>
                            <p className={styles.emailTime}>danhhuy</p>
                        </div>
                    </div>
                    <button className={styles.addEmailButton}>+ Thêm</button>
                </div>)}

            </div>
        </div>
    );
};

export default UserProfile;