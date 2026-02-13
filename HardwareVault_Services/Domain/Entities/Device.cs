namespace HardwareVault_Services.Domain.Entities
{
    public class Device
    {
        public Guid Id { get; set; }
        public int RamSizeInMB { get; set; }
        public int StorageSizeInGB { get; set; }
        public string StorageType { get; set; } = string.Empty;
        public int CpuId { get; set; }
        public int GpuId { get; set; }
        public int PowerSupplyId { get; set; }
        public decimal WeightInKg { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        // Navigation properties
        public Cpu Cpu { get; set; } = null!;
        public Gpu Gpu { get; set; } = null!;
        public PowerSupply PowerSupply { get; set; } = null!;
        public ICollection<DeviceUsbPort> DeviceUsbPorts { get; set; } = new List<DeviceUsbPort>();
    }
}
