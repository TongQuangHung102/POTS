import { authFetch } from "../utils/authFetch";
const apiUrl = process.env.REACT_APP_API_URL;
export const getSubjectGradesByGrade = async (gradeId) => {
    try {
      const response = await authFetch(`${apiUrl}/api/SubjectGrade/get-subject-by-grade/${gradeId}`);
      if (!response.ok) {
        throw new Error("Lỗi khi lấy dữ liệu môn học");
      }
      return await response.json();
    } catch (error) {
      console.error("Có lỗi xảy ra:", error);
      throw error;
    }
  };