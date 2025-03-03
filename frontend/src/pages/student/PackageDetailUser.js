import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import './PackageDetailUser.css';

const PackageDetailUser = () => {
    const { planId } = useParams(); // Lấy planId từ URL
    const [packageDetail, setPackageDetail] = useState(null);
    const [errorMessage, setErrorMessage] = useState('');

    useEffect(() => {
        const fetchPackageDetail = async () => {
            try {
                const response = await fetch(
                    `https://localhost:7259/api/SubscriptionPlan/get-subscriptionplan-by/${planId}`
                );
                if (!response.ok) {
                    throw new Error('Lỗi khi lấy chi tiết gói');
                }
                const data = await response.json();
                setPackageDetail(data);
            } catch (error) {
                setErrorMessage(error.message);
                console.error('Có lỗi khi lấy chi tiết gói:', error);
            }
        };

        fetchPackageDetail();
    }, [planId]);

    if (errorMessage) {
        return <div className='error-message'>{errorMessage}</div>;
    }

    if (!packageDetail) {
        return <div>Đang tải chi tiết gói...</div>;
    }

    return (
        <div className='package-detail-container'>
            <h2>Chi tiết Gói: {packageDetail.planName}</h2>

            {errorMessage && (
                <div className='error-message'>{errorMessage}</div>
            )}

            <div className='detail-item'>
                <label>Tên Gói:</label>
                <span>{packageDetail.planName}</span>
            </div>

            <div className='detail-item'>
                <label>Giá:</label>
                <span>{packageDetail.price} VNĐ</span>
            </div>

            <div className='detail-item'>
                <label>Mô tả:</label>
                <span>{packageDetail.description}</span>
            </div>

            <div className='detail-item'>
                <label>Thời gian (ngày):</label>
                <span>{packageDetail.duration} ngày</span>
            </div>

            <div className='detail-item'>
                <label>Tính năng AI Analysis:</label>
                <span>{packageDetail.isAIAnalysis ? 'Có' : 'Không'}</span>
            </div>

            <div className='detail-item'>
                <label>Tính năng Personalization:</label>
                <span>{packageDetail.isPersonalization ? 'Có' : 'Không'}</span>
            </div>

            <div className='detail-item'>
                <label>Tính năng Basic Statistics:</label>
                <span>{packageDetail.isBasicStatistics ? 'Có' : 'Không'}</span>
            </div>

            <div className='detail-item'>
                <label>Tính năng Advanced Statistics:</label>
                <span>{packageDetail.isAdvancedStatistics ? 'Có' : 'Không'}</span>
            </div>

            <div className='detail-item'>
                <label>Số bài tập tối đa mỗi ngày:</label>
                <span>{packageDetail.maxExercisesPerDay}</span>
            </div>

            <div className='detail-item'>
                <label>Trạng thái:</label>
                <span>{packageDetail.isVisible ? 'Hoạt động' : 'Không hoạt động'}</span>
            </div>

            <div className='detail-item'>
                <label>Ngày tạo:</label>
                <span>{new Date(packageDetail.createdAt).toLocaleDateString()}</span>
            </div>

            <div className='detail-item'>
                <label>Ngày cập nhật:</label>
                <span>
                    {packageDetail.updatedAt
                        ? new Date(packageDetail.updatedAt).toLocaleDateString()
                        : 'Chưa có cập nhật'}
                </span>
            </div>

            <div className='action-buttons'>
                <button className='buy-button'>Mua Khóa Học</button>
            </div>
        </div>
    );
};

export default PackageDetailUser;
