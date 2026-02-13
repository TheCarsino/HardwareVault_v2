// pages/ManufacturersPage.tsx
import React from 'react';
import {
    Container,
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TableRow,
    Paper,
    Chip,
} from '@mui/material';
import { PageHeader } from '../components/common/PageHeader';
import { LoadingOverlay } from '../components/common/LoadingOverlay';
import { ErrorAlert } from '../components/common/ErrorAlert';
import { EmptyState } from '../components/common/EmptyState';
import { useGetManufacturersQuery } from '../api/manufacturerApi';

export const ManufacturersPage: React.FC = () => {
    const { data: manufacturers = [], isLoading, error } = useGetManufacturersQuery();

    if (isLoading) return <LoadingOverlay />;
    if (error) return <ErrorAlert message="Failed to load manufacturers" />;

    return (
        <Container maxWidth="xl" sx={{ py: 4 }}>
            <PageHeader
                title="Manufacturers"
                subtitle="View all CPU and GPU manufacturers"
                breadcrumbs={[
                    { label: 'Dashboard', path: '/dashboard' },
                    { label: 'Manufacturers' },
                ]}
            />

            {manufacturers.length === 0 ? (
                <EmptyState
                    title="No manufacturers found"
                    message="Manufacturers will appear here after importing devices."
                />
            ) : (
                <TableContainer component={Paper}>
                    <Table>
                        <TableHead>
                            <TableRow>
                                <TableCell>Name</TableCell>
                                <TableCell>Type</TableCell>
                                <TableCell>Website</TableCell>
                            </TableRow>
                        </TableHead>
                        <TableBody>
                            {manufacturers.map((manufacturer) => (
                                <TableRow key={manufacturer.manufacturerId} hover>
                                    <TableCell>{manufacturer.name}</TableCell>
                                    <TableCell>
                                        <Chip
                                            label={manufacturer.type}
                                            color={manufacturer.type === 'CPU' ? 'primary' : manufacturer.type === 'GPU' ? 'secondary' : 'default'}
                                            size="small"
                                        />
                                    </TableCell>
                                    <TableCell>
                                        {manufacturer.website ? (
                                            <a href={manufacturer.website} target="_blank" rel="noopener noreferrer">
                                                {manufacturer.website}
                                            </a>
                                        ) : (
                                            '—'
                                        )}
                                    </TableCell>
                                </TableRow>
                            ))}
                        </TableBody>
                    </Table>
                </TableContainer>
            )}
        </Container>
    );
};
