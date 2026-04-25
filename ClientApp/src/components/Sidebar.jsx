import React from 'react';
import { NavLink } from 'react-router-dom';
import { LayoutDashboard, Bed, History, Users, Settings, LogOut, Building2 } from 'lucide-react';
import { useAuth } from '../store/AuthContext';

const Sidebar = () => {
  const { user, logout } = useAuth();
  const isAdmin = user?.role === 'SuperAdmin';

  const menuItems = isAdmin 
    ? [
        { name: 'Dashboard', icon: LayoutDashboard, path: '/super-admin' },
        { name: 'Hostels', icon: Building2, path: '/super-admin/hostels' },
        { name: 'Users', icon: Users, path: '/super-admin/users' },
        { name: 'Settings', icon: Settings, path: '/settings' },
      ]
    : [
        { name: 'Property Grid', icon: Bed, path: '/hostel-admin' },
        { name: 'Bookings', icon: History, path: '/hostel-admin/bookings' },
        { name: 'Guests', icon: Users, path: '/hostel-admin/guests' },
        { name: 'Settings', icon: Settings, path: '/settings' },
      ];

  return (
    <aside className="w-72 h-screen fixed left-0 top-0 glass border-r flex flex-col p-6 z-50">
      <div className="flex items-center gap-3 mb-12 px-2">
        <div className="w-10 h-10 bg-ios-blue rounded-xl flex items-center justify-center text-white font-bold text-xl shadow-lg shadow-ios-blue/20">
          HH
        </div>
        <h2 className="text-xl font-bold tracking-tight">HostelHub</h2>
      </div>

      <nav className="flex-1 space-y-1">
        {menuItems.map((item) => (
          <NavLink
            key={item.path}
            to={item.path}
            className={({ isActive }) => `
              flex items-center gap-3 px-4 py-3 rounded-ios-xl transition-all duration-200
              ${isActive 
                ? 'bg-ios-blue text-white shadow-md shadow-ios-blue/10' 
                : 'text-secondary hover:bg-black/5 hover:text-black'}
            `}
          >
            <item.icon size={20} />
            <span className="font-medium">{item.name}</span>
          </NavLink>
        ))}
      </nav>

      <button 
        onClick={logout}
        className="flex items-center gap-3 px-4 py-3 rounded-ios-xl text-red-500 hover:bg-red-50 transition-all font-medium"
      >
        <LogOut size={20} />
        <span>Log Out</span>
      </button>
    </aside>
  );
};

export default Sidebar;
