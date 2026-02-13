// ============================================================
// DOMAIN LAYER — PARTIAL BUSINESS CLASSES
// ============================================================
// DATABASE-FIRST PATTERN:
//
//   Infrastructure/Data/Entities/ImportJob.cs         ← SCAFFOLDED (data props)
//   Infrastructure/Data/Entities/ImportJob.Partial.cs ← THIS FILE (business logic)
//
// Both are declared as: public partial class ImportJob { }
// The C# compiler merges them into one class at build time.
//
// RULE: Never edit the scaffolded file — only this one.
//       When the schema changes -> re-scaffold -> re-apply partial additions.
// ============================================================

using System;
using HardwareVault_Services.Domain.Enums;

namespace HardwareVault_Services.Infrastructure.Data.Entities
{
    public partial class ImportJob
    {
        // -- Factory Method 
        // Creates a new job in Pending state before parsing begins.
        // The job is persisted immediately so if the server crashes mid-import,
        // evidence of the attempt still exists.
        public static ImportJob Create(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("File name cannot be empty.", nameof(fileName));

            return new ImportJob
            {
                Id           = Guid.NewGuid(),
                FileName     = fileName.Trim(),
                TotalRows    = 0,
                SuccessCount = 0,
                FailureCount = 0,
                Status       = ImportJobStatus.Pending.ToString(),
                ErrorLog     = null,
                StartedAt    = DateTime.UtcNow,
                CompletedAt  = null,
                CreatedBy    = null
            };
        }

        // -- Business Methods 

        // Called immediately before the parser begins.
        public void Start()
        {
            if (Status != ImportJobStatus.Pending.ToString())
                throw new InvalidOperationException(
                    $"Cannot start a job that is already '{Status}'.");

            Status = ImportJobStatus.Processing.ToString();
        }

        // Called after all rows are processed.
        // A job with some failures is still "Completed" — it ran to completion.
        // "Failed" = the job crashed and could not run at all.
        public void Complete(int totalRows, int successCount, int failureCount, string? errorLogJson)
        {
            if (Status != ImportJobStatus.Processing.ToString())
                throw new InvalidOperationException(
                    $"Cannot complete a job that is in status '{Status}'.");

            TotalRows    = totalRows;
            SuccessCount = successCount;
            FailureCount = failureCount;
            ErrorLog     = errorLogJson;
            Status       = ImportJobStatus.Completed.ToString();
            CompletedAt  = DateTime.UtcNow;
        }

        // Called when the file is unreadable or an unhandled exception crashes the import.
        public void Fail(string reason)
        {
            Status      = ImportJobStatus.Failed.ToString();
            ErrorLog    = reason;
            CompletedAt = DateTime.UtcNow;
        }

        // -- Computed Properties 
        public double SuccessRate =>
            TotalRows > 0 ? Math.Round((double)SuccessCount / TotalRows * 100, 1) : 0;

        public long? DurationMs =>
            CompletedAt.HasValue
                ? (long)(CompletedAt.Value - StartedAt).TotalMilliseconds
                : null;
    }
}
