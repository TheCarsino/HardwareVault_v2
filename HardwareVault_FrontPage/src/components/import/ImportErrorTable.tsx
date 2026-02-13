// components/import/ImportErrorTable.tsx
import React from 'react';
import {
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TableRow,
    Paper,
} from '@mui/material';
import { ImportError } from '../../types/import';

interface ImportErrorTableProps {
    errors: ImportError[];
}

export const ImportErrorTable: React.FC<ImportErrorTableProps> = ({ errors }) => {
    return (
        <TableContainer component={Paper} variant="outlined">
            <Table size="small">
                <TableHead>
                    <TableRow>
                        <TableCell><strong>Row</strong></TableCell>
                        <TableCell><strong>Field</strong></TableCell>
                        <TableCell><strong>Raw Value</strong></TableCell>
                        <TableCell><strong>Error</strong></TableCell>
                    </TableRow>
                </TableHead>
                <TableBody>
                    {errors.map((error, index) => (
                        <TableRow key={index} hover>
                            <TableCell>{error.row}</TableCell>
                            <TableCell>{error.field || 'N/A'}</TableCell>
                            <TableCell>
                                <code style={{ fontSize: '0.875rem' }}>{error.rawValue || 'N/A'}</code>
                            </TableCell>
                            <TableCell>{error.message}</TableCell>
                        </TableRow>
                    ))}
                </TableBody>
            </Table>
        </TableContainer>
    );
};
