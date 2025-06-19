import { authFetch } from "../utils/authFetch";
const apiUrl = process.env.REACT_APP_API_URL;

export const fetchPracticeQuestions = async (userId, lessonId) => {
    try {
        const response = await authFetch(`${apiUrl}/api/Question/gen-question-practice`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                userId: userId, 
                lessonId: lessonId
            }),
        });

        const data = await response.json();

        if (!response.ok) {
            const message = data?.message || "Không thể tải câu hỏi luyện tập.";
            throw new Error(message);
        }

        return { data, error: null };
    } catch (error) {
        return { data: null, error: error.message };
    }
};


export const submitPracticeResult = async (data) => {
    try {
        const response = await authFetch(`${apiUrl}/api/PracticeAttempt/add-practice-attempt`, {
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
        const response = await authFetch(`${apiUrl}/api/StudentTest/add-student-test`, {
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
      const response = await authFetch(`${apiUrl}/api/StudentAnswer/save-answers`, {
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