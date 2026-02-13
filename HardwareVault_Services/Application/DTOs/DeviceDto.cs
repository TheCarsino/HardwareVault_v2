namespace HardwareVault_Services.Application.DTOs
{
    public class DeviceDto
    {
        public Guid DeviceId { get; set; }
        public int RamSizeInMB { get; set; }
        public int RamSizeInGB => RamSizeInMB / 1024;
        public int StorageSizeInGB { get; set; }
        public string StorageType { get; set; } = string.Empty;
        public decimal WeightInKg { get; set; }
        public string CpuModel { get; set; } = string.Empty;
        public string CpuManufacturer { get; set; } = string.Empty;
        public string GpuModel { get; set; } = string.Empty;
        public string GpuManufacturer { get; set; } = string.Empty;
        public int PowerSupplyWattage { get; set; }
        public List<UsbPortDto> UsbPorts { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
