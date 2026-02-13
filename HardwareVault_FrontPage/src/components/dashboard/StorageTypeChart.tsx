// components/dashboard/StorageTypeChart.tsx
import React from 'react';
import { Card, CardContent, Typography } from '@mui/material';
import {
    BarChart,
    Bar,
    XAxis,
    YAxis,
    CartesianGrid,
    Tooltip,
    ResponsiveContainer,
    Legend,
} from 'recharts';

interface StorageTypeChartProps {
    data: Record<string, number>;
    title: string;
}

export const StorageTypeChart: React.FC<StorageTypeChartProps> = ({ data, title }) => {
    const chartData = data ? Object.entries(data).map(([name, value]) => ({
        name,
        count: value,
    })) : [];

    if (chartData.length === 0) {
        return (
            <Card>
                <CardContent>
                    <Typography variant="h6" gutterBottom>
                        {title}
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                        No data available
                    </Typography>
                </CardContent>
            </Card>
        );
    }

    return (
        <Card>
            <CardContent>
                <Typography variant="h6" gutterBottom>
                    {title}
                </Typography>
                <ResponsiveContainer width="100%" height={300}>
                    <BarChart data={chartData}>
                        <CartesianGrid strokeDasharray="3 3" />
                        <XAxis dataKey="name" />
                        <YAxis />
                        <Tooltip />
                        <Legend />
                        <Bar dataKey="count" fill="#0D9488" name="Device Count" />
                    </BarChart>
                </ResponsiveContainer>
            </CardContent>
        </Card>
    );
};
