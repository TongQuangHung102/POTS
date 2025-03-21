export const fetchSubjects = async () => {
    try {
        const response = await fetch(`https://localhost:7259/api/Subject/get-all-subject`);

        const data = await response.json();
        if (!response.ok) {
            throw new Error(data.message || "Không thể lấy danh sách môn học học");
        }
        return data;
    } catch (error) {
        console.error("Có lỗi khi lấy môn học:", error.message);
        return [];
    }
};