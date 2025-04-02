export const fetchGrades = async () => { 
    try {
      const response = await fetch(`https://localhost:7259/api/Grade/get-all-grade`);
      if (!response.ok) {
        throw new Error('Lỗi khi lấy dữ liệu lớp');
      }
      return await response.json();
    } catch (error) {
      console.error('Có lỗi khi lấy dữ liệu lớp', error);
      throw error;
    }
};
