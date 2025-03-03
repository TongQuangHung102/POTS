export const fetchChapters = async (gradeId) => {
    try {
      const response = await fetch(`https://localhost:7259/api/Curriculum/get-chapter-by-grade?gradeId=${gradeId}`);
      if (!response.ok) {
        throw new Error("Không thể lấy danh sách chương");
      }
      return await response.json();
    } catch (error) {
      console.error("Có lỗi khi lấy dữ liệu chương", error);
      return [];
    }
  };