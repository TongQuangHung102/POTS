import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import styles from './QuestionManage.module.css';

const QuestionManage = () => {
    // State chứa danh sách câu hỏi từ API
    const { lessonId } = useParams();
    const [questions, setQuestions] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [levelId, setLevelId] = useState('');
    const [levels, setLevels] = useState([]);
    const [isVisible, setIsVisible] = useState('');
    const [totalQuestions, setTotalQuestions] = useState(0);
    const [isEditing, setIsEditing] = useState(false);
    const [editingQuestion, setEditingQuestion] = useState(null);
    const [errorMessage, setErrorMessage] = useState('');
    const [successMessage, setSuccessMessage] = useState('');

    // State cho phân trang
    const [currentPage, setCurrentPage] = useState(1);
    const questionsPerPage = 1;
    const [searchTerm, setSearchTerm] = useState('');
    const totalPages = Math.ceil(totalQuestions / questionsPerPage);

    const navigate = useNavigate();

    // Gọi API để lấy level
    useEffect(() => {
        const fetchLevels = async () => {
            try {
                const response = await fetch('https://localhost:7259/api/Level/get-all-level'); // Gọi API
                if (!response.ok) {
                    throw new Error(`HTTP error! Status: ${response.status}`);
                }
                const data = await response.json();
                setLevels(data);
            } catch (error) {
                setError(error.message);
            } finally {
                setLoading(false);
            }
        };

        fetchLevels();
    }, []);

    // Gọi API để lấy dữ liệu câu hỏi

    const fetchQuestions = async () => {

        setLoading(true);
        setError(null);

        try {
            let apiUrl = `https://localhost:7259/api/Question/get-all-question?lessonId=${lessonId}&page=${currentPage}&pageSize=${questionsPerPage}`;

            const queryParams = [];

            if (levelId) queryParams.push(`levelId=${levelId}`);
            if (isVisible) queryParams.push(`isVisible=${isVisible}`);
            if (searchTerm.trim() !== "") queryParams.push(`searchTerm=${encodeURIComponent(searchTerm)}`);
            if (queryParams.length > 0) apiUrl += `&${queryParams.join("&")}`;

            const response = await fetch(apiUrl);

            if (!response.ok) throw new Error('Lỗi khi lấy dữ liệu');
            const data = await response.json();
            setTotalQuestions(data.totalQuestions);
            const formattedQuestions = data.data.map(q => ({
                id: q.questionId,
                question: q.questionText,
                levelId: q.level.levelId,
                isVisible: q.isVisible,
                options: q.answerQuestions.map(a => ({
                    qId: a.answerQuestionId,
                    id: a.number,
                    text: a.answerText
                })),
                correctAnswer: q.correctAnswer,
                isExpanded: false
            }));

            setQuestions(formattedQuestions);
        } catch (error) {
            setError(error.message);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        const delayDebounce = setTimeout(() => {
            fetchQuestions();
        }, 500);

        return () => clearTimeout(delayDebounce);
    }, [levelId, isVisible, searchTerm, currentPage]);

    const toggleQuestion = (id) => {
        setQuestions(
            questions.map(q =>
                q.id === id ? { ...q, isExpanded: !q.isExpanded } : q
            )
        );
    };

    const handleEdit = (question) => {
        setEditingQuestion(question);
        setIsEditing(true);
    };

    const handleSave = async () => {
        try {
            const requestBody = {
                questionText: editingQuestion.question,
                createAt: editingQuestion.createAt,
                levelId: editingQuestion.levelId,
                correctAnswer: editingQuestion.correctAnswer,
                isVisible: editingQuestion.isVisible,
                createByAI: editingQuestion.createByAI,
                lessonId: editingQuestion.lessonId,
                answerQuestions: editingQuestion.options.map(a => ({
                    answerQuestionId: a.qId,
                    answerText: a.text,
                    number: a.id
                }))
            };
            const response = await fetch(`https://localhost:7259/api/Question/edit-question/${editingQuestion.id}`, {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(requestBody)
            });

            const result = await response.json();
            console.log(result);
            if (!response.ok) {
                throw new Error(result.message);
            }

            setSuccessMessage(result.message);
            alert("Cập nhật thành công!");
            fetchQuestions();

            setIsEditing(false);
        } catch (error) {
            setErrorMessage(error.message)
        }
    };

    const handleClose = () => {
        setIsEditing(false);
        setEditingQuestion(null);
    };
    const handleAddQuestion = () => {
        navigate('/content_manage/add-question');
    };
    return (
        <div className={styles.questionManager}>
            <h1>Quản Lý Câu Hỏi</h1>
            <div className={styles.groupbtn}>
                <button onClick={handleAddQuestion}>Thêm câu hỏi</button>
                <button>Tạo câu hỏi bằng AI</button>
            </div>
            <div className={styles.toolbar}>
                <select className={styles.commonInput} value={levelId} onChange={(e) => setLevelId(e.target.value)}>
                    <option value="">Chọn mức độ</option>
                    {levels?.map((l) => (
                        <option key={l.levelId} value={l.levelId}>
                            {l.levelName}
                        </option>
                    ))}
                </select>

                <select className={styles.commonInput} value={isVisible} onChange={(e) => setIsVisible(e.target.value)}>
                    <option value="">Chọn Trạng Thái</option>
                    <option value="true">Hiển Thị</option>
                    <option value="false">Ẩn</option>
                </select>

                <input className={styles.commonInput}
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
                {questions.map(q => (
                    <div key={q.id} className={styles.questionItem}>
                        <div className={styles.questionHeader}>
                            <span className={styles.questionText}>{q.question}</span>
                            <div className={styles.questionActions}>
                                <span className={`${q.isVisible ? styles.isVisible : styles.inactive}`}> {q.isVisible ? "Hiển Thị" : "Ẩn"}</span>
                                <button className={styles.editButton} onClick={() => handleEdit(q)}>Chỉnh sửa</button>
                                <span className={`${styles.expandIcon} ${q.isExpanded ? styles.expanded : ''}`} onClick={() => toggleQuestion(q.id)}>▼</span>
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

            {/* Form chỉnh sửa */}
            {isEditing && (
                <div className={styles.modal}>
                    <div className={styles.modalContent}>
                        <h3>Chỉnh sửa câu hỏi</h3>
                        <label>
                            Câu hỏi:
                            <input
                                type="text"
                                value={editingQuestion.question}
                                required
                                onChange={(e) =>
                                    setEditingQuestion({ ...editingQuestion, question: e.target.value })
                                }

                            />
                        </label>
                        <label>Trạng thái hiển thị:
                            <select
                                value={editingQuestion.isVisible}
                                onChange={(e) =>
                                    setEditingQuestion({ ...editingQuestion, isVisible: e.target.value })
                                }
                            >
                                <option value="true">Hiển thị</option>
                                <option value="false">Ẩn</option>
                            </select>
                        </label>
                        <label>Mức độ:
                            <select
                                value={editingQuestion.levelId}
                                onChange={(e) =>
                                    setEditingQuestion({ ...editingQuestion, levelId: e.target.value })
                                }
                            >
                                {levels?.map((l) => (
                                    <option key={l.levelId} value={l.levelId}>
                                        {l.levelName}
                                    </option>
                                ))}
                            </select>
                        </label>


                        <label>Các câu trả lời:
                            {editingQuestion.options.map((option, index) => (
                                <div key={option.id} className={styles.answer} >
                                    <input
                                        className={`${styles.answerItem} ${option.id === editingQuestion.correctAnswer ? styles.correctAnswer : ""
                                            }`}
                                        type="text"
                                        value={option.text}
                                        required
                                        onChange={(e) => {
                                            const updatedOptions = [...editingQuestion.options];
                                            updatedOptions[index].text = e.target.value;
                                            setEditingQuestion({ ...editingQuestion, options: updatedOptions });
                                        }}

                                    />
                                    <input className={styles.radioInput}
                                        type="radio"
                                        name="correctAnswer"
                                        value={option.id}
                                        checked={editingQuestion.correctAnswer === option.id}
                                        onChange={(e) =>
                                            setEditingQuestion({ ...editingQuestion, correctAnswer: Number(e.target.value) })
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

export default QuestionManage;
