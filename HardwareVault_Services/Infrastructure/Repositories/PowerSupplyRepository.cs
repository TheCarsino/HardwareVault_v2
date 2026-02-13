// ════════════════════════════════════════════════════════════
// FILE: PowerSupplyRepository.cs
// Ensures 500W PSU is one record, not duplicated per device
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
    public class PowerSupplyRepository
        : Repository<PowerSupply, int>, IPowerSupplyRepository
    {
        public PowerSupplyRepository(ApplicationDbContext context) : base(context) { }

        public async Task<PowerSupply> GetOrCreateAsync(int wattageInWatts)
        {
            // First, check if it already exists in the database
            var existing = await _dbSet
                .FirstOrDefaultAsync(p => p.WattageInWatts == wattageInWatts);

            if (existing is not null)
                return existing;

            // Also check if it's already been added to the change tracker in this batch
            var tracked = _context.ChangeTracker.Entries<PowerSupply>()
                .FirstOrDefault(e => e.Entity.WattageInWatts == wattageInWatts);

            if (tracked is not null)
                return tracked.Entity;

            // Not found anywhere - create new
            var psu = new PowerSupply 
            { 
                WattageInWatts = wattageInWatts,
                CreatedAt = DateTime.UtcNow
            };
            await _dbSet.AddAsync(psu);
            return psu;
        }
    }
}
