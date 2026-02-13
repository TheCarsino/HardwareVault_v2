using System;
using System.Collections.Generic;

namespace HardwareVault_Services.Infrastructure.Data.Entities;

public partial class Device
{
    public Guid Id { get; set; }

    public int RamSizeInMb { get; set; }

    public int StorageSizeInGb { get; set; }

    public string StorageType { get; set; } = null!;

    public int CpuId { get; set; }

    public int GpuId { get; set; }

    public int PowerSupplyId { get; set; }

    public decimal WeightInKg { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Cpu Cpu { get; set; } = null!;

    public virtual ICollection<DeviceUsbPort> DeviceUsbPorts { get; set; } = new List<DeviceUsbPort>();

    public virtual Gpu Gpu { get; set; } = null!;

    public virtual PowerSupply PowerSupply { get; set; } = null!;
}
