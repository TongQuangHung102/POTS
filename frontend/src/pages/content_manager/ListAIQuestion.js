import React, { useState, useEffect } from "react";
import { useParams, useLocation } from "react-router-dom";
import styles from './QuestionManage.module.css';
import BackLink from "../../components/BackLink";
import { authFetch } from "../../utils/authFetch";
import { cleanAndValidateString } from "../../utils/validator";
import Loading from "../../components/Loading";

const ListAIQuestion = () => {
    const { lessonId } = useParams();
    const location = useLocation();
    const lessonName = location.state?.lessonName || "Không có tên bài học";
    const [aiQuestions, setAiQuestions] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    const apiUrl = process.env.REACT_APP_API_URL;

    const [currentPage, setCurrentPage] = useState(1);
    const questionsPerPage = 10;
    const [totalPages, setTotalPages] = useState(1);


    const [status, setStatus] = useState("");
    const [levelId, setLevelId] = useState("");
    const [createdAt, setCreatedAt] = useState("");
    const [levels, setLevels] = useState([]);

    const [showModal, setShowModal] = useState(false);
    const [numQuestions, setNumQuestions] = useState(1);

    const [editModalVisible, setEditModalVisible] = useState(false);
    const [editQuestionData, setEditQuestionData] = useState(null);

    const [errorMessage, setErrorMessage] = useState('');
    const [isNumValid, setIsNumValid] = useState(true);

    useEffect(() => {
        const fetchLevels = async () => {
            try {
                const response = await authFetch(`${apiUrl}/api/Level/get-all-level`); // Gọi API
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

    useEffect(() => {
        fetchAIQuestions();
    }, [lessonId, currentPage, status, levelId, createdAt]);



    const fetchAIQuestions = async () => {
        setLoading(true);
        setError(null);

        try {
            let url = `${apiUrl}/api/AIQuestion/get-all-aiquestion?lessonId=${lessonId}&pageNumber=${currentPage}&pageSize=${questionsPerPage}`;
            if (status) url += `&status=${status}`;
            if (levelId) url += `&levelId=${levelId}`;
            if (createdAt) url += `&createdAt=${createdAt}`;



            const response = await authFetch(url);


            if (response.status === 404) {
                console.warn("Không có dữ liệu!");
                setAiQuestions([]);
                setTotalPages(1);
                return;
            }
            if (!response.ok) throw new Error("Lỗi khi lấy danh sách câu hỏi AI");

            const data = await response.json();
            const formattedQuestions = data.data.map(q => ({
                id: q.questionId,
                question: q.questionText,
                levelId: q.level.levelId,
                levelName: q.level.levelName,
                status: q.status,
                options: q.answerQuestions.map(a => ({
                    qId: a.answerQuestionId,
                    id: a.number,
                    text: a.answerText
                })),
                correctAnswer: q.correctAnswer,
                isExpanded: false
            }));


            setAiQuestions(formattedQuestions || []);
            setTotalPages(data.totalPage || 1);
        } catch (error) {
            setError(error.message);
        } finally {
            setLoading(false);
        }
    };

    const toggleQuestion = (id) => {
        setAiQuestions(
            aiQuestions.map(q =>
                q.id === id ? { ...q, isExpanded: !q.isExpanded } : q
            )
        );
    };

    const handleClose = () => {
        setEditModalVisible(false);
        setEditQuestionData(null);
    };

    const approveQuestion = async (questionId, currentStatus) => {
        if (currentStatus === "Approved") {
            alert("Câu hỏi này đã được phê duyệt");
            return;
        }

        try {
            const response = await authFetch(`${apiUrl}/api/AIQuestion/approve-aiquestion/${questionId}`, {
                method: "PUT",
                headers: { "Content-Type": "application/json" },
            });

            if (!response.ok) {
                throw new Error("Lỗi khi phê duyệt câu hỏi.");
            }

            alert("Phê duyệt câu hỏi thành công!");
            await fetchAIQuestions();
        } catch (error) {
            console.error("Lỗi:", error);
            alert("Có lỗi xảy ra khi phê duyệt câu hỏi.");
        }
    };


    const handleEdit = (question) => {
        setEditQuestionData(question);
        setEditModalVisible(true);
    };
    const handleNumQuestionsChange = (e) => {
        const value = Number(e.target.value);
        setNumQuestions(value);
        if (value < 1 || value > 10) {
            setErrorMessage('Vui lòng nhập số lượng câu hỏi từ 1 đến 10');
            setIsNumValid(false);
        } else {
            setErrorMessage('');
            setIsNumValid(true);
        }
    };
    const handleAddAIQuestions = async () => {
        setLoading(true);
        setShowModal(false);
        if (numQuestions >= 1 && numQuestions <= 10) {
            try {

                const response = await authFetch(`${apiUrl}/api/AIQuestion/generate-ai-questions`, {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify({ lessonId: lessonId, num: numQuestions }),
                });

                const data = await response.json();

                if (!response.ok) {
                    throw new Error(data.error || "Lỗi khi tạo câu hỏi bằng AI");
                }
                console.log("Phản hồi từ API generate-ai-questions:", data);


                if (!data || !data.generatedQuestionIds || data.generatedQuestionIds.length === 0) {
                    console.error("Lỗi: API không trả về danh sách câu hỏi hợp lệ", data);
                    return;
                }

                const questionIds = data.generatedQuestionIds;


                console.log("Dữ liệu gửi đi:", JSON.stringify({
                    lessonId,
                    aiQuestionIds: questionIds
                }));
                if (!lessonId || lessonId <= 0) {
                    console.error("Lỗi: lessonId không hợp lệ:", lessonId);
                    return;
                }
                if (!questionIds || !Array.isArray(questionIds) || questionIds.length === 0) {
                    console.error("Lỗi: aiQuestionIds không hợp lệ:", questionIds);
                    return;
                }

                await authFetch(`${apiUrl}/api/AIQuestion/update-lesson-id`, {
                    method: "PUT",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify({
                        lessonId,
                        aiQuestionIds: questionIds,
                    }),
                })
                    .then(response => response.json())
                    .then(data => console.log("Phản hồi từ server:", data))
                    .catch(error => console.error("Lỗi:", error));

                alert("Thêm câu hỏi thành công");

                await fetchAIQuestions();
            } catch (error) {
                setError(error.message);
            } finally {
                setLoading(false);
            }
        }
    };

    const handleSaveEditQuestion = async () => {
        if (!editQuestionData) return;
        const { valid, cleaned } = cleanAndValidateString(editQuestionData.question);

        if (!cleaned.trim()) {
            setErrorMessage("Câu hỏi không được để trống.");
            return;
        }

        const emptyAnswer = editQuestionData.options.find(
            (answer) => !answer.text || !answer.text.trim()
        );

        if (emptyAnswer) {
            setErrorMessage("Tất cả các đáp án đều phải được điền đầy đủ.");
            await fetchAIQuestions();
            return;
        }

        try {
            const response = await authFetch(`${apiUrl}/api/AIQuestion/update-aiquestion/${editQuestionData.id}`, {
                method: "PUT",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({
                    questionId: editQuestionData.id,
                    questionText: editQuestionData.question,
                    levelId: editQuestionData.levelId,
                    correctAnswer: editQuestionData.correctAnswer,
                    status: editQuestionData.status,
                    answerQuestions: editQuestionData.options.map(answer => ({
                        number: answer.id,
                        answerText: answer.text
                    }))
                })
            });

            if (!response.ok) throw new Error("Lỗi khi cập nhật câu hỏi");

            alert("Cập nhật câu hỏi thành công!");
            setEditModalVisible(false);
            await fetchAIQuestions();
        } catch (error) {
            console.error("Lỗi:", error);
            alert("Có lỗi xảy ra khi cập nhật câu hỏi.");
        }
    };



    return (
        <div className={styles.questionManager}>
            <h2>Danh sách câu hỏi AI {lessonName}</h2>
            <BackLink />
            <div className={styles.groupbtn}>
                <button onClick={() => setShowModal(true)}>
                    Thêm câu hỏi bằng AI
                </button>
            </div>
            <div className={styles.toolbar}>
                <select className={styles.commonInput} value={status} onChange={(e) => setStatus(e.target.value)}>
                    <option value="">Chọn trạng thái</option>
                    <option value="Pending">Chờ kiểm tra</option>
                    <option value="Approved">Đã duyệt</option>
                </select>

                <select className={styles.commonInput} value={levelId} onChange={(e) => setLevelId(e.target.value)}>
                    <option value="">Chọn mức độ</option>
                    {levels?.map((l) => (
                        <option key={l.levelId} value={l.levelId}>
                            {l.levelName}
                        </option>
                    ))}
                </select>

                <input className={styles.commonInput} type="date" value={createdAt} onChange={(e) => setCreatedAt(e.target.value)} />


            </div>

            {error && <p style={{ color: "red" }}>{error}</p>}

            <div className={styles.questionList}>
                {aiQuestions.length > 0 ? (aiQuestions.map(q => (
                    <div key={q.id} className={styles.questionItem}>
                        <div className={styles.questionHeader} onClick={() => toggleQuestion(q.id)}>
                            <span className={styles.questionText}>{q.id}. {q.question}</span>

                            <div className={styles.questionActions}>
                                <span className={styles.levelName}>{q.levelName}</span>
                                <span className={`${q.status === "Approved" ? styles.isVisible : styles.inactive}`}> {q.status === "Approved" ? "Đã duyệt" : q.status === "Pending" ? "Chờ kiểm tra" : q.status}</span>
                                <span className={`${styles.expandIcon} ${q.isExpanded ? styles.expanded : ''}`} >▼</span>
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
                                                <span className={styles.optionLabel}>{option.id}.</span>
                                                <span className={styles.optionText}>{option.text}</span>
                                                {option.id === q.correctAnswer && (
                                                    <span className={styles.correctIndicator}>✓</span>
                                                )}
                                            </div>
                                        ))}
                                    </div>
                                    {q.status !== "Approved" && (<div>
                                        <button className='btn btn-success m-1' onClick={() => approveQuestion(q.id, q.status)} disabled={q.status === "Approved"}>Duyệt</button>
                                        <button className='btn btn-primary m-1' onClick={() => handleEdit(q)}>Chỉnh sửa</button>
                                    </div>)}

                                </div>
                            </div>
                        )}
                    </div>
                ))) : (<p>Chưa có câu hỏi</p>)}
            </div>

            {totalPages > 1 && (
                <div className={styles.pagination}>
                    <button onClick={() => setCurrentPage((prev) => Math.max(prev - 1, 1))} disabled={currentPage === 1}>
                        &laquo;
                    </button>

                    {Array.from({ length: totalPages }, (_, index) => (
                        <button key={index + 1} onClick={() => setCurrentPage(index + 1)} className={currentPage === index + 1 ? styles.active : ""}>
                            {index + 1}
                        </button>
                    ))}

                    <button onClick={() => setCurrentPage((prev) => Math.min(prev + 1, totalPages))} disabled={currentPage === totalPages}>
                        &raquo;
                    </button>
                </div>
            )}
            {showModal && (
                <div className={styles.modal}>
                    <div className={styles.modalContent}>
                        <h2>Thêm câu hỏi bằng AI</h2>
                        <label>Số câu hỏi:</label>
                        <input
                            type="number"
                            value={numQuestions}
                            onChange={handleNumQuestionsChange}
                            min="1"
                            max="10"
                        />
                        {errorMessage && <p className="error-message">{errorMessage}</p>}
                        <div className="button-group">
                            <button onClick={handleAddAIQuestions} disabled={!isNumValid}>Xác nhận</button>
                            <button onClick={() => setShowModal(false)}>Hủy</button>
                        </div>
                    </div>
                </div>
            )}

            {editModalVisible && (
                <div className={styles.modal}>
                    <div className={styles.modalContent}>
                        <h3>Chỉnh sửa câu hỏi</h3>
                        <label>
                            Câu hỏi:
                            <input
                                type="text"
                                value={editQuestionData.question}
                                required
                                onChange={(e) =>
                                    setEditQuestionData({ ...editQuestionData, question: e.target.value })
                                }

                            />
                        </label>

                        <label>Mức độ:
                            <select
                                value={editQuestionData.levelId}
                                onChange={(e) =>
                                    setEditQuestionData({ ...editQuestionData, levelId: e.target.value })
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
                            {editQuestionData.options.map((option, index) => (
                                <div key={option.id} className={styles.answer} >
                                    <input
                                        className={`${styles.answerItem} ${option.id === editQuestionData.correctAnswer ? styles.correctAnswer : ""
                                            }`}
                                        type="text"
                                        value={option.text}
                                        required
                                        onChange={(e) => {
                                            const updatedOptions = [...editQuestionData.options];
                                            updatedOptions[index].text = e.target.value;
                                            setEditQuestionData({ ...editQuestionData, options: updatedOptions });
                                        }}

                                    />
                                    <input className={styles.radioInput}
                                        type="radio"
                                        name="correctAnswer"
                                        value={option.id}
                                        checked={editQuestionData.correctAnswer === option.id}
                                        onChange={(e) =>
                                            setEditQuestionData({ ...editQuestionData, correctAnswer: Number(e.target.value) })
                                        }
                                    /> Đáp án đúng
                                </div>
                            ))}
                        </label>

                        <div className="button-group">
                            <button onClick={handleSaveEditQuestion}>Lưu</button>
                            <button onClick={handleClose}>Đóng</button>
                        </div>
                        {errorMessage && <p className="error-message">{errorMessage}</p>}
                    </div>
                </div>
            )}

            {loading && (<div className="overlay-auth">
                <Loading message="Đang tạo câu hỏi..." />
            </div>)}



        </div>
    );
};

export default ListAIQuestion;
