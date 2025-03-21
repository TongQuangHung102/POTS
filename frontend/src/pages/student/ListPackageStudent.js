import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';


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

    const handleBuyPackage = (planId) => {
        // Mở một popup hoặc chuyển hướng đến trang thanh toán
        console.log(`Mua gói khóa học có ID: ${planId}`);
    };

    return (
        <div className='package-list-container'>
            <h2>Danh Sách Khóa Học</h2>
            {errorMessage && (
                <div className='error-message'>{errorMessage}</div>
            )}
            <table className='package-table'>
                <thead>
                    <tr>
                        <th>Tên Gói</th>
                        <th>Giá</th>
                        <th>Mô tả</th>
                        <th>Trạng thái</th>
                        <th>Hành động</th>
                    </tr>
                </thead>
                <tbody>
                    {packages.length > 0 ? (
                        packages.map((pkg) => (
                            <tr key={pkg.planId}>
                                <td>{pkg.planName}</td>
                                <td>{pkg.price} VNĐ</td>
                                <td>{pkg.description}</td>
                                <td>
                                    {pkg.isVisible ? (
                                        <span style={{ color: "green" }}>Còn Hàng</span>
                                    ) : (
                                        <span style={{ color: "red" }}>Hết Hàng</span>
                                    )}
                                </td>
                                <td>
                                    {pkg.isVisible && (
                                        <button
                                            className='buy-button'
                                            onClick={() => handleBuyPackage(pkg.planId)}
                                        >
                                            Mua
                                        </button>
                                    )}
                                    <Link
                                        to={`/student/package/${pkg.planId}`}
                                        className='detail-link'
                                    >
                                        Xem Chi Tiết
                                    </Link>
                                </td>
                            </tr>
                        ))
                    ) : (
                        <tr>
                            <td colSpan='5'>Không có dữ liệu</td>
                        </tr>
                    )}
                </tbody>
            </table>
        </div>
    );
};

export default ListPackageStudent;
