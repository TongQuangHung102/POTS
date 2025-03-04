export const fetchLessons = async (chapterId) => {
    try {
        const response = await fetch(`https://localhost:7259/api/Curriculum/get-lesson-by-chapterId?chapterId=${chapterId}`);
        const data = await response.json();
        return data;
    } catch (error) {
        console.error("Có lỗi khi lấy bài học", error);
        return [];
    }
};
