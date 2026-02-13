using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HardwareVault_Services.Infrastructure.Data.Entities;

// partial -> merged with Infrastructure/Data/Entities/Device.Partial.cs at compile time
public partial class Device
{
    public Guid Id { get; set; }

    public int RamSizeInMb { get; set; }

    public int StorageSizeInGb { get; set; }

    public string StorageType { get; set; } = null!;  // "SSD" | "HDD"

    public int CpuId { get; set; }

    public int GpuId { get; set; }

    public int PowerSupplyId { get; set; }

    public decimal WeightInKg { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    // Forward navigations — EF Core loads these via .Include()
    public virtual Cpu Cpu { get; set; } = null!;

    public virtual Gpu Gpu { get; set; } = null!;

    public virtual PowerSupply PowerSupply { get; set; } = null!;

    // DeviceUsbPort.Device back-reference would cause JSON cycle
    [JsonIgnore]
    public virtual ICollection<DeviceUsbPort> DeviceUsbPorts { get; set; } = new List<DeviceUsbPort>();
}
