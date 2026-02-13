// routes/RequireAuth.tsx
import React from 'react';
import { Navigate, Outlet } from 'react-router-dom';
import { Box, AppBar, Toolbar, Typography, Button, Container } from '@mui/material';
import { Logout, Dashboard, Devices, Upload, History, Business } from '@mui/icons-material';
import { Link as RouterLink, useNavigate, useLocation } from 'react-router-dom';

export const RequireAuth: React.FC = () => {
    const navigate = useNavigate();
    const location = useLocation();
    const token = localStorage.getItem('HardwareVault_token');

    if (!token) {
        return <Navigate to="/login" state={{ from: location }} replace />;
    }

    const handleLogout = () => {
        localStorage.removeItem('HardwareVault_token');
        localStorage.removeItem('HardwareVault_user');
        navigate('/login');
    };

    const isActive = (path: string) => location.pathname === path;

    return (
        <Box sx={{ display: 'flex', flexDirection: 'column', minHeight: '100vh' }}>
            <AppBar position="static" elevation={0} sx={{ borderBottom: 1, borderColor: 'divider' }}>
                <Toolbar>
                    <Typography
                        variant="h6"
                        component={RouterLink}
                        to="/dashboard"
                        sx={{
                            flexGrow: 1,
                            textDecoration: 'none',
                            color: 'inherit',
                            fontWeight: 700,
                        }}
                    >
                        HardwareVault
                    </Typography>

                    <Box sx={{ display: 'flex', gap: 1, mr: 2 }}>
                        <Button
                            component={RouterLink}
                            to="/dashboard"
                            color="inherit"
                            startIcon={<Dashboard />}
                            sx={{
                                fontWeight: isActive('/dashboard') ? 600 : 400,
                                borderBottom: isActive('/dashboard') ? 2 : 0,
                                borderRadius: 0,
                            }}
                        >
                            Dashboard
                        </Button>
                        <Button
                            component={RouterLink}
                            to="/devices"
                            color="inherit"
                            startIcon={<Devices />}
                            sx={{
                                fontWeight: isActive('/devices') ? 600 : 400,
                                borderBottom: isActive('/devices') ? 2 : 0,
                                borderRadius: 0,
                            }}
                        >
                            Devices
                        </Button>
                        <Button
                            component={RouterLink}
                            to="/import"
                            color="inherit"
                            startIcon={<Upload />}
                            sx={{
                                fontWeight: isActive('/import') ? 600 : 400,
                                borderBottom: isActive('/import') ? 2 : 0,
                                borderRadius: 0,
                            }}
                        >
                            Import
                        </Button>
                        <Button
                            component={RouterLink}
                            to="/imports"
                            color="inherit"
                            startIcon={<History />}
                            sx={{
                                fontWeight: isActive('/imports') ? 600 : 400,
                                borderBottom: isActive('/imports') ? 2 : 0,
                                borderRadius: 0,
                            }}
                        >
                            History
                        </Button>
                        <Button
                            component={RouterLink}
                            to="/manufacturers"
                            color="inherit"
                            startIcon={<Business />}
                            sx={{
                                fontWeight: isActive('/manufacturers') ? 600 : 400,
                                borderBottom: isActive('/manufacturers') ? 2 : 0,
                                borderRadius: 0,
                            }}
                        >
                            Manufacturers
                        </Button>
                    </Box>

                    <Button color="inherit" onClick={handleLogout} startIcon={<Logout />}>
                        Logout
                    </Button>
                </Toolbar>
            </AppBar>

            <Box component="main" sx={{ flexGrow: 1, backgroundColor: 'background.default' }}>
                <Outlet />
            </Box>

            <Box
                component="footer"
                sx={{
                    py: 3,
                    px: 2,
                    mt: 'auto',
                    backgroundColor: 'background.paper',
                    borderTop: 1,
                    borderColor: 'divider',
                }}
            >
                <Container maxWidth="xl">
                    <Typography variant="body2" color="text.secondary" align="center">
                        © {new Date().getFullYear()} HardwareVault. All rights reserved.
                    </Typography>
                </Container>
            </Box>
        </Box>
    );
};
