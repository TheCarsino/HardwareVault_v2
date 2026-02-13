using HardwareVault_Services.Application.DTOs;

namespace HardwareVault_Services.Application.Interfaces
{
    public interface IDeviceService
    {
        Task<PagedResultDto<DeviceDto>> GetDevicesAsync(
            int page = 1, 
            int pageSize = 10,
            string? cpuManufacturer = null,
            string? gpuManufacturer = null,
            int? minRamInGB = null,
            string? storageType = null);
        
        Task<DeviceDto?> GetDeviceByIdAsync(Guid deviceId);
        Task<DeviceDto> CreateDeviceAsync(CreateDeviceDto dto);
        Task<DeviceDto?> UpdateDeviceAsync(Guid deviceId, UpdateDeviceDto dto);
        Task<bool> DeleteDeviceAsync(Guid deviceId);
        Task<IEnumerable<DeviceDto>> GetDevicesByCpuManufacturerAsync(string manufacturerName);
    }
}
