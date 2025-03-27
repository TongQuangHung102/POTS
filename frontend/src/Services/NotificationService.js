export const getNotifications = async (userId) => {
    try {
      const response = await fetch(`https://localhost:7259/api/Notification/get-notification/${userId}`);
      if (!response.ok) {
        throw new Error(`Lỗi HTTP: ${response.status}`);
      }
      return await response.json(); 
    } catch (error) {
      console.error("Lỗi khi gọi API lấy thông báo:", error);
      throw error;
    }
  };