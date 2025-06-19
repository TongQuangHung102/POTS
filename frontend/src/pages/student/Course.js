import React, { useState, useEffect } from 'react';
import styles from './Course.module.css';
import TestPage from './Test';
import BackLink from '../../components/BackLink';
import { useNavigate, useParams } from 'react-router-dom';
import { fetchStudentChapters } from '../../services/ChapterService';
import { authFetch } from '../../utils/authFetch';

// Component cho Badge
const LessonBadge = ({ averageScore, averageTime,id }) => {
    const navigate = useNavigate();
    const { gradeId, subjectId } = useParams();
    if (averageScore == 0 && averageTime == 0) {
        return null;
    }

    return (
        <div className={styles.lessonbadge}>
            {averageScore.toFixed(2)}đ / {averageTime.toFixed(2)}s
            <p className={styles.history} onClick={() => navigate(`/student/grade/${gradeId}/subject/${subjectId}/lesson/${id}/history`)}>Lịch sử</p>
        </div>
    );
}

// Component cho mỗi bài học
const LessonItem = ({ id, number, title, averageScore, averageTime }) => {
    const navigate = useNavigate();
    const { gradeId, subjectId } = useParams();

    const handlePractice = (id) => {
        navigate(`/student/grade/${gradeId}/subject/${subjectId}/lesson/${id}/practice`)
    }

    return (
        <div className={styles.lessonItem}>
            <div className={`${styles.lessonNumber}`}>{number}</div>
            <div className={styles.lessonInfo} onClick={() => handlePractice(id)}>
                <div className={styles.lessonIcon}>📄</div>
                <div className={styles.lessonTitle}>{title}</div>
            </div>
            <LessonBadge
                averageScore={averageScore}
                averageTime={averageTime}
                id={id}
            />
        </div>
    );
};

// Component cho mỗi phần học
const CourseSection = ({ title, order, lessons, initialExpanded = false }) => {
    const [expanded, setExpanded] = useState(initialExpanded);

    const toggleExpand = () => {
        setExpanded(!expanded);
    };

    return (
        <div className={`${styles.courseSection} ${!expanded ? styles.collapsed : ''}`}>
            <div className={styles.sectionHeader} onClick={toggleExpand}>
                <div className={styles.sectionTitleContainer}>
                    <span className={styles.expandIcon}></span>
                    <div style={{ marginLeft: '10px' }}>
                        <div className={styles.sectionTitle}>Chương {order}: {title}</div>
                        <div className={styles.sectionLessons}>{lessons.length} bài tập</div>
                    </div>
                </div>
            </div>

            <div className={styles.lessonContainer}>
                {lessons.map((lesson, index) => (
                    <LessonItem
                        key={index}
                        number={lesson.number}
                        title={lesson.title}
                        id={lesson.id}
                        averageScore={lesson.averageScore}
                        averageTime={lesson.averageTime}
                    />
                ))}

            </div>
        </div>
    );
};

// Component chính
const CourseProgress = () => {
    // Dữ liệu các phần học
    const [sections, setSections] = useState([]);
    const [tests, setTests] = useState([]);
    const [viewTests, setViewTests] = useState(false);
    const { subjectId } = useParams();
    const gradeId = localStorage.getItem("gradeId");
    const userId = localStorage.getItem("userId");
    const apiUrl = process.env.REACT_APP_API_URL;
    const navigate = useNavigate();

    useEffect(() => {
        const fetchData = async () => {
            try {
                const data = await fetchStudentChapters(gradeId, subjectId, userId)
            
                const formattedSections = data
                    .map(chapter => ({
                        order: chapter.order,
                        title: chapter.name,
                        initialExpanded: false,
                        lessons: chapter.lessons
                            .map(lesson => ({
                                number: String(lesson.order).padStart(2, "0"),
                                title: lesson.lessonName,
                                id: lesson.lessonId,
                                averageScore: lesson.averageScore,
                                averageTime: lesson.averageTime
                            }))
                    }));

                setSections(formattedSections);
            } catch (error) {
                console.error("Lỗi khi lấy dữ liệu:", error);
            }
        };

        fetchData();
    }, []);

    useEffect(() => {
        const fetchTest = async () => {
            if (!gradeId) {
                console.error("Không tìm thấy gradeId trong session!");
                return;
            }

            const API_URL = `${apiUrl}/api/Test/get-test-by-grade/${gradeId}/subject/${subjectId}`;

            try {
                const response = await authFetch(API_URL);
                if (!response.ok) {
                    throw new Error("Lỗi khi lấy danh sách bài kiểm tra");
                }

                const testList = await response.json();
                const visibleTests = testList.data.filter(test => test.isVisible);
                setTests(visibleTests);

            } catch (error) {
                console.error("Lỗi khi lấy dữ liệu bài kiểm tra:", error);
            }
        }
        fetchTest();
    }, []);




    return (
        <div className='main-content'>
            <div className={styles.courseContainer}>
                <div className={styles.courseHeader}>
                    <div className={styles.headerLeft}>
                        <BackLink />
                    </div>
                    <div className={styles.headerRight}>
                        <button
                            className={styles.courseButton}
                            onClick={() => setViewTests(false)}
                        >
                            Chương trình học
                        </button>

                        <button
                            className={styles.courseButton}
                            onClick={() => setViewTests(true)}
                        >
                            Đề kiểm tra
                        </button>
                    </div>
                </div>

                {viewTests ? <TestPage listtests={tests} /> : (
                    sections.map((section, index) => (
                        <CourseSection
                            key={index}
                            order={section.order}
                            title={section.title}
                            lessons={section.lessons}
                            initialExpanded={section.initialExpanded}
                        />
                    ))
                )}
            </div>
        </div>
    );
};

export default CourseProgress;