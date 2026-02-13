// pages/LoginPage.tsx
import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {
    Box,
    Card,
    CardContent,
    TextField,
    Button,
    Typography,
    Alert,
} from '@mui/material';
import { useSnackbar } from 'notistack';

export const LoginPage: React.FC = () => {
    const navigate = useNavigate();
    const { enqueueSnackbar } = useSnackbar();
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError('');
        setLoading(true);

        // TODO: Implement actual authentication and login // Provide Auth Methods on .NET
        // Dummy Login
        setTimeout(() => {
            if (email && password) {
                localStorage.setItem('HardwareVault_token', 'dummy-token');
                localStorage.setItem('HardwareVault_user', JSON.stringify({ email }));
                enqueueSnackbar('Login successful', { variant: 'success' });
                navigate('/dashboard');
            } else {
                setError('Please enter both email and password');
            }
            setLoading(false);
        }, 1000);
    };

    return (
        <Box
            sx={{
                minHeight: '100vh',
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                backgroundColor: 'background.default',
            }}
        >
            <Card sx={{ maxWidth: 400, width: '100%', mx: 2 }}>
                <CardContent sx={{ p: 4 }}>
                    <Box sx={{ mb: 4, textAlign: 'center' }}>
                        <Typography variant="h4" component="h1" gutterBottom fontWeight={700}>
                            HardwareVault
                        </Typography>
                        <Typography variant="body2" color="text.secondary">
                            Centralize your hardware inventory
                        </Typography>
                    </Box>

                    {error && (
                        <Alert severity="error" sx={{ mb: 2 }}>
                            {error}
                        </Alert>
                    )}

                    <form onSubmit={handleSubmit}>
                        <TextField
                            label="Email"
                            type="email"
                            fullWidth
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            sx={{ mb: 2 }}
                            autoFocus
                        />
                        <TextField
                            label="Password"
                            type="password"
                            fullWidth
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            sx={{ mb: 3 }}
                        />
                        <Button
                            type="submit"
                            variant="contained"
                            fullWidth
                            size="large"
                            disabled={loading}
                        >
                            {loading ? 'Signing in...' : 'Sign In'}
                        </Button>
                    </form>
                </CardContent>
            </Card>
        </Box>
    );
};
