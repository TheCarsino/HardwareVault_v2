using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HardwareVault_Services.Application.DTOs
{
    // Returned by GET /api/devices/statistics — powers dashboard charts
    public class DeviceStatisticsDto
    {
        [JsonPropertyName("totalDevices")]
        public int TotalDevices { get; set; }

        [JsonPropertyName("activeDevices")]
        public int ActiveDevices { get; set; }

        [JsonPropertyName("recentImportJobsCount")]
        public int RecentImportJobsCount { get; set; }

        [JsonPropertyName("lastImportDate")]
        public string? LastImportDate { get; set; }

        [JsonPropertyName("devicesByCpuManufacturer")]
        public Dictionary<string, int> DevicesByCpuManufacturer { get; set; } = new();

        [JsonPropertyName("devicesByStorageType")]
        public Dictionary<string, int> DevicesByStorageType { get; set; } = new();
    }
}
