import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import styles from './ListPackageStudent.module.css';

const ListPackageStudent = () => {
    const [packages, setPackages] = useState([]);
    const [errorMessage, setErrorMessage] = useState('');

    useEffect(() => {
        const fetchPackages = async () => {
            try {
                const response = await fetch(
                    'https://localhost:7259/api/SubscriptionPlan/get-all-subscriptionplan'
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
                    packages.map((pkg, index) => (
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
                                <div className={styles.period}>Mỗi Tháng</div>
                            </div>

                            <div className={styles.packageDetails}>
                                <div className={styles.featureItem}>
                                    <span>Tên gói:</span> {pkg.planName}
                                </div>
                                <div className={styles.featureItem}>
                                    <span>Thông tin:</span> {pkg.description}
                                </div>
                                <div className={styles.featureItem}>
                                    <span>Thời Hạn:</span> {pkg.duration || 'N/A'} ngày
                                </div>
                                <div className={styles.featureItem}>
                                    <span>Phân Tích AI:</span> {pkg.isAIAnalysis ? 'Có' : 'Không'}
                                </div>
                                <div className={styles.featureItem}>
                                    <span>Cá Nhân Hóa:</span> {pkg.isPersonalization ? 'Có' : 'Không'}
                                </div>
                                <div className={styles.featureItem}>
                                    <span>Dùng Thử:</span> 30 Ngày Miễn Phí
                                </div>
                            </div>

                            <div className={styles.packageActions}>
                                {pkg.isVisible ? (
                                    <button
                                        className={styles.buyButton}
                                        onClick={() => handleBuyPackage(pkg.planId)}
                                    >
                                        Mua Ngay <span className={styles.arrow}>»</span>
                                    </button>
                                ) : (
                                    <span className={styles.unavailable}>Hết Hàng</span>
                                )}

                                <Link
                                    to={`/student/package/${pkg.planId}`}
                                    className={styles.detailLink}
                                >
                                    Xem Chi Tiết
                                </Link>
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
