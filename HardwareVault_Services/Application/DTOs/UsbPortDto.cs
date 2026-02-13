namespace HardwareVault_Services.Application.DTOs
{
    public class UsbPortDto
    {
        public Guid DeviceUsbPortId { get; set; }
        public string UsbPortType { get; set; } = string.Empty;
        public int PortCount { get; set; }
    }
}
