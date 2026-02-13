// ════════════════════════════════════════════════════════════
// FILE: ImportJobRepository.cs
// Powers the Import History page and dashboard recent imports widget
// ════════════════════════════════════════════════════════════

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HardwareVault_Services.Domain.Common;
using HardwareVault_Services.Domain.Interfaces;
using HardwareVault_Services.Infrastructure.Data;
using HardwareVault_Services.Infrastructure.Data.Entities;

namespace HardwareVault_Services.Infrastructure.Repositories
{
    public class ImportJobRepository
        : Repository<ImportJob, Guid>, IImportJobRepository
    {
        public ImportJobRepository(ApplicationDbContext context) : base(context) { }

        public async Task<PagedResult<ImportJob>> GetPagedAsync(int page, int pageSize)
        {
            var totalCount = await _dbSet.CountAsync();

            var data = await _dbSet
                .AsNoTracking()
                .OrderByDescending(j => j.StartedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<ImportJob> { Data = data, TotalCount = totalCount };
        }

        public async Task<IEnumerable<ImportJob>> GetRecentAsync(int limit)
        {
            return await _dbSet
                .AsNoTracking()
                .OrderByDescending(j => j.StartedAt)
                .Take(limit)
                .ToListAsync();
        }
    }
}
