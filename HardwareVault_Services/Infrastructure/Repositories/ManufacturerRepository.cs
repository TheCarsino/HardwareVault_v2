// ════════════════════════════════════════════════════════════
// FILE: ManufacturerRepository.cs
// Handles manufacturer lookup and creation with AMD special case
// ════════════════════════════════════════════════════════════

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HardwareVault_Services.Domain.Interfaces;
using HardwareVault_Services.Infrastructure.Data;
using HardwareVault_Services.Infrastructure.Data.Entities;

namespace HardwareVault_Services.Infrastructure.Repositories
{
    public class ManufacturerRepository
        : Repository<Manufacturer, int>, IManufacturerRepository
    {
        public ManufacturerRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Manufacturer>> GetAllWithCountsAsync()
        {
            return await _dbSet
                .Include(m => m.Cpus)
                .Include(m => m.Gpus)
                .AsNoTracking()
                .OrderBy(m => m.Name)
                .ToListAsync();
        }

        // Find by name (case-insensitive) OR create new.
        // AMD special case: if already exists as CPU and now needed for GPU,
        // upgrade its ProductType to "Both" — no duplicate manufacturer rows.
        public async Task<Manufacturer> GetOrCreateAsync(string name, string type)
        {
            var normalized = name.Trim();

            var existing = await _dbSet.FirstOrDefaultAsync(m =>
                m.Name.ToLower() == normalized.ToLower());

            if (existing is not null)
            {
                bool needsUpgrade =
                    (existing.ProductType == "CPU" && type == "GPU") ||
                    (existing.ProductType == "GPU" && type == "CPU");

                if (needsUpgrade)
                    existing.ProductType = "Both";

                return existing;
            }

            var manufacturer = new Manufacturer 
            { 
                Name = normalized, 
                ProductType = type,
                CreatedAt = DateTime.UtcNow
            };
            await _dbSet.AddAsync(manufacturer);
            return manufacturer;
            // Caller must SaveChangesAsync via UnitOfWork
        }
    }
}
