export const fetchTestQuestions = async (testId) => {
    if (!testId) return { data: [], error: "Không có testId" };

    try {
        const response = await fetch(`https://localhost:7259/api/TestQuestion/get-test-questions?testId=${testId}`);
        if (!response.ok) throw new Error("Không thể tải câu hỏi.");

        const data = await response.json();
        return data;
    } catch (error) {
        return { data: [], error: error.message };
    }
};

export const GenerateTest = async (data) => {
    try {
        console.log({ chapters: data });
        const response = await fetch(`https://localhost:7259/api/Question/generate-test`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({ chapters: data })
        });

        if (!response.ok) {
            throw new Error("Có lỗi khi gửi dữ liệu về API");
        }
        return await response.json();
    } catch (error) {
        throw error;
    }
};

export const AddQuestionsToTest = async (testId, testQuestions) =>{
    if (testQuestions.length === 0) {
        alert("Chưa có câu hỏi nào được chọn!");
        return;
    }

    const payload = {
        testId: testId,
        questionIds: testQuestions.map(q => q.questionId)
    };

    try {
        const response = await fetch("https://localhost:7259/api/TestQuestion/add-questions", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(payload)
        });

        if (!response.ok) {
            throw new Error("Có lỗi xảy ra khi thêm câu hỏi vào bài kiểm tra!");
        }

        const result = await response.json();
        alert("Thêm câu hỏi thành công!");
        
        console.log("Server Response:", result);

    } catch (error) {
        console.error("Error:", error);
        alert("Lỗi: " + error.message);
    }
}

export const UpdateTestQuestions = async (testId, testQuestions) =>{
    const payload = {
        testId: testId,
        questionIds: testQuestions.map(q => q.questionId)
    };
    try {
        const response = await fetch("https://localhost:7259/api/TestQuestion/update-questions", {
            method: "PUT",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(payload),
        });

        const data = await response.json();

        if (!response.ok) {
            throw new Error(data.message || "Cập nhật thất bại");
        }

        alert("Cập nhật câu hỏi thành công!");
    } catch (error) {
        console.error("Lỗi:", error.message);
        alert(error.message);
    }
}



