namespace HardwareVault_Services.Domain.Entities
{
    public class DeviceUsbPort
    {
        public Guid Id { get; set; }
        public Guid DeviceId { get; set; }
        public string UsbPortType { get; set; } = string.Empty;
        public int PortCount { get; set; }

        // Navigation property
        public Device Device { get; set; } = null!;
    }
}
