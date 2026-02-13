// ════════════════════════════════════════════════════════════
// FILE: DeviceRepository.cs
// Powers the main device table, detail panel, and dashboard stats
// ════════════════════════════════════════════════════════════

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HardwareVault_Services.Domain.Common;
using HardwareVault_Services.Domain.Interfaces;
using HardwareVault_Services.Infrastructure.Data;
using HardwareVault_Services.Infrastructure.Data.Entities;

namespace HardwareVault_Services.Infrastructure.Repositories
{
    public class DeviceRepository : Repository<Device, Guid>, IDeviceRepository
    {
        public DeviceRepository(ApplicationDbContext context) : base(context) { }

        // -- GetByIdWithDetailsAsync 
        // Used by the device detail/edit slide-over panel.
        // Loads ALL navigations so the response is complete.
        public async Task<Device?> GetByIdWithDetailsAsync(Guid deviceId)
        {
            return await _dbSet
                .Include(d => d.Cpu).ThenInclude(c => c.Manufacturer)
                .Include(d => d.Gpu).ThenInclude(g => g.Manufacturer)
                .Include(d => d.PowerSupply)
                .Include(d => d.DeviceUsbPorts)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == deviceId);
            // Note: global query filter (IsDeleted = 0) is still active.
            // Use .IgnoreQueryFilters() if you need to fetch a deleted device for restore.
        }

        // -- GetPagedAsync 
        // Powers the main device table in React.
        // Builds one SQL query with only the WHERE clauses the caller provides.
        public async Task<PagedResult<Device>> GetPagedAsync(
            int     page,
            int     pageSize,
            string? cpuManufacturer = null,
            string? gpuManufacturer = null,
            string? storageType     = null,
            int?    minRamInGB      = null,
            string? search          = null)
        {
            var query = _dbSet
                .Include(d => d.Cpu).ThenInclude(c => c.Manufacturer)
                .Include(d => d.Gpu).ThenInclude(g => g.Manufacturer)
                .Include(d => d.PowerSupply)
                .Include(d => d.DeviceUsbPorts)
                .AsNoTracking()
                .AsQueryable();
            // Global HasQueryFilter already restricts to IsDeleted = 0

            // Apply each filter only when provided
            if (!string.IsNullOrWhiteSpace(cpuManufacturer))
                query = query.Where(d => d.Cpu.Manufacturer.Name == cpuManufacturer);

            if (!string.IsNullOrWhiteSpace(gpuManufacturer))
                query = query.Where(d => d.Gpu.Manufacturer.Name == gpuManufacturer);

            if (!string.IsNullOrWhiteSpace(storageType))
                query = query.Where(d => d.StorageType == storageType.ToUpper());

            if (minRamInGB.HasValue)
                query = query.Where(d => d.RamSizeInMb >= minRamInGB.Value * 1024);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(d =>
                    d.Cpu.ModelName.Contains(search) ||
                    d.Gpu.ModelName.Contains(search));

            // COUNT before pagination — needed for the "Page 1 of 8" footer
            var totalCount = await query.CountAsync();

            var data = await query
                .OrderByDescending(d => d.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Device> { Data = data, TotalCount = totalCount };
        }

        // -- GetStatisticsAsync 
        // Powers the dashboard cards and charts.
        // Runs grouped aggregates — never loads individual device rows.
        public async Task<DeviceStatistics> GetStatisticsAsync()
        {
            // IgnoreQueryFilters for total (includes deleted)
            var totalDevices  = await _dbSet.IgnoreQueryFilters().CountAsync();
            var activeDevices = await _dbSet.CountAsync(); // filter active

            var ssdCount = await _dbSet.CountAsync(d => d.StorageType == "SSD");
            var hddCount = await _dbSet.CountAsync(d => d.StorageType == "HDD");

            var avgRam = await _dbSet.AverageAsync(d => (double?)d.RamSizeInMb) ?? 0;
            var avgStorage = await _dbSet.AverageAsync(d => (double?)d.StorageSizeInGb) ?? 0;

            // Group by CPU manufacturer name
            var byCpu = await _dbSet
                .Include(d => d.Cpu).ThenInclude(c => c.Manufacturer)
                .GroupBy(d => d.Cpu.Manufacturer.Name)
                .Select(g => new { Manufacturer = g.Key, Count = g.Count() })
                .AsNoTracking()
                .ToDictionaryAsync(x => x.Manufacturer, x => x.Count);

            // Group by GPU manufacturer name
            var byGpu = await _dbSet
                .Include(d => d.Gpu).ThenInclude(g => g.Manufacturer)
                .GroupBy(d => d.Gpu.Manufacturer.Name)
                .Select(g => new { Manufacturer = g.Key, Count = g.Count() })
                .AsNoTracking()
                .ToDictionaryAsync(x => x.Manufacturer, x => x.Count);

            return new DeviceStatistics
            {
                TotalDevices         = totalDevices,
                ActiveDevices        = activeDevices,
                SsdCount             = ssdCount,
                HddCount             = hddCount,
                AverageRamInGB       = Math.Round(avgRam / 1024, 1),
                AverageStorageInGB   = Math.Round(avgStorage, 1),
                ByCpuManufacturer    = byCpu,
                ByGpuManufacturer    = byGpu
            };
        }
    }
}
