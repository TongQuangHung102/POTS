import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import styles from './Test.module.css'; 

// Component hiển thị mỗi bài kiểm tra
const TestItem = ({ number, title, testId, gradeId, subjectId }) => {
    const navigate = useNavigate();

    return (
        <div className={styles.lessonItem} onClick={() => navigate(`/student/grade/${gradeId}/subject/${subjectId}/course/test/${testId}`)}>
            <div className={styles.lessonNumber}>{number}</div>
            <div className={styles.lessonInfo}>
                <div className={styles.lessonIcon}>📝</div>
                <div className={styles.lessonTitle}>{title}</div>
            </div>
        </div>
    );
};

// Component chính
const TestPage = ({listtests}) => {
    const {gradeId, subjectId} = useParams();
    return (
            <div className={styles.courseContainer}>
                <div className={styles.lessonContainer}>
                {listtests.length > 0 ? (
                listtests.map((test, index) => (
                <TestItem 
                key={index} 
                number={index + 1} 
                title={test.testName} 
                testId={test.testId}
                gradeId={gradeId}
                subjectId={subjectId} />
                ))
                ) : (
                <p className={styles.noData}>Không có bài kiểm tra nào.</p>
                )}
                </div>
            </div>
    );
};

export default TestPage;
