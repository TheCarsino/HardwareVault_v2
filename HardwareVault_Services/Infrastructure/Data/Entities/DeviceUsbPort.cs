using System;
using System.Collections.Generic;

namespace HardwareVault_Services.Infrastructure.Data.Entities;

public partial class DeviceUsbPort
{
    public Guid Id { get; set; }

    public Guid DeviceId { get; set; }

    public string UsbPortType { get; set; } = null!;

    public int PortCount { get; set; }

    public virtual Device Device { get; set; } = null!;
}
