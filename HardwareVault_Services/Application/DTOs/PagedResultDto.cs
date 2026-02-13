using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HardwareVault_Services.Application.DTOs
{
    // Generic paged wrapper — used by devices list and import history
    public class PagedResultDto<T>
    {
        [JsonPropertyName("data")]
        public List<T> Data { get; set; } = new();

        [JsonPropertyName("totalCount")]
        public int TotalCount { get; set; }

        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }

        [JsonPropertyName("totalPages")]
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

        [JsonPropertyName("hasNextPage")]
        public bool HasNextPage => Page < TotalPages;

        [JsonPropertyName("hasPreviousPage")]
        public bool HasPreviousPage => Page > 1;
    }
}
