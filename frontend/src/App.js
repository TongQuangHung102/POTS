// src/App.js
import React from 'react';
import { BrowserRouter as Router } from 'react-router-dom';
import AppRoutes from './rout/AppRoutes'; // Import AppRoutes

const App = () => {
  return (
    <Router>
      <AppRoutes /> {/* Gọi AppRoutes để quản lý các route */}
    </Router>
  );
};

export default App;
