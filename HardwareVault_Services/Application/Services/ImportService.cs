// ============================================================
// APPLICATION LAYER — IMPORT SERVICE
// Orchestrates the Excel import use case in 8 steps:
//   1.  Create ImportJob (Pending) — persisted before work begins
//   2.  Mark job as Processing
//   3.  Parse the Excel file via IExcelDeviceParser
//   4.  For each parsed row: resolve lookup entities + persist device
//   5.  Commit all devices in ONE transaction
//   6.  Consolidate parse errors + persist errors
//   7.  Mark job Completed with error log JSON
//   8.  Return ImportResultDto to controller
//
// If anything catastrophic fails -> mark job Failed, re-throw
// so the controller returns 500 with a safe error message.
// ============================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using HardwareVault_Services.Application.DTOs;
using HardwareVault_Services.Application.Interfaces;
using HardwareVault_Services.Domain.Interfaces;
using HardwareVault_Services.Infrastructure.Data.Entities;

namespace HardwareVault_Services.Application.Services
{
    public class ImportService : IImportService
    {
        private readonly IUnitOfWork _uow;
        private readonly IExcelDeviceParser _parser;
        private readonly ILogger<ImportService> _logger;

        public ImportService(
            IUnitOfWork uow,
            IExcelDeviceParser parser,
            ILogger<ImportService> logger)
        {
            _uow    = uow    ?? throw new ArgumentNullException(nameof(uow));
            _parser = parser ?? throw new ArgumentNullException(nameof(parser));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // -- ImportDevicesAsync 
        public async Task<ImportResultDto> ImportDevicesAsync(
            Stream fileStream, string fileName)
        {
            _logger.LogInformation("Import started: {FileName}", fileName);

            // STEP 1 — Create the job record immediately.
            // If the server crashes mid-import, the Pending record still exists.
            var job = ImportJob.Create(fileName);
            await _uow.ImportJobs.AddAsync(job);
            await _uow.SaveChangesAsync();

            try
            {
                // STEP 2 — Mark Processing
                job.Start();
                await _uow.SaveChangesAsync();

                // STEP 3 — Parse
                // Parser collects per-row errors internally. Never throws for row errors.
                // Only throws if the file itself is completely unreadable.
                var parseResult = _parser.Parse(fileStream);

                _logger.LogInformation(
                    "Parsing done — {S} ok, {F} failed | Job: {Id}",
                    parseResult.SuccessCount, parseResult.FailureCount, job.Id);

                // STEP 4 — Persist each successfully parsed row
                var persistErrors = new List<ParseError>();

                foreach (var row in parseResult.SuccessfulRows)
                {
                    try
                    {
                        await PersistRowAsync(row);
                    }
                    catch (Exception ex)
                    {
                        // Row was valid Excel data but failed to write to DB
                        // (e.g., unique constraint violation, FK missing).
                        // Capture as error — do not abort the rest of the import.
                        _logger.LogWarning(ex,
                            "Row {Row} parsed but failed to persist: {Msg}",
                            row.RowNumber, ex.Message);

                        persistErrors.Add(new ParseError
                        {
                            RowNumber    = row.RowNumber,
                            ErrorMessage = $"Database error: {ex.Message}",
                            FieldName    = "database"
                        });
                    }
                }

                // STEP 5 — One SaveChangesAsync commits ALL device inserts atomically.
                // Every AddAsync above queued EF Core change-tracker entries.
                // This single call wraps them all in one SQL transaction.
                await _uow.SaveChangesAsync();

                // STEP 6 — Merge parse errors + persist errors
                var allErrors = parseResult.FailedRows.Concat(persistErrors).ToList();
                int finalSuccess = parseResult.SuccessCount - persistErrors.Count;

                // STEP 7 — Serialize error log to JSON and complete the job
                var errorJson = allErrors.Count > 0
                    ? JsonSerializer.Serialize(allErrors.Select(e => new
                      {
                          e.RowNumber,
                          e.ErrorMessage,
                          e.FieldName,
                          e.FieldValue
                      }))
                    : null;

                job.Complete(parseResult.TotalRows, finalSuccess, allErrors.Count, errorJson);
                await _uow.SaveChangesAsync();

                _logger.LogInformation(
                    "Import complete — success: {S}, failed: {F} | Job: {Id}",
                    finalSuccess, allErrors.Count, job.Id);

                // STEP 8 — Return result
                return new ImportResultDto
                {
                    ImportJobId  = job.Id,
                    FileName     = job.FileName,
                    TotalRows    = parseResult.TotalRows,
                    SuccessCount = finalSuccess,
                    FailureCount = allErrors.Count,
                    Status       = job.Status,
                    StartedAt    = job.StartedAt,
                    CompletedAt  = job.CompletedAt,
                    Errors       = allErrors.Select(e => new ImportErrorDto
                    {
                        Row      = e.RowNumber,
                        Field    = e.FieldName ?? "unknown",
                        Message  = e.ErrorMessage,
                        RawValue = e.FieldValue
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                // Catastrophic failure — file unreadable, or DB completely unavailable
                _logger.LogError(ex,
                    "Critical import failure | Job: {Id} | File: {File}",
                    job.Id, fileName);

                job.Fail($"Critical error: {ex.Message}");
                await _uow.SaveChangesAsync();

                throw; // Re-throw so controller returns 500
            }
        }

        // -- GetImportHistoryAsync (paged) 
        public async Task<PagedResultDto<ImportJobDto>> GetImportHistoryAsync(
            int page, int pageSize)
        {
            var result = await _uow.ImportJobs.GetPagedAsync(page, pageSize);

            return new PagedResultDto<ImportJobDto>
            {
                Data       = result.Data.Select(MapToJobDto).ToList(),
                TotalCount = result.TotalCount,
                Page       = page,
                PageSize   = pageSize
            };
        }

        // -- GetRecentImportsAsync (dashboard widget) 
        public async Task<List<ImportJobDto>> GetRecentImportsAsync(int limit)
        {
            var jobs = await _uow.ImportJobs.GetRecentAsync(limit);
            return jobs.Select(MapToJobDto).ToList();
        }

        // -- GetImportJobAsync 
        public async Task<ImportJobDto?> GetImportJobAsync(Guid jobId)
        {
            var job = await _uow.ImportJobs.GetByIdAsync(jobId);
            return job is null ? null : MapToJobDto(job);
        }

        // ====================================================================
        // PRIVATE HELPERS
        // ====================================================================

        // -- PersistRowAsync 
        // Resolves all lookup entities for one parsed row, then creates
        // and queues the Device entity.
        // Does NOT call SaveChangesAsync — the caller batches all rows first.
        private async Task PersistRowAsync(DeviceImportDto row)
        {
            // Resolve CPU Manufacturer (get-or-create — no duplicate rows)
            var cpuMfr = await _uow.Manufacturers
                .GetOrCreateAsync(row.CpuManufacturer, "CPU");

            // Resolve CPU model (get-or-create)
            var cpu = await _uow.Cpus
                .GetOrCreateAsync(row.CpuModel, cpuMfr.Id);

            // Resolve GPU Manufacturer
            var gpuMfr = await _uow.Manufacturers
                .GetOrCreateAsync(row.GpuManufacturer, "GPU");

            // Resolve GPU model
            var gpu = await _uow.Gpus
                .GetOrCreateAsync(row.GpuModel, gpuMfr.Id);

            // Resolve Power Supply (get-or-create by wattage)
            var psu = await _uow.PowerSupplies
                .GetOrCreateAsync(row.PowerSupplyWattage);

            // Create Device entity via factory method.
            // Device.Create() validates business rules — throws if invalid.
            var device = Device.Create(
                row.RamSizeInMB,
                row.StorageSizeInGB,
                row.StorageType.ToUpper(),
                cpu.Id,
                gpu.Id,
                psu.Id,
                row.WeightInKg);

            // Attach USB ports
            foreach (var port in row.UsbPorts)
                device.AddUsbPort(port.PortType, port.Count);

            // Queue for insertion — EF Core tracks this change
            // Actual INSERT happens when SaveChangesAsync is called
            await _uow.Devices.AddAsync(device);
        }

        // -- Mapping: ImportJob entity -> ImportJobDto 
        private static ImportJobDto MapToJobDto(ImportJob j) => new()
        {
            ImportJobId  = j.Id,
            FileName     = j.FileName,
            TotalRows    = j.TotalRows,
            SuccessCount = j.SuccessCount,
            FailureCount = j.FailureCount,
            SuccessRate  = j.SuccessRate,
            Status       = j.Status,
            StartedAt    = j.StartedAt,
            CompletedAt  = j.CompletedAt,
            DurationMs   = j.DurationMs,
            HasErrors    = j.FailureCount > 0
        };
    }
}
