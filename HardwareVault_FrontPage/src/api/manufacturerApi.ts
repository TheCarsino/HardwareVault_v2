// api/manufacturerApi.ts
import { baseApi } from './baseApi';
import { Manufacturer } from '../types/manufacturer';

export const manufacturerApi = baseApi.injectEndpoints({
  endpoints: (builder) => ({
    getManufacturers: builder.query<Manufacturer[], void>({
      query: () => '/manufacturers',
      providesTags: (result) =>
        result
          ? [
              ...result.map(({ manufacturerId }) => ({
                type: 'Manufacturer' as const,
                id: manufacturerId,
              })),
              { type: 'Manufacturer', id: 'LIST' },
            ]
          : [{ type: 'Manufacturer', id: 'LIST' }],
    }),
    
    getManufacturerById: builder.query<Manufacturer, number>({
      query: (id) => `/manufacturers/${id}`,
      providesTags: (result, error, id) => [{ type: 'Manufacturer', id }],
    }),
    
    createManufacturer: builder.mutation<Manufacturer, Omit<Manufacturer, 'manufacturerId'>>({
      query: (body) => ({
        url: '/manufacturers',
        method: 'POST',
        body,
      }),
      invalidatesTags: [{ type: 'Manufacturer', id: 'LIST' }],
    }),
    
    updateManufacturer: builder.mutation<Manufacturer, { id: number; body: Partial<Manufacturer> }>({
      query: ({ id, body }) => ({
        url: `/manufacturers/${id}`,
        method: 'PUT',
        body,
      }),
      invalidatesTags: (result, error, { id }) => [
        { type: 'Manufacturer', id },
        { type: 'Manufacturer', id: 'LIST' },
      ],
    }),
    
    deleteManufacturer: builder.mutation<void, number>({
      query: (id) => ({
        url: `/manufacturers/${id}`,
        method: 'DELETE',
      }),
      invalidatesTags: (result, error, id) => [
        { type: 'Manufacturer', id },
        { type: 'Manufacturer', id: 'LIST' },
      ],
    }),
  }),
});

export const {
  useGetManufacturersQuery,
  useGetManufacturerByIdQuery,
  useCreateManufacturerMutation,
  useUpdateManufacturerMutation,
  useDeleteManufacturerMutation,
} = manufacturerApi;
