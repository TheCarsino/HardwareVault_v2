// components/devices/DeviceFilters.tsx
import React, { useState, useEffect } from 'react';
import {
    Box,
    Card,
    CardContent,
    TextField,
    MenuItem,
    Button,
    Typography,
    Slider,
} from '@mui/material';
import { FilterList, Clear } from '@mui/icons-material';
import { DeviceFilters as IDeviceFilters } from '../../types/device';
import { CPU_MANUFACTURERS, GPU_MANUFACTURERS, STORAGE_TYPES, SEARCH_DEBOUNCE_MS } from '../../utils/constants';
import { useAppDispatch, useAppSelector } from '../../app/hooks';
import { setFilters, clearFilters } from '../../features/devices/devicesSlice';

export const DeviceFilters: React.FC = () => {
    const dispatch = useAppDispatch();
    const currentFilters = useAppSelector((state) => state.devices.filters);

    const [localSearch, setLocalSearch] = useState(currentFilters.search || '');

    // Debounce search input
    useEffect(() => {
        const timer = setTimeout(() => {
            if (localSearch !== currentFilters.search) {
                dispatch(setFilters({ search: localSearch || undefined }));
            }
        }, SEARCH_DEBOUNCE_MS);

        return () => clearTimeout(timer);
    }, [localSearch, currentFilters.search, dispatch]);

    const handleFilterChange = (key: keyof IDeviceFilters, value: string | number | undefined) => {
        dispatch(setFilters({ [key]: value || undefined }));
    };

    const handleClearFilters = () => {
        setLocalSearch('');
        dispatch(clearFilters());
    };

    const hasActiveFilters =
        currentFilters.cpuManufacturer ||
        currentFilters.gpuManufacturer ||
        currentFilters.storageType ||
        currentFilters.minRamInGB ||
        currentFilters.search;

    return (
        <Card sx={{ mb: 3 }}>
            <CardContent>
                <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 2 }}>
                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                        <FilterList />
                        <Typography variant="h6">Filters</Typography>
                    </Box>
                    {hasActiveFilters && (
                        <Button
                            size="small"
                            startIcon={<Clear />}
                            onClick={handleClearFilters}
                            color="inherit"
                        >
                            Clear All
                        </Button>
                    )}
                </Box>

                <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', md: 'repeat(2, 1fr)', lg: 'repeat(4, 1fr)' }, gap: 2 }}>
                    <TextField
                        label="Search"
                        placeholder="CPU or GPU model..."
                        value={localSearch}
                        onChange={(e) => setLocalSearch(e.target.value)}
                        size="small"
                        fullWidth
                    />

                    <TextField
                        select
                        label="CPU Manufacturer"
                        value={currentFilters.cpuManufacturer || ''}
                        onChange={(e) => handleFilterChange('cpuManufacturer', e.target.value)}
                        size="small"
                        fullWidth
                    >
                        <MenuItem value="">All</MenuItem>
                        {CPU_MANUFACTURERS.map((manufacturer) => (
                            <MenuItem key={manufacturer} value={manufacturer}>
                                {manufacturer}
                            </MenuItem>
                        ))}
                    </TextField>

                    <TextField
                        select
                        label="GPU Manufacturer"
                        value={currentFilters.gpuManufacturer || ''}
                        onChange={(e) => handleFilterChange('gpuManufacturer', e.target.value)}
                        size="small"
                        fullWidth
                    >
                        <MenuItem value="">All</MenuItem>
                        {GPU_MANUFACTURERS.map((manufacturer) => (
                            <MenuItem key={manufacturer} value={manufacturer}>
                                {manufacturer}
                            </MenuItem>
                        ))}
                    </TextField>

                    <TextField
                        select
                        label="Storage Type"
                        value={currentFilters.storageType || ''}
                        onChange={(e) => handleFilterChange('storageType', e.target.value as 'SSD' | 'HDD' | undefined)}
                        size="small"
                        fullWidth
                    >
                        <MenuItem value="">All</MenuItem>
                        {STORAGE_TYPES.map((type) => (
                            <MenuItem key={type} value={type}>
                                {type}
                            </MenuItem>
                        ))}
                    </TextField>
                </Box>

                <Box sx={{ mt: 3 }}>
                    <Typography variant="body2" gutterBottom>
                        Minimum RAM: {currentFilters.minRamInGB || 0} GB
                    </Typography>
                    <Slider
                        value={currentFilters.minRamInGB || 0}
                        onChange={(_, value) => handleFilterChange('minRamInGB', value as number)}
                        min={0}
                        max={64}
                        step={4}
                        marks={[
                            { value: 0, label: '0 GB' },
                            { value: 16, label: '16 GB' },
                            { value: 32, label: '32 GB' },
                            { value: 64, label: '64 GB' },
                        ]}
                        valueLabelDisplay="auto"
                    />
                </Box>
            </CardContent>
        </Card>
    );
};
