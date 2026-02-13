# HardwareVault v2 - Complete Documentation

## Table of Contents
1. [Project Overview](#project-overview)
2. [Architecture](#architecture)
3. [Backend Services](#backend-services)
4. [Frontend Application](#frontend-application)
5. [Data Flow & Import Process](#data-flow--import-process)
6. [Excel Import Specification](#excel-import-specification)
7. [API Documentation](#api-documentation)
8. [Development Guide](#development-guide)
9. [Deployment](#deployment)

---

## Project Overview

**HardwareVault** is a hardware inventory management system designed for higher education institutions. It transforms messy Excel spreadsheets into a clean, searchable, auditable database of hardware devices.

### Business Context
- **Target Users**: IT departments at universities, colleges, and educational institutions
- **Problem Solved**: Replaces manual Excel tracking with centralized, normalized data
- **Value Proposition**: Zero manual cleanup required - ingest messy files, get clean data

### Key Features
- ✅ Excel import with intelligent error handling (partial success supported)
- ✅ Server-side pagination, filtering, and search
- ✅ Device CRUD operations with audit trail
- ✅ Soft deletes (restorable)
- ✅ Real-time statistics dashboard
- ✅ Manufacturer management with color-coded chips (Intel Blue, AMD Red, NVIDIA Green)
- ✅ Import history with detailed error reporting
- ✅ Creation and update timestamps with icons

---

## Architecture

### Technology Stack

**Backend** (`HardwareVault_Services/`)
- .NET 8.0 Web API
- Entity Framework Core 8.0
- SQL Server (Azure SQL Database)
- EPPlus 8.4.2 (Excel parsing)
- Clean Architecture (Domain, Application, Infrastructure, API layers)

**Frontend** (`HardwareVault_FrontPage/`)
- React 18 with TypeScript
- Redux Toolkit + RTK Query
- Material-UI (MUI) v5
- React Router v6
- Recharts (data visualization)
- Vite (build tooling)

### Hosting Model
- **Unified Host**: React SPA served from .NET `wwwroot/`
- **Rationale**: Zero CORS complexity, single deployment, shared authentication context
- **Details**: Frontend compiled to static files and served by .NET API from same origin

---

## Backend Services

### Project Structure

```
HardwareVault_Services/
├── Api/                    # Controllers, Program.cs
├── Application/            # DTOs, Services, Interfaces
├── Domain/                 # Entities, Interfaces, Business Logic
├── Infrastructure/         # Data, Repositories, Parsers
└── docs/                   # Documentation
    ├── Excel-Import-Template.md
    └── Parser-Validation-Complete.md
```

### Layer Responsibilities

#### **API Layer** (`Api/Controllers/`)
- HTTP endpoints with proper status codes
- Request validation
- Controllers:
  - `DevicesController`: CRUD + statistics + import
  - `ImportController`: Import history queries
  - `ManufacturersController`: Reference data

#### **Application Layer** (`Application/`)
- Use case orchestration
- DTO mapping
- Services:
  - `DeviceService`: Device operations
  - `ImportService`: 8-step import workflow
  - `ManufacturerService`: Manufacturer queries

#### **Infrastructure Layer** (`Infrastructure/`)
- Database access via EF Core
- Excel parsing (`ExcelDeviceParser`)
- Repository pattern (`UnitOfWork`)

#### **Domain Layer** (`Domain/`)
- Business rules
- Entity partial classes
- Domain models

### Database Schema

**Key Entities** (`ApplicationDbContext`):
- `Devices` - Hardware inventory with CPU, GPU, RAM, storage, power supply
- `Cpu`, `Gpu`, `PowerSupply` - Components
- `Manufacturers` - Reference data (Intel, AMD, NVIDIA, etc.)
- `ImportJobs` - Audit trail of imports with error logs
- `DeviceUsbPorts` - USB configurations (USB 2.0, 3.0, USB-C)
- `VwDeviceList` - Denormalized view for efficient queries

### Required Backend Fields

For proper frontend display, ensure all API responses include:

```json
{
  "deviceId": "string",
  "ramSizeInMB": "number",
  "storageSizeInGB": "number",
  "storageType": "SSD | HDD",
  "weightInKg": "number",
  "createdAt": "ISO 8601 string",
  "updatedAt": "ISO 8601 string",
  "cpu": {
    "cpuId": "number",
    "modelName": "string",
    "manufacturer": {
      "manufacturerId": "number",
      "name": "string"  // REQUIRED: e.g., "Intel", "AMD"
    }
  },
  "gpu": {
    "gpuId": "number",
    "modelName": "string",
    "manufacturer": {
      "manufacturerId": "number",
      "name": "string"  // REQUIRED: e.g., "NVIDIA", "AMD", "Intel"
    }
  },
  "powerSupply": {
    "powerSupplyId": "number",
    "wattageInWatts": "number"
  },
  "usbPorts": [
    {
      "deviceUsbPortId": "string",
      "usbPortType": "string",
      "count": "number"
    }
  ]
}
```

---

## Frontend Application

### Project Structure

```
HardwareVault_FrontPage/
├── src/
│   ├── api/              # RTK Query endpoints
│   │   ├── baseApi.ts        # Base config with auth
│   │   ├── deviceApi.ts      # Device endpoints
│   │   ├── importApi.ts      # Import endpoints
│   │   └── manufacturerApi.ts
│   ├── app/              # Redux store
│   │   ├── store.ts          # Store configuration
│   │   └── hooks.ts          # Typed hooks
│   ├── components/       # Reusable UI components
│   │   ├── common/           # Shared components
│   │   ├── dashboard/        # Charts, stat cards
│   │   ├── devices/          # Device table, filters
│   │   └── import/           # Upload, results
│   ├── features/         # Redux slices
│   │   ├── devices/          # Device filters state
│   │   └── notifications/    # Toast notifications
│   ├── pages/            # Page components
│   │   ├── DashboardPage.tsx
│   │   ├── DevicesPage.tsx
│   │   ├── ImportPage.tsx
│   │   ├── ImportHistoryPage.tsx
│   │   ├── ManufacturersPage.tsx
│   │   └── LoginPage.tsx
│   ├── routes/           # Routing + auth
│   │   ├── AppRoutes.tsx
│   │   └── RequireAuth.tsx
│   ├── theme/            # MUI theme
│   │   └── theme.ts
│   ├── types/            # TypeScript types
│   │   ├── device.ts
│   │   ├── import.ts
│   │   └── manufacturer.ts
│   └── utils/            # Helpers, formatters
│       ├── constants.ts
│       └── formatters.ts
├── public/               # Static assets
├── index.html            # HTML entry point
├── vite.config.ts        # Vite configuration
└── package.json
```

### Key Components

#### **DeviceTable** (`src/components/devices/DeviceTable.tsx`)
- Color-coded manufacturer chips:
  - **Intel**: Blue (`#E3F2FD` background, `#1976D2` text)
  - **AMD**: Red (`#FFEBEE` background, `#D32F2F` text)
  - **NVIDIA**: Green (`#E8F5E9` background, `#388E3C` text)
- RAM column with Memory icon
- Created/Updated columns with Calendar and Update icons
- Row height: 60px for optimal readability
- Edit and Delete actions

#### **API Layer** (`src/api/`)
- `baseApi.ts`: RTK Query base with auth headers
- `deviceApi.ts`: Device CRUD, filters, import
- `importApi.ts`: Import history queries
- `manufacturerApi.ts`: Manufacturer list

#### **State Management** (`src/app/store.ts`)
```typescript
export const store = configureStore({
  reducer: {
    [baseApi.reducerPath]: baseApi.reducer,
    devices: devicesReducer,
    notifications: notificationsReducer,
  },
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware().concat(baseApi.middleware),
});
```

#### **Routing** (`src/routes/AppRoutes.tsx`)
- All routes protected by `RequireAuth`
- Token stored in `localStorage` (key: `HardwareVault_token`)
- Redirect to `/login` if unauthenticated

### Environment Configuration

**Development** (`.env.development`):
```env
VITE_API_BASE_URL=https://localhost:7080/api
```

**Production** (`.env.production`):
```env
VITE_API_BASE_URL=https://api.hardwarevault.azurewebsites.net/api
```

**Access in Code**:
```typescript
import.meta.env.VITE_API_BASE_URL
```

### Utility Functions (`src/utils/formatters.ts`)

- `ramToGB(mb)` - Converts MB to GB
- `formatStorage(gb, type)` - "512 GB SSD" or "2.0 TB HDD"
- `formatWeight(kg)` - "1.50 kg"
- `formatWatts(watts)` - "750 W"
- `formatUsbPorts(count)` - "8 ports"
- `formatDate(dateString)` - "Jan 15, 2026"
- `formatDateTime(dateString)` - "Jan 15, 2026, 03:45 PM"
- `formatRelativeTime(dateString)` - "2 hours ago"

---

## Data Flow & Import Process

### Import Workflow (8-Step Process)

**Implemented in** `ImportService.ImportDevicesAsync`:

```
1. CREATE ImportJob (Status: Pending)
   └─ Persisted immediately for audit trail

2. MARK Processing
   └─ job.Start() → status = "Processing"

3. PARSE Excel
   └─ ExcelDeviceParser.Parse(stream)
   └─ Returns: SuccessfulRows + FailedRows

4. PROCESS Each Row
   ├─ Resolve/Create Manufacturer
   ├─ Resolve/Create CPU
   ├─ Resolve/Create GPU
   ├─ Resolve/Create PowerSupply
   └─ Create Device + UsbPorts

5. COMMIT Transaction
   └─ All successful devices saved in one transaction

6. CONSOLIDATE Errors
   └─ Convert ParseErrors to ImportErrorDto

7. MARK Completed
   └─ Save error log as JSON
   └─ Status = "Completed" (even if some rows failed)

8. RETURN ImportResultDto
```

### Parser Architecture

**Component**: `ExcelDeviceParser`

**Philosophy**: 
- **Never throws for row-level errors** - collected in `ParseError` list
- **Only throws for file-level failures** (corrupt file, no worksheets)
- **Resilient** - continues parsing even when rows fail

**Validation Levels**:
1. **File-Level** (throws exception):
   - Empty stream
   - No worksheets
   - Missing required columns
   
2. **Row-Level** (adds to FailedRows):
   - Invalid data types
   - Empty required fields
   - Duplicate detection

**See**: `HardwareVault_Services/docs/Parser-Validation-Complete.md`

---

## Excel Import Specification

### Documentation Files

**Location**: `HardwareVault_Services/docs/`

#### `Excel-Import-Template.md`
- **Audience**: End users (IT administrators)
- **Purpose**: How to format Excel files for import
- **Contains**: Required columns, data types, sample data, common errors

#### `Parser-Validation-Complete.md`
- **Audience**: Developers
- **Purpose**: Complete validation specification
- **Contains**: All 18 validation rules, error messages, test cases

### Required Excel Format

**Column Headers** (case-insensitive):

| Column Name      | Type    | Required | Example        |
| ---------------- | ------- | -------- | -------------- |
| CPU Manufacturer | Text    | Yes      | Intel          |
| CPU Model        | Text    | Yes      | Core i7-12700K |
| GPU Manufacturer | Text    | Yes      | NVIDIA         |
| GPU Model        | Text    | Yes      | RTX 4070       |
| RAM (MB)         | Integer | Yes      | 16384          |
| Storage (GB)     | Integer | Yes      | 512            |
| Storage Type     | Text    | Yes      | SSD            |
| PSU Wattage      | Integer | Yes      | 650            |
| Weight (kg)      | Decimal | Yes      | 1.5            |
| USB 2.0          | Integer | No       | 2              |
| USB 3.0          | Integer | No       | 4              |
| USB-C            | Integer | No       | 1              |

### Import Response Codes

- **HTTP 200**: All rows succeeded
- **HTTP 207 Multi-Status**: Partial success (some rows failed)
- **HTTP 400**: File validation failed (wrong type, too large, missing columns)
- **HTTP 500**: Critical server error

**Example Response** (HTTP 207):
```json
{
  "importJobId": "123e4567-e89b-12d3-a456-426614174000",
  "fileName": "devices.xlsx",
  "totalRows": 10,
  "successCount": 8,
  "failureCount": 2,
  "status": "Completed",
  "errors": [
    {
      "row": 5,
      "field": "RAM (MB)",
      "message": "Column 'RAM (MB)' has invalid integer value: 'abc'",
      "rawValue": "abc"
    }
  ]
}
```

---

## API Documentation

### Base URL
- **Development**: `https://localhost:7080/api`
- **Production**: TBD (Azure App Service)

### Authentication
- **Method**: JWT Bearer token (planned)
- **Current**: No auth (development only)
- **Header**: `Authorization: Bearer {token}`

### Endpoints

#### **Devices**

**GET** `/api/devices`
- **Query Params**:
  - `page` (int, default: 1)
  - `pageSize` (int, default: 20)
  - `cpuManufacturer` (string, optional)
  - `gpuManufacturer` (string, optional)
  - `storageType` (string: "SSD" | "HDD")
  - `minRamInGB` (int, optional)
  - `search` (string, optional)
- **Response**: `PagedResultDto<DeviceDto>`

**GET** `/api/devices/{id}`
- **Response**: `DeviceDto`

**POST** `/api/devices`
- **Body**: `CreateDeviceDto`
- **Response**: `DeviceDto`

**PUT** `/api/devices/{id}`
- **Body**: `UpdateDeviceDto`
- **Response**: `DeviceDto`

**DELETE** `/api/devices/{id}`
- **Response**: `bool` (soft delete)

**GET** `/api/devices/statistics`
- **Response**: `DeviceStatistics`

**POST** `/api/devices/import`
- **Body**: `multipart/form-data` with `file` field
- **File**: `.xlsx` file, max 10 MB
- **Response**: `ImportResultDto` (HTTP 200 or 207)

#### **Import History**

**GET** `/api/import/history`
- **Query**: `page`, `pageSize`
- **Response**: `PagedResultDto<ImportJobDto>`

**GET** `/api/import/recent`
- **Query**: `limit` (default: 5)
- **Response**: `List<ImportJobDto>`

**GET** `/api/import/{jobId}`
- **Response**: `ImportJobDto` with full error log

#### **Manufacturers**

**GET** `/api/manufacturers`
- **Response**: `List<ManufacturerDto>`

---

## Development Guide

### Prerequisites

**Backend**:
- .NET 8.0 SDK
- SQL Server (local) or Azure SQL Database
- Visual Studio 2022 or VS Code

**Frontend**:
- Node.js 18+
- npm or yarn

### Setup Instructions

#### **1. Clone Repository**
```bash
git clone https://github.com/TheCarsino/HardwareVault_v2.git
cd HardwareVault_v2
```

#### **2. Backend Setup**

```bash
cd HardwareVault_Services

# Restore NuGet packages
dotnet restore

# Update connection string in appsettings.json
# Run migrations (if needed)
dotnet ef database update

# Run API
dotnet run
# API available at https://localhost:7080
```

#### **3. Frontend Setup**

```bash
cd HardwareVault_FrontPage

# Install dependencies
npm install

# Start dev server
npm run dev
# Frontend available at http://localhost:3000
```

### Development Workflow

**Local Development**:
1. Run backend: `dotnet run` in `HardwareVault_Services/`
2. Run frontend: `npm run dev` in `HardwareVault_FrontPage/`
3. Frontend proxies API requests to `https://localhost:7080`

**Build Frontend for Production**:
```bash
cd HardwareVault_FrontPage
npm run build
```

**Copy to .NET wwwroot**:
```bash
# From HardwareVault_FrontPage directory
cp -r dist/* ../HardwareVault_Services/Api/wwwroot/
```

**Publish .NET API**:
```bash
cd HardwareVault_Services
dotnet publish -c Release
```

### Testing

**Excel Import Test Files**:
- Create test files based on `docs/Excel-Import-Template.md`
- Test cases in `docs/Parser-Validation-Complete.md`

**Test Scenarios**:
- ✅ Exact duplicates within file
- ✅ Invalid data types
- ✅ Missing required fields
- ✅ Case-insensitive manufacturer matching
- ✅ Partial success (207 Multi-Status)

### Debugging

**Backend**:
- Set breakpoints in Visual Studio
- Enable EF Core logging: `Microsoft.EntityFrameworkCore = Information`
- Check `ImportJob.ErrorLog` for import failures

**Frontend**:
- React DevTools (Components, Profiler)
- Redux DevTools Extension
- Browser console for API errors
- Network tab for request/response inspection

### Common Issues

**CORS Errors**:
- Not applicable in production (unified host)
- In dev: Vite proxy configured in `vite.config.ts`

**Import Failures**:
- Verify column headers match (case-insensitive)
- File must be `.xlsx` (not `.xls`)
- Max file size: 10 MB
- Check error response for specific row errors

**Database Connection**:
- Azure SQL requires firewall rule for local IP
- Connection string uses `Authentication="Active Directory Default"`

**Build Errors**:
- Clear `node_modules` and reinstall: `npm ci`
- Clear Vite cache: `npm run dev -- --force`
- Check TypeScript errors: `npm run type-check`

---

## Deployment

### Azure Architecture

**Resources**:
1. **Azure App Service** (Windows, .NET 8)
   - Hosts both API and React SPA from `wwwroot/`
2. **Azure SQL Database**
   - Tier: S1 Standard
3. **Azure Application Insights** (planned)
   - Logging and monitoring

### Deployment Steps

1. **Build Frontend**:
   ```bash
   cd HardwareVault_FrontPage
   npm run build
   ```

2. **Copy to wwwroot**:
   ```bash
   cp -r dist/* ../HardwareVault_Services/Api/wwwroot/
   ```

3. **Publish Backend**:
   ```bash
   cd ../HardwareVault_Services
   dotnet publish -c Release -o ./publish
   ```

4. **Deploy to Azure**:
   - Use Azure CLI, GitHub Actions, or Visual Studio publish
   - Ensure App Service configuration includes connection string

### Environment Variables (Azure)

**App Service Configuration**:
- `ASPNETCORE_ENVIRONMENT` = `Production`
- `ConnectionStrings__SQLServerConnection` = (Azure SQL connection string)
- `Cors__AllowedOrigins__0` = Production domain (if needed)

---

## Security Considerations

### Current State (Development)
- ❌ No authentication
- ❌ No authorization
- ❌ Public API endpoints

### Planned (Production)
- ✅ JWT authentication
- ✅ Role-based access control (Admin, Viewer)
- ✅ httpOnly cookies for token storage
- ✅ HTTPS enforcement
- ✅ SQL injection protection (via EF Core parameterization)
- ✅ File upload validation (type, size limits)
- ✅ XSS protection (React escapes by default)

---

## Performance Optimization

### Backend
- Denormalized view (`VwDeviceList`) for fast queries
- Server-side pagination
- EF Core query tracking disabled where appropriate
- Bulk insert for import operations

### Frontend
- Code splitting via React Router lazy loading
- Redux state normalization
- RTK Query caching (60s default)
- Virtualized tables for large datasets (planned)

---

## License

Proprietary - All rights reserved

---

## Support & Documentation

**Project Documentation**:
- `HardwareVault_Services/docs/Excel-Import-Template.md` - User guide
- `HardwareVault_Services/docs/Parser-Validation-Complete.md` - Developer spec

**Architecture Decision Records**:
- ADR-001: Project Overview & Business Context
- ADR-002: Hosting Architecture (Unified Host)

---

**Last Updated**: February 13, 2026  
**Version**: 2.0  
**Repository**: [TheCarsino/HardwareVault_v2](https://github.com/TheCarsino/HardwareVault_v2)  
**Maintainer**: TheCarsino
