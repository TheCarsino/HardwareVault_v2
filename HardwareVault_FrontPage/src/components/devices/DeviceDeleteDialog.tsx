// components/devices/DeviceDeleteDialog.tsx
import React from 'react';
import { ConfirmDialog } from '../common/ConfirmDialog';

interface DeviceDeleteDialogProps {
    open: boolean;
    deviceId: string | null;
    onConfirm: () => void;
    onCancel: () => void;
}

export const DeviceDeleteDialog: React.FC<DeviceDeleteDialogProps> = ({
    open,
    deviceId,
    onConfirm,
    onCancel,
}) => {
    return (
        <ConfirmDialog
            open={open}
            title="Delete Device"
            message="Are you sure you want to delete this device? This action cannot be undone."
            confirmLabel="Delete"
            cancelLabel="Cancel"
            onConfirm={onConfirm}
            onCancel={onCancel}
            severity="error"
        />
    );
};
