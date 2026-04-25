import React, { createContext, useContext, useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const navigate = useNavigate();

  useEffect(() => {
    const token = localStorage.getItem('token');
    const role = localStorage.getItem('role');
    if (token) {
      setIsAuthenticated(true);
      setUser({ role });
    }
  }, []);

  const login = (token, role, tenantId) => {
    localStorage.setItem('token', token);
    localStorage.setItem('role', role);
    if (tenantId) localStorage.setItem('tenantId', tenantId);
    
    setIsAuthenticated(true);
    setUser({ role });
    
    if (role === 'SuperAdmin') navigate('/super-admin');
    else navigate('/hostel-admin');
  };

  const logout = () => {
    localStorage.clear();
    setIsAuthenticated(false);
    setUser(null);
    navigate('/login');
  };

  return (
    <AuthContext.Provider value={{ user, isAuthenticated, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => useContext(AuthContext);
