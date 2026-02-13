using System.Collections.Generic;

namespace HardwareVault_Services.Application.DTOs
{
    // Received by PUT /api/devices/{id}
    // All fields nullable — only provided fields are updated (PATCH-like behavior)
    public class UpdateDeviceDto
    {
        public int? RamSizeInMB { get; set; }
        public int? StorageSizeInGB { get; set; }
        public string? StorageType { get; set; }
        public decimal? WeightInKg { get; set; }
        public int? CpuId { get; set; }
        public int? GpuId { get; set; }
        public int? PowerSupplyId { get; set; }

        // If provided, replaces all USB ports entirely (clear + re-add)
        public List<UsbPortDto>? UsbPorts { get; set; }
    }
}
