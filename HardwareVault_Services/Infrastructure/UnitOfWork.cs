using HardwareVault_Services.Domain.Entities;
using HardwareVault_Services.Domain.Interfaces;
using HardwareVault_Services.Infrastructure.Data;
using HardwareVault_Services.Infrastructure.Repositories;

namespace HardwareVault_Services.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        
        public IDeviceRepository Devices { get; }
        public IRepository<Manufacturer> Manufacturers { get; }
        public IRepository<Cpu> Cpus { get; }
        public IRepository<Gpu> Gpus { get; }
        public IRepository<PowerSupply> PowerSupplies { get; }
        public IRepository<ImportJob> ImportJobs { get; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Devices = new DeviceRepository(_context);
            Manufacturers = new Repository<Manufacturer>(_context);
            Cpus = new Repository<Cpu>(_context);
            Gpus = new Repository<Gpu>(_context);
            PowerSupplies = new Repository<PowerSupply>(_context);
            ImportJobs = new Repository<ImportJob>(_context);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> SaveChangesWithResultAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
