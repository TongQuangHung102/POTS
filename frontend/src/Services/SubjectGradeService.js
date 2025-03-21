export const getSubjectGradesByGrade = async (gradeId) => {
    try {
      const response = await fetch(`https://localhost:7259/api/SubjectGrade/get-subject-by-grade/${gradeId}`);
      if (!response.ok) {
        throw new Error("Lỗi khi lấy dữ liệu môn học");
      }
      return await response.json();
    } catch (error) {
      console.error("Có lỗi xảy ra:", error);
      throw error;
    }
  };