import { authFetch } from "../utils/authFetch";

export const fetchLessons = async (chapterId) => {
    const apiUrl = process.env.REACT_APP_API_URL;
    try {
        const response = await authFetch(`${apiUrl}/api/Curriculum/get-lesson-by-chapterId?chapterId=${chapterId}`);

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

