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

export const markAllAsRead = async (userId) => {
  try {
    const response = await fetch(`https://localhost:7259/api/Notification/mark-all-as-read?userId=${userId}`, {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
      },
    });

    if (!response.ok) {
      throw new Error("Lỗi khi đánh dấu thông báo đã đọc");
    }

    return await response.json();
  } catch (error) {
    console.error("Lỗi:", error);
  }
};