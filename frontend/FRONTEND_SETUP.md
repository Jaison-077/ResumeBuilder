# Frontend Setup Instructions

## Overview
This Angular 17 application requires Node.js and npm to run. Follow the steps below to set up the development environment.

## Prerequisites
- **Node.js 18+** - Download from [https://nodejs.org/](https://nodejs.org/)
- **npm** - Comes bundled with Node.js

## Installation Steps

### 1. Install Node Modules
Run the following command in the `frontend` directory:

```bash
npm install --legacy-peer-deps
```

This may take 2-5 minutes depending on your internet connection.

### 2. Verify Installation
Check that the `node_modules` folder was created:

```bash
ls node_modules  # macOS/Linux
dir node_modules # Windows (PowerShell)
ls -la node_modules # Verbose listing
```

## Running the Application

### Development Server
```bash
npm start
```

This command:
- Starts the Angular dev server on `http://localhost:4200`
- Enables hot reloading (changes auto-refresh)
- Proxies `/api` requests to `http://localhost:5000` (backend)

### Production Build
```bash
npm run build:prod
```

Output will be in `dist/resume-builder/` for deployment.

## Common Issues & Solutions

### Issue: "Cannot find module '@angular/core'"
**Solution**: Node modules not installed
```bash
npm install --legacy-peer-deps
```

### Issue: "Port 4200 already in use"
**Solution**: Kill the process or specify a different port
```bash
npm start -- --port 4300
```

### Issue: "Backend API connection failed"
**Solution**: Ensure backend is running on port 5000
```bash
# Backend should be running:
dotnet run --project ../backend/src/Api/Api.csproj
```

### Issue: CORS errors in console
**Solution**: Verify `proxy.conf.json` is correct and backend CORS is configured

## File Structure

```
frontend/
├── src/
│   ├── app/
│   │   ├── modules/          ← Feature pages
│   │   ├── shared/           ← Services & models
│   │   ├── app.module.ts
│   │   ├── app.routing.ts
│   │   ├── app.component.ts
│   │   └── app.component.html
│   ├── environments/         ← Configuration
│   ├── styles.css           ← Global styles
│   ├── main.ts              ← Entry point
│   └── index.html
├── angular.json             ← Angular CLI config
├── tsconfig.json           ← TypeScript config
├── package.json            ← Dependencies
└── proxy.conf.json         ← Dev proxy config
```

## Available npm Scripts

| Script | Description |
|--------|-------------|
| `npm start` | Start dev server (port 4200) |
| `npm run build` | Build for development |
| `npm run build:prod` | Build for production |
| `npm test` | Run unit tests (when added) |
| `npm run lint` | Run linter (when configured) |

## Browser DevTools

### Angular DevTools Extension
Install the [Angular DevTools](https://chrome.google.com/webstore/detail/angular-devtools/ienfalfjdbdpebiobmfihnnbfmbnidlj) Chrome extension for:
- Component inspection
- Service debugging
- Performance profiling

## Next Steps

1. ✅ Install dependencies: `npm install --legacy-peer-deps`
2. ✅ Verify setup: Check `node_modules` exists
3. ✅ Start backend: Run backend on port 5000
4. ✅ Start frontend: `npm start` (opens http://localhost:4200)
5. ✅ Test application: Create resume, optimize for ATS, export

## Troubleshooting Checklist

- [ ] Node.js 18+ installed (`node --version`)
- [ ] npm 9+ installed (`npm --version`)
- [ ] `npm install` completed successfully
- [ ] `node_modules` folder exists
- [ ] Backend running on port 5000
- [ ] No firewall blocking ports 4200, 5000
- [ ] Angular CLI available (`ng --version` or `npx ng --version`)

## Variables & Configuration

### Environment Configuration
Edit `src/environments/environment.ts`:
```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api'  // Change if backend on different port
};
```

### Backend Proxy Configuration
Edit `proxy.conf.json` to change where `/api` requests are routed:
```json
{
  "/api": {
    "target": "http://localhost:5000",
    "pathRewrite": {"^/api": "/api"}
  }
}
```

## Production Deployment

### Build for Production
```bash
npm run build:prod
```

### Deploy to Azure Static Web Apps
1. Push code to GitHub
2. Create Static Web App in Azure Portal
3. Connect GitHub repo
4. Configure build settings:
   - Build location: `frontend`
   - App build location: `dist/resume-builder`
5. Deploy

See [DEPLOYMENT.md](../DEPLOYMENT.md) for complete instructions.

## Getting Help

If you encounter issues:
1. Check this file for solutions
2. Review error messages carefully
3. Check [README.md](../README.md) for architecture overview
4. Review [QUICKSTART.md](../QUICKSTART.md) for quick start guide
5. Ensure backend is running and accessible

Happy coding! 🚀
