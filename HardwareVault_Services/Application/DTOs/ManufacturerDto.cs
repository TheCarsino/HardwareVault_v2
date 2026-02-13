using System;
using System.Text.Json.Serialization;

namespace HardwareVault_Services.Application.DTOs
{
    // Returned by GET /api/manufacturers — used to populate filter dropdowns
    public class ManufacturerDto
    {
        [JsonPropertyName("manufacturerId")]
        public int ManufacturerId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("type")]
        public string Type { get; set; } = "";  // "CPU" | "GPU" | "Both"

        [JsonPropertyName("website")]
        public string? Website { get; set; }

        [JsonPropertyName("cpuCount")]
        public int CpuCount { get; set; }

        [JsonPropertyName("gpuCount")]
        public int GpuCount { get; set; }

        [JsonPropertyName("deviceCount")]
        public int DeviceCount { get; set; }
    }
}
