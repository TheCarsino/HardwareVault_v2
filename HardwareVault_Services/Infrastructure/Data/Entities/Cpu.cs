using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HardwareVault_Services.Infrastructure.Data.Entities;

public partial class Cpu
{
    public int Id { get; set; }

    public string ModelName { get; set; } = null!;

    public int ManufacturerId { get; set; }

    public string? NormalizedModelName { get; set; }

    public DateTime CreatedAt { get; set; }

    // Back-reference — hidden from JSON (Device already owns this relationship)
    [JsonIgnore]
    public virtual ICollection<Device> Devices { get; set; } = new List<Device>();

    // Forward navigation — included in API responses (shows manufacturer name)
    public virtual Manufacturer Manufacturer { get; set; } = null!;
}
