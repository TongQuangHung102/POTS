export const fetchPracticeQuestions = async (userId, lessonId) => {
    try {
        const response = await fetch(`https://localhost:7259/api/Question/gen-question-practice`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                userId: userId, 
                lessonId: lessonId }),
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

export const submitTestResult = async (data) => {
    try {
        const response = await fetch(`https://localhost:7259/api/StudentTest/add-student-test`, {
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

export const saveStudentAnswers = async (answers) => {
    try {
      const response = await fetch(`https://localhost:7259/api/StudentAnswer/save-answers`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(answers),
      });
  
      if (!response.ok) {
        throw new Error(`Lỗi: ${response.statusText}`);
      }
  
      return await response.json();
    } catch (error) {
      console.error("Lỗi khi lưu câu trả lời:", error);
      throw error;
    }
  };