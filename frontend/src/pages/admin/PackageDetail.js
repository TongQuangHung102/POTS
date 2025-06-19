import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { authFetch } from '../../utils/authFetch';
import { getSubscriptionPlanById } from '../../services/subscriptionService';
import { normalizeName } from '../../utils/validator';
import './PackageDetail.css';

const PackageDetail = () => {
    const { planId } = useParams(); // Lấy planId từ URL
    const [packageDetail, setPackageDetail] = useState(null);
    const [errorMessage, setErrorMessage] = useState('');
    const [errors, setErrors] = useState({
        planName: '',
        description: '',
        price: '',
        duration: '',
        maxExercisesPerDay: ''
    });

    const apiUrl = process.env.REACT_APP_API_URL;
    const navigate = useNavigate(); // Hook để điều hướng người dùng

    useEffect(() => {
        const fetchPackageDetail = async () => {
            try {
                const data = await getSubscriptionPlanById(planId);
                setPackageDetail(data);
            } catch (error) {
                setErrorMessage(error.message);
                console.error('Có lỗi khi lấy chi tiết gói:', error);
            }
        };

        fetchPackageDetail();
    }, [planId]);

    const handleSubmit = async (event) => {
        event.preventDefault();

        setErrors({
            planName: '',
            description: '',
            price: '',
            duration: '',
            maxExercisesPerDay: ''
        });

        let formIsValid = true;
        let newErrors = {};

        if (packageDetail.planName.length > 30) {
            newErrors.planName = 'Tên gói không được vượt quá 30 ký tự';
            formIsValid = false;
        }

        if (!formIsValid) {
            setErrors(newErrors);
            return;
        }

        try {
            const response = await authFetch(
                `${apiUrl}/api/SubscriptionPlan/edit-subscriptionplan/${planId}`,
                {
                    method: 'PUT',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(packageDetail)
                }
            );

            if (!response.ok) {
                const data = await response.json();
                throw new Error(data.message || 'Lỗi khi cập nhật gói');
            }

            const updatedPackage = await response.json();
            setPackageDetail(updatedPackage);

            navigate(`/admin/package`);
        } catch (error) {
            alert(error.message);
            console.error('Có lỗi khi cập nhật gói:', error);
        }
    };

    const handleChange = (field, value) => {
        if (field === 'planName' || field === 'description') {
            value = normalizeName(value);
        }
        setPackageDetail({ ...packageDetail, [field]: value });
    };

    const handleCancel = () => {
        navigate('/admin/package');
    };

    if (errorMessage) {
        return <div className='error-message'>{errorMessage}</div>;
    }

    if (!packageDetail) {
        return <div>Đang tải chi tiết gói...</div>;
    }

    return (
        <div className='package-detail-container'>
            <h2>Chi tiết Gói: {packageDetail.planName}</h2>

            <form onSubmit={handleSubmit}>
                <div className='form-group'>
                    <label>Tên Gói</label>
                    <input
                        type='text'
                        value={packageDetail.planName}
                        onChange={(e) =>
                            handleChange('planName', e.target.value)
                        }
                        required
                    />
                    {errors.planName && (
                        <div className='error-text'>{errors.planName}</div>
                    )}
                </div>

                <div className='form-group'>
                    <label>Giá</label>
                    <input
                        type='number'
                        value={packageDetail.price}
                        onChange={(e) => handleChange('price', e.target.value)}
                        required
                    />
                    {errors.price && (
                        <div className='error-text'>{errors.price}</div>
                    )}
                </div>

                <div className='form-group'>
                    <label>Mô tả</label>
                    <input
                        type='text'
                        value={packageDetail.description}
                        onChange={(e) =>
                            handleChange('description', e.target.value)
                        }
                        required
                    />
                    {errors.description && (
                        <div className='error-text'>{errors.description}</div>
                    )}
                </div>

                <div className='form-group'>
                    <label>Thời gian (ngày)</label>
                    <input
                        type='number'
                        value={packageDetail.duration}
                        onChange={(e) =>
                            handleChange('duration', e.target.value)
                        }
                        required
                    />
                    {errors.duration && (
                        <div className='error-text'>{errors.duration}</div>
                    )}
                </div>

                <div className='form-group'>
                    <label>Số bài tập tối đa mỗi ngày</label>
                    <input
                        type='number'
                        value={packageDetail.maxExercisesPerDay}
                        onChange={(e) =>
                            handleChange('maxExercisesPerDay', e.target.value)
                        }
                        required
                    />
                    {errors.maxExercisesPerDay && (
                        <div className='error-text'>
                            {errors.maxExercisesPerDay}
                        </div>
                    )}
                </div>
                <div className='form-group'>
                    <label>
                        Trạng thái:
                        <select
                            value={packageDetail.isVisible}
                            onChange={(e) =>
                                handleChange('isVisible', e.target.value)
                            }
                        >
                            <option value="true">Hoạt động</option>
                            <option value="false">Không hoạt động</option>
                        </select>
                    </label>
                </div>

                <div className='form-group'>
                    <label>Ngày tạo</label>
                    <input
                        type='text'
                        value={new Date(
                            packageDetail.createdAt
                        ).toLocaleDateString()}
                        disabled
                    />
                </div>

                <div className='form-group'>
                    <label>Ngày cập nhật</label>
                    <input
                        type='text'
                        value={
                            packageDetail.updatedAt
                                ? new Date(
                                    packageDetail.updatedAt
                                ).toLocaleDateString()
                                : 'Chưa có cập nhật'
                        }
                        disabled
                    />
                </div>

                <div className='form-group'>
                    <button type='submit' className='save-button'>
                        Cập nhật
                    </button>
                </div>

                <div className='form-group'>
                    <button
                        type='button'
                        className='cancel-button'
                        onClick={handleCancel}
                    >
                        Hủy
                    </button>
                </div>
            </form>
        </div>
    );
};

export default PackageDetail;
