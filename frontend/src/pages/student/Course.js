import React, { useState, useEffect } from 'react';
import styles from './Course.module.css';

// Component cho Badge
const LessonBadge = () => (
    <img
        className={styles.lessonbadge}
        src="data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCAyNCAyNCI+PHBhdGggZD0iTTEyIDJDNi40ODggMiAyIDYuNDg4IDIgMTJzNC40ODggMTAgMTAgMTAgMTAtNC40ODggMTAtMTBTMTcuNTEyIDIgMTIgMnptMCAxOGMtNC40MTEgMC04LTMuNTg5LTgtOHMzLjU4OS04IDgtOCA4IDMuNTg5IDggOC0zLjU4OSA4LTggOHoiIGZpbGw9IiM2Njg1RTEiLz48cGF0aCBkPSJNMTIgMThjLTMuMzA5IDAtNi0yLjY5MS02LTZzMi42OTEtNiA2LTYgNiAyLjY5MSA2IDYtMi42OTEgNi02IDZ6IiBmaWxsPSIjRTdFQ0ZGIi8+PC9zdmc+"
        alt="Badge"
    />
);

// Component cho mỗi bài học
const LessonItem = ({ number, title }) => {
    // Xác định màu dựa trên số thứ tự
  

    return (
        <div className={styles.lessonItem}>
            <div className={`${styles.lessonNumber}`}>{number}</div>
            <div className={styles.lessonInfo}>
                <div className={styles.lessonIcon}>📄</div>
                <div className={styles.lessonTitle}>{title}</div>
            </div>
            <LessonBadge />
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
    const API_URL = "https://localhost:7259/api/Curriculum/get-all-chapter";

    useEffect(() => {
        const fetchData = async () => {
            try {
                const response = await fetch(API_URL);
                const data = await response.json();

                const formattedSections = data.map(chapter => ({
                    order: chapter.order,
                    title: chapter.chapterName,
                    initialExpanded: false,
                    lessons: chapter.lessons.map(lesson => ({
                        number: String(lesson.order).padStart(2, "0"),
                        title: lesson.lessonName
                    }))
                }));

                setSections(formattedSections);
            } catch (error) {
                console.error("Lỗi khi lấy dữ liệu:", error);
            }
        };

        fetchData();
    }, []);

    return (
        <div className='main-content'>
            <div className={styles.courseContainer}>
                <div className={styles.courseHeader}>
                    <div className={styles.courseTitle}>Tên bài học</div>
                    <div className={styles.courseTitle}>Tiến độ học</div>
                </div>

                {sections.map((section, index) => (
                    <CourseSection
                        key={index}
                        order={section.order}
                        title={section.title}
                        lessons={section.lessons}
                        initialExpanded={section.initialExpanded}
                    />
                ))}
            </div>
        </div>
    );
};

export default CourseProgress;