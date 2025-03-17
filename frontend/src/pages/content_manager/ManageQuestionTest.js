import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import styles from './ManageQuestionTest.module.css';
import { fetchChapters } from '../../services/ChapterService';
import { fetchLessons } from '../../services/LessonService';
import { fetchLevels } from '../../services/LevelService';
import { fetchTestQuestions, GenerateTest, UpdateTestQuestions, AddQuestionsToTest } from '../../services/TestQuestion'
import BackLink from '../../components/BackLink';

const ManageQuestionTest = () => {

    const { testId, gradeId } = useParams();

    const [testQuestions, setTestQuestions] = useState([]);
    const [initialQuestions, setInitialQuestions] = useState([]);
    const [questionBank, setQuestionBank] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [levelId, setLevelId] = useState('');
    const [lessonId, setLessonId] = useState('');
    const [levels, setLevels] = useState([]);
    const [chapterId, setChapterId] = useState('');
    const [totalQuestions, setTotalQuestions] = useState(0);
    const [isEditing, setIsEditing] = useState(false);
    const [isShowModal, setIsShowModal] = useState(false);
    const [errorMessage, setErrorMessage] = useState('');
    const [successMessage, setSuccessMessage] = useState('');
    const [chapters, setChapters] = useState([]);
    const [lessons, setLessons] = useState([]);
    const [isModified, setIsModified] = useState(false);


    const [selectedChapters, setSelectedChapters] = useState([]);
    const [questionsConfig, setQuestionsConfig] = useState({});


    // State cho phân trang
    const [currentPage, setCurrentPage] = useState(1);
    const questionsPerPage = 5;
    const [searchTerm, setSearchTerm] = useState('');
    const totalPages = Math.ceil(totalQuestions / questionsPerPage);

    //lay ngan hang cau hoi
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
    }, [chapterId, lessonId, levelId, searchTerm, currentPage]);

    //lay cau hoi trong test
    useEffect(() => {
        const fetchQuestions = async () => {
            const data = await fetchTestQuestions(testId);

            if (data.error) {
                setTestQuestions([]);
            }
            else {
                setTestQuestions(data)
                setInitialQuestions(data);
                setIsEditing(true);
            }

        };
        fetchQuestions();
    }, []);

    //lay danh sach chapter, lesson, level
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

    useEffect(() => {
        setIsModified(JSON.stringify(testQuestions) !== JSON.stringify(initialQuestions));
    }, [testQuestions, initialQuestions]);

    const addQuestionToTest = (question) => {
        if (!testQuestions.some(q => q.questionId === question.questionId)) {
            setTestQuestions([...testQuestions, question]);
        }
        setIsModified(true);
    };

    const removeQuestionFromTest = (questionId) => {
        setTestQuestions(testQuestions.filter(question => question.questionId !== questionId));
        setIsModified(true);
    };

    const addQuestionsToTest = async () => {
        await AddQuestionsToTest(testId, testQuestions)
    };

    const updateTestQuestions = async () => {
        await UpdateTestQuestions(testId, testQuestions)
    };

    //modal add question auto

    const handleAuto = () => {
        setIsShowModal(true);
    };

    const handleClose = () => {
        setIsShowModal(false);
    };


    const handleAddChapter = (chapterId) => {
        if (!selectedChapters.includes(chapterId)) {
            setSelectedChapters([...selectedChapters, chapterId]);
            setQuestionsConfig({ ...questionsConfig, [chapterId]: {} });
        }
    };

    const handleInputChange = (chapterId, levelId, value) => {
        setQuestionsConfig({
            ...questionsConfig,
            [chapterId]: {
                ...questionsConfig[chapterId],
                [levelId]: parseInt(value) || 0,
            },
        });
    };

    const handleRemoveChapter = (chapterId) => {
        setSelectedChapters(selectedChapters.filter((id) => id !== chapterId));
        const newConfig = { ...questionsConfig };
        delete newConfig[chapterId];
        setQuestionsConfig(newConfig);
    };

    const handleConfirm = async () => {
        if (selectedChapters.length === 0) {
            alert("Vui lòng chọn ít nhất một chương!");
            return;
        }

        const formattedData = selectedChapters.map((chapterId) => ({
            chapterId,
            levelRequests: Object.entries(questionsConfig[chapterId] || {}).map(
                ([levelId, questionCount]) => ({
                    levelId: parseInt(levelId),
                    questionCount,
                })
            ),
        })).filter(chapter => chapter.levelRequests.length > 0);

        if (formattedData.length === 0) {
            alert("Mỗi chương phải có ít nhất một câu hỏi!");
            return;
        }
        try {
            const result = await GenerateTest(formattedData);
            console.log(result);
            alert("Thêm tự động thành công!");
            setTestQuestions(result);
            setIsShowModal(false);
        }
        catch (error) {
            alert(error.message);
        }
    };

    const getChapterInfo = (chapterId) => {
        const chapter = chapters.find((c) => c.chapterId === chapterId);
        return chapter ? `Chương ${chapter.order}: ${chapter.chapterName}` : "Không tìm thấy chương";
    };

    return (
        <div className={styles.container}>
            <h2 className={styles.title}>Quản Lý Câu Hỏi Bài Kiểm Tra</h2>
            <BackLink></BackLink>
            <div className={styles.content}>
                <div className={styles.testQuestions}>
                    <h3>Câu hỏi bài kiểm tra</h3>
                    {testQuestions.length === 0 ? (
                        <div>
                            <p className={styles.emptyMessage}>Chưa có câu hỏi nào được thêm vào bài kiểm tra</p>
                            <button className='btn btn-success' onClick={handleAuto}>Thêm câu hỏi tự động</button>
                        </div>
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
                                        ❌
                                    </button>
                                </li>
                            ))}
                            <div>
                                Tổng số câu hỏi : {testQuestions.length}
                            </div>
                            {isEditing ? (
                                <button className='btn btn-primary' onClick={updateTestQuestions} disabled={!isModified}>
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
                                <button className={styles.addButton}>➕</button>
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

            {isShowModal && (
                <div className={styles.modal}>
                    <div className={styles.modalContent}>

                        <h3>Thêm câu hỏi tự động</h3>

                        <div className={styles.selectChapter}>
                            <select onChange={(e) => handleAddChapter(parseInt(e.target.value))}>
                                <option value="">-- Chọn chương --</option>
                                {chapters?.filter((c) => c.isVisible).map((chapter) => (
                                    <option key={chapter.chapterId} value={chapter.chapterId}>
                                        Chương {chapter.order}: {chapter.chapterName}
                                    </option>
                                ))}
                            </select>
                        </div>

                        <div className={styles.selectedChapters}>
                            {selectedChapters.map((chapterId) => (
                                <div key={chapterId} className={styles.chapterRow}>
                                    <span className={styles.chapterName}>
                                        {getChapterInfo(chapterId)}
                                    </span>
                                    <div className={styles.levelInputs}>
                                        {levels.map((level) => (
                                            <div key={level.levelId} className={styles.levelGroup}>
                                                <label>{level.levelName}</label>
                                                <input
                                                    type="number"
                                                    min="0"
                                                    onChange={(e) =>
                                                        handleInputChange(chapterId, level.levelId, e.target.value)
                                                    }
                                                />
                                            </div>
                                        ))}
                                    </div>
                                    <button className={styles.removeButton} onClick={() => handleRemoveChapter(chapterId)}>
                                        ❌
                                    </button>
                                </div>
                            ))}
                        </div>

                        {/* Nút xác nhận & đóng */}
                        <div className={styles.buttonGroup}>
                            <button onClick={handleConfirm}>Xác nhận</button>
                            <button onClick={handleClose}>Đóng</button>
                        </div>
                        {errorMessage && <p className="error-message">{errorMessage}</p>}
                    </div>
                </div>

            )}
        </div>
    );
};

export default ManageQuestionTest;