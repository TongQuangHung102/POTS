import React, { useState, useEffect, useRef } from "react";
import { useSearchParams } from "react-router-dom";
import styles from "./Quiz.module.css";

const Quiz = () => {
    const [searchParams] = useSearchParams();
    const testId = searchParams.get("testId"); // Lấy testId từ URL
    const [questions, setQuestions] = useState([]);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState(null);
    const [userAnswers, setUserAnswers] = useState([]);
    const [showScore, setShowScore] = useState(false);
    const [score, setScore] = useState(0);
    const questionRefs = useRef([]);

    const [duration, setDuration] = useState(0);
    const [timeLeft, setTimeLeft] = useState(0);
    useEffect(() => {
        if (duration > 0) {
            setTimeLeft(duration * 60); // Chuyển đổi phút → giây
        }
    }, [duration]);
    useEffect(() => {
        if (timeLeft <= 0) return;

        const timer = setInterval(() => {
            setTimeLeft((prevTime) => prevTime - 1);
        }, 1000);

        return () => clearInterval(timer);
    }, [timeLeft]);
    const formatTime = (seconds) => {
        const minutes = Math.floor(seconds / 60);
        const secs = seconds % 60;
        return `${minutes}:${secs < 10 ? "0" : ""}${secs}`;
    };

    useEffect(() => {
        if (!testId) return;
        setIsLoading(true);

        fetch(`https://localhost:7259/api/TestQuestion/get-test-questions?testId=${testId}`)
            .then((res) => res.json())
            .then((data) => {
                setQuestions(data);
                console.log(data);
                setUserAnswers(Array(data.length).fill(null));

                if (data.length > 0) {
                    setDuration(data[0].test.durationInMinutes);
                }

                setIsLoading(false);
            })
            .catch((err) => {
                setError("Không thể tải câu hỏi.");
                setIsLoading(false);
            });
    }, [testId]);


    const handleAnswerClick = (questionIndex, answerIndex) => {
        const newUserAnswers = [...userAnswers];
        newUserAnswers[questionIndex] = answerIndex;
        setUserAnswers(newUserAnswers);
    };

    const calculateScore = () => {
        let totalScore = 0;
        userAnswers.forEach((answer, index) => {
            if (answer !== null && questions[index].answers[answer].isCorrect) {
                totalScore += 1;
            }
        });
        setScore(totalScore);
        setShowScore(true);
    };

    if (isLoading) return <p>Đang tải dữ liệu...</p>;
    if (error) return <p>{error}</p>;

    return (
        <div className={styles.quizContainer}>
            <div className={styles.sidebar}>
                <div className={styles.quizHeader}>
                    <h4>Bài kiểm tra: {questions.length > 0 ? questions[0].test.testName : "N/A"}</h4>
                    <p>🕒 Thời gian còn lại: {formatTime(timeLeft)}</p>
                </div>
                <h4>Danh sách câu hỏi</h4>
                {questions.map((_, index) => (
                    <button
                        key={index}
                        className={`${styles.questionButton} ${userAnswers[index] !== null ? styles.answered : ""
                            }`}
                        onClick={() => questionRefs.current[index]?.scrollIntoView({ behavior: "smooth" })}
                    >
                        Câu {index + 1} {userAnswers[index] !== null && "✓"}
                    </button>
                ))}
                <button
                    className={styles.submitButton}
                    onClick={calculateScore}
                    disabled={userAnswers.includes(null)}
                >
                    Nộp bài
                </button>
            </div>

            <div className={styles.mainContent}>
                {showScore ? (
                    <div className={styles.scoreCard}>
                        <h2 className={styles.scoreTitle}>Kết quả</h2>
                        <p>Bạn đã trả lời đúng {score} trên {questions.length} câu hỏi</p>
                        <button className={styles.restartButton} onClick={() => window.location.reload()}>
                            Làm lại bài kiểm tra
                        </button>
                    </div>
                ) : (
                    questions.map((question, questionIndex) => (
                        <div key={questionIndex} ref={(el) => (questionRefs.current[questionIndex] = el)} className={styles.questionCard}>
                            <p className={styles.questionText}>{question.questionText}</p>
                            <div className={styles.answerOptions}>
                                {question.answerQuestions
                                    .map((answer, answerIndex) => (
                                        <button
                                            key={answerIndex}
                                            className={`${styles.answerButton} ${userAnswers[questionIndex] === answerIndex ? styles.selected : ""
                                                }`}
                                            onClick={() => handleAnswerClick(questionIndex, answerIndex)}
                                        >
                                            {answer.answerText}
                                        </button>
                                    ))}
                            </div>
                        </div>
                    ))
                )}
            </div>
        </div>
    );
};

export default Quiz;
