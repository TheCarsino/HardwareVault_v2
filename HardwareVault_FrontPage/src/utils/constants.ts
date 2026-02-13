// utils/constants.ts

/**
 * Page size options for data tables
 */
export const PAGE_SIZES = [10, 20, 50] as const;

/**
 * Default page size
 */
export const DEFAULT_PAGE_SIZE = 20;

/**
 * Storage type options
 */
export const STORAGE_TYPES = ['SSD', 'HDD'] as const;

/**
 * CPU manufacturer options
 */
export const CPU_MANUFACTURERS = ['Intel', 'AMD'] as const;

/**
 * GPU manufacturer options
 */
export const GPU_MANUFACTURERS = ['NVIDIA', 'AMD'] as const;

/**
 * RAM size options (in GB)
 */
export const RAM_OPTIONS = [4, 8, 16, 32, 64] as const;

/**
 * USB port types
 */
export const USB_PORT_TYPES = [
  'USB 2.0',
  'USB 3.0',
  'USB 3.1',
  'USB 3.2',
  'USB-C',
  'Thunderbolt 3',
  'Thunderbolt 4',
] as const;

/**
 * Import file configuration
 */
export const IMPORT_CONFIG = {
  maxFileSize: 10 * 1024 * 1024, // 10MB
  acceptedFileTypes: {
    'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet': ['.xlsx'],
  },
  maxFileSizeLabel: '10 MB',
} as const;

/**
 * API endpoints (relative paths)
 */
export const API_ENDPOINTS = {
  devices: '/devices',
  imports: '/imports',
  manufacturers: '/manufacturers',
  statistics: '/devices/statistics',
} as const;

/**
 * Local storage keys
 */
export const STORAGE_KEYS = {
  token: 'HardwareVault_token',
  user: 'HardwareVault_user',
  filters: 'HardwareVault_filters',
} as const;

/**
 * Debounce delay for search input (ms)
 */
export const SEARCH_DEBOUNCE_MS = 300;
