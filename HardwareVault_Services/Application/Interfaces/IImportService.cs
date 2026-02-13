using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using HardwareVault_Services.Application.DTOs;

namespace HardwareVault_Services.Application.Interfaces
{
    public interface IImportService
    {
        // POST /api/devices/import — parse Excel + persist devices
        Task<ImportResultDto> ImportDevicesAsync(Stream fileStream, string fileName);

        // GET /api/import/history — import history list (paged)
        Task<PagedResultDto<ImportJobDto>> GetImportHistoryAsync(int page, int pageSize);

        // GET /api/import/history (simple) — for dashboard recent widget
        Task<List<ImportJobDto>> GetRecentImportsAsync(int limit);

        // GET /api/import/{jobId} — single job with full error log
        Task<ImportJobDto?> GetImportJobAsync(Guid jobId);
    }
}
