import React, { useState, useEffect, useRef } from "react";
import { useLocation, useParams } from "react-router-dom";
import styles from "./Quiz.module.css";
import { fetchTestQuestions } from "../../services/TestQuestion";
import { fetchPracticeQuestions, submitPracticeResult  } from "../../services/PracticeService";
import { formatElapsedTime } from "../../utils/timeUtils"
import BackLink from "../../components/BackLink";
import QuizResult from "./QuizResult";
import { useAuth } from "../../hooks/useAuth";
const Quiz = () => {
    const { testId, lessonId } = useParams();
    const [questions, setQuestions] = useState([]);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState(null);
    const [userAnswers, setUserAnswers] = useState([]);
    const [showScore, setShowScore] = useState(false);
    const [score, setScore] = useState(0);

    const questionRefs = useRef([]);
    const hasFetched = useRef(false); 

    const [elapsedTime, setElapsedTime] = useState(0);
    const [duration, setDuration] = useState(0);
    const [timeLeft, setTimeLeft] = useState(0);
    const [totalTime, setTotalTime] = useState(0);

    const {user} = useAuth();
    const location = useLocation();

    const isPremium = false; //goi cua hoc sinh

    const mode = location.pathname.includes("practice") ? "practice" : "test";

    useEffect(() => {
        if (mode === "test" && duration > 0) {
            setTimeLeft(duration * 60);
        } else if (mode !== "test") {
            setElapsedTime(0);
        }
    }, [duration, mode]);

    useEffect(() => {
        if (timeLeft > 0) {
            const timer = setInterval(() => {
                setTimeLeft(prevTime => prevTime - 1);
            }, 1000);
            return () => clearInterval(timer);
        }
    }, [timeLeft]);

    useEffect(() => {
        if (mode !== "test" || timeLeft <= 0) {
            const timer = setInterval(() => {
                setElapsedTime(prevTime => prevTime + 1);
            }, 1000);
            return () => clearInterval(timer);
        }
        
    }, [timeLeft, mode]);
    

    const formatTime = (seconds) => {
        const minutes = Math.floor(seconds / 60);
        const secs = seconds % 60;
        return `${minutes} phút ${secs} giây`;
    };

    const restartQuiz = () => {
        setUserAnswers(Array(questions.length).fill(null));
        setShowScore(false);
        setElapsedTime(0);
        setTimeLeft(duration * 60);
    };

    useEffect(() => {
        if (hasFetched.current) return; // Nếu đã fetch thì không gọi lại
    hasFetched.current = true;

        setIsLoading(true);


        const fetchQuestions = async () => {
            let result;
            if (mode === "test") {
                result = await fetchTestQuestions(testId);
            } else {
                result = await fetchPracticeQuestions(lessonId, 10);
            }
            if (result.error) {
                setError(result.error);
            } else {
                if (mode === "test") {
                    setQuestions(result.data);
                    setUserAnswers(Array(result.data.length).fill(null));
                }
                else {
                    setQuestions(result.data.questions);
                    setUserAnswers(Array(result.data.questions.length).fill(null));
                }
                if (mode === "test" && result.data.length > 0) {
                    setDuration(result.data[0].test.durationInMinutes);
                }
            }
            console.log('in ra 1 lan roi');

            setIsLoading(false);
            setElapsedTime(0);
        };

        fetchQuestions();
        hasFetched.current = true;
    }, [testId, mode]);


    const handleAnswerClick = (questionIndex, answerIndex) => {
        const newUserAnswers = [...userAnswers];
        newUserAnswers[questionIndex] = answerIndex;
        setUserAnswers(newUserAnswers);
    };

    const handleSubmit = async () => {
        let totalScore = 0;
        console.log(userAnswers);
        userAnswers.forEach((answer, index) => {
            console.log(questions[index].correctAnswer);
            console.log((answer + 1));
            if (answer !== null && questions[index].correctAnswer === (answer + 1)) {
                totalScore += 1;
            }
        });

        if (mode === "test") {
            setTotalTime(duration * 60 - timeLeft); 
        } else {
            setTotalTime(elapsedTime); 
        }
        
        setScore(totalScore);
        setShowScore(true);

        if (mode !== "test") {
            const formattedTime = formatElapsedTime(elapsedTime); 
            const data = {
                correctAnswers: totalScore,
                level: 2,
                time: formattedTime,
                userId: user.userId, 
                lessonId: lessonId
            };
    
            try {
                await submitPracticeResult(data);
                console.log("Gửi kết quả thành công:", data);
            } catch (error) {
                console.error("Lỗi khi gửi kết quả:", error);
            }
        }
    };
    if (isLoading) return <div className={styles.loading}>
        <p>Đang tải câu hỏi...</p>
    </div>
    if (error) return <div className={styles.loading}>
        <p>{error}</p>
    </div>

    return (
        <div className={styles.quizContainer}>
            <div className={styles.sidebar}>
                <div className={styles.back}><BackLink /></div>
                <div className={styles.quizHeader}>
                    <h5>{mode === "test" ? "Bài kiểm tra" : "Luyện tập"}: {questions.length > 0 ? questions[0].test?.testName : "N/A"}</h5>
                </div>
                {showScore ? (
                    // Khi đã nộp bài, hiển thị kết quả
                    <div className={styles.resultSection}>
                        <h6>Kết quả</h6>
                        <p>Bạn đã trả lời đúng <strong>{score}</strong> trên <strong>{questions.length}</strong> câu hỏi</p>
                        <p>⏳ Thời gian làm bài: <strong>{formatTime(totalTime)}</strong></p>
                        <button
                        className={styles.submitButton}
                           onClick={restartQuiz}
                        >
                            Làm lại bài kiểm tra
                        </button>
                    </div>
                ) : (
                    <>
                        <h6>Bảng câu hỏi</h6>
                        <div className={styles.timer}>
                            {mode === "test" ? (
                                <p>Thời gian còn lại: {formatTime(timeLeft)}</p>
                            ) : (
                                <p>Thời gian đã làm: {formatTime(elapsedTime)}</p>
                            )}
                        </div>
                        {questions.map((_, index) => (
                            <button
                                key={index}
                                className={`${styles.questionButton} ${userAnswers[index] !== null ? styles.answered : ""}`}
                                onClick={() => questionRefs.current[index]?.scrollIntoView({ behavior: "smooth" })}
                            >
                                Câu {index + 1} {userAnswers[index] !== null && "✓"}
                            </button>
                        ))}
                        <button
                            className={styles.submitButton}
                            onClick={handleSubmit}
                            disabled={userAnswers.includes(null)}
                        >
                            Nộp bài
                        </button>
                    </>
                )}
            </div>


            <div className={styles.mainContent}>
                {showScore ? (
                    isPremium ? (
                        <QuizResult
                            questions={questions}
                            userAnswers={userAnswers}
                            score={score}
                            onRestart={() => window.location.reload()}
                            isPremium={isPremium}
                        />
                    ) : (
                        <div className={styles.upgradeContainer}>
                            <div className={styles.upgradeContent}>
                                <h2>Nâng cấp để xem chi tiết kết quả</h2>
                                <p>Vui lòng nâng cấp lên gói cao cấp để xem đáp án đúng và phân tích chi tiết bài làm của bạn.</p>
                                <button className={styles.upgradeButton}>Nâng cấp ngay</button>
                            </div>
                        </div>
                    )
                ) : (
                    questions.map((question, questionIndex) => (
                        <div key={questionIndex} ref={(el) => (questionRefs.current[questionIndex] = el)} className={styles.questionCard}>
                            <p className={styles.questionText}>{question.questionText}</p>
                            <div className={styles.answerOptions}>
                                {question.answerQuestions.map((answer, answerIndex) => (
                                    <button
                                        key={answerIndex}
                                        className={`${styles.answerButton} ${userAnswers[questionIndex] === answerIndex ? styles.selected : ""}`}
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
