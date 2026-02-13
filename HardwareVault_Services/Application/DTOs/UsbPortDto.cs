using System.Text.Json.Serialization;

namespace HardwareVault_Services.Application.DTOs
{
    // Nested inside DeviceDto — one entry per USB port type
    public class UsbPortDto
    {
        [JsonPropertyName("usbPortType")]
        public string PortType { get; set; } = "";

        [JsonPropertyName("count")]
        public int Count { get; set; }
    }
}
