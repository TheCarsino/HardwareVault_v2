// api/baseApi.ts
import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';

const baseUrl = import.meta.env.VITE_API_BASE_URL || '/api';

console.log('API Base URL:', baseUrl);
export const baseApi = createApi({
  reducerPath: 'api',
  baseQuery: fetchBaseQuery({
    baseUrl,
    prepareHeaders: (headers) => {
      const token = localStorage.getItem('HardwareVault_token');
      if (token) {
        headers.set('Authorization', `Bearer ${token}`);
      }
      headers.set('Content-Type', 'application/json');
      return headers;
    },
  }),
  keepUnusedDataFor: 60,  // 60 seconds to cached / store data to reduce unnecessary refetches
  tagTypes: ['Device', 'ImportJob', 'Manufacturer', 'Statistics'],
  endpoints: () => ({}),
});
