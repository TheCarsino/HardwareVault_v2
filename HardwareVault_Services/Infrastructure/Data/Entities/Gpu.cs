using System;
using System.Collections.Generic;

namespace HardwareVault_Services.Infrastructure.Data.Entities;

public partial class Gpu
{
    public int Id { get; set; }

    public string ModelName { get; set; } = null!;

    public int ManufacturerId { get; set; }

    public string? NormalizedModelName { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Device> Devices { get; set; } = new List<Device>();

    public virtual Manufacturer Manufacturer { get; set; } = null!;
}
