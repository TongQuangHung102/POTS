import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { authFetch } from '../../utils/authFetch';
import { normalizeName } from '../../utils/validator';
import './AddPackage.css';

const AddPackage = () => {
    const [newPackage, setNewPackage] = useState({
        planName: '',
        price: '',
        description: '',
        duration: '',
        maxExercisesPerDay: '',
        isVisible: true
    });
    const apiUrl = process.env.REACT_APP_API_URL;

    const [errors, setErrors] = useState({});
    const [errorMessage, setErrorMessage] = useState('');
    const navigate = useNavigate();

    const validateForm = () => {
        let newErrors = {};
        let formIsValid = true;

        if (newPackage.planName.length > 30) {
            newErrors.planName = 'Tên gói không được vượt quá 30 ký tự';
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
                    : field === 'planName' || field === 'description'
                        ? normalizeName(value)
                        : value
        });
    };
    

    const handleSubmit = async (event) => {
        event.preventDefault();

        if (!validateForm()) return;

        try {
            const response = await authFetch(
                ` ${apiUrl}/api/SubscriptionPlan/add-subscriptionplan`,
                {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(newPackage)
                }
            );

            const data = await response.json();

            if (!response.ok) {
                throw new Error(data.message || 'Lỗi khi tạo gói mới!');
            }

            navigate('/admin/package');
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
            <p><strong>Lưu ý:</strong> <em>Giá trị âm sẽ được hiểu là</em> <strong>không giới hạn</strong>.</p>
            <p><strong>Ví dụ:</strong> <em>Để chỉ định thời gian không giới hạn, bạn có thể nhập giá trị</em> <strong>-1</strong> vào ô thời gian.</p>

            <form onSubmit={handleSubmit}>
                <div className='form-group'>
                    <label>Tên Gói</label>
                    <input
                        type='text'
                        value={newPackage.planName}
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
                        value={newPackage.price}
                        onChange={(e) => {
                            const value = e.target.value;
                            handleChange('price', value === '' ? '' : parseFloat(value));
                        }}
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
                        value={newPackage.description}
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
                        value={newPackage.duration}
                        onChange={(e) =>
                            handleChange(
                                'duration',
                                parseInt(e.target.value) || ''
                            )
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
                        value={newPackage.maxExercisesPerDay}
                        onChange={(e) =>
                            handleChange(
                                'maxExercisesPerDay',
                                parseInt(e.target.value) || ''
                            )
                        }
                        required
                    />
                    {errors.maxExercisesPerDay && (
                        <div className='error-text'>
                            {errors.maxExercisesPerDay}
                        </div>
                    )}
                </div>

                {/* Checkbox cho các tính năng */}

                {/* Nút Lưu và Hủy */}
                <div className='form-group'>
                    <button type='submit' className='save-button'>
                        Thêm Gói
                    </button>
                    <button
                        type='button'
                        className='cancel-button'
                        onClick={() => navigate('/admin/package')}
                    >
                        Hủy
                    </button>
                </div>
            </form>
        </div>
    );
};

export default AddPackage;
