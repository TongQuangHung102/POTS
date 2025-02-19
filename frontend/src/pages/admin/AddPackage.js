import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import './AddPackage.css';

const AddPackage = () => {
    const [newPackage, setNewPackage] = useState({
        planName: '',
        price: '',
        description: '',
        duration: '',
        maxExercisesPerDay: '',
        isAIAnalysis: false,
        isPersonalization: false,
        isBasicStatistics: false,
        isAdvancedStatistics: false
    });

    const [errors, setErrors] = useState({});
    const [errorMessage, setErrorMessage] = useState('');
    const navigate = useNavigate();

    const validateForm = () => {
        let newErrors = {};
        let formIsValid = true;

        if (!newPackage.planName.trim()) {
            newErrors.planName = 'Tên Gói không được để trống!';
            formIsValid = false;
        }
        if (!newPackage.description.trim()) {
            newErrors.description = 'Mô tả không được để trống!';
            formIsValid = false;
        }
        if (!newPackage.price || newPackage.price <= 0) {
            newErrors.price = 'Giá phải lớn hơn 0!';
            formIsValid = false;
        }
        if (!newPackage.duration || newPackage.duration <= 0) {
            newErrors.duration = 'Thời gian phải lớn hơn 0!';
            formIsValid = false;
        }
        if (
            newPackage.maxExercisesPerDay === '' ||
            newPackage.maxExercisesPerDay < 0
        ) {
            newErrors.maxExercisesPerDay =
                'Số bài tập tối đa mỗi ngày không được âm!';
            formIsValid = false;
        }

        setErrors(newErrors);
        return formIsValid;
    };

    const handleChange = (field, value) => {
        setNewPackage({
            ...newPackage,
            [field]:
                field === 'maxExercisesPerDay' ||
                field === 'price' ||
                field === 'duration'
                    ? value === ''
                        ? ''
                        : parseInt(value, 10)
                    : value
        });
    };

    const handleSubmit = async (event) => {
        event.preventDefault();

        if (!validateForm()) return;

        try {
            const response = await fetch(
                'https://localhost:7259/api/SubscriptionPlan',
                {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(newPackage)
                }
            );

            if (!response.ok) throw new Error('Lỗi khi tạo gói mới!');

            navigate('/admin/listpackage'); // Chuyển về danh sách gói
        } catch (error) {
            setErrorMessage(error.message);
        }
    };

    return (
        <div className='package-detail-container'>
            <h2>Thêm Gói Mới</h2>

            {errorMessage && (
                <div className='error-message'>{errorMessage}</div>
            )}

            <form onSubmit={handleSubmit}>
                <div className='form-group'>
                    <label>Tên Gói</label>
                    <input
                        type='text'
                        value={newPackage.planName}
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
                        value={newPackage.price}
                        onChange={(e) =>
                            handleChange(
                                'price',
                                parseFloat(e.target.value) || ''
                            )
                        }
                    />
                    {errors.price && (
                        <div className='error-text'>{errors.price}</div>
                    )}
                </div>

                <div className='form-group'>
                    <label>Mô tả</label>
                    <input
                        type='text'
                        value={newPackage.description}
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
                        value={newPackage.duration}
                        onChange={(e) =>
                            handleChange(
                                'duration',
                                parseInt(e.target.value) || ''
                            )
                        }
                    />
                    {errors.duration && (
                        <div className='error-text'>{errors.duration}</div>
                    )}
                </div>

                <div className='form-group'>
                    <label>Số bài tập tối đa mỗi ngày</label>
                    <input
                        type='number'
                        value={newPackage.maxExercisesPerDay}
                        onChange={(e) =>
                            handleChange(
                                'maxExercisesPerDay',
                                parseInt(e.target.value) || ''
                            )
                        }
                    />
                    {errors.maxExercisesPerDay && (
                        <div className='error-text'>
                            {errors.maxExercisesPerDay}
                        </div>
                    )}
                </div>

                {/* Checkbox cho các tính năng */}
                <div className='form-group'>
                    <label>Tính năng AI Analysis</label>
                    <input
                        type='checkbox'
                        checked={newPackage.isAIAnalysis}
                        onChange={(e) =>
                            handleChange('isAIAnalysis', e.target.checked)
                        }
                    />
                </div>

                <div className='form-group'>
                    <label>Tính năng Personalization</label>
                    <input
                        type='checkbox'
                        checked={newPackage.isPersonalization}
                        onChange={(e) =>
                            handleChange('isPersonalization', e.target.checked)
                        }
                    />
                </div>

                <div className='form-group'>
                    <label>Tính năng Basic Statistics</label>
                    <input
                        type='checkbox'
                        checked={newPackage.isBasicStatistics}
                        onChange={(e) =>
                            handleChange('isBasicStatistics', e.target.checked)
                        }
                    />
                </div>

                <div className='form-group'>
                    <label>Tính năng Advanced Statistics</label>
                    <input
                        type='checkbox'
                        checked={newPackage.isAdvancedStatistics}
                        onChange={(e) =>
                            handleChange(
                                'isAdvancedStatistics',
                                e.target.checked
                            )
                        }
                    />
                </div>

                {/* Nút Lưu và Hủy */}
                <div className='form-group'>
                    <button type='submit' className='save-button'>
                        Thêm Gói
                    </button>
                    <button
                        type='button'
                        className='cancel-button'
                        onClick={() => navigate('/admin/listpackage')}
                    >
                        Hủy
                    </button>
                </div>
            </form>
        </div>
    );
};

export default AddPackage;
