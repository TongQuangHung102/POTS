import { authFetch } from "../utils/authFetch";
const apiUrl = process.env.REACT_APP_API_URL;

export const fetchChapters = async (gradeId, subjectId) => {
  try {
      const response = await authFetch(`${apiUrl}/api/Curriculum/get-chapter-by-grade/${gradeId}/subject/${subjectId}`);
      const data = await response.json();
   

      if (!response.ok) {
          throw new Error(data.message || "Không thể lấy danh sách chương");
      }
      return data;
  } catch (error) {
      console.error("Có lỗi khi lấy dữ liệu chương:", error.message);
      return [];
  }
};

export const fetchChaptersWithNumQuestion = async (gradeId, subjectId) => {
  try {
      const response = await authFetch(`${apiUrl}/api/Curriculum/get-chapter-and-num-question-by-grade/${gradeId}/subject/${subjectId}`);
      const data = await response.json();

      if (!response.ok) {
          throw new Error(data.message || "Không thể lấy danh sách chương");
      }
      return data;
  } catch (error) {
      console.error("Có lỗi khi lấy dữ liệu chương:", error.message);
      return [];
  }
};

export const fetchStudentChapters = async (gradeId, subjectId , studentId) => {
    try {
        const response = await authFetch(`${apiUrl}/api/Curriculum/get-student-chapter/${gradeId}/subject/${subjectId}/student/${studentId}`);
        const data = await response.json();
  
        if (!response.ok) {
            throw new Error(data.message || "Không thể lấy danh sách chương");
        }
        return data;
    } catch (error) {
        console.error("Có lỗi khi lấy dữ liệu chương:", error.message);
        return [];
    }
  };

  // services/chapterService.js

export const addChapter = async (subjectgradeId, newChapterTitle, selectedSemester) => {
    if (!newChapterTitle.trim()) {
      throw new Error("Tên chương không được bỏ trống");
    }
    
  
    try {
      const response = await authFetch(`${apiUrl}/api/Curriculum/add-chapters`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          subjectgradeId: subjectgradeId,
          input: newChapterTitle,
          semester: selectedSemester
        })
      });
  
      if (!response.ok) {
        const errorResponse = await response.text();
        throw new Error(errorResponse || 'Không thể thêm chương mới');
      }
  
      return { success: true, message: "Chương đã được thêm thành công" };
    } catch (error) {
      return { success: false, message: error.message };
    }
  };
  