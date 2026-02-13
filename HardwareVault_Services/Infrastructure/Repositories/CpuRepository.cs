// ════════════════════════════════════════════════════════════
// FILE: CpuRepository.cs
// Handles CPU lookup and creation
// ════════════════════════════════════════════════════════════

using System;
using System.Linq;
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

            // First, check if it already exists in the database
            var existing = await _dbSet.FirstOrDefaultAsync(c =>
                c.ModelName.ToLower() == normalized.ToLower());

            if (existing is not null)
                return existing;

            // Also check if it's already been added to the change tracker in this batch
            var tracked = _context.ChangeTracker.Entries<Cpu>()
                .FirstOrDefault(e => e.Entity.ModelName.ToLower() == normalized.ToLower());

            if (tracked is not null)
                return tracked.Entity;

            // Not found anywhere - create new
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
