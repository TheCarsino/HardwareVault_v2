// features/devices/devicesSlice.ts
import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { DeviceFilters } from '../../types/device';
import { DEFAULT_PAGE_SIZE } from '../../utils/constants';

interface DevicesState {
  filters: DeviceFilters;
}

const initialState: DevicesState = {
  filters: {
    page: 1,
    pageSize: DEFAULT_PAGE_SIZE,
  },
};

export const devicesSlice = createSlice({
  name: 'devices',
  initialState,
  reducers: {
    setFilters: (state, action: PayloadAction<Partial<DeviceFilters>>) => {
      state.filters = { ...state.filters, ...action.payload, page: 1 };
    },
    setPage: (state, action: PayloadAction<number>) => {
      state.filters.page = action.payload;
    },
    setPageSize: (state, action: PayloadAction<number>) => {
      state.filters.pageSize = action.payload;
      state.filters.page = 1;
    },
    clearFilters: (state) => {
      state.filters = {
        page: 1,
        pageSize: state.filters.pageSize,
      };
    },
  },
});

export const { setFilters, setPage, setPageSize, clearFilters } = devicesSlice.actions;
export default devicesSlice.reducer;
