// components/common/StatusBadge.tsx
import React from 'react';
import { Chip } from '@mui/material';
import { ImportStatus } from '../../types/import';

const STATUS_CONFIG: Record<ImportStatus, { color: 'success' | 'error' | 'warning' | 'default'; label: string }> = {
    Completed: { color: 'success', label: 'Completed' },
    Failed: { color: 'error', label: 'Failed' },
    Processing: { color: 'warning', label: 'Processing' },
    Pending: { color: 'default', label: 'Pending' },
};

interface StatusBadgeProps {
    status: ImportStatus;
    size?: 'small' | 'medium';
}

export const StatusBadge: React.FC<StatusBadgeProps> = ({ status, size = 'small' }) => {
    const config = STATUS_CONFIG[status];

    return (
        <Chip
            size={size}
            color={config.color}
            label={config.label}
            sx={{ fontWeight: 500 }}
        />
    );
};
