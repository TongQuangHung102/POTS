import React, { useState, useEffect } from 'react';
import { useParams, useNavigate, useLocation } from 'react-router-dom';
import styles from '../content_manager/QuestionManage.module.css';
import BackLink from '../../components/BackLink';
import { updateReport } from '../../services/ReportService';
import useManagedGrades from '../../hooks/useManagedGrades';
import { getSubjectGradesByGrade } from '../../services/SubjectGradeService';

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
    const [subjectGradeId, setSubjectGradeId] = useState(null);
    const [subjects, setSubjects] = useState([]);

    const { managedGrades, setManagedGrades, selectedGrade, setSelectedGrade, isData } = useManagedGrades();

    // State cho phân trang
    const [currentPage, setCurrentPage] = useState(1);
    const questionsPerPage = 5;
    const totalPages = Math.ceil(totalQuestions / questionsPerPage);


    const navigate = useNavigate();

    const fetchSubjectGrades = async (gradeId) => {
        try {
            const data = await getSubjectGradesByGrade(gradeId);
            setSubjects(data);
            if (data.length > 0) {
                setSubjectGradeId(data[0].id);
            }
        } catch (error) {
            console.error("Có lỗi khi lấy dữ liệu lớp", error);
        }
    };

    //lay mon hoc dua tren gradeId
    useEffect(() => {
        if (selectedGrade) {
            fetchSubjectGrades(selectedGrade.id);
        }
    }, [selectedGrade]);


    const fetchReport = async () => {
        setLoading(true);
        setError(null);

        try {
            let apiUrl = `https://localhost:7259/api/Report/get-all-report?pageNumber=${currentPage}&pageSize=${questionsPerPage}&subjectGradeId=${subjectGradeId}`;

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
        } finally {
            setLoading(false);
        }

    };

    useEffect(() => {
        const delayDebounce = setTimeout(() => {
            fetchReport();
        }, 500);

        return () => clearTimeout(delayDebounce);
    }, [status, subjectGradeId]);

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
        Reject: "Không hợp lệ",
        Resolved: "Hợp lệ"
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

    const handleResolved = async () => {
        const updatedReport = { ...editingReport, status: 'Resolved' };
        console.log(updatedReport);
        await handleSave(updatedReport);
    };

    const handleClose = () => {
        setIsEditing(false);
        setIsEditing(null);
    };

    const handleGradeChange = (event) => {
        const gradeId = parseInt(event.target.value, 10);
        const grade = managedGrades.find(g => g.id === gradeId);
        setSelectedGrade(grade);
    };

    return (
        <div className={styles.questionManager}>
            <h2>Quản Lý Báo Cáo</h2>
            <BackLink></BackLink>
            <div className={styles.toolbar}>
                {managedGrades.length > 0 ? (
                    <select className={styles.commonInput} value={selectedGrade?.id} onChange={handleGradeChange}>
                        {managedGrades.map((grade) => (
                            <option key={grade.id} value={grade.id}>
                                {grade.name}
                            </option>
                        ))}
                    </select>
                ) : (
                    <p>Không có khối nào để quản lý</p>
                )}
                {subjects.length > 0 ? (
                    <select className={styles.commonInput} value={subjectGradeId} onChange={(e) => {
                        setSubjectGradeId(e.target.value);
                    }}>
                        {subjects.map((subject) => (
                            <option key={subject.id} value={subject.id}>{subject.name}</option>
                        ))}
                    </select>
                ) : (
                    <p>Không có môn học nào</p>
                )}
                <select className={styles.commonInput} value={status} onChange={(e) => setStatus(e.target.value)}>
                    <option value="">Chọn Trạng Thái</option>
                    <option value="Pending">Chờ giải quyết</option>
                    <option value="Reject">Không hợp lệ</option>
                    <option value="Resolved">Hợp lệ</option>
                </select>
            </div>

            {/* Hiển thị trạng thái tải dữ liệu */}
            {loading && <p style={{ textAlign: "center", fontStyle: "italic", color: "gray" }}>Đang tải câu hỏi...</p>}
            {error && <p style={{ color: 'red' }}>{error}</p>}

            {/* Danh sách câu hỏi */}
            {reports.length > 0 ? (<div className={styles.questionList}>
                {reports.map(q => (
                    <div key={q.reportId} className={styles.questionItem} onClick={() => toggleQuestion(q.reportId)}>
                        <div className={styles.questionHeader}>
                            <span className={styles.questionText}>(Câu hỏi ID : {q.questionId}) {q.reason} ({q.count})</span>
                            <div className={styles.questionActions}>
                                <span className={`${styles.status} ${getStatusClass(q.status)}`}> {statusMap[q.status] || "Không xác định"}</span>
                                <span className={`${styles.expandIcon} ${q.isExpanded ? styles.expanded : ''}`} >▼</span>
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
                                    {q.status === 'Pending' && (
                                        <div>
                                            <button className='btn btn-danger me-2 mt-2' onClick={() => handleReject(q)}>Từ Chối</button>
                                            <button className='btn btn-primary mt-2' onClick={() => handleEdit(q)}>Chỉnh sửa</button>
                                        </div>
                                    )}

                                </div>
                            </div>
                        )}
                    </div>
                ))}
            </div>) : (
                <p style={{ textAlign: "center", fontStyle: "italic", color: "gray" }}>Chưa có báo cáo nào</p>
            )}
            

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
                                        value={option.number}
                                        checked={editingReport.correctAnswer === option.number}
                                        onChange={(e) =>
                                            setEditingReport({ ...editingReport, correctAnswer: Number(e.target.value) })
                                        }
                                    /> Đáp án đúng
                                </div>
                            ))}
                        </label>

                        <div className="button-group">
                            <button onClick={handleResolved}>Lưu</button>
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
