// pages/ImportPage.tsx
import React, { useState } from 'react';
import { Container, Box, Card, CardContent, Typography, Stepper, Step, StepLabel } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import { PageHeader } from '../components/common/PageHeader';
import { ImportDropzone } from '../components/import/ImportDropzone';
import { ImportResults } from '../components/import/ImportResults';
import { useImportDevicesMutation } from '../api/importApi';
import { LoadingOverlay } from '../components/common/LoadingOverlay';
import { ImportJob } from '../types/import';
import { useSnackbar } from 'notistack';

const steps = ['Upload File', 'Review Results'];

export const ImportPage: React.FC = () => {
    const navigate = useNavigate();
    const { enqueueSnackbar } = useSnackbar();
    const [importDevices, { isLoading }] = useImportDevicesMutation();

    const [activeStep, setActiveStep] = useState(0);
    const [selectedFile, setSelectedFile] = useState<File | null>(null);
    const [importResult, setImportResult] = useState<ImportJob | null>(null);

    const handleFileSelect = async (file: File) => {
        setSelectedFile(file);

        const formData = new FormData();
        formData.append('file', file);

        try {
            const result = await importDevices(formData).unwrap();
            setImportResult(result);
            setActiveStep(1);

            if (result.failureCount === 0) {
                enqueueSnackbar(`Successfully imported ${result.successCount} devices`, { variant: 'success' });
            } else {
                enqueueSnackbar(`Imported with ${result.failureCount} errors`, { variant: 'warning' });
            }
        } catch (err) {
            enqueueSnackbar('Failed to import file', { variant: 'error' });
        }
    };

    const handleImportAnother = () => {
        setActiveStep(0);
        setSelectedFile(null);
        setImportResult(null);
    };

    return (
        <Container maxWidth="lg" sx={{ py: 4 }}>
            <PageHeader
                title="Import Devices"
                subtitle="Upload an Excel file to import device records"
                breadcrumbs={[
                    { label: 'Dashboard', path: '/dashboard' },
                    { label: 'Import' },
                ]}
            />

            <Card>
                <CardContent sx={{ p: 4 }}>
                    <Stepper activeStep={activeStep} sx={{ mb: 4 }}>
                        {steps.map((label) => (
                            <Step key={label}>
                                <StepLabel>{label}</StepLabel>
                            </Step>
                        ))}
                    </Stepper>

                    {activeStep === 0 && (
                        <Box>
                            <Typography variant="h6" gutterBottom>
                                Upload Your Excel File
                            </Typography>
                            <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
                                Select an .xlsx file containing your hardware inventory data.
                            </Typography>

                            {isLoading ? (
                                <LoadingOverlay message="Processing your file..." />
                            ) : (
                                <ImportDropzone onFileSelect={handleFileSelect} disabled={isLoading} />
                            )}
                        </Box>
                    )}

                    {activeStep === 1 && importResult && (
                        <ImportResults
                            importJob={importResult}
                            onViewDevices={() => navigate('/devices')}
                            onImportAnother={handleImportAnother}
                        />
                    )}
                </CardContent>
            </Card>
        </Container>
    );
};
