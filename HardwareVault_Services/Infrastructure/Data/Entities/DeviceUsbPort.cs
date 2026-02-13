using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HardwareVault_Services.Infrastructure.Data.Entities;

public partial class DeviceUsbPort
{
    public Guid Id { get; set; }

    public Guid DeviceId { get; set; }

    public string UsbPortType { get; set; } = null!;  // "USB 2.0" | "USB 3.0" | "USB C"

    public int PortCount { get; set; }

    // Back-reference to Device — hidden from JSON
    [JsonIgnore]
    public virtual Device Device { get; set; } = null!;
}
