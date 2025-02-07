import axios from 'axios';

const API_URL = 'http://localhost:5095/api/User'; 

class UserService {
  async register(username, email, password) {
    try {
      const response = await axios.post(`${API_URL}/register`, {
        username,
        email,
        password
      });
      return response.data;
    }  catch (error) {
        console.error("Error details:", error);  // In chi tiết lỗi ra console
        if (error.response) {
          console.error("API Response Error:", error.response);  // In lỗi từ API
          return error.response.data; 
        } else {
          return 'Đã xảy ra lỗi, vui lòng thử lại sau.';
        }
    }
  }
}

export default new UserService();