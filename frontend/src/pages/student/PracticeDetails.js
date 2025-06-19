import React, { useEffect, useState } from "react";
import styles from "./Quiz.module.css";
import { useNavigate, useParams } from 'react-router-dom';
import { authFetch } from "../../utils/authFetch";
import BackLink from "../../components/BackLink";
const apiUrl = process.env.REACT_APP_API_URL;

const PracticeDetails = () => {

    const [questions, setQuestions] = useState([])
    const { gradeId, subjectId, lessonId, practiceId } = useParams();

    const fetchPracticeAttemptDetail = async (practiceId) => {
        try {
            const response = await authFetch(`${apiUrl}/api/PracticeAttempt/history/detail/${practiceId}`, {
                method: "GET",
                headers: {
                    "Content-Type": "application/json"
                }
            });

            if (!response.ok) {
                throw new Error(`Lỗi khi lấy chi tiết bài làm: ${response.status}`);
            }

            const data = await response.json();
            return data;
        } catch (error) {
            console.error("Lỗi:", error.message);
            return null;
        }
    };

    useEffect(() => {
        const fetchData = async () => {
            const data = await fetchPracticeAttemptDetail(practiceId);
            if (data) {
           
                setQuestions(data);
            }
        };
        fetchData();
    }, [practiceId]);

    return (
        <div className={styles.practiceDetails}>
            <div className={styles.scoreCard}>
                <h2 className={styles.scoreTitle}>Kết quả làm bài</h2>
                <BackLink></BackLink>
                <div className={styles.resultContainer}>
                    {questions.map((question, questionIndex) => (
                        <div key={questionIndex} className={styles.resultCard}>
                            <p className={styles.questionText}>{question.question.questionText}</p>
                            <div className={styles.answerOptions}>
                                {question.question.answers.map((answer, answerIndex) => {
                                    const isUserAnswer = question.selectedAnswer === (answerIndex + 1);
                                    const isCorrect = question.question.correctAnswer === (answerIndex + 1);

                                    return (
                                        <button
                                            key={answerIndex}
                                            className={`
                                                ${styles.answerButton} 
                                                ${isUserAnswer ? (isCorrect ? styles.correct : styles.wrong) : ""}
                                                ${isCorrect ? styles.correctAnswer : ""}
                                            `}
                                        >
                                            {answer.answerText} {isCorrect && "✅"} {isUserAnswer && !isCorrect && "❌"}
                                        </button>
                                    );
                                })}
                            </div>
                        </div>
                    ))}
                </div>
            </div>
        </div>
    );
};

export default PracticeDetails;