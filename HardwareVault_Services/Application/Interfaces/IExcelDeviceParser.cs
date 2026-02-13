// ============================================================
// APPLICATION LAYER — EXCEL PARSER INTERFACE
// Abstracts the parsing implementation from the service layer
// ============================================================

using System.Collections.Generic;
using System.IO;
using HardwareVault_Services.Application.DTOs;

namespace HardwareVault_Services.Application.Interfaces
{
    public interface IExcelDeviceParser
    {
        ParseResult Parse(Stream fileStream);
    }

    // Returned by IExcelDeviceParser.Parse()
    public class ParseResult
    {
        public int TotalRows { get; set; }
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
        public List<DeviceImportDto> SuccessfulRows { get; set; } = new();
        public List<ParseError> FailedRows { get; set; } = new();
    }

    // Represents a single row-level error during parsing
    public class ParseError
    {
        public int RowNumber { get; set; }
        public string ErrorMessage { get; set; } = "";
        public string? FieldName { get; set; }
        public string? FieldValue { get; set; }
    }
}
