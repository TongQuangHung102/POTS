import React, { useState, useEffect } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { authFetch } from '../../utils/authFetch';
import styles from './ListPackageStudent.module.css';

const ListPackageStudent = () => {
    const [packages, setPackages] = useState([]);
    const [errorMessage, setErrorMessage] = useState('');
    const navigate = useNavigate();
    const apiUrl = process.env.REACT_APP_API_URL;
    useEffect(() => {
        const fetchPackages = async () => {
            try {
                const response = await authFetch(
                    `${apiUrl}/api/SubscriptionPlan/get-all-subscriptionplan`
                );
                if (!response.ok) {
                    throw new Error('Lỗi khi lấy danh sách gói');
                }
                const data = await response.json();
                setPackages(data);
            } catch (error) {
                setErrorMessage(error.message);
                console.error('Có lỗi khi lấy danh sách gói:', error);
            }
        };

        fetchPackages();
    }, []);

    // Hàm xác định màu thẻ dựa trên chỉ số
    const getCardColor = (index) => {
        const colors = ['blue', 'purple', 'orange'];
        return colors[index % colors.length];
    };

    const handleBuyPackage = (planId) => {
        console.log(`Mua gói khóa học có ID: ${planId}`);
    };

    return (
        <div className={styles.packageContainer}>
            <h2 className={styles.packageTitle}>Danh Sách Khóa Học</h2>

            {errorMessage && (
                <div className={styles.errorMessage}>{errorMessage}</div>
            )}

            <div className={styles.packageGrid}>
                {packages.length > 0 ? (
                    packages.filter(p => p.isVisible).map((pkg, index) => (
                        <div
                            key={pkg.planId}
                            className={`${styles.packageCard} ${styles[getCardColor(index)]}`}
                        >
                            <div className={styles.priceCircle}>
                                <div className={styles.price}>
                                    <span className={styles.currency}>₫</span>
                                    {Math.floor(pkg.price / 1000)}
                                    <span className={styles.decimal}>.000</span>
                                </div>
                            </div>

                            <div className={styles.packageDetails}>
                                <div className={styles.featureItem}>
                                    <span>Tên gói:</span> {pkg.planName}
                                </div>
                                <div className={styles.featureItem}>
                                    <span>Thông tin:</span> {pkg.description}
                                </div>
                                <div className={styles.featureItem}>
                                    <span>Thời hạn:</span>{' '}
                                    {pkg.duration == null || pkg.duration < 0 ? 'Không giới hạn' : `${pkg.duration} ngày`}

                                </div>
                                <div className={styles.featureItem}>
                                    <span>Số lần luyện tập mỗi ngày</span> {pkg.maxExercisesPerDay < 0 ? 'Không giới hạn' : pkg.maxExercisesPerDay}
                                </div>
                            </div>

                            <div className={styles.packageActions}>
                                <button
                                    className={styles.buyButton}
                                    onClick={() => navigate(`/parent/package/${pkg.planId}/confirm`)}
                                    disabled={pkg.price <= 0}
                                    style={{
                                        opacity: pkg.price <= 0 ? 0.5 : 1,
                                        cursor: pkg.price <= 0 ? 'not-allowed' : 'pointer'
                                    }}
                                >
                                    Mua Ngay <span className={styles.arrow}>»</span>
                                </button>
                            </div>

                        </div>
                    ))
                ) : (
                    <div className={styles.noData}>Không có dữ liệu</div>
                )}
            </div>
        </div>
    );
};

export default ListPackageStudent;
