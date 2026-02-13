// components/common/PageHeader.tsx
import React from 'react';
import { Box, Typography, Button, Breadcrumbs, Link } from '@mui/material';
import { Link as RouterLink } from 'react-router-dom';

interface BreadcrumbItem {
    label: string;
    path?: string;
}

interface PageHeaderProps {
    title: string;
    subtitle?: string;
    breadcrumbs?: BreadcrumbItem[];
    action?: {
        label: string;
        onClick: () => void;
        icon?: React.ReactNode;
        variant?: 'contained' | 'outlined' | 'text';
    };
    children?: React.ReactNode;
}

export const PageHeader: React.FC<PageHeaderProps> = ({
    title,
    subtitle,
    breadcrumbs,
    action,
    children,
}) => {
    return (
        <Box sx={{ mb: 4 }}>
            {breadcrumbs && breadcrumbs.length > 0 && (
                <Breadcrumbs sx={{ mb: 1 }}>
                    {breadcrumbs.map((crumb, index) => {
                        const isLast = index === breadcrumbs.length - 1;
                        return isLast || !crumb.path ? (
                            <Typography key={index} color="text.secondary" fontSize="0.875rem">
                                {crumb.label}
                            </Typography>
                        ) : (
                            <Link
                                key={index}
                                component={RouterLink}
                                to={crumb.path}
                                underline="hover"
                                color="inherit"
                                fontSize="0.875rem"
                            >
                                {crumb.label}
                            </Link>
                        );
                    })}
                </Breadcrumbs>
            )}

            <Box sx={{ display: 'flex', alignItems: 'flex-start', justifyContent: 'space-between' }}>
                <Box sx={{ flex: 1 }}>
                    <Typography variant="h4" component="h1" gutterBottom>
                        {title}
                    </Typography>
                    {subtitle && (
                        <Typography variant="body1" color="text.secondary">
                            {subtitle}
                        </Typography>
                    )}
                </Box>

                {action && (
                    <Button
                        variant={action.variant || 'contained'}
                        onClick={action.onClick}
                        startIcon={action.icon}
                        sx={{ ml: 2, flexShrink: 0 }}
                    >
                        {action.label}
                    </Button>
                )}
            </Box>

            {children}
        </Box>
    );
};
