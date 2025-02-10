
import React, { useState } from 'react';
import UserService from '../Services/UserService'; 
import '../Components/Register.css';

const Register = () => {
  const [username, setUsername] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [successMessage, setSuccessMessage] = useState('');

  const handleSubmit = async (e) => {
    e.preventDefault();

    const result = await UserService.register(username, email, password);

    if (result === 'Đăng ký thành công!') {
      setSuccessMessage(result);
      setError('');
    } else {
      setError(result);
      setSuccessMessage('');
    }
  };

  return (
    <div className="register-container">
      <h2>Đăng ký tài khoản</h2>
      
      {successMessage && <div className="success-message">{successMessage}</div>}
      {error && <div className="error-message">{error}</div>}
      
      <form onSubmit={handleSubmit}>
        <div>
          <label>Username:</label>
          <input
            type="text"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            required
          />
        </div>
        
        <div>
          <label>Email:</label>
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
          />
        </div>

        <div>
          <label>Password:</label>
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />
        </div>

        <button type="submit">Đăng ký</button>
      </form>
    </div>
  );
};

export default Register;
