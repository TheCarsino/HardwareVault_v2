using System;
using System.Collections.Generic;

namespace HardwareVault_Services.Infrastructure.Data.Entities;

public partial class VwDeviceList
{
    public Guid Id { get; set; }

    public int RamSizeInMb { get; set; }

    public int StorageSizeInGb { get; set; }

    public string StorageType { get; set; } = null!;

    public decimal WeightInKg { get; set; }

    public string CpuModel { get; set; } = null!;

    public string CpuManufacturer { get; set; } = null!;

    public string GpuModel { get; set; } = null!;

    public string GpuManufacturer { get; set; } = null!;

    public int PowerSupplyWattage { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
