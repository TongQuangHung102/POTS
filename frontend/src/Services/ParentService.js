export const getStudentsByUserId = async (userId) => {
    try {
      const response = await fetch(`https://localhost:7259/api/UserParentStudent/get-students/${userId}`);
      if (!response.ok) {
        throw new Error(`Lỗi HTTP! Mã trạng thái: ${response.status}`);
      }
      const data = await response.json();
      return data;
    } catch (error) {
      console.error("Lỗi khi gọi API lấy danh sách học sinh:", error);
      throw error; 
    }
  };