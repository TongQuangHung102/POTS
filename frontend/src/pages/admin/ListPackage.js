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

    return (
        <div className='package-list-container'>
            <h2>Danh Sách Gói</h2>
            {errorMessage && (
                <div className='error-message'>{errorMessage}</div>
            )}
            <div className='add-package-button-container'>
                <Link to='/admin/package/add' className='add-package-button'>
                    Thêm Gói Mới
                </Link>
            </div>
            <table className='package-table'>
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Tên Gói</th>
                        <th>Giá</th>
                        <th>Tính năng AI Analysis</th>
                        <th>Tính năng Advanced Statistics</th>
                        <th>Tính năng Basic Statistics</th>
                        <th>Tính năng Personalization</th>
                        <th>Trạng thái</th>
                        <th>Hành động</th>
                    </tr>
                </thead>
                <tbody>
                    {packages.length > 0 ? (
                        packages.map((pkg) => (
                            <tr key={pkg.planId}>
                                <td>{pkg.planId}</td>
                                <td>{pkg.planName}</td>
                                <td>{pkg.price}</td>
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
                                <td>{pkg.isVisible ? <span style={{ color: "green" }}>Hoạt động</span> : <span style={{ color: "red" }}>Không hoạt động</span>}</td>
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
