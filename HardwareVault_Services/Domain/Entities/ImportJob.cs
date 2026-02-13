namespace HardwareVault_Services.Domain.Entities
{
    public class ImportJob
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public int TotalRows { get; set; }
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
        public string Status { get; set; } = "Pending";
        public string? ErrorLog { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? CreatedBy { get; set; }
    }
}
