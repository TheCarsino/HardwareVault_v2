// components/devices/DeviceTable.tsx
import React from 'react';
import { DataGrid, GridColDef, GridPaginationModel } from '@mui/x-data-grid';
import { Box, Chip, IconButton, Tooltip } from '@mui/material';
import { Edit, Delete, Memory, CalendarToday, Update } from '@mui/icons-material';
import { Device } from '../../types/device';
import { ramToGB, formatStorage, formatWeight, formatWatts, formatUsbPorts, formatDate } from '../../utils/formatters';
import { useAppDispatch, useAppSelector } from '../../app/hooks';
import { setPage, setPageSize } from '../../features/devices/devicesSlice';

interface DeviceTableProps {
    devices: Device[];
    totalCount: number;
    loading: boolean;
    onEdit: (device: Device) => void;
    onDelete: (deviceId: string) => void;
}

export const DeviceTable: React.FC<DeviceTableProps> = ({
    devices,
    totalCount,
    loading,
    onEdit,
    onDelete,
}) => {
    const dispatch = useAppDispatch();
    const { page, pageSize } = useAppSelector((state) => state.devices.filters);

    const getManufacturerColor = (manufacturerName: string): { bgcolor: string; color: string } => {
        const lowerName = manufacturerName.toLowerCase();
        if (lowerName.includes('intel')) {
            return { bgcolor: '#E3F2FD', color: '#1976D2' }; // Blue
        } else if (lowerName.includes('amd')) {
            return { bgcolor: '#FFEBEE', color: '#D32F2F' }; // Red
        } else if (lowerName.includes('nvidia')) {
            return { bgcolor: '#E8F5E9', color: '#388E3C' }; // Green
        }
        return { bgcolor: '#F5F5F5', color: '#757575' }; // Default gray
    };

    const columns: GridColDef<Device>[] = [
        {
            field: 'cpu',
            headerName: 'CPU',
            flex: 1,
            minWidth: 200,
            renderCell: (params) => {
                const colors = getManufacturerColor(params.row.cpu.manufacturer.name);
                return (
                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                        <Chip
                            label={params.row.cpu.manufacturer.name}
                            size="small"
                            sx={{
                                bgcolor: colors.bgcolor,
                                color: colors.color,
                                fontWeight: 500
                            }}
                        />
                        <Box sx={{ fontWeight: 500 }}>{params.row.cpu.modelName}</Box>
                    </Box>
                );
            },
        },
        {
            field: 'gpu',
            headerName: 'GPU',
            flex: 1,
            minWidth: 200,
            renderCell: (params) => {
                const colors = getManufacturerColor(params.row.gpu.manufacturer.name);
                return (
                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                        <Chip
                            label={params.row.gpu.manufacturer.name}
                            size="small"
                            sx={{
                                bgcolor: colors.bgcolor,
                                color: colors.color,
                                fontWeight: 500
                            }}
                        />
                        <Box sx={{ fontWeight: 500 }}>{params.row.gpu.modelName}</Box>
                    </Box>
                );
            },
        },
        {
            field: 'ram',
            headerName: 'RAM',
            width: 120,
            valueGetter: (value, row) => row.ramSizeInMB,
            renderCell: (params) => (
                <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
                    <Memory fontSize="small" sx={{ color: 'text.secondary' }} />
                    {ramToGB(params.row.ramSizeInMB)}
                </Box>
            ),
        },
        {
            field: 'storage',
            headerName: 'STORAGE',
            width: 150,
            renderCell: (params) => formatStorage(params.row.storageSizeInGB, params.row.storageType),
        },
        {
            field: 'usbPorts',
            headerName: 'USB PORTS',
            width: 120,
            renderCell: (params) => {
                const totalPorts = params.row.usbPorts.reduce((sum, port) => sum + port.count, 0);
                return formatUsbPorts(totalPorts);
            },
        },
        {
            field: 'weight',
            headerName: 'WEIGHT',
            width: 120,
            renderCell: (params) => formatWeight(params.row.weightInKg),
        },
        {
            field: 'powerSupply',
            headerName: 'PSU',
            width: 100,
            renderCell: (params) => formatWatts(params.row.powerSupply.wattageInWatts),
        },
        {
            field: 'createdAt',
            headerName: 'CREATED',
            width: 150,
            renderCell: (params) => (
                <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
                    <CalendarToday fontSize="small" sx={{ color: 'text.secondary' }} />
                    {formatDate(params.row.createdAt)}
                </Box>
            ),
        },
        {
            field: 'updatedAt',
            headerName: 'UPDATED',
            width: 150,
            renderCell: (params) => (
                <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
                    <Update fontSize="small" sx={{ color: 'text.secondary' }} />
                    {formatDate(params.row.updatedAt)}
                </Box>
            ),
        },
        {
            field: 'actions',
            headerName: 'ACTIONS',
            width: 120,
            sortable: false,
            filterable: false,
            renderCell: (params) => (
                <Box>
                    <Tooltip title="Edit">
                        <IconButton
                            size="small"
                            onClick={() => onEdit(params.row)}
                            aria-label="Edit device"
                        >
                            <Edit fontSize="small" />
                        </IconButton>
                    </Tooltip>
                    <Tooltip title="Delete">
                        <IconButton
                            size="small"
                            onClick={() => onDelete(params.row.deviceId)}
                            aria-label="Delete device"
                            color="error"
                        >
                            <Delete fontSize="small" />
                        </IconButton>
                    </Tooltip>
                </Box>
            ),
        },
    ];

    const handlePaginationModelChange = (model: GridPaginationModel) => {
        if (model.page + 1 !== page) {
            dispatch(setPage(model.page + 1));
        }
        if (model.pageSize !== pageSize) {
            dispatch(setPageSize(model.pageSize));
        }
    };

    return (
        <Box sx={{ height: 600, width: '100%' }}>
            <DataGrid
                rows={devices}
                columns={columns}
                getRowId={(row) => row.deviceId}
                rowCount={totalCount}
                loading={loading}
                paginationMode="server"
                paginationModel={{ page: page - 1, pageSize }}
                onPaginationModelChange={handlePaginationModelChange}
                pageSizeOptions={[10, 20, 50]}
                disableRowSelectionOnClick
                sx={{
                    '& .MuiDataGrid-cell': {
                        py: 1.5,
                    },
                }}
            />
        </Box>
    );
};
