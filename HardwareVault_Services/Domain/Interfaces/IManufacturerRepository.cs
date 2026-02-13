using System.Collections.Generic;
using System.Threading.Tasks;
using HardwareVault_Services.Infrastructure.Data.Entities;

namespace HardwareVault_Services.Domain.Interfaces
{
    public interface IManufacturerRepository : IRepository<Manufacturer, int>
    {
        // Returns all manufacturers with their CPU/GPU counts for the admin page
        Task<IEnumerable<Manufacturer>> GetAllWithCountsAsync();

        // Find by name (case-insensitive) or create if missing.
        // AMD special case: if found as CPU and requested as GPU, upgrades to Both.
        // Does NOT call SaveChangesAsync — caller batches with UoW.
        Task<Manufacturer> GetOrCreateAsync(string name, string type);
    }
}
