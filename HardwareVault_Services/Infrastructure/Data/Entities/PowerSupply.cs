using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HardwareVault_Services.Infrastructure.Data.Entities;

public partial class PowerSupply
{
    public int Id { get; set; }

    public int WattageInWatts { get; set; }

    public DateTime CreatedAt { get; set; }

    // Back-reference — hidden from JSON (Device already owns this relationship)
    [JsonIgnore]
    public virtual ICollection<Device> Devices { get; set; } = new List<Device>();
}
