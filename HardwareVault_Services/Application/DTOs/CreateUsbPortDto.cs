namespace HardwareVault_Services.Application.DTOs
{
    public class CreateUsbPortDto
    {
        public string UsbPortType { get; set; } = string.Empty;
        public int PortCount { get; set; }
    }
}
