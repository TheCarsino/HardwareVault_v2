namespace HardwareVault_Services.Application.DTOs
{
    public class GpuDto
    {
        public int GpuId { get; set; }
        public string ModelName { get; set; } = string.Empty;
        public string ManufacturerName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
