import React from 'react';
import { useAuth } from '../contexts/AuthContext';
import { useNavigate } from 'react-router-dom';

const Dashboard: React.FC = () => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <div style={{ padding: '20px' }}>
      <h1>Personal Finance Dashboard </h1>
      <p>Welcome, {user?.firstName} {user?.lastName}!</p>
      <button onClick={handleLogout} style={{ padding: '10px 20px', cursor: 'pointer' }}>
        Logout
      </button>
      <div style={{ marginTop: '30px' }}>
        <p>Dashboard content coming soon...</p>
      </div>
    </div>
  );
};

export default Dashboard;