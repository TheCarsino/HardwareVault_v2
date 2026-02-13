using System.Threading.Tasks;
using HardwareVault_Services.Infrastructure.Data.Entities;

namespace HardwareVault_Services.Domain.Interfaces
{
    public interface IGpuRepository : IRepository<Gpu, int>
    {
        Task<Gpu> GetOrCreateAsync(string modelName, int manufacturerId);
    }
}
