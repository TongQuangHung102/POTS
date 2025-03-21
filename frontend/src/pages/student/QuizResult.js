import React from "react";
import styles from "./Quiz.module.css";
import { BiInfoCircle } from "react-icons/bi";

const QuizResult = ({ questions, userAnswers, mode, onReport, isPremium }) => {
    return (
        <div className={styles.scoreCard}>
            <h2 className={styles.scoreTitle}>Kết quả làm bài</h2>

            {isPremium && (
                <div className={styles.resultContainer}>
                    {questions.map((question, questionIndex) => (
                        <div key={questionIndex} className={styles.resultCard}>
                            <div className={styles.questionContainer}>
                                <p className={styles.questionText}>{question.questionText}</p>
                                {mode === "test" && (
                                    <button 
                                        className={styles.reportButton} 
                                        onClick={() => onReport(question)}
                                    >
                                       <BiInfoCircle size={23} color="black" />
                                    </button>
                                )}
                            </div>
                            
                            <div className={styles.answerOptions}>
                                {question.answerQuestions.map((answer, answerIndex) => {
                                    const isUserAnswer = userAnswers[questionIndex] === answerIndex;
                                    const isCorrect = question.correctAnswer === (answerIndex + 1);

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
            )}
        </div>
    );
};

export default QuizResult;
