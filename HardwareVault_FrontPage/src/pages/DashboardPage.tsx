// pages/DashboardPage.tsx
import React from 'react';
import { Container, Grid, Box, Button } from '@mui/material';
import { Devices, CheckCircle, Upload, CalendarToday } from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';
import { PageHeader } from '../components/common/PageHeader';
import { StatCard } from '../components/dashboard/StatCard';
import { ManufacturerChart } from '../components/dashboard/ManufacturerChart';
import { StorageTypeChart } from '../components/dashboard/StorageTypeChart';
import { LoadingOverlay } from '../components/common/LoadingOverlay';
import { ErrorAlert } from '../components/common/ErrorAlert';
import { useGetStatisticsQuery } from '../api/deviceApi';
import { useGetRecentImportJobsQuery } from '../api/importApi';
import { formatDate } from '../utils/formatters';
import { StatusBadge } from '../components/common/StatusBadge';
import {
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TableRow,
    Paper,
    Typography,
} from '@mui/material';

export const DashboardPage: React.FC = () => {
    const navigate = useNavigate();
    const { data: statistics, isLoading: statsLoading, error: statsError } = useGetStatisticsQuery();
    const { data: recentImports = [], isLoading: importsLoading } = useGetRecentImportJobsQuery(5);

    if (statsLoading) return <LoadingOverlay />;
    if (statsError) return <ErrorAlert message="Failed to load dashboard statistics" />;
    if (!statistics) return null;

    return (
        <Container maxWidth="xl" sx={{ py: 4 }}>
            <PageHeader
                title="Dashboard"
                subtitle="Welcome to HardwareVault — Your hardware inventory at a glance"
            />

            <Grid container spacing={3} sx={{ mb: 4 }}>
                <Grid item xs={12} sm={6} md={3}>
                    <StatCard
                        title="Total Devices"
                        value={statistics.totalDevices}
                        icon={<Devices fontSize="large" />}
                        color="primary"
                    />
                </Grid>
                <Grid item xs={12} sm={6} md={3}>
                    <StatCard
                        title="Active Devices"
                        value={statistics.activeDevices}
                        icon={<CheckCircle fontSize="large" />}
                        color="success"
                    />
                </Grid>
                <Grid item xs={12} sm={6} md={3}>
                    <StatCard
                        title="Import Jobs (30 days)"
                        value={statistics.recentImportJobsCount}
                        icon={<Upload fontSize="large" />}
                        color="secondary"
                    />
                </Grid>
                <Grid item xs={12} sm={6} md={3}>
                    <StatCard
                        title="Last Import"
                        value={statistics.lastImportDate ? formatDate(statistics.lastImportDate) : 'Never'}
                        icon={<CalendarToday fontSize="large" />}
                        color="info"
                    />
                </Grid>
            </Grid>

            <Grid container spacing={3} sx={{ mb: 4 }}>
                <Grid item xs={12} md={6}>
                    <ManufacturerChart
                        data={statistics.devicesByCpuManufacturer}
                        title="Devices by CPU Manufacturer"
                    />
                </Grid>
                <Grid item xs={12} md={6}>
                    <StorageTypeChart
                        data={statistics.devicesByStorageType}
                        title="Devices by Storage Type"
                    />
                </Grid>
            </Grid>

            <Box sx={{ mb: 4 }}>
                <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 2 }}>
                    <Typography variant="h6">Recent Import Jobs</Typography>
                    <Button onClick={() => navigate('/imports')} size="small">
                        View All
                    </Button>
                </Box>

                <TableContainer component={Paper}>
                    <Table>
                        <TableHead>
                            <TableRow>
                                <TableCell>File Name</TableCell>
                                <TableCell>Date</TableCell>
                                <TableCell>Status</TableCell>
                                <TableCell align="right">Rows Imported</TableCell>
                                <TableCell align="right">Errors</TableCell>
                            </TableRow>
                        </TableHead>
                        <TableBody>
                            {importsLoading ? (
                                <TableRow>
                                    <TableCell colSpan={5} align="center">
                                        <Typography variant="body2" color="text.secondary">
                                            Loading...
                                        </Typography>
                                    </TableCell>
                                </TableRow>
                            ) : recentImports.length === 0 ? (
                                <TableRow>
                                    <TableCell colSpan={5} align="center">
                                        <Typography variant="body2" color="text.secondary">
                                            No import jobs yet
                                        </Typography>
                                    </TableCell>
                                </TableRow>
                            ) : (
                                recentImports.map((job) => (
                                    <TableRow key={job.importJobId} hover>
                                        <TableCell>{job.fileName}</TableCell>
                                        <TableCell>{formatDate(job.startedAt)}</TableCell>
                                        <TableCell>
                                            <StatusBadge status={job.status} />
                                        </TableCell>
                                        <TableCell align="right">{job.successCount}</TableCell>
                                        <TableCell align="right">{job.failureCount}</TableCell>
                                    </TableRow>
                                ))
                            )}
                        </TableBody>
                    </Table>
                </TableContainer>
            </Box>

            <Box sx={{ display: 'flex', gap: 2 }}>
                <Button variant="contained" onClick={() => navigate('/import')} startIcon={<Upload />}>
                    Import New File
                </Button>
                <Button variant="outlined" onClick={() => navigate('/devices')}>
                    View All Devices
                </Button>
            </Box>
        </Container>
    );
};
