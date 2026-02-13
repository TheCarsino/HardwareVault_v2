// ============================================================
// INFRASTRUCTURE LAYER — EXCEL DEVICE PARSER
// Parses .xlsx files and converts rows to DeviceImportDto
//
// Uses EPPlus library for Excel reading.
// Never throws for row-level errors — collects them in ParseError list.
// Only throws if the file itself is completely unreadable.
// ============================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using HardwareVault_Services.Application.DTOs;
using HardwareVault_Services.Application.Interfaces;

namespace HardwareVault_Services.Infrastructure.Parsers
{
    public class ExcelDeviceParser : IExcelDeviceParser
    {
        private readonly ILogger<ExcelDeviceParser> _logger;

        // Expected column headers (case-insensitive)
        private const string COL_CPU_MANUFACTURER = "CPU Manufacturer";
        private const string COL_CPU_MODEL = "CPU Model";
        private const string COL_GPU_MANUFACTURER = "GPU Manufacturer";
        private const string COL_GPU_MODEL = "GPU Model";
        private const string COL_RAM_MB = "RAM (MB)";
        private const string COL_STORAGE_GB = "Storage (GB)";
        private const string COL_STORAGE_TYPE = "Storage Type";
        private const string COL_PSU_WATTAGE = "PSU Wattage";
        private const string COL_WEIGHT_KG = "Weight (kg)";
        private const string COL_USB_2_0 = "USB 2.0";
        private const string COL_USB_3_0 = "USB 3.0";
        private const string COL_USB_C = "USB-C";

        public ExcelDeviceParser(ILogger<ExcelDeviceParser> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // -- Parse 
        public ParseResult Parse(Stream fileStream)
        {
            if (fileStream == null || fileStream.Length == 0)
                throw new ArgumentException("File stream is empty", nameof(fileStream));

            var successfulRows = new List<DeviceImportDto>();
            var failedRows = new List<ParseError>();

            try
            {
                using var package = new ExcelPackage(fileStream);
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();

                if (worksheet == null)
                {
                    throw new InvalidOperationException("Excel file contains no worksheets");
                }

                _logger.LogInformation(
                    "Parsing worksheet: {Name}, rows: {Rows}, cols: {Cols}",
                    worksheet.Name,
                    worksheet.Dimension?.Rows ?? 0,
                    worksheet.Dimension?.Columns ?? 0);

                if (worksheet.Dimension == null || worksheet.Dimension.Rows < 2)
                {
                    _logger.LogWarning("Worksheet is empty or has no data rows");
                    return new ParseResult
                    {
                        TotalRows = 0,
                        SuccessfulRows = successfulRows,
                        FailedRows = failedRows
                    };
                }

                // Read header row (row 1)
                var headers = ReadHeaders(worksheet);
                var columnMap = MapColumns(headers);

                // Validate required columns exist
                var missingColumns = ValidateRequiredColumns(columnMap);
                if (missingColumns.Any())
                {
                    throw new InvalidOperationException(
                        $"Missing required columns: {string.Join(", ", missingColumns)}");
                }

                // Parse data rows (starting from row 2)
                int totalRows = worksheet.Dimension.Rows;
                for (int row = 2; row <= totalRows; row++)
                {
                    try
                    {
                        var device = ParseRow(worksheet, row, columnMap);
                        successfulRows.Add(device);
                    }
                    catch (Exception ex)
                    {
                        // Row-level error — don't abort, just collect the error
                        _logger.LogWarning(ex, "Failed to parse row {Row}: {Message}", row, ex.Message);
                        
                        failedRows.Add(new ParseError
                        {
                            RowNumber = row,
                            ErrorMessage = ex.Message,
                            FieldName = "row",
                            FieldValue = null
                        });
                    }
                }

                _logger.LogInformation(
                    "Parsing complete — {Success} successful, {Failed} failed",
                    successfulRows.Count, failedRows.Count);

                return new ParseResult
                {
                    TotalRows = totalRows - 1, // Exclude header row
                    SuccessfulRows = successfulRows,
                    FailedRows = failedRows
                };
            }
            catch (Exception ex) when (ex is not InvalidOperationException)
            {
                // File is completely unreadable — this is a critical failure
                _logger.LogError(ex, "Failed to read Excel file");
                throw new InvalidOperationException("Unable to read Excel file. Please ensure it is a valid .xlsx file.", ex);
            }
        }

        // -- ReadHeaders 
        private Dictionary<int, string> ReadHeaders(ExcelWorksheet worksheet)
        {
            var headers = new Dictionary<int, string>();
            int colCount = worksheet.Dimension.Columns;

            for (int col = 1; col <= colCount; col++)
            {
                var headerValue = worksheet.Cells[1, col].Text?.Trim();
                if (!string.IsNullOrWhiteSpace(headerValue))
                {
                    headers[col] = headerValue;
                }
            }

            return headers;
        }

        // -- MapColumns 
        // Maps expected column names to actual column indices
        private Dictionary<string, int> MapColumns(Dictionary<int, string> headers)
        {
            var map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (var (colIndex, headerName) in headers)
            {
                map[headerName] = colIndex;
            }

            return map;
        }

        // -- ValidateRequiredColumns 
        private List<string> ValidateRequiredColumns(Dictionary<string, int> columnMap)
        {
            var required = new[]
            {
                COL_CPU_MANUFACTURER, COL_CPU_MODEL,
                COL_GPU_MANUFACTURER, COL_GPU_MODEL,
                COL_RAM_MB, COL_STORAGE_GB, COL_STORAGE_TYPE,
                COL_PSU_WATTAGE, COL_WEIGHT_KG
            };

            return required.Where(col => !columnMap.ContainsKey(col)).ToList();
        }

        // -- ParseRow 
        private DeviceImportDto ParseRow(
            ExcelWorksheet worksheet,
            int rowNumber,
            Dictionary<string, int> columnMap)
        {
            var dto = new DeviceImportDto { RowNumber = rowNumber };

            // Parse CPU
            dto.CpuManufacturer = GetStringValue(worksheet, rowNumber, columnMap, COL_CPU_MANUFACTURER, required: true);
            dto.CpuModel = GetStringValue(worksheet, rowNumber, columnMap, COL_CPU_MODEL, required: true);

            // Parse GPU
            dto.GpuManufacturer = GetStringValue(worksheet, rowNumber, columnMap, COL_GPU_MANUFACTURER, required: true);
            dto.GpuModel = GetStringValue(worksheet, rowNumber, columnMap, COL_GPU_MODEL, required: true);

            // Parse RAM
            dto.RamSizeInMB = GetIntValue(worksheet, rowNumber, columnMap, COL_RAM_MB, required: true);

            // Parse Storage
            dto.StorageSizeInGB = GetIntValue(worksheet, rowNumber, columnMap, COL_STORAGE_GB, required: true);
            dto.StorageType = GetStringValue(worksheet, rowNumber, columnMap, COL_STORAGE_TYPE, required: true);

            // Parse PSU
            dto.PowerSupplyWattage = GetIntValue(worksheet, rowNumber, columnMap, COL_PSU_WATTAGE, required: true);

            // Parse Weight
            dto.WeightInKg = GetDecimalValue(worksheet, rowNumber, columnMap, COL_WEIGHT_KG, required: true);

            // Parse USB ports (optional)
            dto.UsbPorts = new List<UsbPortDto>();

            var usb20Count = GetIntValue(worksheet, rowNumber, columnMap, COL_USB_2_0, required: false);
            if (usb20Count > 0)
                dto.UsbPorts.Add(new UsbPortDto { PortType = "USB 2.0", Count = usb20Count });

            var usb30Count = GetIntValue(worksheet, rowNumber, columnMap, COL_USB_3_0, required: false);
            if (usb30Count > 0)
                dto.UsbPorts.Add(new UsbPortDto { PortType = "USB 3.0", Count = usb30Count });

            var usbCCount = GetIntValue(worksheet, rowNumber, columnMap, COL_USB_C, required: false);
            if (usbCCount > 0)
                dto.UsbPorts.Add(new UsbPortDto { PortType = "USB-C", Count = usbCCount });

            return dto;
        }

        // -- GetStringValue 
        private string GetStringValue(
            ExcelWorksheet worksheet,
            int row,
            Dictionary<string, int> columnMap,
            string columnName,
            bool required)
        {
            if (!columnMap.TryGetValue(columnName, out int col))
            {
                if (required)
                    throw new InvalidOperationException($"Column '{columnName}' not found");
                return string.Empty;
            }

            var value = worksheet.Cells[row, col].Text?.Trim();

            if (required && string.IsNullOrWhiteSpace(value))
                throw new InvalidOperationException($"Column '{columnName}' is required but empty");

            return value ?? string.Empty;
        }

        // -- GetIntValue 
        private int GetIntValue(
            ExcelWorksheet worksheet,
            int row,
            Dictionary<string, int> columnMap,
            string columnName,
            bool required)
        {
            if (!columnMap.TryGetValue(columnName, out int col))
            {
                if (required)
                    throw new InvalidOperationException($"Column '{columnName}' not found");
                return 0;
            }

            var cellValue = worksheet.Cells[row, col].Text?.Trim();

            if (string.IsNullOrWhiteSpace(cellValue))
            {
                if (required)
                    throw new InvalidOperationException($"Column '{columnName}' is required but empty");
                return 0;
            }

            if (!int.TryParse(cellValue, out int result))
                throw new InvalidOperationException($"Column '{columnName}' has invalid integer value: '{cellValue}'");

            return result;
        }

        // -- GetDecimalValue 
        private decimal GetDecimalValue(
            ExcelWorksheet worksheet,
            int row,
            Dictionary<string, int> columnMap,
            string columnName,
            bool required)
        {
            if (!columnMap.TryGetValue(columnName, out int col))
            {
                if (required)
                    throw new InvalidOperationException($"Column '{columnName}' not found");
                return 0m;
            }

            var cellValue = worksheet.Cells[row, col].Text?.Trim();

            if (string.IsNullOrWhiteSpace(cellValue))
            {
                if (required)
                    throw new InvalidOperationException($"Column '{columnName}' is required but empty");
                return 0m;
            }

            if (!decimal.TryParse(cellValue, out decimal result))
                throw new InvalidOperationException($"Column '{columnName}' has invalid decimal value: '{cellValue}'");

            return result;
        }
    }
}
