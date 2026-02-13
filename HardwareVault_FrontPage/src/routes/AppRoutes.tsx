// routes/AppRoutes.tsx
import { createBrowserRouter, Navigate } from 'react-router-dom';
import { RequireAuth } from './RequireAuth';
import { LoginPage } from '../pages/LoginPage';
import { DashboardPage } from '../pages/DashboardPage';
import { DevicesPage } from '../pages/DevicesPage';
import { ImportPage } from '../pages/ImportPage';
import { ImportHistoryPage } from '../pages/ImportHistoryPage';
import { ManufacturersPage } from '../pages/ManufacturersPage';

export const router = createBrowserRouter([
    {
        path: '/login',
        element: <LoginPage />,
    },
    {
        path: '/',
        element: <RequireAuth />,
        children: [
            {
                index: true,
                element: <Navigate to="/dashboard" replace />,
            },
            {
                path: 'dashboard',
                element: <DashboardPage />,
            },
            {
                path: 'devices',
                element: <DevicesPage />,
            },
            {
                path: 'import',
                element: <ImportPage />,
            },
            {
                path: 'imports',
                element: <ImportHistoryPage />,
            },
            {
                path: 'manufacturers',
                element: <ManufacturersPage />,
            },
        ],
    },
    {
        path: '*',
        element: <Navigate to="/dashboard" replace />,
    },
]);
