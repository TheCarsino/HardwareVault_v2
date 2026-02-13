using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HardwareVault_Services.Application.DTOs
{
    // Returned by GET /api/devices and GET /api/devices/{id}
    // Matches TypeScript Device interface with nested objects
    public class DeviceDto
    {
        [JsonPropertyName("deviceId")]
        public string DeviceId { get; set; } = "";

        [JsonPropertyName("ramSizeInMB")]
        public int RamSizeInMB { get; set; }

        [JsonPropertyName("storageSizeInGB")]
        public int StorageSizeInGB { get; set; }

        [JsonPropertyName("storageType")]
        public string StorageType { get; set; } = "";  // "SSD" | "HDD"

        [JsonPropertyName("weightInKg")]
        public decimal WeightInKg { get; set; }

        [JsonPropertyName("createdAt")]
        public string CreatedAt { get; set; } = "";

        [JsonPropertyName("updatedAt")]
        public string UpdatedAt { get; set; } = "";

        [JsonPropertyName("cpu")]
        public CpuInfoDto Cpu { get; set; } = new();

        [JsonPropertyName("gpu")]
        public GpuInfoDto Gpu { get; set; } = new();

        [JsonPropertyName("powerSupply")]
        public PowerSupplyInfoDto PowerSupply { get; set; } = new();

        [JsonPropertyName("usbPorts")]
        public List<UsbPortDto> UsbPorts { get; set; } = new();
    }

    // Nested CPU information
    public class CpuInfoDto
    {
        [JsonPropertyName("cpuId")]
        public int CpuId { get; set; }

        [JsonPropertyName("modelName")]
        public string ModelName { get; set; } = "";

        [JsonPropertyName("manufacturer")]
        public ManufacturerInfoDto Manufacturer { get; set; } = new();
    }

    // Nested GPU information
    public class GpuInfoDto
    {
        [JsonPropertyName("gpuId")]
        public int GpuId { get; set; }

        [JsonPropertyName("modelName")]
        public string ModelName { get; set; } = "";

        [JsonPropertyName("manufacturer")]
        public ManufacturerInfoDto Manufacturer { get; set; } = new();
    }

    // Nested power supply information
    public class PowerSupplyInfoDto
    {
        [JsonPropertyName("powerSupplyId")]
        public int PowerSupplyId { get; set; }

        [JsonPropertyName("wattageInWatts")]
        public int WattageInWatts { get; set; }
    }

    // Manufacturer information (nested in CPU/GPU)
    public class ManufacturerInfoDto
    {
        [JsonPropertyName("manufacturerId")]
        public int ManufacturerId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("type")]
        public string Type { get; set; } = "";  // "CPU" | "GPU" | "Both"
    }
}
