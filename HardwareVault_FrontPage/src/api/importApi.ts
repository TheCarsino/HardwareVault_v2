// api/importApi.ts
import { baseApi } from './baseApi';
import { ImportJob, PagedResult } from '../types/import';

export const importApi = baseApi.injectEndpoints({
  endpoints: (builder) => ({
    importDevices: builder.mutation<ImportJob, FormData>({
      query: (formData) => ({
        url: '/devices/import',
        method: 'POST',
        body: formData,
        // Don't set Content-Type header - browser will set it with boundary for FormData
        prepareHeaders: (headers) => {
          headers.delete('Content-Type');
          return headers;
        },
      }),
      invalidatesTags: [
        { type: 'Device', id: 'LIST' },
        { type: 'ImportJob', id: 'LIST' },
        'Statistics',
      ],
    }),
    
    getImportJobs: builder.query<PagedResult<ImportJob>, { page?: number; pageSize?: number }>({
      query: ({ page = 1, pageSize = 20 }) => `/imports?page=${page}&pageSize=${pageSize}`,
      providesTags: (result) =>
        result
          ? [
              ...result.data.map(({ importJobId }) => ({
                type: 'ImportJob' as const,
                id: importJobId,
              })),
              { type: 'ImportJob', id: 'LIST' },
            ]
          : [{ type: 'ImportJob', id: 'LIST' }],
    }),
    
    getImportJobById: builder.query<ImportJob, string>({
      query: (id) => `/imports/${id}`,
      providesTags: (result, error, id) => [{ type: 'ImportJob', id }],
    }),
    
    getRecentImportJobs: builder.query<ImportJob[], number>({
      query: (limit = 5) => `/imports/recent?limit=${limit}`,
      providesTags: [{ type: 'ImportJob', id: 'RECENT' }],
    }),
  }),
});

export const {
  useImportDevicesMutation,
  useGetImportJobsQuery,
  useGetImportJobByIdQuery,
  useGetRecentImportJobsQuery,
} = importApi;
