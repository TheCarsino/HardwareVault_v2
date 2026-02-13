// ============================================================
// DOMAIN LAYER — COMMON MODELS
// ============================================================
// Shared result wrapper used by paged repository methods.

using System.Collections.Generic;
using System.Linq;

namespace HardwareVault_Services.Domain.Common
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Data       { get; set; } = Enumerable.Empty<T>();
        public int            TotalCount { get; set; }
    }

    // Returned by GetStatisticsAsync — powers dashboard cards and charts
    public class DeviceStatistics
    {
        public int TotalDevices  { get; set; }
        public int ActiveDevices { get; set; }
        public int SsdCount      { get; set; }
        public int HddCount      { get; set; }
        public Dictionary<string, int> ByCpuManufacturer { get; set; } = new();
        public Dictionary<string, int> ByGpuManufacturer { get; set; } = new();
        public double AverageRamInGB     { get; set; }
        public double AverageStorageInGB { get; set; }
    }
}
