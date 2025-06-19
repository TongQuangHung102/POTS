// src/hooks/useAuth.js
import { useState, useEffect } from 'react';

export function useAuth() {
  const [user, setUser] = useState(null);  
  const [loading, setLoading] = useState(true); 

  useEffect(() => {
    const token = localStorage.getItem('token');
    const roleId = localStorage.getItem('roleId');
    const userId = localStorage.getItem('userId')
    const gradeId = localStorage.getItem('gradeId')
    if (roleId, userId) {
      setUser({ token, roleId, userId, gradeId });
    } else {
      setUser(null); 
    }

    setLoading(false);  
  }, []);

  return { user, loading, setUser };
};
