using System;
using System.Collections.Generic;

namespace HardwareVault_Services.Infrastructure.Data.Entities;

public partial class ImportJob
{
    public Guid Id { get; set; }

    public string FileName { get; set; } = null!;

    public int TotalRows { get; set; }

    public int SuccessCount { get; set; }

    public int FailureCount { get; set; }

    public string Status { get; set; } = null!;

    public string? ErrorLog { get; set; }

    public DateTime StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public string? CreatedBy { get; set; }
}
