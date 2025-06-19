import { authFetch } from "../utils/authFetch";
const apiUrl = process.env.REACT_APP_API_URL;
export const fetchLevels = async () => {
    try {
        const response = await authFetch(`${apiUrl}/api/Level/get-all-level`); 
        const data = await response.json(); 

        if (!response.ok) {
            throw new Error(data.message || `HTTP error! Status: ${response.status}`);
        }
        
        return data;
    } catch (error) {
        console.error("Có lỗi khi lấy levels:", error.message);
        return [];
    }
};