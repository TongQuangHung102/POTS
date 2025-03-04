import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import styles from './ManageQuestionTest.module.css';
import { fetchChapters } from '../../services/ChapterService';
import { fetchLessons } from '../../services/LessonService';
import { fetchLevels } from '../../services/LevelService';
import BackLink from '../../components/BackLink';

const ManageQuestionTest = () => {

    const { testId, gradeId } = useParams();

    const [testQuestions, setTestQuestions] = useState([]);
    const [questionBank, setQuestionBank] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [levelId, setLevelId] = useState('');
    const [lessonId, setLessonId] = useState('');
    const [levels, setLevels] = useState([]);
    const [chapterId, setChapterId] = useState('');
    const [totalQuestions, setTotalQuestions] = useState(0);
    const [isEditing, setIsEditing] = useState(false);
    const [editingQuestion, setEditingQuestion] = useState(null);
    const [errorMessage, setErrorMessage] = useState('');
    const [successMessage, setSuccessMessage] = useState('');
    const [chapters, setChapters] = useState([]);
    const [lessons, setLessons] = useState([]);


    // State cho phân trang
    const [currentPage, setCurrentPage] = useState(1);
    const questionsPerPage = 5;
    const [searchTerm, setSearchTerm] = useState('');
    const totalPages = Math.ceil(totalQuestions / questionsPerPage);

    useEffect(() => {
        const fetchData = async () => {
            try {
                let apiUrl = `https://localhost:7259/api/Question/get-all-question?&page=${currentPage}&pageSize=${questionsPerPage}`;

                const queryParams = [];
                if (lessonId) queryParams.push(`lessonId=${lessonId}`);
                if (levelId) queryParams.push(`levelId=${levelId}`);
                if (chapterId) queryParams.push(`chapterId=${chapterId}`);
                if (searchTerm.trim() !== "") queryParams.push(`searchTerm=${encodeURIComponent(searchTerm)}`);
                if (queryParams.length > 0) apiUrl += `&${queryParams.join("&")}`;

                const response = await fetch(apiUrl);

                if (!response.ok) throw new Error('Lỗi khi lấy dữ liệu');
                const data = await response.json();
                setTotalQuestions(data.totalQuestions);
                console.log(data)
                setQuestionBank(data.data);
            } catch (error) {
                setError(error.message);
            } finally {
                setLoading(false);
            }
        };
        const delayDebounce = setTimeout(() => {
            fetchData();
        }, 300);
        return () => clearTimeout(delayDebounce);
    }, [chapterId, lessonId, levelId, searchTerm,currentPage]);

    useEffect(() => {
        const fetchQuestions = async () => {
            try {
                const response = await fetch(`https://localhost:7259/api/TestQuestion/get-test-questions?testId=${testId}`);
                if (!response.ok) throw new Error("Lỗi khi lấy dữ liệu");

                const data = await response.json();
                if (data.length === 0) return;
                setTestQuestions(data);
                setIsEditing('true');
            } catch (err) {
                setError(err.message);
            } finally {
                setLoading(false);
            }
        };

        fetchQuestions();
    }, []);

    useEffect(() => {
        const loadChapters = async () => {
            const data = await fetchChapters(gradeId);
            setChapters(data);
        };
        const loadLevels = async () => {
            const data = await fetchLevels();
            setLevels(data);
        }
        loadLevels();
        loadChapters();
    }, []);

    useEffect(() => {
        if (!chapterId) return;
        const loadLessons = async () => {
            const data = await fetchLessons(chapterId);
            setLessons(data);
        };
        loadLessons();
    }, [chapterId]);

    const addQuestionToTest = (question) => {
        if (!testQuestions.some(q => q.questionId === question.questionId)) {
            setTestQuestions([...testQuestions, question]);
        }
    };

    const removeQuestionFromTest = (questionId) => {
        setTestQuestions(testQuestions.filter(question => question.questionId !== questionId));
    };

    const addQuestionsToTest = async () => {
        if (testQuestions.length === 0) {
            alert("Chưa có câu hỏi nào được chọn!");
            return;
        }

        const payload = {
            testId: 1, 
            questionIds: testQuestions.map(q => q.questionId)
        };

        try {
            const response = await fetch("https://localhost:7259/api/TestQuestion/add-questions", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(payload)
            });

            if (!response.ok) {
                throw new Error("Có lỗi xảy ra khi thêm câu hỏi vào bài kiểm tra!");
            }

            const result = await response.json();
            alert("Thêm câu hỏi thành công!");
            console.log("Server Response:", result);

        } catch (error) {
            console.error("Error:", error);
            alert("Lỗi: " + error.message);
        }
    };

    const updateTestQuestions = async () => {
        const payload = {
            testId: testId,
            questionIds: testQuestions.map(q => q.questionId)
        };
        try {
            const response = await fetch("https://localhost:7259/api/TestQuestion/update-questions", {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(payload),
            });
    
            const data = await response.json();
    
            if (!response.ok) {
                throw new Error(data.message || "Cập nhật thất bại");
            }
    
            alert("Cập nhật câu hỏi thành công!");
        } catch (error) {
            console.error("Lỗi:", error.message);
            alert(error.message);
        }
    };


    return (
        <div className={styles.container}>
            <h2 className={styles.title}>Quản Lý Câu Hỏi Bài Kiểm Tra</h2>
            <BackLink></BackLink>
            <div className={styles.content}>
                <div className={styles.testQuestions}>
                    <h3>Câu hỏi bài kiểm tra</h3>
                    {testQuestions.length === 0 ? (
                        <p className={styles.emptyMessage}>Chưa có câu hỏi nào được thêm vào bài kiểm tra</p>
                    ) : (
                        <ul className={styles.questionList}>
                            {testQuestions.map(question => (
                                <li key={question.questionId} className={styles.questionItem}>
                                    <div className={styles.questionContent}>
                                        <div className={styles.questionHeader}>
                                                <span className={styles.questionLevel}>
                                                    {question.level.levelName}
                                                </span>
                                            <span className={styles.questionLesson}>
                                                {question.lesson.lessonName}
                                            </span>
                                        </div>
                                        <p className={styles.questionText}>{question.questionText}</p>
                                        <ul className={styles.options}>
                                            {question.answerQuestions.map((answer) => (
                                                <li
                                                    key={answer.answerQuestionId}
                                                    className={question.correctAnswer === answer.number ? styles.correctAnswer : ''}
                                                >
                                                    {answer.number}. {answer.answerText}
                                                    {question.correctAnswer === answer.number &&
                                                        <span className={styles.correctBadge}>✓</span>
                                                    }
                                                </li>
                                            ))}
                                        </ul>
                                        <div className={styles.questionFooter}>
                                            {question.createByAI &&
                                                <span className={styles.aiGenerated}>Câu hỏi tạo bằng AI</span>
                                            }
                                        </div>
                                    </div>
                                    <button
                                        className={styles.removeButton}
                                        onClick={() => removeQuestionFromTest(question.questionId)}
                                    >
                                        Xóa
                                    </button>
                                </li>
                            ))}
                            <div>
                                Tổng số câu hỏi : {testQuestions.length}
                            </div>
                            {isEditing ? (
                                <button className={styles.addButton} onClick={updateTestQuestions}>
                                Lưu
                            </button>) : (
                                <button className={styles.addButton} onClick={addQuestionsToTest}>
                                Thêm vào bài kiểm tra
                            </button>)}

                        </ul>
                    )}
                </div>

                {/* Phần bên phải - Ngân hàng câu hỏi */}
                <div className={styles.questionBank}>
                    <h3>Ngân hàng câu hỏi</h3>
                    <div className={styles.toolbar}>
                        <select className={styles.commonInput} value={chapterId} onChange={(e) => setChapterId(e.target.value)}>
                            <option value="">Chọn Chương</option>
                            {chapters?.filter((c) => c.isVisible).map((c) => (
                                <option key={c.chapterId} value={c.chapterId}>
                                    Chương {c.order}: {c.chapterName}
                                </option>
                            ))}
                        </select>

                        <select className={styles.commonInput} value={lessonId} onChange={(e) => setLessonId(e.target.value)}>
                            <option value="">Chọn Bài</option>
                            {Array.isArray(lessons) ?
                                lessons.filter((l) => l.isVisible).map((l) => (
                                    <option key={l.lessonId} value={l.lessonId}>
                                        Bài {l.order}: {l.lessonName}
                                    </option>
                                ))
                                : null}

                        </select>

                        <select className={styles.commonInput} value={levelId} onChange={(e) => setLevelId(e.target.value)}>
                            <option value="">Chọn mức độ</option>
                            {levels?.map((l) => (
                                <option key={l.levelId} value={l.levelId}>
                                    {l.levelName}
                                </option>
                            ))}
                        </select>

                        <input className={styles.commonInput}
                            type="text"
                            placeholder="Tìm kiếm câu hỏi..."
                            value={searchTerm}
                            onChange={(e) => setSearchTerm(e.target.value)}
                        />
                    </div>
                    <ul className={styles.questionList}>
                        {questionBank.map(question => (
                            <li
                                key={question.questionId}
                                className={styles.questionItem}
                                onClick={() => addQuestionToTest(question)}
                            >
                                <div className={styles.questionContent}>
                                    <div className={styles.questionHeader}>
                                        <span className={styles.questionLevel}>
                                            {question.level.levelName}
                                        </span>
                                        <span className={styles.questionLesson}>
                                            {question.lesson.lessonName}
                                        </span>
                                    </div>
                                    <p className={styles.questionText}>{question.questionText}</p>
                                    <ul className={styles.options}>
                                        {question.answerQuestions.map((answer) => (
                                            <li
                                                key={answer.answerQuestionId}
                                                className={question.correctAnswer === answer.number ? styles.correctAnswer : ''}
                                            >
                                                {answer.number}. {answer.answerText}
                                                {question.correctAnswer === answer.number &&
                                                    <span className={styles.correctBadge}>✓</span>
                                                }
                                            </li>
                                        ))}
                                    </ul>
                                    <div className={styles.questionFooter}>
                                        {question.createByAI &&
                                            <span className={styles.aiGenerated}>Câu hỏi tạo bằng AI</span>
                                        }
                                    </div>
                                </div>
                                <button className={styles.addButton}>Thêm</button>
                            </li>
                        ))}
                    </ul>
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
            </div>
        </div>
    );
};

export default ManageQuestionTest;