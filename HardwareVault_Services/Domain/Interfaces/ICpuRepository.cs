using System.Threading.Tasks;
using HardwareVault_Services.Infrastructure.Data.Entities;

namespace HardwareVault_Services.Domain.Interfaces
{
    public interface ICpuRepository : IRepository<Cpu, int>
    {
        // Find CPU by model name + manufacturer, or create if not found.
        // Does NOT call SaveChangesAsync.
        Task<Cpu> GetOrCreateAsync(string modelName, int manufacturerId);
    }
}
