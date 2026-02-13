using Microsoft.EntityFrameworkCore;
using HardwareVault_Services.Domain.Entities;
using HardwareVault_Services.Infrastructure.Data;
using HardwareVault_Services.Domain.Interfaces;

namespace HardwareVault_Services.Infrastructure.Repositories
{
    public class DeviceRepository : Repository<Device>, IDeviceRepository
    {
        public DeviceRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Device>> GetDevicesWithDetailsAsync()
        {
            return await _dbSet
                .Include(d => d.Cpu)
                    .ThenInclude(c => c.Manufacturer)
                .Include(d => d.Gpu)
                    .ThenInclude(g => g.Manufacturer)
                .Include(d => d.PowerSupply)
                .Include(d => d.DeviceUsbPorts)
                .Where(d => !d.IsDeleted)
                .ToListAsync();
        }

        public async Task<Device?> GetDeviceWithDetailsAsync(Guid deviceId)
        {
            return await _dbSet
                .Include(d => d.Cpu)
                    .ThenInclude(c => c.Manufacturer)
                .Include(d => d.Gpu)
                    .ThenInclude(g => g.Manufacturer)
                .Include(d => d.PowerSupply)
                .Include(d => d.DeviceUsbPorts)
                .FirstOrDefaultAsync(d => d.Id == deviceId && !d.IsDeleted);
        }

        public async Task<IEnumerable<Device>> GetDevicesByCpuManufacturerAsync(string manufacturerName)
        {
            return await _dbSet
                .Include(d => d.Cpu)
                    .ThenInclude(c => c.Manufacturer)
                .Where(d => !d.IsDeleted && d.Cpu.Manufacturer.Name == manufacturerName)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Device> Devices, int TotalCount)> GetPagedDevicesAsync(
            int page, 
            int pageSize,
            string? cpuManufacturer = null,
            string? gpuManufacturer = null,
            int? minRamInGB = null,
            string? storageType = null)
        {
            var query = _dbSet
                .Include(d => d.Cpu)
                    .ThenInclude(c => c.Manufacturer)
                .Include(d => d.Gpu)
                    .ThenInclude(g => g.Manufacturer)
                .Include(d => d.PowerSupply)
                .Where(d => !d.IsDeleted);

            // Apply filters
            if (!string.IsNullOrEmpty(cpuManufacturer))
            {
                query = query.Where(d => d.Cpu.Manufacturer.Name == cpuManufacturer);
            }

            if (!string.IsNullOrEmpty(gpuManufacturer))
            {
                query = query.Where(d => d.Gpu.Manufacturer.Name == gpuManufacturer);
            }

            if (minRamInGB.HasValue)
            {
                int minRamInMB = minRamInGB.Value * 1024;
                query = query.Where(d => d.RamSizeInMB >= minRamInMB);
            }

            if (!string.IsNullOrEmpty(storageType))
            {
                query = query.Where(d => d.StorageType == storageType);
            }

            // Get total count
            int totalCount = await query.CountAsync();

            // Apply pagination
            var devices = await query
                .OrderByDescending(d => d.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (devices, totalCount);
        }
    }
}
