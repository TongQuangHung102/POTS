import React, { useState, useEffect, useRef } from "react";
import { data, useLocation, useParams } from "react-router-dom";
import styles from "./Quiz.module.css";
import { fetchTestQuestions } from "../../services/TestQuestion";
import { getReportReasons } from "../../services/ReportService";
import { fetchPracticeQuestions, submitPracticeResult, submitTestResult } from "../../services/PracticeService";


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
    const [sampleQuestion, setsampleQuestion] = useState('');
    const [selectedQuestion, setSelectedQuestion] = useState(null);

    const [byAI, setByAI] = useState(true);

    const questionRefs = useRef([]);
    const hasFetched = useRef(false);

    const [elapsedTime, setElapsedTime] = useState(0);
    const [duration, setDuration] = useState(0);
    const [timeLeft, setTimeLeft] = useState(0);
    const [totalTime, setTotalTime] = useState(0);
    const [startTime, setStartTime] = useState(new Date().toISOString());

    const [isReport, setIsReport] = useState(false);
    const [isQuestionBank, setIsQuestionBank] = useState(false);
    const [reportReason, setReportReason] = useState();
    const [reasons, setReasons] = useState([]);

    const { user, loading } = useAuth();
    const location = useLocation();

    const isPremium = true; //goi cua hoc sinh

    const mode = location.pathname.includes("practice") ? "practice" : "test";


    useEffect(() => {
        if (mode === "test") {
            setTimeLeft(duration * 60);
            setStartTime(new Date().toISOString());
        } else if (mode !== "test") {
            setElapsedTime(0);
        }
    }, [duration]);

    useEffect(() => {
        if (mode === "test" && timeLeft > 0) {
            const timer = setInterval(() => {
                setTimeLeft(prevTime => {
                    if (prevTime - 1 <= 0) {
                        clearInterval(timer);
                        handleSubmit();
                        return 0;
                    }
                    return prevTime - 1;
                });
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
        window.location.reload();
    };

    useEffect(() => {
        if (loading) return;
        if (hasFetched.current) return;
        hasFetched.current = true;

        setIsLoading(true);

        const fetchQuestions = async () => {
            let result;
            if (mode === "test") {
                result = await fetchTestQuestions(testId);
            } else {
                result = await fetchPracticeQuestions(user.userId, lessonId);
            }
            if (result.error) {
                setError(result.error);
            } else {
                if (mode === "test") {
                    setQuestions(result.data);
                    setUserAnswers(Array(result.data.length).fill(null));
                    setIsQuestionBank(true);
                }
                else {
                    console.log(result.data.questions);
                    setQuestions(result.data.questions);
                    setByAI(result.data.byAi);
                    setsampleQuestion(result.data.questions[0].questionText);
                    setUserAnswers(Array(result.data.questions.length).fill(null));
                }
                if (mode === "test") {
                    setDuration(result.test.durationInMinutes);
                    setStartTime(new Date().toISOString());
                }
            }
            setIsLoading(false);
            setElapsedTime(0);
        };

        fetchQuestions();
        hasFetched.current = true;
    }, [loading, isLoading]);

    useEffect(() => {
        if(isQuestionBank){
            const fetchReasons = async () => {
                const data = await getReportReasons();
                setReasons(data);
            };
            fetchReasons();
        }
    }, [isQuestionBank]);


    const handleAnswerClick = (questionIndex, answerIndex) => {
        const newUserAnswers = [...userAnswers];
        newUserAnswers[questionIndex] = answerIndex;
        setUserAnswers(newUserAnswers);
    };

    const handleSubmit = async () => {
        let totalScore = 0;
        let studentAnswers = [];
        
        userAnswers.forEach((answer, index) => {
            if (answer !== null) {
                let isCorrect = questions[index].correctAnswer === (answer + 1);
                if (isCorrect) {
                    totalScore += 1;
                }
                studentAnswers.push({
                    questionId: questions[index].questionId,
                    selectedAnswer: answer + 1,
                });
            }
        });

        const endTime = new Date().toISOString();

        if (mode === "test") {
            setTotalTime(duration * 60 - timeLeft);
        } else {
            setTotalTime(elapsedTime);
        }

        if (mode !== "test" && byAI) {
            const data = {
                correctAnswers: totalScore,
                level: 2,
                timePractice: elapsedTime,
                userId: user.userId,
                lessonId: lessonId,
                sampleQuestion: sampleQuestion,
                answers: studentAnswers,
            };
            try {
                console.log(data);
                await submitPracticeResult(data);
                console.log("Gửi kết quả thành công:", data);
            } catch (error) {
                console.error("Lỗi khi gửi kết quả:", error);
            }
        } else {
            if(mode === 'test'){
                const data = {
                    startTime: startTime,
                    endTime: endTime,
                    score: totalScore,
                    testId: testId,
                    userId: user.userId,
                };
                try {
                    await submitTestResult(data);
                    console.log("Gửi kết quả bài kiểm tra thành công:", data);
                } catch (error) {
                    console.error("Lỗi khi gửi kết quả bài kiểm tra:", error);
                }
            }else{
                setIsQuestionBank(true);
            }
         
        }
        setTimeLeft(0);
        setScore(totalScore);
        setShowScore(true);
    };

    const handleReport = (question) => {
        setSelectedQuestion(question);
        setIsReport(true);
    };

    const handleClose = () => {
        setSelectedQuestion(null);
        setIsReport(false);
    };

    const handleReportSubmit = async () => {
        if (!reportReason.trim()) {
            alert("Vui lòng nhập lý do báo cáo!");
            return;
        }

        try {
            const response = await fetch("https://localhost:7259/api/Report/add-report", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({
                    questionId: selectedQuestion.questionId,
                    reason: reportReason,
                    userId: user.userId,
                }),
            });
            if (!response.ok) {
                const errorData = await response.json();
                console.log(errorData);
                alert(errorData.details || "Đã xảy ra lỗi khi gửi báo cáo.");
                return;
            }
            const responseData = await response.json();
            alert(responseData.message || "Báo cáo đã được gửi!");
            setIsReport(false);
            setReportReason("");
        } catch (error) {
            alert("Đã xảy ra lỗi, vui lòng thử lại sau.");
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
                    <h5>{mode === "test" ? "Bài kiểm tra" : "Luyện tập"}</h5>
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
                            Tiếp lục luyện tập
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
                        {!byAI && <div><p className={styles.messageAi}>*Tính năng AI tạm thời không khả dụng, câu hỏi đang được lấy từ ngân hàng câu hỏi.</p></div>}
                    </>
                )}
            </div>


            <div className={styles.mainContent}>
                {showScore ? (
                    isPremium ? (
                        <QuizResult
                            questions={questions}
                            userAnswers={userAnswers}
                            isPremium={isPremium}
                            isQuestionBank={isQuestionBank}
                            onReport={(question) => handleReport(question)}
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

            {isReport && (
                <div className={styles.modal}>
                    <div className={styles.modalContent}>
                        <h3>Báo cáo câu hỏi</h3>

                        <label>
                            Lý do báo cáo:
                            <select
                                value={reportReason}
                                onChange={(e) => setReportReason(e.target.value)}
                                className={styles.select}
                            >
                                <option value="">Chọn lý do...</option>
                                {reasons.map((reason, index) => (
                                    <option key={index} value={reason.id}>
                                        {reason.name}
                                    </option>
                                ))}
                            </select>
                        </label>

                        <div className="button-group">
                            <button onClick={handleReportSubmit} >Gửi báo cáo</button>
                            <button onClick={handleClose}>Đóng</button>
                        </div>

                    </div>
                </div>
            )}


        </div>
    );
};

export default Quiz;
