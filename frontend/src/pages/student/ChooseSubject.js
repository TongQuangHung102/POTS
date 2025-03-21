import { useState, useEffect } from "react";
import { useNavigate, Link } from "react-router-dom";
import styles from "./ChooseSubject.module.css";

const ChooseSubject = () => {
    const [subjects, setSubject] = useState([]);
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);
    const gradeId = sessionStorage.getItem('gradeId')
    useEffect(() => {
        const fetchSubject = async () => {
            try {
                setLoading(true);
                const response = await fetch(`https://localhost:7259/api/SubjectGrade/get-subject-by-grade/${gradeId}`);
                const data = await response.json();
                setSubject(data);
            } catch (error) {
                console.error("Lỗi khi lấy danh sách lớp:", error);
            } finally {
                setLoading(false);
            }
        };

        fetchSubject();
    }, []);

    const handleToCurriculum = (id) =>{
        navigate(`/student/grade/${gradeId}/subject/${id}/course`)
    };

    return (
        <div className={styles.container}>
          <h1 className={styles.heading}>Luyện tập trắc nghiệm online</h1>
          <div className={styles.subjectsGrid}>
            {subjects.map((subject) => (
              <div key={subject.id} className={styles.subjectCard}>
                <div className={styles.textContainer}>
                  {subject.name}
                </div>
                <h2 onClick={() => handleToCurriculum(subject.id)} className={styles.subjectTitle}>Luyện tập ngay</h2>
              </div>
            ))}
          </div>
        </div>
      );
    };

export default ChooseSubject;
