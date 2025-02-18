// src/hooks/useAuth.js
import { useState, useEffect } from 'react';

export function useAuth() {
  const [user, setUser] = useState(null);  
  const [loading, setLoading] = useState(true); 

  useEffect(() => {
    const token = sessionStorage.getItem('token');
    const roleId = sessionStorage.getItem('roleId');

    if (token && roleId) {
      setUser({ token, roleId });
    } else {
      setUser(null); 
    }

    setLoading(false);  
  }, []);

  return { user, loading };
};
