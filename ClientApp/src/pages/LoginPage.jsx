import React, { useState } from 'react';
import api from '../api/client';
import { useAuth } from '../store/AuthContext';
import { motion } from 'framer-motion';

const LoginPage = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const { login } = useAuth();
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    try {
      const response = await api.post('/Auth/login', { email, password });
      const { token } = response.data;
      
      // Decode JWT to get role (simplified for now - normally use jwt-decode)
      // For now, let's just assume one for testing or handle better later
      const base64Url = token.split('.')[1];
      const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
      const payload = JSON.parse(window.atob(base64));
      
      const role = payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
      const tenantId = payload["TenantId"];

      login(token, role, tenantId);
    } catch (err) {
      alert('Login failed. Please check your credentials.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-[#F2F2F7] p-6">
      <motion.div 
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        className="w-full max-w-md ios-card"
      >
        <div className="text-center mb-8">
          <h1 className="text-3xl font-bold tracking-tight mb-2">HostelHub</h1>
          <p className="text-secondary">Sign in to manage your property</p>
        </div>

        <form onSubmit={handleSubmit} className="space-y-6">
          <div>
            <label className="block text-sm font-medium mb-1 ml-1 text-secondary">Email Address</label>
            <input 
              type="email" 
              className="ios-input"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              placeholder="e.g. admin@hostel.com"
              required
            />
          </div>

          <div>
            <label className="block text-sm font-medium mb-1 ml-1 text-secondary">Password</label>
            <input 
              type="password" 
              className="ios-input"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              placeholder="••••••••"
              required
            />
          </div>

          <button 
            type="submit" 
            disabled={loading}
            className="w-full btn-primary"
          >
            {loading ? 'Authenticating...' : 'Sign In'}
          </button>
        </form>
      </motion.div>
    </div>
  );
};

export default LoginPage;
