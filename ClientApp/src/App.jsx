import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { AuthProvider, useAuth } from './store/AuthContext';
import LoginPage from './pages/LoginPage';
import Sidebar from './components/Sidebar';

// Placeholder Pages
const SuperAdminDashboard = () => <div className="ml-72 p-8"><h1 className="text-3xl font-bold mb-8">System Overview (Global)</h1><div className="grid grid-cols-3 gap-6"><div className="ios-card">Total Hostels<div className="text-4xl font-bold mt-2">124</div></div><div className="ios-card">Global Occupancy<div className="text-4xl font-bold mt-2">82%</div></div><div className="ios-card">Revenue (mtd)<div className="text-4xl font-bold mt-2">$42.5k</div></div></div></div>;

const BedGrid = () => {
  // Real implementation will fetch from API
  const rooms = [
    { number: '101', type: 'Dorm', beds: [{id: 1, status: 'Available'}, {id: 2, status: 'Occupied'}, {id: 3, status: 'Maintenance'}] },
    { number: '102', type: 'Private', beds: [{id: 4, status: 'Occupied'}] },
    { number: '103', type: 'Dorm', beds: [{id: 5, status: 'Available'}, {id: 6, status: 'Available'}] },
  ];

  return (
    <div className="ml-72 p-8">
      <h1 className="text-3xl font-bold mb-8">Property Inventory</h1>
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
        {rooms.map(room => (
          <div key={room.number} className="ios-card">
            <div className="flex justify-between items-center mb-6">
              <span className="text-lg font-bold">Room {room.number}</span>
              <span className="text-xs uppercase tracking-widest bg-black/5 px-2 py-1 rounded-full font-semibold">{room.type}</span>
            </div>
            <div className="grid grid-cols-4 gap-3">
              {room.beds.map(bed => (
                <div 
                  key={bed.id} 
                  className={`
                    h-12 rounded-lg flex items-center justify-center font-bold text-xs
                    ${bed.status === 'Available' ? 'bg-green-100 text-green-700' : 
                      bed.status === 'Occupied' ? 'bg-ios-blue text-white' : 'bg-orange-100 text-orange-700'}
                  `}
                >
                  B{bed.id}
                </div>
              ))}
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};

const ProtectedLayout = ({ children }) => {
  const { isAuthenticated } = useAuth();
  if (!isAuthenticated) return <Navigate to="/login" />;
  return (
    <div className="flex">
      <Sidebar />
      <main className="flex-1 min-h-screen bg-[#F2F2F7]">
        {children}
      </main>
    </div>
  );
};

const queryClient = new QueryClient();

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <Router>
        <AuthProvider>
          <Routes>
            <Route path="/login" element={<LoginPage />} />
            <Route path="/super-admin" element={<ProtectedLayout><SuperAdminDashboard /></ProtectedLayout>} />
            <Route path="/hostel-admin" element={<ProtectedLayout><BedGrid /></ProtectedLayout>} />
            <Route path="/" element={<Navigate to="/login" />} />
          </Routes>
        </AuthProvider>
      </Router>
    </QueryClientProvider>
  );
}

export default App;
