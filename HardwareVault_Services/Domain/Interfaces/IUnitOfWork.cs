// ============================================================
// DOMAIN LAYER — UNIT OF WORK INTERFACE
// Aggregates all repositories and manages transactions
// ============================================================

using System;
using System.Threading.Tasks;

namespace HardwareVault_Services.Domain.Interfaces
{
    /// <summary>
    /// Unit of Work pattern — coordinates multiple repositories
    /// and ensures all changes are committed in a single transaction.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        // Specialized repositories
        IDeviceRepository Devices { get; }
        IManufacturerRepository Manufacturers { get; }
        ICpuRepository Cpus { get; }
        IGpuRepository Gpus { get; }
        IPowerSupplyRepository PowerSupplies { get; }
        IImportJobRepository ImportJobs { get; }

        /// <summary>
        /// Commits all pending changes to the database.
        /// Returns the number of affected rows.
        /// </summary>
        Task<int> SaveChangesAsync();
    }
}
