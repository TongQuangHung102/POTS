export const fetchTestQuestions = async (testId) => {
    if (!testId) return { data: [], error: "Không có testId" };

    try {
        const response = await fetch(`https://localhost:7259/api/TestQuestion/get-test-questions?testId=${testId}`);
        if (!response.ok) throw new Error("Không thể tải câu hỏi.");

        const data = await response.json();
        return { data, error: null };
    } catch (error) {
        return { data: [], error: error.message };
    }
};



