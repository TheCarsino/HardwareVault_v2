# Excel Import Template for Device Import

## Expected File Format
- **File Type:** `.xlsx` (Excel 2007+)
- **Max File Size:** 10 MB
- **Sheet:** First worksheet will be used

## Required Columns (Header Row)

The first row must contain these column headers (case-insensitive):

| Column Name         | Type    | Required | Example           | Notes                          |
|---------------------|---------|----------|-------------------|--------------------------------|
| CPU Manufacturer    | Text    | Yes      | Intel             | e.g., Intel, AMD               |
| CPU Model           | Text    | Yes      | Core i7-12700K    | Full CPU model name            |
| GPU Manufacturer    | Text    | Yes      | NVIDIA            | e.g., NVIDIA, AMD              |
| GPU Model           | Text    | Yes      | RTX 4070          | Full GPU model name            |
| RAM (MB)            | Integer | Yes      | 16384             | RAM size in megabytes          |
| Storage (GB)        | Integer | Yes      | 512               | Storage size in gigabytes      |
| Storage Type        | Text    | Yes      | SSD               | SSD or HDD                     |
| PSU Wattage         | Integer | Yes      | 750               | Power supply wattage           |
| Weight (kg)         | Decimal | Yes      | 8.5               | Device weight in kilograms     |
| USB 2.0             | Integer | No       | 2                 | Number of USB 2.0 ports        |
| USB 3.0             | Integer | No       | 4                 | Number of USB 3.0 ports        |
| USB-C               | Integer | No       | 1                 | Number of USB-C ports          |

## Sample Data

| CPU Manufacturer | CPU Model      | GPU Manufacturer | GPU Model | RAM (MB) | Storage (GB) | Storage Type | PSU Wattage | Weight (kg) | USB 2.0 | USB 3.0 | USB-C |
|------------------|----------------|------------------|-----------|----------|--------------|--------------|-------------|-------------|---------|---------|-------|
| Intel            | Core i7-12700K | NVIDIA           | RTX 4070  | 32768    | 1024         | SSD          | 850         | 9.2         | 2       | 4       | 2     |
| AMD              | Ryzen 9 7950X  | AMD              | RX 7900XT | 65536    | 2048         | SSD          | 1000        | 10.5        | 0       | 6       | 2     |
| Intel            | Core i5-13600K | NVIDIA           | RTX 4060  | 16384    | 512          | SSD          | 650         | 7.8         | 2       | 4       | 1     |

## Validation Rules

### RAM (MB)
- Must be a positive integer
- Minimum: 1024 MB (1 GB)
- Typical values: 8192, 16384, 32768, 65536

### Storage (GB)
- Must be a positive integer
- Minimum: 1 GB
- Typical values: 256, 512, 1024, 2048

### Storage Type
- Must be either "SSD" or "HDD" (case-insensitive)

### PSU Wattage
- Must be a positive integer
- Minimum: 1 watt
- Typical values: 450, 550, 650, 750, 850, 1000, 1200

### Weight (kg)
- Must be a positive decimal number
- Minimum: 0.01 kg
- Maximum: 99.99 kg

### USB Ports (Optional)
- If omitted or 0, no USB port entry will be created
- Must be non-negative integers if provided

## Import Behavior

### Manufacturer/Component Resolution
- If a CPU/GPU manufacturer doesn't exist, it will be created automatically
- If a CPU/GPU model doesn't exist with that manufacturer, it will be created
- If a power supply with the specified wattage doesn't exist, it will be created
- **AMD Special Case:** If AMD is used for both CPU and GPU, the manufacturer type will be set to "Both"

### Error Handling
- The import process continues even if some rows fail
- Failed rows are reported with detailed error messages
- Successful rows are committed to the database in a single transaction
- HTTP 200: All rows succeeded
- HTTP 207 Multi-Status: Some rows failed (see Errors array in response)
- HTTP 400: File validation failed (wrong type, too large, etc.)
- HTTP 500: Critical server error

## Example Import Response

### Full Success (HTTP 200)
```json
{
  "importJobId": "123e4567-e89b-12d3-a456-426614174000",
  "fileName": "devices.xlsx",
  "totalRows": 10,
  "successCount": 10,
  "failureCount": 0,
  "status": "Completed",
  "startedAt": "2024-01-15T10:30:00Z",
  "completedAt": "2024-01-15T10:30:05Z",
  "errors": []
}
```

### Partial Success (HTTP 207)
```json
{
  "importJobId": "123e4567-e89b-12d3-a456-426614174000",
  "fileName": "devices.xlsx",
  "totalRows": 10,
  "successCount": 8,
  "failureCount": 2,
  "status": "Completed",
  "startedAt": "2024-01-15T10:30:00Z",
  "completedAt": "2024-01-15T10:30:05Z",
  "errors": [
    {
      "row": 5,
      "field": "RAM (MB)",
      "message": "Column 'RAM (MB)' has invalid integer value: 'abc'",
      "rawValue": "abc"
    },
    {
      "row": 7,
      "field": "Storage Type",
      "message": "Column 'Storage Type' is required but empty",
      "rawValue": null
    }
  ]
}
```

## API Endpoints

### Import Devices
```
POST /api/devices/import
Content-Type: multipart/form-data
```

**Request:**
- Form field: `file` (IFormFile)
- Must be a `.xlsx` file
- Max 10 MB

**Response:**
- 200 OK: All rows imported successfully
- 207 Multi-Status: Partial success with error details
- 400 Bad Request: Invalid file or validation error
- 500 Internal Server Error: Critical failure

### Get Import History
```
GET /api/import/history?page=1&pageSize=20
```

### Get Recent Imports
```
GET /api/import/recent?limit=5
```

### Get Specific Import Job
```
GET /api/import/{jobId}
```

## Tips

1. **Use the exact column names** shown above (case-insensitive matching is supported)
2. **Don't add extra columns** between the required ones - they will be ignored
3. **Start data from row 2** (row 1 is the header)
4. **Avoid merged cells** in the data area
5. **Use numeric values** without units (e.g., `16384` not `16,384 MB`)
6. **Keep manufacturer names consistent** to avoid duplicates (e.g., always "Intel", not "intel" or "INTEL")
7. **Test with a small file first** (5-10 rows) to verify the format

## Common Errors

| Error Message | Cause | Solution |
|---------------|-------|----------|
| "Missing required columns: ..." | Column header doesn't match expected name | Check spelling and exact column names |
| "Column '...' has invalid integer value: '...'" | Non-numeric value in numeric column | Ensure cell contains only numbers |
| "Column '...' is required but empty" | Required field is blank | Fill in the missing value |
| "Storage Type must be either SSD or HDD" | Invalid storage type value | Use only "SSD" or "HDD" |
| "RAM size must be at least 1024 MB" | RAM value too small | Increase to minimum 1024 MB (1 GB) |
| "Weight must be between 0.01 and 99.99 kg" | Weight out of range | Adjust weight to valid range |

---

**Last Updated:** 2024-01-15
**API Version:** 1.0
**Parser Version:** ExcelDeviceParser v1.0
