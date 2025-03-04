import React, { useState, useEffect } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import styles from './Test.module.css'; // Dùng lại CSS từ CourseProgress.js

// Component hiển thị mỗi bài kiểm tra
const TestItem = ({ number, title, testId }) => {
    const navigate = useNavigate();

    return (
        <div className={styles.lessonItem} onClick={() => navigate(`/student/quiz?testId=${testId}`)}>
            <div className={styles.lessonNumber}>{number}</div>
            <div className={styles.lessonInfo}>
                <div className={styles.lessonIcon}>📝</div>
                <div className={styles.lessonTitle}>{title}</div>
            </div>
        </div>
    );
};

// Component chính
const TestPage = () => {
    const navigate = useNavigate();
    const [searchParams] = useSearchParams();
    const gradeId = searchParams.get("gradeId"); // Lấy gradeId từ URL

    const [tests, setTests] = useState([]);

    useEffect(() => {
        const storedTests = sessionStorage.getItem("testList");
        if (storedTests) {
            setTests(JSON.parse(storedTests));
        }
    }, []);

    return (
        <div className="main-content">
            <div className={styles.courseContainer}>
                <div className={styles.courseHeader}>
                    <div className={styles.headerLeft}>
                        <div className={styles.courseTitle}>Tên bài kiểm tra</div>
                        <div className={styles.courseTitle}>Trạng thái</div>
                    </div>
                    <div className={styles.headerRight}>
                        <button className={styles.courseButton} onClick={() => navigate(-1)}>
                            Quay lại
                        </button>
                    </div>
                </div>

                <div className={styles.lessonContainer}>
                {tests.length > 0 ? (
                tests.map((test, index) => (
                <TestItem key={index} number={index + 1} title={test.testName} testId={test.testId} />
                ))
                ) : (
                <p className={styles.noData}>Không có bài kiểm tra nào.</p>
                )}
                </div>
            </div>
        </div>
    );
};

export default TestPage;
