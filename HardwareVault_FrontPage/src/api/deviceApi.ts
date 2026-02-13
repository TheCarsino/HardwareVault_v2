// api/deviceApi.ts
import { baseApi } from './baseApi';
import { Device, DeviceFilters, CreateDeviceDto, UpdateDeviceDto, DeviceStatistics } from '../types/device';
import { PagedResult } from '../types/import';

/*
User deletes device abc-123
  → deleteDevice mutation succeeds
  → invalidatesTags: ['Device']
  → RTK Query finds: getDevices cached data has providesTags: ['Device']
  → RTK Query refetches GET /api/devices automatically
  → Device table updates with the device gone
*/

export const deviceApi = baseApi.injectEndpoints({
  endpoints: (builder) => ({
    getDevices: builder.query<PagedResult<Device>, DeviceFilters>({
      query: (filters) => {
        const params = new URLSearchParams();
        if (filters.page) params.append('page', filters.page.toString());
        if (filters.pageSize) params.append('pageSize', filters.pageSize.toString());
        if (filters.cpuManufacturer) params.append('cpuManufacturer', filters.cpuManufacturer);
        if (filters.gpuManufacturer) params.append('gpuManufacturer', filters.gpuManufacturer);
        if (filters.storageType) params.append('storageType', filters.storageType);
        if (filters.minRamInGB) params.append('minRamInGB', filters.minRamInGB.toString());
        if (filters.search) params.append('search', filters.search);
        
        return `/devices?${params.toString()}`;
      },
      providesTags: (result) =>
        result
          ? [
              ...result.data.map(({ deviceId }) => ({ type: 'Device' as const, id: deviceId })),
              { type: 'Device', id: 'LIST' },
            ]
          : [{ type: 'Device', id: 'LIST' }],
    }),
    
    getDeviceById: builder.query<Device, string>({
      query: (id) => `/devices/${id}`,
      providesTags: (result, error, id) => [{ type: 'Device' as const, id }],
    }),
    
    createDevice: builder.mutation<Device, CreateDeviceDto>({
      query: (body) => ({
        url: '/devices',
        method: 'POST',
        body,
      }),
      invalidatesTags: [{ type: 'Device', id: 'LIST' }, 'Statistics'],
    }),
    
    updateDevice: builder.mutation<Device, { id: string; body: UpdateDeviceDto }>({
      query: ({ id, body }) => ({
        url: `/devices/${id}`,
        method: 'PUT',
        body,
      }),
      invalidatesTags: (result, error, { id }) => [
        { type: 'Device', id },
        { type: 'Device', id: 'LIST' },
        'Statistics',
      ],
    }),
    
    deleteDevice: builder.mutation<void, string>({
      query: (id) => ({
        url: `/devices/${id}`,
        method: 'DELETE',
      }),
      invalidatesTags: (result, error, id) => [
        { type: 'Device', id },
        { type: 'Device', id: 'LIST' },
        'Statistics',
      ],
    }),
    
    getStatistics: builder.query<DeviceStatistics, void>({
      query: () => '/devices/statistics',
      providesTags: ['Statistics'],
    }),
  }),
});

export const {
  useGetDevicesQuery,
  useGetDeviceByIdQuery,
  useCreateDeviceMutation,
  useUpdateDeviceMutation,
  useDeleteDeviceMutation,
  useGetStatisticsQuery,
} = deviceApi;
