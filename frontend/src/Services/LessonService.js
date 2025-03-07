export const fetchLessons = async (chapterId) => {
    try {
        const response = await fetch(`https://localhost:7259/api/Curriculum/get-lesson-by-chapterId?chapterId=${chapterId}`);

        const data = await response.json();
        if (!response.ok) {
            throw new Error(data.message || "Không thể lấy danh sách bài học");
        }
        return data;
    } catch (error) {
        console.error("Có lỗi khi lấy bài học:", error.message);
        return [];
    }
};

