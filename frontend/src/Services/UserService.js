export const getUserProfileById = async (userId) => {
    try {
      const response = await fetch(`https://localhost:7259/api/User/get-user-profile/${userId}`);
      
      if (!response.ok) {
        throw new Error(`Lỗi HTTP! Mã trạng thái: ${response.status}`);
      }
  
      const data = await response.json();
      return data;
    } catch (error) {
      console.error("Lỗi khi gọi API:", error);
      throw error; 
    }
  };

  export const changePassword = async (userId, oldPassword, newPassword) => {
    try {
        const response = await fetch(`https://localhost:7259/api/User/change-password`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({ userId, oldPassword, newPassword }),
        });

        const data = await response.json();
        if (!response.ok) {
            throw new Error(data.message || "Đổi mật khẩu thất bại.");
        }

        return { success: true, message: data.message };
    } catch (error) {
        return { success: false, message: error.message };
    }
};

export const linkChildAccount = async (email, parentId) => {
  try {
      const response = await fetch(`https://localhost:7259/api/UserParentStudent/link-account`, {
          method: "POST",
          headers: {
              "Content-Type": "application/json",
          },
          body: JSON.stringify({ email, parentId }),
      });

      const result = await response.json();

        if (!response.ok) {
            throw new Error(result.message || "Có lỗi xảy ra khi liên kết tài khoản"); 
        }

        return result;
    } catch (error) {
        console.error("Lỗi liên kết tài khoản:", error.message);
        throw error; 
    }
};

export const verifyChildAccount = async (studentEmail, parentId, code) => {
  try {
      const response = await fetch(`https://localhost:7259/api/UserParentStudent/verify-link-account`, {
          method: "POST",
          headers: {
              "Content-Type": "application/json",
          },
          body: JSON.stringify({ parentId, studentEmail, code}),
      });

      const result = await response.json();

        if (!response.ok) {
            throw new Error(result.message || "Có lỗi xảy ra khi liên kết tài khoản"); 
        }

        return result;
    } catch (error) {
        console.error("Lỗi liên kết tài khoản:", error.message);
        throw error; 
    }
};

export const reSentCode = async (email, parentId) => {
  try {
      const response = await fetch(`https://localhost:7259/api/UserParentStudent/resend-verification-code`, {
          method: "POST",
          headers: {
              "Content-Type": "application/json",
          },
          body: JSON.stringify({ email, parentId }),
      });

      const result = await response.json();

        if (!response.ok) {
            throw new Error(result.message || "Có lỗi xảy ra khi liên kết tài khoản"); 
        }

        return result;
    } catch (error) {
        console.error("Lỗi liên kết tài khoản:", error.message);
        throw error; 
    }
};

export const unlinkStudent = async (parentId, studentId) => {
  try {
      const response = await fetch(`https://localhost:7259/api/UserParentStudent/unlink?parentId=${parentId}&studentId=${studentId}`, {
          method: "DELETE",
          headers: { "Content-Type": "application/json" },
      });

      if (!response.ok) {
          const errorData = await response.json();
          throw new Error(errorData.message || "Có lỗi xảy ra khi gỡ liên kết.");
      }

      return await response.json();
  } catch (error) {
      throw new Error(error.message || "Lỗi kết nối máy chủ.");
  }
};

export const createAccount = async (data) => {
    try {
        const response = await fetch(`https://localhost:7259/api/User/create-student-account`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(data),
        });
        console.log(response);
        const result = await response.json();
  
          if (!response.ok) {
              throw new Error(result.message || "Có lỗi xảy ra khi tạo thài khoản"); 
          }
  
          return result;
      } catch (error) {
          console.error("Lỗi tạo tài khoản:", error.message);
          throw error; 
      }
  };

  export const changeGrade = async (userId, gradeId) => {
    try {
        const response = await fetch(`https://localhost:7259/api/User/change-grade`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({
                userId: userId,
                gradeId: gradeId,
            }),
        });

        if (!response.ok) {
            const errorData = await response.json();
            throw new Error(errorData.error || "Có lỗi xảy ra khi đổi lớp.");
        }

        return await response.json();
    } catch (error) {
        console.error("Lỗi khi gọi API:", error);
        throw error;
    }
};