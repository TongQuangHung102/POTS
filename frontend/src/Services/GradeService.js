import { authFetch } from "../utils/authFetch";
const apiUrl = process.env.REACT_APP_API_URL; 
export const fetchGrades = async () => { 
    try {
      const response = await authFetch(`${apiUrl}/api/Grade/get-all-grade`);
      if (!response.ok) {
        throw new Error('Lỗi khi lấy dữ liệu lớp');
      }
      return await response.json();
    } catch (error) {
      console.error('Có lỗi khi lấy dữ liệu lớp', error);
    }
};
