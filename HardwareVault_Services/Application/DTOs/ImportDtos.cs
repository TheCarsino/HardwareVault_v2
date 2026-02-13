// ============================================================
// All import-related DTOs in one file (they're small and cohesive)
// ============================================================

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HardwareVault_Services.Application.DTOs
{
    // Returned by POST /api/devices/import
    // HTTP 200 when all rows succeed
    // HTTP 207 Multi-Status when some rows fail
    public class ImportResultDto
    {
        [JsonPropertyName("importJobId")]
        public Guid ImportJobId { get; set; }

        [JsonPropertyName("fileName")]
        public string FileName { get; set; } = "";

        [JsonPropertyName("totalRows")]
        public int TotalRows { get; set; }

        [JsonPropertyName("successCount")]
        public int SuccessCount { get; set; }

        [JsonPropertyName("failureCount")]
        public int FailureCount { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = "";

        [JsonPropertyName("startedAt")]
        public DateTime StartedAt { get; set; }

        [JsonPropertyName("completedAt")]
        public DateTime? CompletedAt { get; set; }

        [JsonPropertyName("errors")]
        public List<ImportErrorDto> Errors { get; set; } = new();
    }

    // One entry per failed row in the import
    public class ImportErrorDto
    {
        [JsonPropertyName("row")]
        public int Row { get; set; }

        [JsonPropertyName("field")]
        public string Field { get; set; } = "";

        [JsonPropertyName("message")]
        public string Message { get; set; } = "";

        [JsonPropertyName("rawValue")]
        public string? RawValue { get; set; }
    }

    // Returned by GET /api/import/history — lightweight list item
    public class ImportJobDto
    {
        [JsonPropertyName("importJobId")]
        public Guid ImportJobId { get; set; }

        [JsonPropertyName("fileName")]
        public string FileName { get; set; } = "";

        [JsonPropertyName("totalRows")]
        public int TotalRows { get; set; }

        [JsonPropertyName("successCount")]
        public int SuccessCount { get; set; }

        [JsonPropertyName("failureCount")]
        public int FailureCount { get; set; }

        [JsonPropertyName("successRate")]
        public double SuccessRate { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = "";

        [JsonPropertyName("startedAt")]
        public DateTime StartedAt { get; set; }

        [JsonPropertyName("completedAt")]
        public DateTime? CompletedAt { get; set; }

        [JsonPropertyName("durationMs")]
        public long? DurationMs { get; set; }

        [JsonPropertyName("hasErrors")]
        public bool HasErrors { get; set; }
    }

    // Internal — created by ExcelDeviceParser, consumed by ImportService
    // Never returned to the API client
    public class DeviceImportDto
    {
        public int RowNumber { get; set; }
        public int RamSizeInMB { get; set; }
        public int StorageSizeInGB { get; set; }
        public string StorageType { get; set; } = "";
        public string CpuModel { get; set; } = "";
        public string CpuManufacturer { get; set; } = "";
        public string GpuModel { get; set; } = "";
        public string GpuManufacturer { get; set; } = "";
        public int PowerSupplyWattage { get; set; }
        public decimal WeightInKg { get; set; }
        public List<UsbPortDto> UsbPorts { get; set; } = new();
    }
}
