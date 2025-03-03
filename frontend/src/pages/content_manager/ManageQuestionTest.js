import React, { useState, useEffect } from 'react';
import styles from './ManageQuestionTest.module.css';
import { fetchChapters } from '../../services/ChapterService';
import { fetchLessons } from '../../services/LessonService';
import { fetchLevels } from '../../services/LevelService';

const ManageQuestionTest = () => {
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
    const questionsPerPage = 10;
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
    }, [chapterId, lessonId, levelId, searchTerm]);

    useEffect(() => {
        const loadChapters = async () => {
            const data = await fetchChapters("1");
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


    return (
        <div className={styles.container}>
            <h2 className={styles.title}>Quản Lý Câu Hỏi Bài Kiểm Tra</h2>
            <div className={styles.content}>
                {/* Phần bên trái - Câu hỏi bài kiểm tra */}
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
                                            <div className={styles.questionLevel}>
                                                <span>
                                                    {question.level.levelName}
                                                </span>
                                            </div>

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
                                                <span className={styles.aiGenerated}>AI</span>
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
                </div>
            </div>
        </div>
    );
};

export default ManageQuestionTest;