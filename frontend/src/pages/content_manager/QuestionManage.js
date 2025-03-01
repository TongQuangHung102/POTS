import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import styles from './QuestionManage.module.css';

const QuestionManage = () => {
    // State chứa danh sách câu hỏi từ API
    const { lessonId } = useParams();
    const [questions, setQuestions] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [levelId, setLevelId] = useState('');
    const [isVisible, setIsVisible] = useState(''); 

    // State cho phân trang
    const [currentPage, setCurrentPage] = useState(1);
    const questionsPerPage = 3;
    const [searchTerm, setSearchTerm] = useState('');

    // Gọi API để lấy dữ liệu câu hỏi
    useEffect(() => {
        const fetchQuestions = async () => {

            setLoading(true);
            setError(null);

            try {
                let apiUrl = `https://localhost:7259/api/Question/get-all-question?lessonId=${lessonId}&page=${currentPage}&pageSize=${questionsPerPage}`;

                const queryParams = [];

                if (levelId) queryParams.push(`levelId=${levelId}`);
                if (isVisible) queryParams.push(`isVisible=${isVisible}`);
                if (queryParams.length > 0) apiUrl += `?${queryParams.join('&')}`;

                const response = await fetch(apiUrl);

                if (!response.ok) throw new Error('Lỗi khi lấy dữ liệu');
                const data = await response.json();
                console.log("Questions state:", data);
                // Định dạng dữ liệu cho React
                const formattedQuestions = data.date.map(q => ({
                    id: q.id,
                    question: q.questionText,
                    options: q.answerQuestions.map(a => ({
                        id: a.number.toString(),
                        text: a.answerText
                    })),
                    correctAnswer: q.correctAnswer.toString(),
                    isExpanded: false
                }));

                setQuestions(formattedQuestions);
            } catch (error) {
                setError(error.message);
            } finally {
                setLoading(false);
            }
        };

        fetchQuestions();
    },  [lessonId, levelId, isVisible, currentPage]);

    // Lọc câu hỏi theo từ khóa tìm kiếm
    const filteredQuestions = questions.filter(q =>
        q.question.toLowerCase().includes(searchTerm.toLowerCase())
    );


    return (
        <div className={styles.questionManager}>
            <h1>Quản Lý Câu Hỏi</h1>

            <div className={styles.toolbar}>
                <select value={levelId} onChange={(e) => setLevelId(e.target.value)}>
                    <option value="">Chọn Level</option>
                    <option value="1">Level 1</option>
                    <option value="2">Level 2</option>
                    <option value="3">Level 3</option>
                </select>

                <select value={isVisible} onChange={(e) => setIsVisible(e.target.value)}>
                    <option value="">Chọn Trạng Thái</option>
                    <option value="true">Hiển Thị</option>
                    <option value="false">Ẩn</option>
                </select>

                <input
                    type="text"
                    placeholder="Tìm kiếm câu hỏi..."
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                />
            </div>

            {/* Hiển thị trạng thái tải dữ liệu */}
            {loading && <p>Đang tải câu hỏi...</p>}
            {error && <p style={{ color: 'red' }}>{error}</p>}

            {/* Danh sách câu hỏi */}
            <div className={styles.questionList}>
                {filteredQuestions.map(q => (
                    <div key={q.id} className={styles.questionItem}>
                        <div className={styles.questionHeader} onClick={() => {
                            setQuestions(prev =>
                                prev.map(question =>
                                    question.id === q.id ? { ...question, isExpanded: !question.isExpanded } : question
                                )
                            );
                        }}>
                            <span>{q.question}</span>
                            <span className={styles.expandIcon}>{q.isExpanded ? '▲' : '▼'}</span>
                        </div>
                        {q.isExpanded && (
                            <div className={styles.questionDetails}>
                                {q.options.map(opt => (
                                    <div key={opt.id} className={opt.id === q.correctAnswer ? styles.correctAnswer : ''}>
                                        {opt.id.toUpperCase()}. {opt.text}
                                    </div>
                                ))}
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
                <span>Trang {currentPage}</span>
                <button onClick={() => setCurrentPage(prev => prev + 1)}>
                    &raquo;
                </button>
            </div>
        </div>
    );
};

export default QuestionManage;
