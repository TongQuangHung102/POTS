import { useState, useEffect  } from "react";
import { useParams } from "react-router-dom";
import styles from "./AddQuestionForm.module.css"; // Import CSS Module

const AddQuestionForm = ({ onAddQuestion }) => {
    const [questionText, setQuestionText] = useState("");
    const [levelId, setLevelId] = useState("");
    const [isVisible, setIsVisible] = useState(true);
    const [createByAI, setCreateByAI] = useState(false);
    const [correctAnswer, setCorrectAnswer] = useState(null);
    const [answerQuestions, setAnswerQuestions] = useState([
        { answerText: "", number: 1 },
        { answerText: "", number: 2 },
    ]);

    const [levels, setLevels] = useState([]);
    const [error, setError] = useState(null); 
    const [loading, setLoading] = useState(true);

    const {lessonId} = useParams();
    console.log(lessonId);

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

    // Hàm thêm đáp án mới
    const addAnswer = () => {
        setAnswerQuestions([...answerQuestions, { answerText: "", number: answerQuestions.length + 1 }]);
    };

    // Hàm cập nhật nội dung đáp án
    const handleAnswerChange = (index, text) => {
        const updatedAnswers = [...answerQuestions];
        updatedAnswers[index].answerText = text;
        setAnswerQuestions(updatedAnswers);
    };

    // Hàm xóa đáp án
    const removeAnswer = (index) => {
        const updatedAnswers = answerQuestions.filter((_, i) => i !== index);
        setAnswerQuestions(updatedAnswers);
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        const newQuestion = {
            questionText: questionText,
            createAt: new Date().toISOString(),
            levelId: parseInt(levelId),
            lessonId: lessonId,
            isVisible: true,
            createByAI: false,
            correctAnswer: parseInt(correctAnswer),
            answerQuestions: answerQuestions
        };
        console.log(newQuestion);
        try {
            const response = await fetch("https://localhost:7259/api/Question/add-question", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(newQuestion),
            });

            const result = await response.json();
            if (response.ok) {
                alert("Câu hỏi đã được thêm thành công!");
                setQuestionText("");
                setLevelId("");
                setCorrectAnswer("");
                setAnswerQuestions([{ answerText: "", number: 1 }]);
            } else {
                alert("Lỗi: " + result.Message);
            }
        } catch (error) {
            console.error("Lỗi khi gọi API:", error);
            alert("Có lỗi xảy ra khi gửi yêu cầu.");
        }
    };

    return (
        <form className={styles.form} onSubmit={handleSubmit}>
            <h2>Thêm Câu Hỏi</h2>

            <label>Câu Hỏi:</label>
            <textarea 
                value={questionText} 
                onChange={(e) => setQuestionText(e.target.value)}
                required
            />

            <label>Chọn Level:</label>
            <select value={levelId} onChange={(e) => setLevelId(e.target.value)} required>
                <option value="" disabled>-- Chọn Level --</option>
                {levels.map(level => (
                    <option key={level.levelId} value={level.levelId}>
                        {level.levelName}
                    </option>
                ))}
            </select>
            <label>Đáp Án:</label>
            {answerQuestions.map((ans, index) => (
                <div key={index} className={styles.answer}>
                    <input 
                        type="text" 
                        value={ans.answerText} 
                        onChange={(e) => handleAnswerChange(index, e.target.value)}
                        required
                    />
                    <input 
                        type="radio" 
                        name="correctAnswer" 
                        value={ans.number} 
                        checked={correctAnswer === ans.number}
                        onChange={() => setCorrectAnswer(ans.number)}
                    /> Đáp án đúng
                    <button 
                        type="button" 
                        className={styles.removeButton} 
                        onClick={() => removeAnswer(index)}
                        disabled={answerQuestions.length <= 2}
                    >
                        ❌
                    </button>
                </div>
            ))}

            <button type="button" className={styles.addButton} onClick={addAnswer}>
                ➕ Thêm Đáp Án
            </button>

            <button type="submit" className={styles.button}>Thêm Câu Hỏi</button>
        </form>
    );
};

export default AddQuestionForm;
