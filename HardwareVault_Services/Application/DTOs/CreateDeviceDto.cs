using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HardwareVault_Services.Application.DTOs
{
    // Received by POST /api/devices
    public class CreateDeviceDto
    {
        [Required]
        [Range(512, 1_048_576, ErrorMessage = "RAM must be between 512 MB and 1 TB")]
        public int RamSizeInMB { get; set; }

        [Required]
        [Range(1, 100_000, ErrorMessage = "Storage must be between 1 GB and 100 TB")]
        public int StorageSizeInGB { get; set; }

        [Required]
        [RegularExpression("^(SSD|HDD)$", ErrorMessage = "StorageType must be 'SSD' or 'HDD'")]
        public string StorageType { get; set; } = "";

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "CpuId must be a positive integer")]
        public int CpuId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "GpuId must be a positive integer")]
        public int GpuId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "PowerSupplyId must be a positive integer")]
        public int PowerSupplyId { get; set; }

        [Required]
        [Range(0.1, 500, ErrorMessage = "Weight must be between 0.1 kg and 500 kg")]
        public decimal WeightInKg { get; set; }

        public List<UsbPortDto> UsbPorts { get; set; } = new();
    }
}
