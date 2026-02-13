namespace HardwareVault_Services.Application.DTOs
{
    public class ManufacturerDto
    {
        public int ManufacturerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? Website { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
