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

        [JsonPropertyName("deletedDevices")]
        public int DeletedDevices { get; set; }

        [JsonPropertyName("ssdCount")]
        public int SsdCount { get; set; }

        [JsonPropertyName("hddCount")]
        public int HddCount { get; set; }

        [JsonPropertyName("averageRamInGB")]
        public double AverageRamInGB { get; set; }

        [JsonPropertyName("averageStorageInGB")]
        public double AverageStorageInGB { get; set; }

        [JsonPropertyName("byCpuManufacturer")]
        public Dictionary<string, int> ByCpuManufacturer { get; set; } = new();

        [JsonPropertyName("byGpuManufacturer")]
        public Dictionary<string, int> ByGpuManufacturer { get; set; } = new();
    }
}
