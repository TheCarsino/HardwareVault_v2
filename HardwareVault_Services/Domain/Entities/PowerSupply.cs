namespace HardwareVault_Services.Domain.Entities
{
    public class PowerSupply
    {
        public int Id { get; set; }
        public int WattageInWatts { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public ICollection<Device> Devices { get; set; } = new List<Device>();
    }
}
