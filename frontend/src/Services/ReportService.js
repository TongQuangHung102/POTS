
export const updateReport = async (report) => {
    try {
        const requestBody = {
            reportId: report.reportId,
            questionId: report.questionId,
            status: report.status,
            question: {
                correctAnswer: report.correctAnswer,
                questionText: report.questionText,
                answerQuestions: report.answerQuestions.map(a => ({
                    answerQuestionId: a.id,
                    answerText: a.text,
                    number: a.number
                }))
            },
        };

        const response = await fetch(`https://localhost:7259/api/Report/update-report`, {
            method: "PUT",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(requestBody),
        });

        const result = await response.json();
        if (!response.ok) throw new Error(result.message);

        return { success: true, message: "Cập nhật thành công!" };
    } catch (error) {
        return { success: false, message: error.message };
    }
};
