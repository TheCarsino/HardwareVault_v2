namespace HardwareVault_Services.Application.DTOs
{
    public class CreateDeviceDto
    {
        public int RamSizeInMB { get; set; }
        public int StorageSizeInGB { get; set; }
        public string StorageType { get; set; } = string.Empty;
        public decimal WeightInKg { get; set; }
        public int CpuId { get; set; }
        public int GpuId { get; set; }
        public int PowerSupplyId { get; set; }
        public List<CreateUsbPortDto> UsbPorts { get; set; } = new();
    }
}
