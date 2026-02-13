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

        // Powers the dashboard cards and charts.
        // Runs grouped aggregates — never loads individual device rows.
        public async Task<DeviceStatistics> GetStatisticsAsync()
        {
            var totalDevices  = await _dbSet.CountAsync(); // Active devices only
            var activeDevices = totalDevices; // Same as total (soft-deleted are filtered out)

            // Group by storage type
            var byStorageType = await _dbSet
                .GroupBy(d => d.StorageType)
                .Select(g => new { StorageType = g.Key, Count = g.Count() })
                .AsNoTracking()
                .ToDictionaryAsync(x => x.StorageType, x => x.Count);

            // Group by CPU manufacturer name
            var byCpu = await _dbSet
                .Include(d => d.Cpu).ThenInclude(c => c.Manufacturer)
                .GroupBy(d => d.Cpu.Manufacturer.Name)
                .Select(g => new { Manufacturer = g.Key, Count = g.Count() })
                .AsNoTracking()
                .ToDictionaryAsync(x => x.Manufacturer, x => x.Count);

            // Get import job statistics
            var recentImportJobsCount = await _context.ImportJobs
                .Where(j => j.CompletedAt.HasValue && 
                            j.CompletedAt.Value >= DateTime.UtcNow.AddDays(-30))
                .CountAsync();

            var lastImportDate = await _context.ImportJobs
                .Where(j => j.CompletedAt.HasValue)
                .OrderByDescending(j => j.CompletedAt)
                .Select(j => j.CompletedAt)
                .FirstOrDefaultAsync();

            return new DeviceStatistics
            {
                TotalDevices = totalDevices,
                ActiveDevices = activeDevices,
                RecentImportJobsCount = recentImportJobsCount,
                LastImportDate = lastImportDate,
                ByCpuManufacturer = byCpu,
                ByStorageType = byStorageType
            };
        }
    }
}
