import React, { useState, useRef } from 'react';
import styles from './Quiz.module.css';

const Quiz = () => {
    const [showScore, setShowScore] = useState(false);
    const [score, setScore] = useState(0);
    const questionRefs = useRef([]);
  
    const questions = [
      {
        questionText: 'Câu 1: Thủ đô của Việt Nam là gì?',
        answerOptions: [
          { answerText: 'Hà Nội', isCorrect: true },
          { answerText: 'Hồ Chí Minh', isCorrect: false },
          { answerText: 'Đà Nẵng', isCorrect: false },
          { answerText: 'Huế', isCorrect: false },
        ],
      },
      {
        questionText: 'Câu 2: 1 + 1 = ?',
        answerOptions: [
          { answerText: '1', isCorrect: false },
          { answerText: '2', isCorrect: true },
          { answerText: '3', isCorrect: false },
          { answerText: '4', isCorrect: false },
        ],
      },
      {
        questionText: 'Câu 3: HTML là viết tắt của?',
        answerOptions: [
          { answerText: 'Hyper Text Markup Language', isCorrect: true },
          { answerText: 'High Text Machine Language', isCorrect: false },
          { answerText: 'Hyper Text Machine Language', isCorrect: false },
          { answerText: 'High Text Markup Language', isCorrect: false },
        ],
      },
      {
        questionText: 'Câu 1: Thủ đô của Việt Nam là gì?',
        answerOptions: [
          { answerText: 'Hà Nội', isCorrect: true },
          { answerText: 'Hồ Chí Minh', isCorrect: false },
          { answerText: 'Đà Nẵng', isCorrect: false },
          { answerText: 'Huế', isCorrect: false },
        ],
      },
      {
        questionText: 'Câu 2: 1 + 1 = ?',
        answerOptions: [
          { answerText: '1', isCorrect: false },
          { answerText: '2', isCorrect: true },
          { answerText: '3', isCorrect: false },
          { answerText: '4', isCorrect: false },
        ],
      },
      {
        questionText: 'Câu 3: HTML là viết tắt của?',
        answerOptions: [
          { answerText: 'Hyper Text Markup Language', isCorrect: true },
          { answerText: 'High Text Machine Language', isCorrect: false },
          { answerText: 'Hyper Text Machine Language', isCorrect: false },
          { answerText: 'High Text Markup Language', isCorrect: false },
        ],
      },
    ];

    const [userAnswers, setUserAnswers] = useState(Array(questions.length).fill(null));
  
    const handleAnswerClick = (questionIndex, answerIndex) => {
      const newUserAnswers = [...userAnswers];
      newUserAnswers[questionIndex] = answerIndex;
      setUserAnswers(newUserAnswers);
    };
  
    const scrollToQuestion = (index) => {
      questionRefs.current[index]?.scrollIntoView({ behavior: 'smooth' });
    };
  
    const calculateScore = () => {
      let totalScore = 0;
      userAnswers.forEach((answer, index) => {
        if (answer !== null && questions[index].answerOptions[answer].isCorrect) {
          totalScore += 1;
        }
      });
      setScore(totalScore);
      setShowScore(true);
    };
  
    const restartQuiz = () => {
      setScore(0);
      setShowScore(false);
      setUserAnswers(Array(questions.length).fill(null));
      window.scrollTo(0, 0);
    };
  
    return (
      <div className={styles.quizContainer}>
        {/* Sidebar */}
        <div className={styles.sidebar}>
          <div className={styles.sidebarHeader}>
            <h4>Danh sách câu hỏi</h4>
          </div>
          <div className={styles.questionList}>
            {questions.map((_, index) => (
              <button
                key={index}
                className={`${styles.questionButton} ${
                  userAnswers[index] !== null ? styles.answered : ''
                }`}
                onClick={() => scrollToQuestion(index)}
              >
                Câu {index + 1}
                {userAnswers[index] !== null && ' ✓'}
              </button>
            ))}
          </div>
          <div className={styles.progressInfo}>
            Đã trả lời: {userAnswers.filter(answer => answer !== null).length}/{questions.length}
          </div>
          <button
            className={styles.submitButton}
            onClick={calculateScore}
            disabled={userAnswers.includes(null)}
          >
            Nộp bài
          </button>
        </div>
  
        {/* Main Content */}
        <div className={styles.mainContent}>
          {showScore ? (
            <div className={styles.scoreCard}>
              <h2 className={styles.scoreTitle}>Kết quả của bạn</h2>
              <div className={styles.scoreResult}>
                Bạn đã trả lời đúng {score} trên {questions.length} câu hỏi
              </div>
              <button className={styles.restartButton} onClick={restartQuiz}>
                Làm lại bài kiểm tra
              </button>
            </div>
          ) : (
            questions.map((question, questionIndex) => (
              <div
                key={questionIndex}
                ref={el => questionRefs.current[questionIndex] = el}
                className={styles.questionCard}
              >
                <div className={styles.questionText}>{question.questionText}</div>
                <div className={styles.answerOptions}>
                  {question.answerOptions.map((answerOption, answerIndex) => (
                    <button
                      key={answerIndex}
                      className={`${styles.answerButton} ${
                        userAnswers[questionIndex] === answerIndex ? styles.selected : ''
                      }`}
                      onClick={() => handleAnswerClick(questionIndex, answerIndex)}
                    >
                      {answerOption.answerText}
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