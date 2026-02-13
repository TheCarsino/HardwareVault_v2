using System;
using System.Collections.Generic;

namespace HardwareVault_Services.Infrastructure.Data.Entities;

public partial class PowerSupply
{
    public int Id { get; set; }

    public int WattageInWatts { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Device> Devices { get; set; } = new List<Device>();
}
