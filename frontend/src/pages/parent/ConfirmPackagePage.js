import React, { useEffect, useState } from 'react';
import styles from './ConfirmPackagePage.module.css';
import { useParams, useNavigate } from 'react-router-dom';
import { getStudentsByUserId } from '../../services/ParentService';
import BackLink from '../../components/BackLink';
import { getSubscriptionPlanById } from '../../services/subscriptionService';
import { authFetch } from '../../utils/authFetch';

const ConfirmPackagePage = () => {
    const { packageId } = useParams();
    const [plan, setPlan] = useState(null);
    const [children, setChildren] = useState([]);
    const [selectedChildId, setSelectedChildId] = useState();
    const [loading, setLoading] = useState(true);
    const userId = localStorage.getItem('userId');
    const navigate = useNavigate();
    const apiUrl = process.env.REACT_APP_API_URL;

    useEffect(() => {
        const fetchData = async () => {
            try {
                const fetchedPlan = await getSubscriptionPlanById(packageId);
                const fetchedChildren = await getStudentsByUserId(userId);
   
                setPlan(fetchedPlan);
                setChildren(fetchedChildren);
            } catch (err) {
                console.error(err);
            } finally {
                setLoading(false);
            }
        };

        fetchData();
    }, [packageId, userId]);

    const handleProceedToPayment = async () => {
        const payload = {
            planId: packageId,
            studentId: selectedChildId,
            userId: userId,
            amount: plan.price
        };

     

        try {
            const response = await authFetch(`${apiUrl}/api/Payments/create`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(payload),
            });

            if (!response.ok) {
                throw new Error('Tạo thanh toán thất bại');
            }

            const payment = await response.json();

            navigate('/parent/package/payment', { state: { payment } });

        } catch (error) {
            console.error('Lỗi thanh toán:', error);
        }
    };


    if (loading) return <p>Đang tải dữ liệu...</p>;

    return (
        <div className={styles.cfpgContainer}>
            <h2>Đăng ký gói học cho con</h2>

            <div className={styles.planInfo}>
                <h3>{plan.planName}</h3>
                <p>{plan.description}</p>
                <p><strong>Giá:</strong> {plan.price <= 0 ? 'Miễn phí' : `${plan.price.toLocaleString()} VNĐ`}</p>
                <p><strong>Thời hạn:</strong> {plan.duration > 0 ? `${plan.duration} ngày` : 'Không giới hạn'}</p>
                <div>
                    <p><strong>🔔 Lưu ý: </strong><em>Bạn vẫn có thể mua thêm gói học cho con ngay cả khi gói hiện tại vẫn còn thời gian. Khi mua thêm, thời hạn sử dụng sẽ được cộng dồn vào thời gian còn lại của gói cũ, không bị thay thế hay làm mất đi thời gian đã mua trước đó.</em></p>
                </div>

            </div>

            <div className={styles.childSelect}>
                <label>Chọn học sinh:</label>
                <select value={selectedChildId} onChange={(e) => setSelectedChildId(parseInt(e.target.value))}>
                    <option value="">-- Chọn --</option>
                    {children.filter(g => g.isActive).map((child) => (
                        <option key={child.userId} value={child.userId}>
                            {child.userName}
                        </option>
                    ))}
                </select>
            </div>

            <button
                onClick={handleProceedToPayment}
                className={styles.confirmButton}
                disabled={!selectedChildId}
            >
                Tiếp tục đến trang thanh toán
            </button>
            <BackLink></BackLink>
        </div>
    );
};

export default ConfirmPackagePage;
