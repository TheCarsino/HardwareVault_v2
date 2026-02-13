using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HardwareVault_Services.Domain.Common;
using HardwareVault_Services.Infrastructure.Data.Entities;

namespace HardwareVault_Services.Domain.Interfaces
{
    public interface IDeviceRepository : IRepository<Device, Guid>
    {
        // Full device with all navigations loaded — used for detail/edit panel
        Task<Device?> GetByIdWithDetailsAsync(Guid deviceId);

        // Paged + filtered list — powers the main device table in React
        Task<PagedResult<Device>> GetPagedAsync(
            int     page,
            int     pageSize,
            string? cpuManufacturer = null,
            string? gpuManufacturer = null,
            string? storageType     = null,
            int?    minRamInGB      = null,
            string? search          = null);

        // Aggregates for the dashboard stat cards and charts
        Task<DeviceStatistics> GetStatisticsAsync();
    }
}
