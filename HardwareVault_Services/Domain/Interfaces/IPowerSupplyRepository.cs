using System.Threading.Tasks;
using HardwareVault_Services.Infrastructure.Data.Entities;

namespace HardwareVault_Services.Domain.Interfaces
{
    public interface IPowerSupplyRepository : IRepository<PowerSupply, int>
    {
        // Find PSU by wattage or create if not found.
        // Ensures 500W PSU is one record, not duplicated per device.
        Task<PowerSupply> GetOrCreateAsync(int wattageInWatts);
    }
}
