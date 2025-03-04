export const fetchLevels = async () => {
    try {
        const response = await fetch('https://localhost:7259/api/Level/get-all-level'); 
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