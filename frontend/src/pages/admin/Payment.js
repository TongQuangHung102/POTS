import React, { useState, useEffect } from 'react';
import { useParams, useNavigate, useLocation } from 'react-router-dom';
import styles from './Payment.module.css';
import BackLink from '../../components/BackLink';
import { authFetch } from '../../utils/authFetch';
import { formatDateTime } from '../../utils/timeUtils';
import { cleanAndValidateString } from '../../utils/validator';

const Payment = () => {
    // State chứa danh sách câu hỏi từ API
    const [payments, setPayments] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    const [total, setTotal] = useState(0);
    const [isEditing, setIsEditing] = useState(false);
    const [editingPayment, setEditingPayment] = useState(null);
    const [errorMessage, setErrorMessage] = useState('');
    const [successMessage, setSuccessMessage] = useState('');

    // State cho phân trang
    const [currentPage, setCurrentPage] = useState(1);
    const paymentPerPage = 10;
    const [transactionId, setTransactionId] = useState('');
    const [status, setStatus] = useState('');
    const [userEmail, setUserEmail] = useState('');
    const totalPages = Math.ceil(total / paymentPerPage);


    const apiUrl = process.env.REACT_APP_API_URL;

    const navigate = useNavigate();

    // Gọi API để lấy dữ liệu câu hỏi

    const fetchPayments = async () => {
        setLoading(true);
        setError(null);

        try {
            let api = `${apiUrl}/api/Payments/get-all-payment?page=${currentPage}&pageSize=${paymentPerPage}`;

            const queryParams = [];
            if (transactionId.trim() !== "") queryParams.push(`transactionId=${encodeURIComponent(transactionId)}`);
            if (status.trim() !== "") queryParams.push(`status=${encodeURIComponent(status)}`);
            if (userEmail.trim() !== "") queryParams.push(`userEmail=${encodeURIComponent(userEmail)}`);
            if (queryParams.length > 0) api += `&${queryParams.join("&")}`;

            const response = await authFetch(api);

            if (!response.ok) throw new Error('Lỗi khi lấy dữ liệu');
            const data = await response.json();
            setTotal(data.totalPayment);
            setPayments(data.data);
        } catch (error) {
            setError(error.message);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchPayments();
    }, [transactionId, status, userEmail, currentPage]);

    const handleEdit = (p) => {
        setEditingPayment(p);
        setIsEditing(true);
    };

    const handelClose = () =>{
        setIsEditing(false)
        setEditingPayment(null);
    }

    const handelSave = async () => {
        try {
            const response = await authFetch(`${apiUrl}/api/Payments/review`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    paymentId: editingPayment.paymentId,
                    isReview: editingPayment.isManuallyReviewed
                }),
            });
    
            if (response.ok) {
                alert('Cập nhật thành công');
                handelClose();
                fetchPayments();
            } else {
                const err = await response.text();
                setErrorMessage(err || 'Đã xảy ra lỗi khi cập nhật.');
            }
        } catch (error) {
            console.error('Lỗi fetch:', error);
            setErrorMessage('Lỗi kết nối đến máy chủ.');
        }
    };
    


    if (loading) return <div className={styles.loading}>
        <p>Đang tải câu hỏi...</p>
    </div>

    return (
        <div className={styles.questionManager}>
            <h2>Quản Lý Giao Dịch</h2>
            <BackLink></BackLink>
            <div className={styles.toolbar}>
                <select className={styles.commonInput} value={status} onChange={(e) => setStatus(e.target.value)}>
                    <option value="">Chọn Trạng Thái</option>
                    <option value="Pending">Chờ thanh toán</option>
                    <option value="Confirmed">Thành công</option>
                    <option value="Failed">Thất bại</option>
                </select>

                <input className={styles.commonInput}
                    type="text"
                    placeholder="Tìm kiếm bằng email"
                    value={userEmail}
                    onChange={(e) => setUserEmail(e.target.value)}
                />
                <input className={styles.commonInput}
                    type="text"
                    placeholder="Tìm kiếm theo mã giao dịch"
                    value={transactionId}
                    onChange={(e) => setTransactionId(e.target.value)}
                />
            </div>

            {/* Hiển thị trạng thái tải dữ liệu */}
            {loading && <p>Đang tải</p>}
            {error && <p style={{ color: 'red' }}>{error}</p>}

            <table className={styles.paymentTable}>
                <thead>
                    <tr>
                        <th style={{ width: "3%" }}>Id</th>
                        <th style={{ width: "10%" }}>Mã giao dịch</th>
                        <th style={{ width: "10%" }}>Người dùng</th>
                        <th style={{ width: "10%" }}>Ngày tạo</th>
                        <th style={{ width: "10%" }}>Tên gói</th>
                        <th style={{ width: "6%" }}>Giá</th>
                        <th style={{ width: "6%" }}>Số tiền chuyển</th>
                        <th style={{ width: "8%" }}>Trạng thái</th>
                        <th style={{ width: "12%" }}>Lý do thất bại</th>
                        <th style={{ width: "12%" }}>Ghi chú</th>
                        <th style={{ width: "3%" }}>Xem xét</th>
                        <th style={{ width: "5%" }}>Hành động</th>
                    </tr>
                </thead>
                <tbody>
                    {payments.length > 0 ? (payments.map((p) => (
                        <tr key={p.paymentId}>
                            <td>{p.paymentId}</td>
                            <td>{p.transactionId ?? ''}</td>
                            <td>{p.userEmail}</td>
                            <td>{formatDateTime(p.createdAt)}</td>
                            <td>{p.planName}</td>
                            <td>{p.amount.toLocaleString('vi-VN')} đ</td>
                            <td>{p.receivedAmount.toLocaleString('vi-VN')} đ</td>
                            <td>
                                {
                                    p.paymentStatus === 'Pending' ? 'Chờ thanh toán' :
                                        p.paymentStatus === 'Confirmed' ? 'Thành công' :
                                            p.paymentStatus === 'Failed' ? 'Thất bại' :
                                                p.paymentStatus
                                }
                            </td>
                            <td>{p.failureReason ?? ''}</td>
                            <td>{p.manualReviewerNote ?? ''}</td>
                            <td>
                                <input type="checkbox" checked={p.isManuallyReviewed === true} disabled />
                            </td>
                            <td>
                                <button disabled={p.receivedAmount <= p.amount || p.isManuallyReviewed === true} onClick={() => handleEdit(p)}>Sửa</button>
                            </td>
                        </tr>
                    ))
                    ) : (
                        <tr>
                            <td colSpan="11" style={{ textAlign: "center", fontStyle: "italic", color: "gray" }}>
                                Chưa có giao dịch nào.
                            </td>
                        </tr>
                    )}

                </tbody>
            </table>

            {/* Phân trang */}
            <div className={styles.pagination}>
                <button onClick={() => setCurrentPage(prev => Math.max(prev - 1, 1))} disabled={currentPage === 1}>
                    &laquo;
                </button>

                {[...Array(totalPages)].map((_, index) => {
                    const pageNum = index + 1;
                    return (
                        <button
                            key={pageNum}
                            onClick={() => setCurrentPage(pageNum)}
                            className={currentPage === pageNum ? styles.active : ""}
                        >
                            {pageNum}
                        </button>
                    );
                })}

                <button onClick={() => setCurrentPage(prev => Math.min(prev + 1, totalPages))} disabled={currentPage === totalPages}>
                    &raquo;
                </button>
            </div>
            {isEditing && (
                <div className={styles.modal}>
                    <div className={styles.modalContent}>
                        <h3>Chỉnh sửa giao dịch</h3>
                        <label>
                            Mã giao dịch
                            <input
                                type="text"
                                value={editingPayment.transactionId}
                                required
                                disabled

                            />
                        </label>
                        <label>Người dùng
                            <input
                                type="text"
                                value={editingPayment.userEmail}
                                required
                                disabled
                            />
                        </label>
                        <label>Trạng thái
                            <select
                                value={editingPayment.paymentStatus}
                                onChange={(e) =>
                                    setEditingPayment({ ...editingPayment, paymentStatus: e.target.value })
                                }
                                disabled={editingPayment.paymentStatus === "Confirmed"}
                            >
                                <option value="Pending">Chờ thanh toán</option>
                                <option value="Confirmed">Thành công</option>
                                <option value="Failed">Thất bại</option>
                            </select>
                        </label>
                        <label>Ghi chú
                            <select
                                value={editingPayment.isManuallyReviewed}
                                onChange={(e) =>
                                    setEditingPayment({ ...editingPayment, isManuallyReviewed: e.target.value === "true" })
                                }
                            >
                                <option value="false">Chưa xem xét</option>
                                <option value="true">Đã xem xét</option>
                            </select>
                        </label>


                        <div className="button-group">
                            <button onClick={() => handelSave()}>Lưu</button>
                            <button onClick={() => handelClose()} >Đóng</button>
                        </div>
                        {errorMessage && <p className="error-message">{errorMessage}</p>}
                    </div>
                </div>
            )}

        </div>
    );
};

export default Payment;
