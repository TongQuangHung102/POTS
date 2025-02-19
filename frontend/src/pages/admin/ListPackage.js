import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import './ListPackage.css';

const ListPackage = () => {
    const [packages, setPackages] = useState([]);
    const [errorMessage, setErrorMessage] = useState('');

    useEffect(() => {
        const fetchPackages = async () => {
            try {
                const response = await fetch(
                    'https://localhost:7259/api/SubscriptionPlan'
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

    return (
        <div className='package-list-container'>
            <h2>Danh Sách Gói</h2>
            {errorMessage && (
                <div className='error-message'>{errorMessage}</div>
            )}
            <div className='add-package-button-container'>
                <Link to='/admin/addpackage' className='add-package-button'>
                    Thêm Gói Mới
                </Link>
            </div>
            <table className='package-table'>
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Tên Gói</th>
                        <th>Giá</th>
                        <th>Mô tả</th>
                        <th>AI Analysis</th>
                        <th>Thống kê nâng cao</th>
                        <th>Thống kê cơ bản</th>
                        <th>Personalization</th>
                        <th>Chi tiết</th>
                    </tr>
                </thead>
                <tbody>
                    {packages.length > 0 ? (
                        packages.map((pkg) => (
                            <tr key={pkg.planId}>
                                <td>{pkg.planId}</td>
                                <td>{pkg.planName}</td>
                                <td>{pkg.price}</td>
                                <td>{pkg.description}</td>
                                <td>
                                    <input
                                        type='checkbox'
                                        checked={pkg.isAIAnalysis}
                                        disabled
                                    />
                                </td>
                                <td>
                                    <input
                                        type='checkbox'
                                        checked={pkg.isAdvancedStatistics}
                                        disabled
                                    />
                                </td>
                                <td>
                                    <input
                                        type='checkbox'
                                        checked={pkg.isBasicStatistics}
                                        disabled
                                    />
                                </td>
                                <td>
                                    <input
                                        type='checkbox'
                                        checked={pkg.isPersonalization}
                                        disabled
                                    />
                                </td>
                                <td>
                                    <Link
                                        to={`/admin/package/${pkg.planId}`}
                                        className='detail-link'
                                    >
                                        Chi tiết
                                    </Link>
                                </td>
                            </tr>
                        ))
                    ) : (
                        <tr>
                            <td colSpan='9'>Không có dữ liệu</td>
                        </tr>
                    )}
                </tbody>
            </table>
        </div>
    );
};

export default ListPackage;
