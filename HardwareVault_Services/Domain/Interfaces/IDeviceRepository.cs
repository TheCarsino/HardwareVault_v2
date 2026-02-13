using HardwareVault_Services.Domain.Entities;

namespace HardwareVault_Services.Domain.Interfaces
{
    public interface IDeviceRepository : IRepository<Device>
    {
        Task<IEnumerable<Device>> GetDevicesWithDetailsAsync();
        Task<Device?> GetDeviceWithDetailsAsync(Guid deviceId);
        Task<IEnumerable<Device>> GetDevicesByCpuManufacturerAsync(string manufacturerName);
        Task<(IEnumerable<Device> Devices, int TotalCount)> GetPagedDevicesAsync(
            int page, 
            int pageSize, 
            string? cpuManufacturer = null,
            string? gpuManufacturer = null,
            int? minRamInGB = null,
            string? storageType = null);
    }
}
