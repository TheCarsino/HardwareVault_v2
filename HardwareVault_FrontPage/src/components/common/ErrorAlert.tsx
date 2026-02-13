// components/common/ErrorAlert.tsx
import React from 'react';
import { Alert, AlertTitle, Box } from '@mui/material';

interface ErrorAlertProps {
    title?: string;
    message: string;
    onRetry?: () => void;
}

export const ErrorAlert: React.FC<ErrorAlertProps> = ({
    title = 'Error',
    message,
    onRetry,
}) => {
    return (
        <Box sx={{ my: 2 }}>
            <Alert severity="error">
                <AlertTitle>{title}</AlertTitle>
                {message}
            </Alert>
        </Box>
    );
};
