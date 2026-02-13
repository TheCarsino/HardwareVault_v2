// components/devices/DeviceDetailPanel.tsx
import React, { useEffect } from 'react';
import {
    Drawer,
    Box,
    Typography,
    IconButton,
    Divider,
    TextField,
    Button,
    Grid,
    MenuItem,
    Alert,
} from '@mui/material';
import { Close } from '@mui/icons-material';
import { useForm, Controller, useFieldArray } from 'react-hook-form';
import { Device, UpdateDeviceDto, CreateUsbPortDto } from '../../types/device';
import { useUpdateDeviceMutation, useGetDeviceByIdQuery } from '../../api/deviceApi';
import { useGetManufacturersQuery } from '../../api/manufacturerApi';
import { LoadingOverlay } from '../common/LoadingOverlay';
import { STORAGE_TYPES, USB_PORT_TYPES } from '../../utils/constants';
import { formatDateTime } from '../../utils/formatters';
import { useSnackbar } from 'notistack';

interface DeviceDetailPanelProps {
    open: boolean;
    deviceId: string | null;
    onClose: () => void;
}

interface FormValues {
    ramSizeInMB: number;
    storageSizeInGB: number;
    storageType: 'SSD' | 'HDD';
    weightInKg: number;
    cpuId: number;
    gpuId: number;
    powerSupplyWattageInWatts: number;
    usbPorts: CreateUsbPortDto[];
}

export const DeviceDetailPanel: React.FC<DeviceDetailPanelProps> = ({
    open,
    deviceId,
    onClose,
}) => {
    const { enqueueSnackbar } = useSnackbar();
    const { data: device, isLoading, error } = useGetDeviceByIdQuery(deviceId || '', {
        skip: !deviceId,
    });
    const { data: manufacturers = [] } = useGetManufacturersQuery();
    const [updateDevice, { isLoading: isUpdating }] = useUpdateDeviceMutation();

    const { control, handleSubmit, reset, formState: { errors, isDirty } } = useForm<FormValues>({
        defaultValues: {
            ramSizeInMB: 0,
            storageSizeInGB: 0,
            storageType: 'SSD',
            weightInKg: 0,
            cpuId: 0,
            gpuId: 0,
            powerSupplyWattageInWatts: 0,
            usbPorts: [],
        },
    });

    const { fields, append, remove } = useFieldArray({
        control,
        name: 'usbPorts',
    });

    useEffect(() => {
        if (device) {
            reset({
                ramSizeInMB: device.ramSizeInMB,
                storageSizeInGB: device.storageSizeInGB,
                storageType: device.storageType,
                weightInKg: device.weightInKg,
                cpuId: device.cpu.cpuId,
                gpuId: device.gpu.gpuId,
                powerSupplyWattageInWatts: device.powerSupply.wattageInWatts,
                usbPorts: device.usbPorts.map(port => ({
                    usbPortType: port.usbPortType,
                    portCount: port.portCount,
                })),
            });
        }
    }, [device, reset]);

    const cpuManufacturers = manufacturers.filter(m => m.type === 'CPU' || m.type === 'Both');
    const gpuManufacturers = manufacturers.filter(m => m.type === 'GPU' || m.type === 'Both');

    const onSubmit = async (data: FormValues) => {
        if (!deviceId) return;

        try {
            await updateDevice({
                id: deviceId,
                body: data,
            }).unwrap();

            enqueueSnackbar('Device updated successfully', { variant: 'success' });
            onClose();
        } catch (err) {
            enqueueSnackbar('Failed to update device', { variant: 'error' });
        }
    };

    return (
        <Drawer
            anchor="right"
            open={open}
            onClose={onClose}
            sx={{ '& .MuiDrawer-paper': { width: { xs: '100%', sm: 600 } } }}
        >
            <Box sx={{ p: 3 }}>
                <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 2 }}>
                    <Typography variant="h5">Edit Device</Typography>
                    <IconButton onClick={onClose} aria-label="Close">
                        <Close />
                    </IconButton>
                </Box>

                <Divider sx={{ mb: 3 }} />

                {isLoading && <LoadingOverlay />}
                {error && <Alert severity="error">Failed to load device</Alert>}

                {device && (
                    <form onSubmit={handleSubmit(onSubmit)}>
                        <Grid container spacing={2}>
                            <Grid item xs={12}>
                                <Controller
                                    name="cpuId"
                                    control={control}
                                    rules={{ required: 'CPU is required' }}
                                    render={({ field }) => (
                                        <TextField
                                            {...field}
                                            select
                                            label="CPU"
                                            fullWidth
                                            error={!!errors.cpuId}
                                            helperText={errors.cpuId?.message}
                                        >
                                            {cpuManufacturers.map((manufacturer) => (
                                                <MenuItem key={manufacturer.manufacturerId} value={manufacturer.manufacturerId}>
                                                    {manufacturer.name}
                                                </MenuItem>
                                            ))}
                                        </TextField>
                                    )}
                                />
                            </Grid>

                            <Grid item xs={12}>
                                <Controller
                                    name="gpuId"
                                    control={control}
                                    rules={{ required: 'GPU is required' }}
                                    render={({ field }) => (
                                        <TextField
                                            {...field}
                                            select
                                            label="GPU"
                                            fullWidth
                                            error={!!errors.gpuId}
                                            helperText={errors.gpuId?.message}
                                        >
                                            {gpuManufacturers.map((manufacturer) => (
                                                <MenuItem key={manufacturer.manufacturerId} value={manufacturer.manufacturerId}>
                                                    {manufacturer.name}
                                                </MenuItem>
                                            ))}
                                        </TextField>
                                    )}
                                />
                            </Grid>

                            <Grid item xs={6}>
                                <Controller
                                    name="ramSizeInMB"
                                    control={control}
                                    rules={{ required: 'RAM is required', min: { value: 1, message: 'Must be > 0' } }}
                                    render={({ field }) => (
                                        <TextField
                                            {...field}
                                            type="number"
                                            label="RAM (MB)"
                                            fullWidth
                                            error={!!errors.ramSizeInMB}
                                            helperText={errors.ramSizeInMB?.message}
                                            onChange={(e) => field.onChange(Number(e.target.value))}
                                        />
                                    )}
                                />
                            </Grid>

                            <Grid item xs={6}>
                                <Controller
                                    name="storageSizeInGB"
                                    control={control}
                                    rules={{ required: 'Storage is required', min: { value: 1, message: 'Must be > 0' } }}
                                    render={({ field }) => (
                                        <TextField
                                            {...field}
                                            type="number"
                                            label="Storage (GB)"
                                            fullWidth
                                            error={!!errors.storageSizeInGB}
                                            helperText={errors.storageSizeInGB?.message}
                                            onChange={(e) => field.onChange(Number(e.target.value))}
                                        />
                                    )}
                                />
                            </Grid>

                            <Grid item xs={12}>
                                <Controller
                                    name="storageType"
                                    control={control}
                                    rules={{ required: 'Storage type is required' }}
                                    render={({ field }) => (
                                        <TextField
                                            {...field}
                                            select
                                            label="Storage Type"
                                            fullWidth
                                            error={!!errors.storageType}
                                            helperText={errors.storageType?.message}
                                        >
                                            {STORAGE_TYPES.map((type) => (
                                                <MenuItem key={type} value={type}>
                                                    {type}
                                                </MenuItem>
                                            ))}
                                        </TextField>
                                    )}
                                />
                            </Grid>

                            <Grid item xs={6}>
                                <Controller
                                    name="weightInKg"
                                    control={control}
                                    rules={{ required: 'Weight is required', min: { value: 0.1, message: 'Must be > 0' } }}
                                    render={({ field }) => (
                                        <TextField
                                            {...field}
                                            type="number"
                                            label="Weight (kg)"
                                            fullWidth
                                            inputProps={{ step: 0.1 }}
                                            error={!!errors.weightInKg}
                                            helperText={errors.weightInKg?.message}
                                            onChange={(e) => field.onChange(Number(e.target.value))}
                                        />
                                    )}
                                />
                            </Grid>

                            <Grid item xs={6}>
                                <Controller
                                    name="powerSupplyWattageInWatts"
                                    control={control}
                                    rules={{ required: 'PSU wattage is required', min: { value: 1, message: 'Must be > 0' } }}
                                    render={({ field }) => (
                                        <TextField
                                            {...field}
                                            type="number"
                                            label="PSU (W)"
                                            fullWidth
                                            error={!!errors.powerSupplyWattageInWatts}
                                            helperText={errors.powerSupplyWattageInWatts?.message}
                                            onChange={(e) => field.onChange(Number(e.target.value))}
                                        />
                                    )}
                                />
                            </Grid>

                            <Grid item xs={12}>
                                <Typography variant="subtitle2" gutterBottom>
                                    USB Ports
                                </Typography>
                                {fields.map((field, index) => (
                                    <Box key={field.id} sx={{ display: 'flex', gap: 1, mb: 1 }}>
                                        <Controller
                                            name={`usbPorts.${index}.usbPortType`}
                                            control={control}
                                            rules={{ required: 'Port type is required' }}
                                            render={({ field }) => (
                                                <TextField
                                                    {...field}
                                                    select
                                                    label="Port Type"
                                                    size="small"
                                                    sx={{ flex: 2 }}
                                                >
                                                    {USB_PORT_TYPES.map((type) => (
                                                        <MenuItem key={type} value={type}>
                                                            {type}
                                                        </MenuItem>
                                                    ))}
                                                </TextField>
                                            )}
                                        />
                                        <Controller
                                            name={`usbPorts.${index}.portCount`}
                                            control={control}
                                            rules={{ required: 'Count is required', min: 1 }}
                                            render={({ field }) => (
                                                <TextField
                                                    {...field}
                                                    type="number"
                                                    label="Count"
                                                    size="small"
                                                    sx={{ flex: 1 }}
                                                    onChange={(e) => field.onChange(Number(e.target.value))}
                                                />
                                            )}
                                        />
                                        <Button onClick={() => remove(index)} color="error" size="small">
                                            Remove
                                        </Button>
                                    </Box>
                                ))}
                                <Button onClick={() => append({ usbPortType: 'USB 3.0', portCount: 1 })} size="small">
                                    Add USB Port
                                </Button>
                            </Grid>

                            <Grid item xs={12}>
                                <Divider sx={{ my: 2 }} />
                                <Typography variant="caption" color="text.secondary">
                                    Created: {formatDateTime(device.createdAt)}
                                </Typography>
                                <br />
                                <Typography variant="caption" color="text.secondary">
                                    Updated: {formatDateTime(device.updatedAt)}
                                </Typography>
                            </Grid>
                        </Grid>

                        <Box sx={{ display: 'flex', gap: 2, mt: 3 }}>
                            <Button
                                type="submit"
                                variant="contained"
                                disabled={!isDirty || isUpdating}
                                fullWidth
                            >
                                {isUpdating ? 'Saving...' : 'Save Changes'}
                            </Button>
                            <Button onClick={onClose} variant="outlined" fullWidth>
                                Cancel
                            </Button>
                        </Box>
                    </form>
                )}
            </Box>
        </Drawer>
    );
};
