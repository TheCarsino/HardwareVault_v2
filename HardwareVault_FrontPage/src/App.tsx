// App.tsx
import React from 'react';
import { RouterProvider } from 'react-router-dom';
import { Provider } from 'react-redux';
import { ThemeProvider, CssBaseline } from '@mui/material';
import { SnackbarProvider } from 'notistack';
import { store } from './app/store';
import { theme } from './theme/theme';
import { router } from './routes/AppRoutes';

export const App: React.FC = () => {
    return (
        <Provider store={store}>
            <ThemeProvider theme={theme}>
                <CssBaseline />
                <SnackbarProvider
                    maxSnack={3}
                    anchorOrigin={{
                        vertical: 'bottom',
                        horizontal: 'right',
                    }}
                    autoHideDuration={5000}
                >
                    <RouterProvider router={router} />
                </SnackbarProvider>
            </ThemeProvider>
        </Provider>
    );
};
