namespace HardwareVault_Services.Application.DTOs
{
    public class UpdateDeviceDto
    {
        public int? RamSizeInMB { get; set; }
        public int? StorageSizeInGB { get; set; }
        public string? StorageType { get; set; }
        public decimal? WeightInKg { get; set; }
        public int? CpuId { get; set; }
        public int? GpuId { get; set; }
        public int? PowerSupplyId { get; set; }
    }
}
