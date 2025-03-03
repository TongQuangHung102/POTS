import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
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
    const navigate = useNavigate(); // Hook để điều hướng người dùng

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

    const handleSubmit = async (event) => {
        event.preventDefault();

        // Reset lỗi trước khi kiểm tra mới
        setErrors({
            planName: '',
            description: '',
            price: '',
            duration: '',
            maxExercisesPerDay: ''
        });

        let formIsValid = true;
        let newErrors = {};

        // Kiểm tra dữ liệu nhập vào và cập nhật lỗi cho từng trường
        if (!packageDetail.planName.trim()) {
            newErrors.planName = 'Tên Gói không được để trống!';
            formIsValid = false;
        }
        if (!packageDetail.description.trim()) {
            newErrors.description = 'Mô tả không được để trống!';
            formIsValid = false;
        }
        if (packageDetail.price <= 0) {
            newErrors.price = 'Giá không được âm hoặc bằng 0!';
            formIsValid = false;
        }
        if (packageDetail.duration <= 0) {
            newErrors.duration = 'Thời gian không được âm hoặc bằng 0!';
            formIsValid = false;
        }
        if (packageDetail.maxExercisesPerDay < 0) {
            newErrors.maxExercisesPerDay =
                'Số bài tập tối đa mỗi ngày không được âm!';
            formIsValid = false;
        }

        // Nếu có lỗi, không tiếp tục gửi dữ liệu
        if (!formIsValid) {
            setErrors(newErrors);
            return;
        }

        try {
            const response = await fetch(
                `https://localhost:7259/api/SubscriptionPlan/edit-subscriptionplan/${planId}`,
                {
                    method: 'PUT',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify(packageDetail)
                }
            );

            if (!response.ok) {
                throw new Error('Lỗi khi cập nhật gói');
            }

            const updatedPackage = await response.json();
            setPackageDetail(updatedPackage);

            navigate(`/admin/package`);
        } catch (error) {
            setErrorMessage(error.message);
            console.error('Có lỗi khi cập nhật gói:', error);
        }
    };

    const handleChange = (field, value) => {
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

            {errorMessage && (
                <div className='error-message'>{errorMessage}</div>
            )}

            <form onSubmit={handleSubmit}>
                <div className='form-group'>
                    <label>Tên Gói</label>
                    <input
                        type='text'
                        value={packageDetail.planName}
                        onChange={(e) =>
                            handleChange('planName', e.target.value)
                        }
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
                    />
                    {errors.duration && (
                        <div className='error-text'>{errors.duration}</div>
                    )}
                </div>

                <div className='form-group'>
                    <label>Tính năng AI Analysis</label>
                    <input
                        type='checkbox'
                        checked={packageDetail.isAIAnalysis}
                        onChange={(e) =>
                            handleChange('isAIAnalysis', e.target.checked)
                        }
                    />
                </div>

                <div className='form-group'>
                    <label>Tính năng Personalization</label>
                    <input
                        type='checkbox'
                        checked={packageDetail.isPersonalization}
                        onChange={(e) =>
                            handleChange('isPersonalization', e.target.checked)
                        }
                    />
                </div>

                <div className='form-group'>
                    <label>Tính năng Basic Statistics</label>
                    <input
                        type='checkbox'
                        checked={packageDetail.isBasicStatistics}
                        onChange={(e) =>
                            handleChange('isBasicStatistics', e.target.checked)
                        }
                    />
                </div>

                <div className='form-group'>
                    <label>Tính năng Advanced Statistics</label>
                    <input
                        type='checkbox'
                        checked={packageDetail.isAdvancedStatistics}
                        onChange={(e) =>
                            handleChange(
                                'isAdvancedStatistics',
                                e.target.checked
                            )
                        }
                    />
                </div>

                <div className='form-group'>
                    <label>Số bài tập tối đa mỗi ngày</label>
                    <input
                        type='number'
                        value={packageDetail.maxExercisesPerDay}
                        onChange={(e) =>
                            handleChange('maxExercisesPerDay', e.target.value)
                        }
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
