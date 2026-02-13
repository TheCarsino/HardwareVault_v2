// components/import/ImportResults.tsx
import React from 'react';
import { Box, Card, CardContent, Typography, Button, Alert } from '@mui/material';
import { CheckCircle, Error, TableView, Upload } from '@mui/icons-material';
import { ImportJob } from '../../types/import';
import { formatSuccessRate } from '../../utils/formatters';
import { ImportErrorTable } from './ImportErrorTable';

interface ImportResultsProps {
    importJob: ImportJob;
    onViewDevices: () => void;
    onImportAnother: () => void;
}

export const ImportResults: React.FC<ImportResultsProps> = ({
    importJob,
    onViewDevices,
    onImportAnother,
}) => {
    const hasErrors = importJob.failureCount > 0;
    const successRate = formatSuccessRate(importJob.successCount, importJob.totalRows);

    return (
        <Box>
            <Alert
                severity={hasErrors ? 'warning' : 'success'}
                icon={hasErrors ? <Error /> : <CheckCircle />}
                sx={{ mb: 3 }}
            >
                <Typography variant="h6" gutterBottom>
                    {hasErrors
                        ? `Import completed with errors (${successRate} success rate)`
                        : 'Import completed successfully!'}
                </Typography>
                <Typography variant="body2">
                    {importJob.successCount} of {importJob.totalRows} rows imported successfully
                    {hasErrors && ` • ${importJob.failureCount} rows failed`}
                </Typography>
            </Alert>

            <Card sx={{ mb: 3 }}>
                <CardContent>
                    <Typography variant="h6" gutterBottom>
                        Import Summary
                    </Typography>
                    <Box sx={{ display: 'grid', gridTemplateColumns: 'repeat(3, 1fr)', gap: 2, mt: 2 }}>
                        <Box>
                            <Typography variant="caption" color="text.secondary">
                                File Name
                            </Typography>
                            <Typography variant="body1" fontWeight={500}>
                                {importJob.fileName}
                            </Typography>
                        </Box>
                        <Box>
                            <Typography variant="caption" color="text.secondary">
                                Total Rows
                            </Typography>
                            <Typography variant="body1" fontWeight={500}>
                                {importJob.totalRows}
                            </Typography>
                        </Box>
                        <Box>
                            <Typography variant="caption" color="text.secondary">
                                Success Rate
                            </Typography>
                            <Typography variant="body1" fontWeight={500} color={hasErrors ? 'warning.main' : 'success.main'}>
                                {successRate}
                            </Typography>
                        </Box>
                    </Box>
                </CardContent>
            </Card>

            {hasErrors && importJob.errors && importJob.errors.length > 0 && (
                <Box sx={{ mb: 3 }}>
                    <Typography variant="h6" gutterBottom>
                        Import Errors ({importJob.errors.length})
                    </Typography>
                    <ImportErrorTable errors={importJob.errors} />
                </Box>
            )}

            <Box sx={{ display: 'flex', gap: 2 }}>
                <Button
                    variant="contained"
                    startIcon={<TableView />}
                    onClick={onViewDevices}
                    fullWidth
                >
                    View Imported Devices
                </Button>
                <Button
                    variant="outlined"
                    startIcon={<Upload />}
                    onClick={onImportAnother}
                    fullWidth
                >
                    Import Another File
                </Button>
            </Box>
        </Box>
    );
};
