export const fetchChapters = async (gradeId) => {
  try {
      const response = await fetch(`https://localhost:7259/api/Curriculum/get-chapter-by-grade?gradeId=${gradeId}`);
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

export const fetchStudentChapters = async (gradeId, studentId) => {
    try {
        const response = await fetch(`https://localhost:7259/api/Curriculum/get-student-chapter/${gradeId}/${studentId}`);
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