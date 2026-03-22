import React, { createContext, useContext, useState, useEffect, type ReactNode } from 'react';
import api from '../api/axiosConfig';
import { type User, type AuthDto } from '../types';

interface AuthContextType {
  user: User | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  login: (data: AuthDto) => Promise<void>;
  register: (data: AuthDto) => Promise<void>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
  const [user, setUser] = useState<User | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const token = localStorage.getItem('token');
    const savedUsername = localStorage.getItem('username');

    if (token && savedUsername) {
      setUser({ username: savedUsername });
    }
    setIsLoading(false); // Finished checking
  }, []);

  const login = async (data: AuthDto) => {
    const response = await api.post('/auth/login', data);
    const { token } = response.data;

    localStorage.setItem('token', token);
    localStorage.setItem('username', data.username);
    
    setUser({ username: data.username });
  };

  const register = async (data: AuthDto) => {
    await api.post('/auth/register', data);
    await login(data);
  };

  const logout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('username');
    setUser(null);
  };

  return (
    <AuthContext.Provider value={{ user, isAuthenticated: !!user, isLoading, login, register, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};