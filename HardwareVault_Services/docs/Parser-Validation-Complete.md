# Excel Parser Validation - Complete List

## Overview
The ExcelDeviceParser performs comprehensive validation at multiple levels to ensure data quality and prevent errors during device import.

---

## File-Level Validations

### 1. **Empty Stream Check**
- **What:** Validates that the file stream is not null or empty
- **When:** Before attempting to read the Excel file
- **Action:** Throws `ArgumentException`
- **Error Message:** "File stream is empty"

### 2. **Worksheet Existence**
- **What:** Validates that the Excel file contains at least one worksheet
- **When:** After opening the Excel package
- **Action:** Throws `InvalidOperationException`
- **Error Message:** "Excel file contains no worksheets"

### 3. **Data Row Check**
- **What:** Validates that the worksheet has at least 2 rows (1 header + 1 data)
- **When:** After loading the worksheet
- **Action:** Returns empty ParseResult (not a critical error)
- **Error Message:** Log warning "Worksheet is empty or has no data rows"

### 4. **Required Column Validation**
- **What:** Validates presence of all 9 required columns in header row
- **Required Columns:**
  - CPU Manufacturer
  - CPU Model
  - GPU Manufacturer
  - GPU Model
  - RAM (MB)
  - Storage (GB)
  - Storage Type
  - PSU Wattage
  - Weight (kg)
- **When:** After reading header row
- **Action:** Throws `InvalidOperationException`
- **Error Message:** "Missing required columns: [list of missing columns]"

### 5. **Column Header Matching**
- **What:** Maps column headers to expected names (case-insensitive)
- **When:** After reading header row
- **Flexibility:** Case-insensitive matching ("CPU Model" = "cpu model" = "CPU MODEL")

---

## Row-Level Validations

### String Field Validations

#### 6. **CPU Manufacturer** (Required)
- **Type:** String
- **Required:** Yes
- **Validation:** Must not be null, empty, or whitespace
- **Error:** "Column 'CPU Manufacturer' is required but empty"

#### 7. **CPU Model** (Required)
- **Type:** String
- **Required:** Yes
- **Validation:** Must not be null, empty, or whitespace
- **Error:** "Column 'CPU Model' is required but empty"

#### 8. **GPU Manufacturer** (Required)
- **Type:** String
- **Required:** Yes
- **Validation:** Must not be null, empty, or whitespace
- **Error:** "Column 'GPU Manufacturer' is required but empty"

#### 9. **GPU Model** (Required)
- **Type:** String
- **Required:** Yes
- **Validation:** Must not be null, empty, or whitespace
- **Error:** "Column 'GPU Model' is required but empty"

#### 10. **Storage Type** (Required)
- **Type:** String
- **Required:** Yes
- **Validation:** Must not be null, empty, or whitespace
- **Error:** "Column 'Storage Type' is required but empty"
- **Note:** Business logic validation (must be "SSD" or "HDD") happens in `Device.Create()`, not in parser

### Integer Field Validations

#### 11. **RAM (MB)** (Required)
- **Type:** Integer
- **Required:** Yes
- **Validations:**
  - Must not be empty
  - Must be a valid integer format
- **Errors:**
  - "Column 'RAM (MB)' is required but empty"
  - "Column 'RAM (MB)' has invalid integer value: '{value}'"

#### 12. **Storage (GB)** (Required)
- **Type:** Integer
- **Required:** Yes
- **Validations:**
  - Must not be empty
  - Must be a valid integer format
- **Errors:**
  - "Column 'Storage (GB)' is required but empty"
  - "Column 'Storage (GB)' has invalid integer value: '{value}'"

#### 13. **PSU Wattage** (Required)
- **Type:** Integer
- **Required:** Yes
- **Validations:**
  - Must not be empty
  - Must be a valid integer format
- **Errors:**
  - "Column 'PSU Wattage' is required but empty"
  - "Column 'PSU Wattage' has invalid integer value: '{value}'"

### Decimal Field Validations

#### 14. **Weight (kg)** (Required)
- **Type:** Decimal
- **Required:** Yes
- **Validations:**
  - Must not be empty
  - Must be a valid decimal format
- **Errors:**
  - "Column 'Weight (kg)' is required but empty"
  - "Column 'Weight (kg)' has invalid decimal value: '{value}'"

### Optional Integer Field Validations

#### 15. **USB 2.0** (Optional)
- **Type:** Integer
- **Required:** No
- **Default:** 0 if missing or empty
- **Validation:** Must be valid integer if provided
- **Error:** "Column 'USB 2.0' has invalid integer value: '{value}'"

#### 16. **USB 3.0** (Optional)
- **Type:** Integer
- **Required:** No
- **Default:** 0 if missing or empty
- **Validation:** Must be valid integer if provided
- **Error:** "Column 'USB 3.0' has invalid integer value: '{value}'"

#### 17. **USB-C** (Optional)
- **Type:** Integer
- **Required:** No
- **Default:** 0 if missing or empty
- **Validation:** Must be valid integer if provided
- **Error:** "Column 'USB-C' has invalid integer value: '{value}'"

---

## Duplicate Detection Validation ? NEW

#### 18. **Duplicate Device Detection** (NEW)
- **What:** Detects if a row is an exact duplicate of a previously parsed row in the same file
- **When:** After successfully parsing each row
- **How:** Creates a unique signature from all device properties:
  - CPU Manufacturer + CPU Model (normalized, lowercase)
  - GPU Manufacturer + GPU Model (normalized, lowercase)
  - RAM Size in MB
  - Storage Size in GB + Storage Type (uppercase)
  - PSU Wattage
  - Weight (formatted to 2 decimal places)
  - USB Ports (sorted by type for consistency)
- **Action:** Row is added to `FailedRows` instead of `SuccessfulRows`
- **Error:** "Duplicate device: All values match a previous row in the file"
- **Field Name:** "duplicate"

**Signature Format Example:**
```
intel||core i7-12700k||nvidia||rtx 4070||32768||1024||SSD||850||9.20||USB 2.0:2|USB 3.0:4|USB-C:2
```

**Behavior:**
- ? First occurrence ? Imported successfully
- ? Second occurrence ? Rejected as duplicate
- ? Third occurrence ? Rejected as duplicate
- Uses `HashSet<string>` for O(1) lookup performance

---

## Validation Behavior Summary

### **Complete Import Failure** (Throws Exception)
These errors stop the entire import process:
- ? Empty file stream
- ? No worksheets in Excel file
- ? Missing required columns in header row
- ? File is completely unreadable/corrupted

### **Row-Level Failure** (Added to FailedRows)
These errors only fail individual rows, import continues:
- ?? Empty required string fields
- ?? Invalid integer format (e.g., "abc" instead of number)
- ?? Invalid decimal format (e.g., "xyz" instead of 8.5)
- ?? **Duplicate device (all values match previous row)** ? NEW
- ?? Any other exception during row parsing

### **Delegated to Business Layer** (Not Parser's Responsibility)
These validations happen later in `Device.Create()` or `ImportService.PersistRowAsync()`:
- ? RAM minimum/maximum values (e.g., must be ? 1024 MB)
- ? Storage minimum/maximum values
- ? Storage Type must be exactly "SSD" or "HDD"
- ? Weight range constraints (0.01 - 99.99 kg)
- ? PSU wattage minimum value
- ? Manufacturer/CPU/GPU existence in database
- ? Database-level duplicate devices (same device already in DB)
- ? Business rule constraints

---

## Error Collection Strategy

### **Resilient Parsing**
- ? Row-level errors don't stop the entire import
- ? Multiple rows can fail independently
- ? Successful rows are still imported

### **Detailed Error Reporting**
Each error includes:
- **RowNumber:** Excel row number (starting from 2, since row 1 is header)
- **ErrorMessage:** Human-readable description
- **FieldName:** Which field/column caused the error
- **FieldValue:** The actual invalid value (if applicable)

### **Structured Logging**
- All parsing errors are logged with `ILogger`
- Log level: `Warning` for row-level errors
- Log level: `Error` for critical file-level failures
- Includes contextual information (row number, error details)

---

## Example Error Responses

### Validation Error Example
```json
{
  "row": 5,
  "field": "RAM (MB)",
  "message": "Column 'RAM (MB)' has invalid integer value: 'abc'",
  "rawValue": "abc"
}
```

### Duplicate Error Example ? NEW
```json
{
  "row": 8,
  "field": "duplicate",
  "message": "Duplicate device: All values match a previous row in the file",
  "rawValue": null
}
```

### Empty Field Error Example
```json
{
  "row": 3,
  "field": "CPU Model",
  "message": "Column 'CPU Model' is required but empty",
  "rawValue": null
}
```

---

## Performance Considerations

### Duplicate Detection Performance
- **Algorithm:** HashSet with string signatures
- **Time Complexity:** O(1) lookup per row
- **Space Complexity:** O(n) where n = number of unique devices
- **Impact:** Minimal - string hashing is very fast
- **Memory:** ~200-500 bytes per unique device signature

### Overall Parser Performance
- **File Reading:** Handled by EPPlus (optimized)
- **Row Parsing:** Sequential O(n) where n = number of rows
- **Total Complexity:** O(n) - linear with file size
- **Typical Performance:** ~1000-5000 rows/second

---

## Testing Recommendations

### Test Case 1: Exact Duplicates Within File
**File Content:**
```
| Row | CPU       | GPU        | RAM   | Storage | ... |
|-----|-----------|------------|-------|---------|-----|
| 2   | Intel i7  | NVIDIA RTX | 32768 | 1024    | ... |
| 3   | Intel i7  | NVIDIA RTX | 32768 | 1024    | ... |  ? Duplicate
| 4   | AMD Ryzen | AMD RX     | 16384 | 512     | ... |
| 5   | Intel i7  | NVIDIA RTX | 32768 | 1024    | ... |  ? Duplicate
```

**Expected Result:**
- ? Row 2: Imported
- ? Row 3: Rejected (duplicate)
- ? Row 4: Imported
- ? Row 5: Rejected (duplicate)
- **Total:** 2 successful, 2 failed

### Test Case 2: Similar But Not Duplicate
**File Content:**
```
| Row | CPU       | GPU        | RAM   | Storage | ... |
|-----|-----------|------------|-------|---------|-----|
| 2   | Intel i7  | NVIDIA RTX | 32768 | 1024    | ... |
| 3   | Intel i7  | NVIDIA RTX | 16384 | 1024    | ... |  ? Different RAM
```

**Expected Result:**
- ? Row 2: Imported
- ? Row 3: Imported (not a duplicate - RAM is different)

### Test Case 3: Case Insensitive Matching
**File Content:**
```
| Row | CPU       | GPU        | ... |
|-----|-----------|------------|-----|
| 2   | Intel i7  | NVIDIA RTX | ... |
| 3   | INTEL I7  | nvidia rtx | ... |  ? Same (case-insensitive)
```

**Expected Result:**
- ? Row 2: Imported
- ? Row 3: Rejected (duplicate - case-insensitive match)

---

## Summary Statistics

| Category | Count | Notes |
|----------|-------|-------|
| **File-Level Validations** | 5 | Stop entire import if failed |
| **Required Field Validations** | 9 | Empty value ? row fails |
| **Format Validations** | 7 | Invalid format ? row fails |
| **Optional Field Validations** | 3 | Default to 0 if missing |
| **Duplicate Detection** | 1 | Exact match ? row fails |
| **Total Validations** | **18** | Comprehensive coverage |

---

**Last Updated:** 2024-01-15  
**Parser Version:** ExcelDeviceParser v1.1  
**Feature Added:** Duplicate device detection within file
