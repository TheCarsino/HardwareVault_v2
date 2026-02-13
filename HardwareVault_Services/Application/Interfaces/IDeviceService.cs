using System;
using System.Threading.Tasks;
using HardwareVault_Services.Application.DTOs;

namespace HardwareVault_Services.Application.Interfaces
{
    public interface IDeviceService
    {
        // GET /api/devices — main table with filtering + pagination
        Task<PagedResultDto<DeviceDto>> GetDevicesAsync(
            int page,
            int pageSize,
            string? cpuManufacturer = null,
            string? gpuManufacturer = null,
            string? storageType = null,
            int? minRamInGB = null,
            string? search = null);

        // GET /api/devices/{id} — detail panel / edit form
        Task<DeviceDto?> GetDeviceByIdAsync(Guid id);

        // POST /api/devices — manual device creation
        Task<DeviceDto> CreateDeviceAsync(CreateDeviceDto dto);

        // PUT /api/devices/{id} — edit device
        Task<DeviceDto?> UpdateDeviceAsync(Guid id, UpdateDeviceDto dto);

        // DELETE /api/devices/{id} — soft delete
        Task<bool> DeleteDeviceAsync(Guid id);

        // GET /api/devices/statistics — dashboard aggregates
        Task<DeviceStatisticsDto> GetStatisticsAsync();
    }
}
