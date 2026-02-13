# HardwareVault Frontend

Modern React.js single-page application for hardware inventory management.

## Tech Stack

- **React 18** with TypeScript
- **Redux Toolkit** + RTK Query for state management
- **Material-UI (MUI) v5** for UI components
- **React Router v6** for routing
- **Recharts** for data visualization
- **React Hook Form** + Zod for form handling
- **Vite** for build tooling

## Getting Started

### Prerequisites

- Node.js 18+ and npm

### Installation

```bash
npm install
```

### Development

```bash
npm run dev
```

The app will be available at `http://localhost:3000`.

### Build for Production

```bash
npm run build
```

The compiled output will be in the `build/` directory.

### Deploy to .NET Backend

After building:

```bash
# Copy build output to .NET wwwroot
cp -r build/* ../HardwareVault_Services/wwwroot/
```

## Environment Variables

Create `.env.development` and `.env.production` files:

```env
VITE_API_BASE_URL=http://localhost:5000/api
VITE_APP_NAME=HardwareVault
```

## Project Structure

```
src/
├── api/              # RTK Query API definitions
├── app/              # Redux store configuration
├── components/       # Reusable UI components
├── features/         # Redux slices
├── pages/            # Page components
├── routes/           # Routing configuration
├── theme/            # MUI theme customization
├── types/            # TypeScript type definitions
└── utils/            # Utility functions
```

## Features

- ✅ Device inventory management (CRUD)
- ✅ Excel file import with error reporting
- ✅ Real-time statistics dashboard
- ✅ Advanced filtering and search
- ✅ Server-side pagination
- ✅ Responsive design
- ✅ Role-based authentication

## License

Proprietary - All rights reserved
