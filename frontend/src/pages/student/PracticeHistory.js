import React, { useEffect, useState } from "react";
import { useNavigate, useParams } from 'react-router-dom';
import styles from "./PracticeHistory.module.css"; 
import BackLink from "../../components/BackLink";

const PracticeHistory = () => {
    const [history, setHistory] = useState([]);
    const [currentPage, setCurrentPage] = useState(1);
    const [totalPages, setTotalPages] = useState(1);
    const pageSize = 10;
    const { gradeId, subjectId, lessonId } = useParams();
    const userId = sessionStorage.getItem("userId");

    const navigate = useNavigate();

    useEffect(() => {
        fetchHistory(currentPage);
    }, [currentPage]);

    const fetchHistory = async (page) => {
        try {
            const response = await fetch(`https://localhost:7259/api/PracticeAttempt/history/${lessonId}/${userId}?pageNumber=${page}&pageSize=${pageSize}`);
            if (!response.ok) throw new Error("Lỗi khi tải lịch sử làm bài.");
            const data = await response.json();
            setHistory(data.data);
            setTotalPages(data.totalPages);
        } catch (error) {
            console.error(error.message);
        }
    };

    return (
        <div className={styles.container}>
            <h2 className={styles.title}>Lịch sử làm bài</h2>
            <BackLink></BackLink>
            <table className={styles.table}>
                <thead>
                    <tr>
                        <th>Thời gian</th>
                        <th>Số câu đúng</th>
                        <th>Thời gian làm bài (giây)</th>
                        <th>Hành động</th>
                    </tr>
                </thead>
                <tbody>
                    {history.length === 0 ? (
                        <tr>
                            <td colSpan="5" className="text-center p-3">Không có dữ liệu</td>
                        </tr>
                    ) : (
                        history.map((attempt) => (
                            <tr key={attempt.practiceId}>
                                <td>{new Date(attempt.createAt).toLocaleString()}</td>
                                <td>{attempt.correctAnswers}</td>
                                <td>{attempt.timePractice}</td>
                                <td>
                                    <button
                                        className={styles.button}
                                        onClick={() => navigate(`/student/grade/${gradeId}/subject/${subjectId}/lesson/${lessonId}/history/practice/${attempt.practiceId}`)}
                                    >
                                        Xem chi tiết
                                    </button>
                                </td>
                            </tr>
                        ))
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

        </div>
    );
};

export default PracticeHistory;
