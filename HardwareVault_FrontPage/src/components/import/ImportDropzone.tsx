// components/import/ImportDropzone.tsx
import React, { useCallback } from 'react';
import { useDropzone } from 'react-dropzone';
import { Box, Typography, Paper } from '@mui/material';
import { CloudUpload } from '@mui/icons-material';
import { IMPORT_CONFIG } from '../../utils/constants';

interface ImportDropzoneProps {
    onFileSelect: (file: File) => void;
    disabled?: boolean;
}

export const ImportDropzone: React.FC<ImportDropzoneProps> = ({ onFileSelect, disabled = false }) => {
    const onDrop = useCallback((acceptedFiles: File[]) => {
        if (acceptedFiles.length > 0) {
            onFileSelect(acceptedFiles[0]);
        }
    }, [onFileSelect]);

    const { getRootProps, getInputProps, isDragActive, fileRejections } = useDropzone({
        onDrop,
        accept: IMPORT_CONFIG.acceptedFileTypes,
        maxFiles: 1,
        maxSize: IMPORT_CONFIG.maxFileSize,
        disabled,
    });

    const hasErrors = fileRejections.length > 0;

    return (
        <Box>
            <Paper
                {...getRootProps()}
                elevation={0}
                sx={{
                    border: '2px dashed',
                    borderColor: hasErrors ? 'error.main' : isDragActive ? 'primary.main' : 'divider',
                    borderRadius: 2,
                    p: 6,
                    textAlign: 'center',
                    cursor: disabled ? 'not-allowed' : 'pointer',
                    backgroundColor: isDragActive ? 'action.hover' : 'background.paper',
                    transition: 'all 0.2s',
                    '&:hover': {
                        borderColor: disabled ? 'divider' : 'primary.main',
                        backgroundColor: disabled ? 'background.paper' : 'action.hover',
                    },
                }}
            >
                <input {...getInputProps()} />
                <CloudUpload sx={{ fontSize: 64, color: 'text.secondary', mb: 2 }} />

                {isDragActive ? (
                    <Typography variant="h6" color="primary">
                        Drop the file here
                    </Typography>
                ) : (
                    <>
                        <Typography variant="h6" gutterBottom>
                            Drop your Excel file here or click to browse
                        </Typography>
                        <Typography variant="body2" color="text.secondary">
                            Only .xlsx files up to {IMPORT_CONFIG.maxFileSizeLabel}
                        </Typography>
                    </>
                )}
            </Paper>

            {hasErrors && (
                <Box sx={{ mt: 2 }}>
                    {fileRejections.map(({ file, errors }) => (
                        <Typography key={file.name} variant="body2" color="error">
                            {file.name}: {errors.map(e => e.message).join(', ')}
                        </Typography>
                    ))}
                </Box>
            )}
        </Box>
    );
};
