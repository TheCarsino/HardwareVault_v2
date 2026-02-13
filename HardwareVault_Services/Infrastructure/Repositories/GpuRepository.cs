// ════════════════════════════════════════════════════════════
// FILE: GpuRepository.cs
// Handles GPU lookup and creation
// ════════════════════════════════════════════════════════════

using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HardwareVault_Services.Domain.Interfaces;
using HardwareVault_Services.Infrastructure.Data;
using HardwareVault_Services.Infrastructure.Data.Entities;

namespace HardwareVault_Services.Infrastructure.Repositories
{
    public class GpuRepository : Repository<Gpu, int>, IGpuRepository
    {
        public GpuRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Gpu> GetOrCreateAsync(string modelName, int manufacturerId)
        {
            var normalized = modelName.Trim();

            var existing = await _dbSet.FirstOrDefaultAsync(g =>
                g.ModelName.ToLower() == normalized.ToLower());

            if (existing is not null)
                return existing;

            var gpu = new Gpu 
            { 
                ModelName = normalized, 
                ManufacturerId = manufacturerId,
                NormalizedModelName = normalized.ToLower(),
                CreatedAt = DateTime.UtcNow
            };
            await _dbSet.AddAsync(gpu);
            return gpu;
        }
    }
}
