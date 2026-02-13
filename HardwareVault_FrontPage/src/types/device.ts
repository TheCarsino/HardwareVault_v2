// types/device.ts
import { Manufacturer } from './manufacturer';

export interface Device {
  deviceId: string;
  ramSizeInMB: number;
  storageSizeInGB: number;
  storageType: 'SSD' | 'HDD';
  weightInKg: number;
  createdAt: string;
  updatedAt: string;
  cpu: {
    cpuId: number;
    modelName: string;
    manufacturer: Manufacturer;
  };
  gpu: {
    gpuId: number;
    modelName: string;
    manufacturer: Manufacturer;
  };
  powerSupply: {
    powerSupplyId: number;
    wattageInWatts: number;
  };
  usbPorts: UsbPort[];
}

export interface UsbPort {
  deviceUsbPortId: string;
  usbPortType: string;
  count: number;
}

export interface DeviceFilters {
  cpuManufacturer?: string;
  gpuManufacturer?: string;
  storageType?: 'SSD' | 'HDD';
  minRamInGB?: number;
  search?: string;
  page: number;
  pageSize: number;
}

export interface CreateDeviceDto {
  ramSizeInMB: number;
  storageSizeInGB: number;
  storageType: 'SSD' | 'HDD';
  weightInKg: number;
  cpuId: number;
  gpuId: number;
  powerSupplyWattageInWatts: number;
  usbPorts: CreateUsbPortDto[];
}

export interface CreateUsbPortDto {
  usbPortType: string;
  count: number;
}

export interface UpdateDeviceDto {
  ramSizeInMB?: number;
  storageSizeInGB?: number;
  storageType?: 'SSD' | 'HDD';
  weightInKg?: number;
  cpuId?: number;
  gpuId?: number;
  powerSupplyWattageInWatts?: number;
  usbPorts?: CreateUsbPortDto[];
}

export interface DeviceStatistics {
  totalDevices: number;
  activeDevices: number;
  recentImportJobsCount: number;
  lastImportDate?: string;
  devicesByCpuManufacturer: Record<string, number>;
  devicesByStorageType: Record<string, number>;
}
