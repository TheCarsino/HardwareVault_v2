// ════════════════════════════════════════════════════════════
// FILE: CpuRepository.cs
// Handles CPU lookup and creation
// ════════════════════════════════════════════════════════════

using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HardwareVault_Services.Domain.Interfaces;
using HardwareVault_Services.Infrastructure.Data;
using HardwareVault_Services.Infrastructure.Data.Entities;

namespace HardwareVault_Services.Infrastructure.Repositories
{
    public class CpuRepository : Repository<Cpu, int>, ICpuRepository
    {
        public CpuRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Cpu> GetOrCreateAsync(string modelName, int manufacturerId)
        {
            var normalized = modelName.Trim();

            var existing = await _dbSet.FirstOrDefaultAsync(c =>
                c.ModelName.ToLower() == normalized.ToLower());

            if (existing is not null)
                return existing;

            var cpu = new Cpu 
            { 
                ModelName = normalized, 
                ManufacturerId = manufacturerId,
                NormalizedModelName = normalized.ToLower(),
                CreatedAt = DateTime.UtcNow
            };
            await _dbSet.AddAsync(cpu);
            return cpu;
        }
    }
}
