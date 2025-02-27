import React, { useState } from 'react';
import styles from './QuestionManage.module.css';

const QuestionManage = () => {
    // Dữ liệu mẫu với 4 đáp án cho mỗi câu hỏi
    const [questions, setQuestions] = useState([
        {
            id: 1,
            question: 'Thủ đô của Việt Nam là gì?',
            options: [
                { id: 'a', text: 'Hà Nội' },
                { id: 'b', text: 'Hồ Chí Minh' },
                { id: 'c', text: 'Đà Nẵng' },
                { id: 'd', text: 'Huế' }
            ],
            correctAnswer: 'a',
            isExpanded: false
        },
        {
            id: 2,
            question: '1 + 1 = ?',
            options: [
                { id: 'a', text: '1' },
                { id: 'b', text: '2' },
                { id: 'c', text: '3' },
                { id: 'd', text: '4' }
            ],
            correctAnswer: 'b',
            isExpanded: false
        },
        {
            id: 3,
            question: 'HTML là viết tắt của gì?',
            options: [
                { id: 'a', text: 'HyperText Markup Language' },
                { id: 'b', text: 'High Tech Modern Language' },
                { id: 'c', text: 'Home Tool Markup Language' },
                { id: 'd', text: 'Hyperlinks and Text Markup Language' }
            ],
            correctAnswer: 'a',
            isExpanded: false
        },
        {
            id: 4,
            question: 'CSS dùng để làm gì?',
            options: [
                { id: 'a', text: 'Tạo cấu trúc trang web' },
                { id: 'b', text: 'Định dạng và trang trí trang web' },
                { id: 'c', text: 'Tạo chức năng tương tác trang web' },
                { id: 'd', text: 'Tạo cơ sở dữ liệu cho trang web' }
            ],
            correctAnswer: 'b',
            isExpanded: false
        },
        {
            id: 5,
            question: 'JavaScript là ngôn ngữ lập trình phía máy chủ hay phía máy khách?',
            options: [
                { id: 'a', text: 'Chỉ phía máy chủ' },
                { id: 'b', text: 'Chỉ phía máy khách' },
                { id: 'c', text: 'Cả hai, nhưng chủ yếu là phía máy khách' },
                { id: 'd', text: 'Không phải là ngôn ngữ lập trình' }
            ],
            correctAnswer: 'c',
            isExpanded: false
        },
    ]);

    // State cho phân trang
    const [currentPage, setCurrentPage] = useState(1);
    const questionsPerPage = 3;

    // State cho bộ lọc
    const [searchTerm, setSearchTerm] = useState('');

    // Xử lý thêm câu hỏi mới
    const handleAddQuestion = () => {
        const newQuestion = {
            id: questions.length + 1,
            question: 'Câu hỏi mới',
            options: [
                { id: 'a', text: 'Đáp án A' },
                { id: 'b', text: 'Đáp án B' },
                { id: 'c', text: 'Đáp án C' },
                { id: 'd', text: 'Đáp án D' }
            ],
            correctAnswer: 'a',
            isExpanded: false
        };
        setQuestions([...questions, newQuestion]);
    };

    // Xử lý mở rộng/thu gọn câu hỏi
    const toggleQuestion = (id) => {
        setQuestions(
            questions.map(q =>
                q.id === id ? { ...q, isExpanded: !q.isExpanded } : q
            )
        );
    };

    // Xử lý lọc câu hỏi
    const filteredQuestions = questions.filter(q =>
        q.question.toLowerCase().includes(searchTerm.toLowerCase())
    );

    // Xử lý phân trang
    const indexOfLastQuestion = currentPage * questionsPerPage;
    const indexOfFirstQuestion = indexOfLastQuestion - questionsPerPage;
    const currentQuestions = filteredQuestions.slice(indexOfFirstQuestion, indexOfLastQuestion);
    const totalPages = Math.ceil(filteredQuestions.length / questionsPerPage);

    const paginate = (pageNumber) => setCurrentPage(pageNumber);

    return (
        <div className={styles.questionManager}>
            <h1>Quản Lý Câu Hỏi</h1>

            {/* Thanh công cụ trên đầu */}
            <div className={styles.toolbar}>
                <button className={styles.addButton} onClick={handleAddQuestion}>
                    + Thêm Câu Hỏi
                </button>
                <div className={styles.filter}>
                <select id="id_name" className={styles.dropFilter}>
                        <option value="1">Active </option>
                        <option value="2">NotActive </option>
                    </select>

                    <input
                        type="text"
                        placeholder="Tìm kiếm câu hỏi..."
                        value={searchTerm}
                        onChange={(e) => setSearchTerm(e.target.value)}
                    />
                </div>
            </div>

            {/* Danh sách câu hỏi */}
            <div className={styles.questionList}>
                {currentQuestions.map(q => (
                    <div key={q.id} className={styles.questionItem}>
                        <div className={styles.questionHeader} onClick={() => toggleQuestion(q.id)}>
                            <span className={styles.questionText}>{q.question}</span>
                            <div className={styles.questionActions}>
                                <span className={`${styles.isActive} ${styles.active}`}>Active</span>
                                <button className={styles.editButton}>Chỉnh sửa</button>
                                <span className={`${styles.expandIcon} ${q.isExpanded ? styles.expanded : ''}`}>▼</span>
                            </div>
                        </div>

                        {q.isExpanded && (
                            <div className={styles.questionDetails}>
                                <div className={styles.optionsSection}>
                                    <p className={styles.optionsTitle}><strong>Các đáp án:</strong></p>
                                    <div className={styles.optionsList}>
                                        {q.options.map(option => (
                                            <div
                                                key={option.id}
                                                className={`${styles.optionItem} ${option.id === q.correctAnswer ? styles.correctAnswer : ''}`}
                                            >
                                                <span className={styles.optionLabel}>{option.id.toUpperCase()}.</span>
                                                <span className={styles.optionText}>{option.text}</span>
                                                {option.id === q.correctAnswer && (
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
            {totalPages > 1 && (
                <div className={styles.pagination}>
                    <button
                        onClick={() => paginate(currentPage - 1)}
                        disabled={currentPage === 1}
                    >
                        &laquo;
                    </button>

                    {Array.from({ length: totalPages }, (_, i) => (
                        <button
                            key={i + 1}
                            onClick={() => paginate(i + 1)}
                            className={currentPage === i + 1 ? styles.active : ''}
                        >
                            {i + 1}
                        </button>
                    ))}

                    <button
                        onClick={() => paginate(currentPage + 1)}
                        disabled={currentPage === totalPages}
                    >
                        &raquo;
                    </button>
                </div>
            )}
        </div>
    );
};

export default QuestionManage;