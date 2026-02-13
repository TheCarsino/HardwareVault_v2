// components/devices/DeviceTable.tsx
import React from 'react';
import { DataGrid, GridColDef, GridPaginationModel } from '@mui/x-data-grid';
import { Box, Chip, IconButton, Tooltip } from '@mui/material';
import { Edit, Delete } from '@mui/icons-material';
import { Device } from '../../types/device';
import { ramToGB, formatStorage, formatWeight, formatWatts, formatUsbPorts } from '../../utils/formatters';
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

    const columns: GridColDef<Device>[] = [
        {
            field: 'cpu',
            headerName: 'CPU',
            flex: 1,
            minWidth: 200,
            renderCell: (params) => (
                <Box>
                    <Box sx={{ fontWeight: 500 }}>{params.row.cpu.modelName}</Box>
                    <Chip
                        label={params.row.cpu.manufacturer.name}
                        size="small"
                        sx={{ mt: 0.5 }}
                    />
                </Box>
            ),
        },
        {
            field: 'gpu',
            headerName: 'GPU',
            flex: 1,
            minWidth: 200,
            renderCell: (params) => (
                <Box>
                    <Box sx={{ fontWeight: 500 }}>{params.row.gpu.modelName}</Box>
                    <Chip
                        label={params.row.gpu.manufacturer.name}
                        size="small"
                        sx={{ mt: 0.5 }}
                    />
                </Box>
            ),
        },
        {
            field: 'ram',
            headerName: 'RAM',
            width: 120,
            valueGetter: (params) => params.row.ramSizeInMB,
            renderCell: (params) => ramToGB(params.row.ramSizeInMB),
        },
        {
            field: 'storage',
            headerName: 'Storage',
            width: 150,
            renderCell: (params) => formatStorage(params.row.storageSizeInGB, params.row.storageType),
        },
        {
            field: 'usbPorts',
            headerName: 'USB Ports',
            width: 120,
            renderCell: (params) => {
                const totalPorts = params.row.usbPorts.reduce((sum, port) => sum + port.portCount, 0);
                return formatUsbPorts(totalPorts);
            },
        },
        {
            field: 'weight',
            headerName: 'Weight',
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
            field: 'actions',
            headerName: 'Actions',
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
