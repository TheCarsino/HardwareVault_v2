using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HardwareVault_Services.Infrastructure.Data.Entities;

public partial class Manufacturer
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string ProductType { get; set; } = null!;

    public string? Website { get; set; }

    public DateTime CreatedAt { get; set; }

    // Back-references — JsonIgnore prevents serialization cycle:
    //   Manufacturer -> Cpu -> Manufacturer -> Cpu -> ...
    [JsonIgnore]
    public virtual ICollection<Cpu> Cpus { get; set; } = new List<Cpu>();

    [JsonIgnore]
    public virtual ICollection<Gpu> Gpus { get; set; } = new List<Gpu>();
}
