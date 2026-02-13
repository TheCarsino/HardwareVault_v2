namespace HardwareVault_Services.Domain.Entities
{
    public class Manufacturer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ProductType { get; set; } = string.Empty;
        public string? Website { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public ICollection<Cpu> Cpus { get; set; } = new List<Cpu>();
        public ICollection<Gpu> Gpus { get; set; } = new List<Gpu>();
    }
}
