// pages/DevicesPage.tsx
import React, { useState } from 'react';
import { Container, Box, Chip } from '@mui/material';
import { Upload } from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';
import { PageHeader } from '../components/common/PageHeader';
import { DeviceFilters } from '../components/devices/DeviceFilters';
import { DeviceTable } from '../components/devices/DeviceTable';
import { DeviceDetailPanel } from '../components/devices/DeviceDetailPanel';
import { DeviceDeleteDialog } from '../components/devices/DeviceDeleteDialog';
import { EmptyState } from '../components/common/EmptyState';
import { useGetDevicesQuery, useDeleteDeviceMutation } from '../api/deviceApi';
import { useAppSelector } from '../app/hooks';
import { Device } from '../types/device';
import { useSnackbar } from 'notistack';

export const DevicesPage: React.FC = () => {
    const navigate = useNavigate();
    const { enqueueSnackbar } = useSnackbar();
    const filters = useAppSelector((state) => state.devices.filters);
    const { data, isLoading, error } = useGetDevicesQuery(filters);
    const [deleteDevice] = useDeleteDeviceMutation();

    const [selectedDevice, setSelectedDevice] = useState<Device | null>(null);
    const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
    const [deviceToDelete, setDeviceToDelete] = useState<string | null>(null);

    const handleEdit = (device: Device) => {
        setSelectedDevice(device);
    };

    const handleDelete = (deviceId: string) => {
        setDeviceToDelete(deviceId);
        setDeleteDialogOpen(true);
    };

    const handleConfirmDelete = async () => {
        if (!deviceToDelete) return;

        try {
            await deleteDevice(deviceToDelete).unwrap();
            enqueueSnackbar('Device deleted successfully', { variant: 'success' });
        } catch (err) {
            enqueueSnackbar('Failed to delete device', { variant: 'error' });
        } finally {
            setDeleteDialogOpen(false);
            setDeviceToDelete(null);
        }
    };

    const handleCancelDelete = () => {
        setDeleteDialogOpen(false);
        setDeviceToDelete(null);
    };

    return (
        <Container maxWidth="xl" sx={{ py: 4 }}>
            <PageHeader
                title="Hardware Inventory"
                subtitle="Manage and view all your device records"
                action={{
                    label: 'Import Devices',
                    onClick: () => navigate('/import'),
                    icon: <Upload />,
                }}
            >
                {data && (
                    <Box sx={{ mt: 2 }}>
                        <Chip label={`${data.totalCount} total devices`} color="primary" />
                    </Box>
                )}
            </PageHeader>

            <DeviceFilters />

            {!isLoading && data && data.totalCount === 0 ? (
                <EmptyState
                    title="No devices found"
                    message="Import your first Excel file to get started, or adjust your filters."
                    action={{
                        label: 'Import Devices',
                        onClick: () => navigate('/import'),
                    }}
                />
            ) : (
                <DeviceTable
                    devices={data?.data || []}
                    totalCount={data?.totalCount || 0}
                    loading={isLoading}
                    onEdit={handleEdit}
                    onDelete={handleDelete}
                />
            )}

            <DeviceDetailPanel
                open={!!selectedDevice}
                deviceId={selectedDevice?.deviceId || null}
                onClose={() => setSelectedDevice(null)}
            />

            <DeviceDeleteDialog
                open={deleteDialogOpen}
                deviceId={deviceToDelete}
                onConfirm={handleConfirmDelete}
                onCancel={handleCancelDelete}
            />
        </Container>
    );
};
