using System;
using System.Threading.Tasks;
using HardwareVault_Services.Domain.Interfaces;
using HardwareVault_Services.Infrastructure.Data;
using HardwareVault_Services.Infrastructure.Repositories;

namespace HardwareVault_Services.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        private IDeviceRepository? _devices;
        private IManufacturerRepository? _manufacturers;
        private ICpuRepository? _cpus;
        private IGpuRepository? _gpus;
        private IPowerSupplyRepository? _powerSupplies;
        private IImportJobRepository? _importJobs;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public IDeviceRepository Devices
            => _devices ??= new DeviceRepository(_context);

        public IManufacturerRepository Manufacturers
            => _manufacturers ??= new ManufacturerRepository(_context);

        public ICpuRepository Cpus
            => _cpus ??= new CpuRepository(_context);

        public IGpuRepository Gpus
            => _gpus ??= new GpuRepository(_context);

        public IPowerSupplyRepository PowerSupplies
            => _powerSupplies ??= new PowerSupplyRepository(_context);

        public IImportJobRepository ImportJobs
            => _importJobs ??= new ImportJobRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
