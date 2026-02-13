using HardwareVault_Services.Application.DTOs;
using HardwareVault_Services.Application.Interfaces;
using HardwareVault_Services.Domain.Entities;
using HardwareVault_Services.Domain.Interfaces;

namespace HardwareVault_Services.Application.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeviceService> _logger;

        public DeviceService(IUnitOfWork unitOfWork, ILogger<DeviceService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<PagedResultDto<DeviceDto>> GetDevicesAsync(
            int page = 1, 
            int pageSize = 10,
            string? cpuManufacturer = null,
            string? gpuManufacturer = null,
            int? minRamInGB = null,
            string? storageType = null)
        {
            var (devices, totalCount) = await _unitOfWork.Devices.GetPagedDevicesAsync(
                page, 
                pageSize,
                cpuManufacturer,
                gpuManufacturer,
                minRamInGB,
                storageType);

            var deviceDtos = devices.Select(MapToDto).ToList();

            return new PagedResultDto<DeviceDto>
            {
                Data = deviceDtos,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<DeviceDto?> GetDeviceByIdAsync(Guid deviceId)
        {
            var device = await _unitOfWork.Devices.GetDeviceWithDetailsAsync(deviceId);
            return device == null ? null : MapToDto(device);
        }

        public async Task<DeviceDto> CreateDeviceAsync(CreateDeviceDto dto)
        {
            // Validate storage type
            if (!new[] { "SSD", "HDD", "NVMe" }.Contains(dto.StorageType))
            {
                throw new ArgumentException($"Invalid storage type: {dto.StorageType}. Must be SSD, HDD, or NVMe");
            }

            // Create device
            var device = new Device
            {
                Id = Guid.NewGuid(),
                RamSizeInMB = dto.RamSizeInMB,
                StorageSizeInGB = dto.StorageSizeInGB,
                StorageType = dto.StorageType,
                CpuId = dto.CpuId,
                GpuId = dto.GpuId,
                PowerSupplyId = dto.PowerSupplyId,
                WeightInKg = dto.WeightInKg,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            // Add USB ports if provided
            if (dto.UsbPorts.Any())
            {
                foreach (var usbPortDto in dto.UsbPorts)
                {
                    var usbPort = new DeviceUsbPort
                    {
                        Id = Guid.NewGuid(),
                        DeviceId = device.Id,
                        UsbPortType = usbPortDto.UsbPortType,
                        PortCount = usbPortDto.PortCount
                    };
                    device.DeviceUsbPorts.Add(usbPort);
                }
            }

            await _unitOfWork.Devices.AddAsync(device);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Created new device with ID: {DeviceId}", device.Id);

            // Load related data for DTO
            var createdDevice = await _unitOfWork.Devices.GetDeviceWithDetailsAsync(device.Id);
            return MapToDto(createdDevice!);
        }

        public async Task<DeviceDto?> UpdateDeviceAsync(Guid deviceId, UpdateDeviceDto dto)
        {
            var device = await _unitOfWork.Devices.GetByIdAsync(deviceId);
            if (device == null)
            {
                return null;
            }

            // Update properties if provided
            if (dto.RamSizeInMB.HasValue)
                device.RamSizeInMB = dto.RamSizeInMB.Value;

            if (dto.StorageSizeInGB.HasValue)
                device.StorageSizeInGB = dto.StorageSizeInGB.Value;

            if (!string.IsNullOrEmpty(dto.StorageType))
            {
                if (new[] { "SSD", "HDD", "NVMe" }.Contains(dto.StorageType))
                    device.StorageType = dto.StorageType;
            }

            if (dto.WeightInKg.HasValue)
                device.WeightInKg = dto.WeightInKg.Value;

            if (dto.CpuId.HasValue)
                device.CpuId = dto.CpuId.Value;

            if (dto.GpuId.HasValue)
                device.GpuId = dto.GpuId.Value;

            if (dto.PowerSupplyId.HasValue)
                device.PowerSupplyId = dto.PowerSupplyId.Value;

            device.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Devices.Update(device);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Updated device with ID: {DeviceId}", deviceId);

            // Load related data for DTO
            var updatedDevice = await _unitOfWork.Devices.GetDeviceWithDetailsAsync(deviceId);
            return MapToDto(updatedDevice!);
        }

        public async Task<bool> DeleteDeviceAsync(Guid deviceId)
        {
            var device = await _unitOfWork.Devices.GetByIdAsync(deviceId);
            if (device == null)
            {
                return false;
            }

            // Soft delete
            device.IsDeleted = true;
            device.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Devices.Update(device);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Soft deleted device with ID: {DeviceId}", deviceId);
            return true;
        }

        public async Task<IEnumerable<DeviceDto>> GetDevicesByCpuManufacturerAsync(string manufacturerName)
        {
            var devices = await _unitOfWork.Devices.GetDevicesByCpuManufacturerAsync(manufacturerName);
            return devices.Select(MapToDto);
        }

        private DeviceDto MapToDto(Device device)
        {
            return new DeviceDto
            {
                DeviceId = device.Id,
                RamSizeInMB = device.RamSizeInMB,
                StorageSizeInGB = device.StorageSizeInGB,
                StorageType = device.StorageType,
                WeightInKg = device.WeightInKg,
                CpuModel = device.Cpu.ModelName,
                CpuManufacturer = device.Cpu.Manufacturer.Name,
                GpuModel = device.Gpu.ModelName,
                GpuManufacturer = device.Gpu.Manufacturer.Name,
                PowerSupplyWattage = device.PowerSupply.WattageInWatts,
                UsbPorts = device.DeviceUsbPorts.Select(up => new UsbPortDto
                {
                    DeviceUsbPortId = up.Id,
                    UsbPortType = up.UsbPortType,
                    PortCount = up.PortCount
                }).ToList(),
                CreatedAt = device.CreatedAt,
                UpdatedAt = device.UpdatedAt
            };
        }
    }
}
