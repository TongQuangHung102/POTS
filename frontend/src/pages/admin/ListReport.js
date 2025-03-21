import React, { useState, useEffect } from 'react';
import { useParams, useNavigate, useLocation } from 'react-router-dom';
import styles from '../content_manager/QuestionManage.module.css';
import BackLink from '../../components/BackLink';
import { updateReport } from '../../services/ReportService'; 

const ListReport = () => {
    // State chứa danh sách câu hỏi từ API
    const [reports, setReports] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    const [status, setStatus] = useState('');
    const [totalQuestions, setTotalQuestions] = useState(0);
    const [isEditing, setIsEditing] = useState(false);
    const [editingReport, setEditingReport] = useState(null);
    const [errorMessage, setErrorMessage] = useState('');
    const [successMessage, setSuccessMessage] = useState('');


    // State cho phân trang
    const [currentPage, setCurrentPage] = useState(1);
    const questionsPerPage = 5;
    const totalPages = Math.ceil(totalQuestions / questionsPerPage);


    const navigate = useNavigate();


    const fetchReport = async () => {
        console.log('aaaaaaaaaaaaaaaaa');
        setLoading(true);
        setError(null);

        try {
            let apiUrl = `https://localhost:7259/api/Report/get-all-report?pageNumber=${currentPage}&pageSize=${questionsPerPage}`;

            const queryParams = [];

            if (status) queryParams.push(`status=${status}`);
            if (queryParams.length > 0) apiUrl += `&${queryParams.join("&")}`;

            const response = await fetch(apiUrl);

            if (!response.ok) throw new Error('Lỗi khi lấy dữ liệu');
            const data = await response.json();
            console.log(data);
            setTotalQuestions(data.totalReport);
            setReports(data.data);
        } catch (error) {
            setError(error.message);
        }finally {
            setLoading(false);
        }
        
    };

    useEffect(() => {
        const delayDebounce = setTimeout(() => {
            fetchReport();
            setLoading(false);
        }, 500);

        return () => clearTimeout(delayDebounce);
    }, [status]);

    const toggleQuestion = (id) => {
        setReports(
            reports.map(q =>
                q.reportId === id ? { ...q, isExpanded: !q.isExpanded } : q
            )
        );
    };


    const handleSave = async (report) => {
        const result = await updateReport(report);

        if (result.success) {
            alert(result.message);
            fetchReport(); 
            setIsEditing(false);
        } else {
            setErrorMessage(result.message);
        }
    };

    const statusMap = {
        Pending: "Chưa xem xét",
        Reject: "Từ chối",
        Resolved: "Đã giải quyết"
    };

    const getStatusClass = (status) => {
        switch (status) {
          case "Pending":
            return styles.pending;
          case "Reject":
            return styles.reject;
          case "Resolved":
            return styles.resolved;
          default:
            return styles.unknown;
        }
      };

      const handleEdit = (question) => {
        setEditingReport(question);
        setIsEditing(true);
    };

    const handleReject = async (report) => {
        const updatedReport = { ...report, status: 'Reject' };
        await handleSave(updatedReport);
    };

    const handleClose = () => {
        setIsEditing(false);
        setIsEditing(null);
    };

    if (loading) return <div className={styles.loading}>
        <p>Đang tải câu hỏi...</p>
    </div>

    return (
        <div className={styles.questionManager}>
            <h2>Quản Lý Báo Cáo</h2>
            <BackLink></BackLink>
            <div className={styles.toolbar}>
                <select className={styles.commonInput} value={status} onChange={(e) => setStatus(e.target.value)}>
                    <option value="">Chọn Trạng Thái</option>
                    <option value="Pending">Pending</option>
                    <option value="Rejected">Rejected</option>
                    <option value="Resolved">Resolved</option>
                </select>
            </div>

            {/* Hiển thị trạng thái tải dữ liệu */}
            {loading && <p>Đang tải câu hỏi...</p>}
            {error && <p style={{ color: 'red' }}>{error}</p>}

            {/* Danh sách câu hỏi */}
            <div className={styles.questionList}>
                {reports.map(q => (
                    <div key={q.reportId} className={styles.questionItem}>
                        <div className={styles.questionHeader}>
                            <span className={styles.questionText}>{q.reportId}: {q.reason}</span>
                            <div className={styles.questionActions}>
                                <span className={`${styles.status} ${getStatusClass(q.status)}`}> {statusMap[q.status] || "Không xác định"}</span>
                                <button className={styles.editButton} onClick={() => handleReject(q)}>Từ Chối</button>
                                <button className={styles.editButton} onClick={() => handleEdit(q)}>Chỉnh sửa</button>
                                <span className={`${styles.expandIcon} ${q.isExpanded ? styles.expanded : ''}`} onClick={() => toggleQuestion(q.reportId)}>▼</span>
                            </div>
                        </div>

                        {q.isExpanded && (
                            <div className={styles.questionDetails}>
                                <div className={styles.optionsSection}>
                                    <p className={styles.optionsTitle}><strong>{q.questionText}</strong></p>
                                    <div className={styles.optionsList}>
                                        {q.answerQuestions.map(option => (
                                            <div
                                                key={option.id}
                                                className={`${styles.optionItem} ${option.number === q.correctAnswer ? styles.correctAnswer : ''}`}
                                            >
                                                <span className={styles.optionLabel}>{option.id}.</span>
                                                <span className={styles.optionText}>{option.text}</span>
                                                {option.number === q.correctAnswer && (
                                                    <span className={styles.correctIndicator}>✓</span>
                                                )}
                                            </div>
                                        ))}
                                    </div>
                                </div>
                            </div>
                        )}
                    </div>
                ))}
            </div>

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
            {errorMessage && <p className="error-message">{errorMessage}</p>}

            {isEditing && (
                <div className={styles.modal}>
                    <div className={styles.modalContent}>
                        <h3>Chỉnh sửa câu hỏi</h3>
                        <label>
                            Câu hỏi:
                            <input
                                type="text"
                                value={editingReport.questionText}
                                required
                                onChange={(e) =>
                                    setEditingReport({ ...editingReport, questionText: e.target.value })
                                }

                            />
                        </label>
                        <label>Các câu trả lời:
                            {editingReport.answerQuestions.map((option, index) => (
                                <div key={option.id} className={styles.answer} >
                                    <input
                                        className={`${styles.answerItem} ${option.number === editingReport.correctAnswer ? styles.correctAnswer : ""
                                            }`}
                                        type="text"
                                        value={option.text}
                                        required
                                        onChange={(e) => {
                                            const updatedOptions = [...editingReport.options];
                                            updatedOptions[index].text = e.target.value;
                                            setEditingReport({ ...editingReport, options: updatedOptions });
                                        }}

                                    />
                                    <input className={styles.radioInput}
                                        type="radio"
                                        name="correctAnswer"
                                        value={option.id}
                                        checked={editingReport.correctAnswer === option.number}
                                        onChange={(e) =>
                                            setEditingReport({ ...editingReport, correctAnswer: Number(e.target.value) })
                                        }
                                    /> Đáp án đúng
                                </div>
                            ))}
                        </label>

                        <div className="button-group">
                            <button onClick={handleSave}>Lưu</button>
                            <button onClick={handleClose}>Đóng</button>
                        </div>
                        {errorMessage && <p className="error-message">{errorMessage}</p>}
                    </div>
                </div>
            )}
        </div>
    );
};

export default ListReport;
