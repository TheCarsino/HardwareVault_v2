// pages/ImportHistoryPage.tsx
import React, { useState } from 'react';
import {
    Container,
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TableRow,
    Paper,
    TablePagination,
    Dialog,
    DialogTitle,
    DialogContent,
    IconButton,
    Box,
    Typography,
} from '@mui/material';
import { Close, Visibility } from '@mui/icons-material';
import { PageHeader } from '../components/common/PageHeader';
import { StatusBadge } from '../components/common/StatusBadge';
import { LoadingOverlay } from '../components/common/LoadingOverlay';
import { EmptyState } from '../components/common/EmptyState';
import { ImportErrorTable } from '../components/import/ImportErrorTable';
import { useGetImportJobsQuery } from '../api/importApi';
import { formatDateTime, formatSuccessRate } from '../utils/formatters';
import { ImportJob } from '../types/import';

export const ImportHistoryPage: React.FC = () => {
    const [page, setPage] = useState(0);
    const [pageSize, setPageSize] = useState(20);
    const [selectedJob, setSelectedJob] = useState<ImportJob | null>(null);

    const { data, isLoading } = useGetImportJobsQuery({
        page: page + 1,
        pageSize,
    });

    const handleChangePage = (_: unknown, newPage: number) => {
        setPage(newPage);
    };

    const handleChangeRowsPerPage = (event: React.ChangeEvent<HTMLInputElement>) => {
        setPageSize(parseInt(event.target.value, 10));
        setPage(0);
    };

    if (isLoading) return <LoadingOverlay />;

    return (
        <Container maxWidth="xl" sx={{ py: 4 }}>
            <PageHeader
                title="Import History"
                subtitle="View all past import jobs and their results"
                breadcrumbs={[
                    { label: 'Dashboard', path: '/dashboard' },
                    { label: 'Import History' },
                ]}
            />

            {!data || data.totalCount === 0 ? (
                <EmptyState
                    title="No import jobs yet"
                    message="Import your first Excel file to see it here."
                />
            ) : (
                <>
                    <TableContainer component={Paper}>
                        <Table>
                            <TableHead>
                                <TableRow>
                                    <TableCell>File Name</TableCell>
                                    <TableCell>Status</TableCell>
                                    <TableCell align="right">Total Rows</TableCell>
                                    <TableCell align="right">Success %</TableCell>
                                    <TableCell>Started At</TableCell>
                                    <TableCell align="center">Actions</TableCell>
                                </TableRow>
                            </TableHead>
                            <TableBody>
                                {data.data.map((job) => (
                                    <TableRow key={job.importJobId} hover>
                                        <TableCell>{job.fileName}</TableCell>
                                        <TableCell>
                                            <StatusBadge status={job.status} />
                                        </TableCell>
                                        <TableCell align="right">{job.totalRows}</TableCell>
                                        <TableCell align="right">
                                            {formatSuccessRate(job.successCount, job.totalRows)}
                                        </TableCell>
                                        <TableCell>{formatDateTime(job.startedAt)}</TableCell>
                                        <TableCell align="center">
                                            {job.errors && job.errors.length > 0 && (
                                                <IconButton
                                                    size="small"
                                                    onClick={() => setSelectedJob(job)}
                                                    title="View errors"
                                                >
                                                    <Visibility fontSize="small" />
                                                </IconButton>
                                            )}
                                        </TableCell>
                                    </TableRow>
                                ))}
                            </TableBody>
                        </Table>
                    </TableContainer>

                    <TablePagination
                        component="div"
                        count={data.totalCount}
                        page={page}
                        onPageChange={handleChangePage}
                        rowsPerPage={pageSize}
                        onRowsPerPageChange={handleChangeRowsPerPage}
                        rowsPerPageOptions={[10, 20, 50]}
                    />
                </>
            )}

            <Dialog
                open={!!selectedJob}
                onClose={() => setSelectedJob(null)}
                maxWidth="md"
                fullWidth
            >
                {selectedJob && (
                    <>
                        <DialogTitle>
                            <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                                <Typography variant="h6">Import Errors - {selectedJob.fileName}</Typography>
                                <IconButton onClick={() => setSelectedJob(null)}>
                                    <Close />
                                </IconButton>
                            </Box>
                        </DialogTitle>
                        <DialogContent>
                            {selectedJob.errors && selectedJob.errors.length > 0 ? (
                                <ImportErrorTable errors={selectedJob.errors} />
                            ) : (
                                <Typography variant="body2" color="text.secondary">
                                    No errors found
                                </Typography>
                            )}
                        </DialogContent>
                    </>
                )}
            </Dialog>
        </Container>
    );
};
