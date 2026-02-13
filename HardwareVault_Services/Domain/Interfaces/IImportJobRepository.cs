using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HardwareVault_Services.Domain.Common;
using HardwareVault_Services.Infrastructure.Data.Entities;

namespace HardwareVault_Services.Domain.Interfaces
{
    public interface IImportJobRepository : IRepository<ImportJob, Guid>
    {
        // Paged list ordered by StartedAt desc — powers the Import History page
        Task<PagedResult<ImportJob>> GetPagedAsync(int page, int pageSize);

        // Simple limit-based list — used by the dashboard "recent imports" widget
        Task<IEnumerable<ImportJob>> GetRecentAsync(int limit);
    }
}
