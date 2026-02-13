namespace HardwareVault_Services.Domain.Entities
{
    public class Gpu
    {
        public int Id { get; set; }
        public string ModelName { get; set; } = string.Empty;
        public int ManufacturerId { get; set; }
        public string? NormalizedModelName { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public Manufacturer Manufacturer { get; set; } = null!;
        public ICollection<Device> Devices { get; set; } = new List<Device>();
    }
}
