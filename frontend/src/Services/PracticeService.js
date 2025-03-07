export const fetchPracticeQuestions = async (subject, numQuestions) => {
    try {
        const response = await fetch(`https://localhost:7259/api/Question/gen-question-practice`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                question: subject, 
                numQuestions: numQuestions }),
        });

        if (!response.ok) throw new Error("Không thể tải câu hỏi luyện tập.");

        const data = await response.json();
        return { data, error: null };
    } catch (error) {
        return { data: null, error: error.message };
    }
};

export const submitPracticeResult = async (data) => {
    try {
        const response = await fetch(`https://localhost:7259/api/PracticeAttempt/add-practice-attempt`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(data)
        });

        if (!response.ok) {
            throw new Error("Có lỗi khi gửi dữ liệu về API");
        }
        return await response.json();
    } catch (error) {
        throw error;
    }
};