// ============================================================
// APPLICATION LAYER — DEVICE SERVICE
// Orchestrates device use cases:
//   - Talks to repositories through IUnitOfWork
//   - Calls business methods on domain entities
//   - Maps entities -> DTOs before returning to controller
//   - Never touches EF Core, SQL, or HTTP directly
// ============================================================

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using HardwareVault_Services.Application.DTOs;
using HardwareVault_Services.Application.Interfaces;
using HardwareVault_Services.Infrastructure.Data.Entities;
using HardwareVault_Services.Domain.Interfaces;
using HardwareVault_Services.Domain.Common;

namespace HardwareVault_Services.Application.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<DeviceService> _logger;

        public DeviceService(IUnitOfWork uow, ILogger<DeviceService> logger)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PagedResultDto<DeviceDto>> GetDevicesAsync(
            int page,
            int pageSize,
            string? cpuManufacturer = null,
            string? gpuManufacturer = null,
            string? storageType = null,
            int? minRamInGB = null,
            string? search = null)
        {
            var result = await _uow.Devices.GetPagedAsync(
                page, pageSize,
                cpuManufacturer, gpuManufacturer,
                storageType, minRamInGB, search);

            return new PagedResultDto<DeviceDto>
            {
                Data = result.Data.Select(MapToDto).ToList(),
                TotalCount = result.TotalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<DeviceDto?> GetDeviceByIdAsync(Guid id)
        {
            var device = await _uow.Devices.GetByIdWithDetailsAsync(id);
            return device is null ? null : MapToDto(device);
        }

        public async Task<DeviceDto> CreateDeviceAsync(CreateDeviceDto dto)
        {
            // Device.Create() validates all business rules.
            // Throws ArgumentException if any rule is violated.
            var device = Device.Create(
                dto.RamSizeInMB,
                dto.StorageSizeInGB,
                dto.StorageType.ToUpper(),
                dto.CpuId,
                dto.GpuId,
                dto.PowerSupplyId,
                dto.WeightInKg);

            foreach (var port in dto.UsbPorts)
                device.AddUsbPort(port.PortType, port.Count);

            await _uow.Devices.AddAsync(device);
            await _uow.SaveChangesAsync();

            _logger.LogInformation("Device created: {DeviceId}", device.Id);

            // Re-fetch with navigations so the response DTO is complete
            var created = await _uow.Devices.GetByIdWithDetailsAsync(device.Id);
            return MapToDto(created!);
        }

        public async Task<DeviceDto?> UpdateDeviceAsync(Guid id, UpdateDeviceDto dto)
        {
            // GetByIdWithDetailsAsync respects the global soft-delete filter
            var device = await _uow.Devices.GetByIdWithDetailsAsync(id);
            if (device is null) return null;

            // Only update fields that were provided (null = unchanged)
            if (dto.RamSizeInMB.HasValue)
                device.UpdateRam(dto.RamSizeInMB.Value);

            if (dto.StorageSizeInGB.HasValue || dto.StorageType is not null)
                device.UpdateStorage(
                    dto.StorageSizeInGB ?? device.StorageSizeInGb,
                    dto.StorageType ?? device.StorageType);

            if (dto.WeightInKg.HasValue)
                device.UpdateWeight(dto.WeightInKg.Value);

            if (dto.CpuId.HasValue)
                device.UpdateCpu(dto.CpuId.Value);

            if (dto.GpuId.HasValue)
                device.UpdateGpu(dto.GpuId.Value);

            if (dto.PowerSupplyId.HasValue)
                device.UpdatePowerSupply(dto.PowerSupplyId.Value);

            if (dto.UsbPorts is not null)
            {
                device.ClearUsbPorts();
                foreach (var port in dto.UsbPorts)
                    device.AddUsbPort(port.PortType, port.Count);
            }

            // EF Core is already tracking this entity (loaded from same DbContext)
            // No explicit Update() call needed - SaveChangesAsync picks up changes
            await _uow.SaveChangesAsync();

            _logger.LogInformation("Device updated: {DeviceId}", id);

            var updated = await _uow.Devices.GetByIdWithDetailsAsync(id);
            return MapToDto(updated!);
        }

        public async Task<bool> DeleteDeviceAsync(Guid id)
        {
            var device = await _uow.Devices.GetByIdAsync(id);
            if (device is null) return false;

            device.SoftDelete();   // Sets IsDeleted = true, UpdatedAt = now
            await _uow.SaveChangesAsync();

            _logger.LogInformation("Device soft-deleted: {DeviceId}", id);
            return true;
        }

        public async Task<DeviceStatisticsDto> GetStatisticsAsync()
        {
            var stats = await _uow.Devices.GetStatisticsAsync();

            return new DeviceStatisticsDto
            {
                TotalDevices = stats.TotalDevices,
                ActiveDevices = stats.ActiveDevices,
                DeletedDevices = stats.TotalDevices - stats.ActiveDevices,
                SsdCount = stats.SsdCount,
                HddCount = stats.HddCount,
                AverageRamInGB = stats.AverageRamInGB,
                AverageStorageInGB = stats.AverageStorageInGB,
                ByCpuManufacturer = stats.ByCpuManufacturer,
                ByGpuManufacturer = stats.ByGpuManufacturer
            };
        }

        // Private: only this service is allowed to map Device -> DeviceDto.
        // Null-coalescing guards against null navigations on partial loads.
        private static DeviceDto MapToDto(Device d) => new()
        {
            DeviceId = d.Id.ToString(),
            RamSizeInMB = d.RamSizeInMb,
            StorageSizeInGB = d.StorageSizeInGb,
            StorageType = d.StorageType,
            WeightInKg = d.WeightInKg,
            CreatedAt = d.CreatedAt.ToString("o"),  // ISO 8601 format
            UpdatedAt = d.UpdatedAt.ToString("o"),

            Cpu = new CpuInfoDto
            {
                CpuId = d.CpuId,
                ModelName = d.Cpu?.ModelName ?? "",
                Manufacturer = new ManufacturerInfoDto
                {
                    ManufacturerId = d.Cpu?.Manufacturer?.Id ?? 0,
                    Name = d.Cpu?.Manufacturer?.Name ?? "",
                    Type = d.Cpu?.Manufacturer?.ProductType ?? ""
                }
            },

            Gpu = new GpuInfoDto
            {
                GpuId = d.GpuId,
                ModelName = d.Gpu?.ModelName ?? "",
                Manufacturer = new ManufacturerInfoDto
                {
                    ManufacturerId = d.Gpu?.Manufacturer?.Id ?? 0,
                    Name = d.Gpu?.Manufacturer?.Name ?? "",
                    Type = d.Gpu?.Manufacturer?.ProductType ?? ""
                }
            },

            PowerSupply = new PowerSupplyInfoDto
            {
                PowerSupplyId = d.PowerSupplyId,
                WattageInWatts = d.PowerSupply?.WattageInWatts ?? 0
            },

            UsbPorts = d.DeviceUsbPorts
                .Select(p => new UsbPortDto
                {
                    PortType = p.UsbPortType,
                    Count = p.PortCount
                }).ToList()
        };
    }
}

