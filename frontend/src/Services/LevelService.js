export const fetchLevels = async () => {
    try {
        const response = await fetch('https://localhost:7259/api/Level/get-all-level'); 
        if (!response.ok) {
            throw new Error(`HTTP error! Status: ${response.status}`);
        }
        const data = await response.json();
        return data;
    } catch (error) {
        console.error("Có lỗi khi lấy levels, error");
        return [];
    }
};