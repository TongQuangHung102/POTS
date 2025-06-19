import React, { useState, useEffect } from "react";
import { Link, useNavigate } from "react-router-dom";
import BackLink from "../../components/BackLink";
import { authFetch } from "../../utils/authFetch";
import './ListLesson.css';


const ListGradesAssign = () => {
    const [grades, setGrades] = useState([]);
    const navigate = useNavigate();
    const apiUrl = process.env.REACT_APP_API_URL;
    
    const fetchGrades = async (userId) => {
        try {
            const response = await authFetch(`${apiUrl}/api/Grade/get-grade-by-userId/${userId}`);
            const data = await response.json();
            setGrades(data);
        } catch (error) {
            console.error("Có lỗi khi lấy dữ liệu lớp", error);
        }
    };

    useEffect(() => {
        const userId = localStorage.getItem('userId')
        fetchGrades(userId)
    }, []);

    return (
        <div className="chapter-list-container">
            <h2>Danh Sách Khối Lớp Quản lý</h2>
            <div className="group-header">
                <div>
                    <BackLink />
                </div>
            </div>
            <table className="chapter-table">
                <thead>
                    <tr>
                        <th style={{ width: "15%" }}>Id</th>
                        <th style={{ width: "20%" }}>Tên khối lớp</th>
                        <th style={{ width: "20%" }}>Mô tả</th>
                        <th style={{ width: "20%" }}>Hành động</th>
                    </tr>
                </thead>
                <tbody>
                    {grades.length > 0 ? (
                        grades.map((grade) => (
                            <tr key={grade.gradeId}>
                                <td>{grade.gradeId}</td>
                                <td>{grade.gradeName}</td>
                                <td>{grade.description}</td>
                                <td>
                                    <button>
                                        <Link to={`/content_manage/grades/${grade.gradeId}`}>Môn học</Link>
                                    </button>
                                </td>
                            </tr>
                        ))
                    ) : (
                        <tr>
                            <td colSpan="4" style={{ textAlign: "center", padding: "10px" }}>
                                Không có dữ liệu khối để hiển thị.
                            </td>
                        </tr>
                    )}
                </tbody>

            </table>
        </div>
    );
};

export default ListGradesAssign;
