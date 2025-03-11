import React, { useState, useEffect } from "react";
import { useParams, useLocation} from "react-router-dom";
import styles from "./ListAIQuestion.module.css";

const ListAIQuestion = () => {
    const { lessonId } = useParams();
    const location = useLocation();
    const lessonName = location.state?.lessonName || "Không có tên bài học";
    const [aiQuestions, setAiQuestions] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    // Phân trang
    const [currentPage, setCurrentPage] = useState(1);
    const questionsPerPage = 10;
    const [totalPages, setTotalPages] = useState(1);

    // Bộ lọc
    const [status, setStatus] = useState("");
    const [levelId, setLevelId] = useState("");
    const [createdAt, setCreatedAt] = useState("");

    const [showModal, setShowModal] = useState(false);
    const [numQuestions, setNumQuestions] = useState(1);
    useEffect(() => {
        fetchAIQuestions();
    }, [lessonId, currentPage, status, levelId, createdAt]);
    const fetchAIQuestions = async () => {
        setLoading(true);
        setError(null);

        try {
            let apiUrl = `https://localhost:7259/api/AIQuestion/get-all-aiquestion?lessonId=${lessonId}&pageNumber=${currentPage}&pageSize=${questionsPerPage}`;
            if (status) apiUrl += `&status=${status}`;
            if (levelId) apiUrl += `&levelId=${levelId}`;
            if (createdAt) apiUrl += `&createdAt=${createdAt}`;

            console.log("Fetching API:", apiUrl);

            const response = await fetch(apiUrl);

            console.log("Response status:", response.status);

            if (!response.ok) throw new Error("Lỗi khi lấy danh sách câu hỏi AI");

            const data = await response.json();

            console.log("Fetched data:", data); 

            setAiQuestions(data.questions || []);
            setTotalPages(data.totalPages || 1);
        } catch (error) {
            setError(error.message);
        } finally {
            setLoading(false);
        }
    };

    const approveQuestion = (id) => {
        console.log(`Phê duyệt câu hỏi với ID: ${id}`);

    };

    const editQuestion = (id) => {
        console.log(`Chỉnh sửa câu hỏi với ID: ${id}`);
   
    };
    const handleAddAIQuestions = async () => {
        setLoading(true);
        setShowModal(false);
    
        try {
         
            const response = await fetch("https://localhost:7259/api/AIQuestion/generate-ai-questions", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ question: lessonName, num_questions: numQuestions }),
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
          
            await fetch("https://localhost:7259/api/AIQuestion/update-lesson-id", {
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
    };
    
    

    
    return (
        <div className={styles.listAIQuestion}>
            <h1>Danh sách câu hỏi AI {lessonName}</h1>
            <button className={styles.addButton} onClick={() => setShowModal(true)}>
                Thêm câu hỏi bằng AI
            </button>
          
            <div className={styles.filters}>
                <select value={status} onChange={(e) => setStatus(e.target.value)}>
                    <option value="">Chọn trạng thái</option>
                    <option value="Pending">Chờ duyệt</option>
                    <option value="Approved">Đã duyệt</option>
                    {/* <option value="Rejected">Bị từ chối</option> */}
                </select>

                <select value={levelId} onChange={(e) => setLevelId(e.target.value)}>
                    <option value="">Chọn mức độ</option>
                    <option value="1">Yếu</option>
                    <option value="2">Trung bình</option>
                    <option value="3">Khá</option>
                    <option value="4">Giỏi</option>
                </select>

                <input type="date" value={createdAt} onChange={(e) => setCreatedAt(e.target.value)} />

              
            </div>

            {loading && <p>Đang tải...</p>}
            {error && <p style={{ color: "red" }}>{error}</p>}

       
            <table className={styles.questionTable}>
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Câu hỏi</th>
                        <th>Trạng thái</th>
                        <th>Mức độ</th>
                        <th>Hành động</th>
                    </tr>
                </thead>
                <tbody>
                    {aiQuestions.length > 0 ? (
                        aiQuestions.map((q) => (
                            <tr key={q.questionId}>
                                <td>{q.questionId}</td>
                                <td>{q.questionText}</td>
                                <td>{q.status}</td>
                                <td>
                                    {q.levelId === 1 ? "Yếu" : q.levelId === 2 ? "Trung bình" : q.levelId === 3 ? "Khá" : "Giỏi"}
                                </td>
                                <td>
                                    <button className={styles.approveBtn} onClick={() => approveQuestion(q.questionId)}>Phê Duyệt</button>
                                    <button className={styles.editBtn} onClick={() => editQuestion(q.questionId)}>Chỉnh Sửa</button>
                                </td>
                            </tr>
                        ))
                    ) : (
                        <tr>
                            <td colSpan="5" className={styles.noData}>Không có câu hỏi nào.</td>
                        </tr>
                    )}
                </tbody>
            </table>

        
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
                            onChange={(e) => setNumQuestions(Number(e.target.value))}
                            min="1"
                            max="10"
                        />
                        <div className={styles.modalActions}>
                            <button onClick={handleAddAIQuestions}>Xác nhận</button>
                            <button onClick={() => setShowModal(false)}>Hủy</button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
};

export default ListAIQuestion;
