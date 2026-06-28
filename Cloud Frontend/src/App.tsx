import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom';
import { AppSplashGate } from './components/AppSplashGate';
import { RequireAuth } from './components/RequireAuth';
import { Layout } from './components/Layout';
import { Login } from './pages/Login';
import { LiveFeed } from './pages/LiveFeed';
import { Dashboard } from './pages/Dashboard';
import { EntityList } from './pages/EntityList';
import { Settlements } from './pages/Settlements';
import './App.css';

export default function App() {
  return (
    <BrowserRouter>
      <AppSplashGate>
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route
          element={
            <RequireAuth>
              <Layout />
            </RequireAuth>
          }
        >
          <Route index element={<LiveFeed />} />
          <Route path="dashboard" element={<Dashboard />} />
          <Route path="quotations" element={<EntityList entity="quotations" />} />
          <Route path="estimations" element={<EntityList entity="estimations" />} />
          <Route path="sales" element={<EntityList entity="sales" />} />
          <Route path="sales-pipes" element={<EntityList entity="sales-pipes" />} />
          <Route path="sales-traders" element={<EntityList entity="sales-traders" />} />
          <Route path="vouchers" element={<EntityList entity="vouchers" />} />
          <Route path="settlements" element={<Settlements />} />
          <Route path="*" element={<Navigate to="/" replace />} />
        </Route>
      </Routes>
      </AppSplashGate>
    </BrowserRouter>
  );
}
