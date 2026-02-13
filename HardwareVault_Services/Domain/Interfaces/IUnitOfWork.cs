using HardwareVault_Services.Domain.Entities;

namespace HardwareVault_Services.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IDeviceRepository Devices { get; }
        IRepository<Manufacturer> Manufacturers { get; }
        IRepository<Cpu> Cpus { get; }
        IRepository<Gpu> Gpus { get; }
        IRepository<PowerSupply> PowerSupplies { get; }
        IRepository<ImportJob> ImportJobs { get; }
        
        Task<int> SaveChangesAsync();
        Task<bool> SaveChangesWithResultAsync();
    }
}
