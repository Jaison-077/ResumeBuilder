# Quick Start Guide - AI Resume Generator

## Super Quick Start (5 minutes)

### Prerequisites
- .NET 8 SDK installed
- Node.js 18+ installed
- Visual Studio Code or Visual Studio 2022

### 1. Backend Setup

```bash
# Navigate to backend
cd backend

# Restore dependencies and build
dotnet restore
dotnet build

# Run the API
dotnet run --project src/Api/Api.csproj
```

**Expected output**:
```
Building...
Build succeeded.
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
```

✅ Visit `https://localhost:5001` - you should see Swagger UI

### 2. Frontend Setup (New Terminal)

```bash
# Navigate to frontend
cd frontend

# Install dependencies (one-time)
npm install

# Start dev server
npm start
```

**Expected output**:
```
✔ Compiled successfully.
Compiled successfully.
✔ Ng Build Succeeded.

Watch mode started. If you change any source files then the application will be automatically recompiled.
```

✅ Visit `http://localhost:4200` - you should see the app

## Environment Configuration

### Backend (appsettings.Development.json)

For **development**, the file uses mock Azure services. To enable real services:

```json
{
  "AzureOpenAI": {
    "Endpoint": "https://YOUR_RESOURCE.openai.azure.com/",
    "Key": "YOUR_API_KEY",
    "DeploymentName": "gpt-4-turbo"
  },
  "AzureBlob": {
    "ConnectionString": "YOUR_STORAGE_CONNECTION_STRING",
    "ContainerName": "resumes"
  }
}
```

### Frontend (environment.ts)

Default already points to backend via proxy at `/api`.

## Common Tasks

### Generate Resume
1. Click "Create Resume" in navbar
2. Fill in personal info (Step 1)
3. Add experiences (Step 2)
4. Add education (Step 3)
5. Add skills (Step 4)
6. Click "Generate Resume with AI"

### Optimize for ATS
1. Click "ATS Optimizer"
2. Paste job description
3. Click "Analyze Resume Match"
4. Review match score and suggestions
5. Click "Apply Optimized Resume" to use improvements

### Export Resume
1. Click "Preview & Export"
2. Select template (Minimal, Modern, Classic)
3. Click "Download PDF" or "Download DOCX"

## API Testing with Curl

### Test Health Endpoint
```bash
curl https://localhost:5001/health
# Expected: {"status":"healthy","timestamp":"2024-03-15T10:30:00Z"}
```

### Test Resume Generation
```bash
curl -X POST https://localhost:5001/api/resume/generate \
  -H "Content-Type: application/json" \
  -d '{
    "personalInfo": {
      "firstName": "John",
      "lastName": "Doe",
      "title": "Software Engineer",
      "email": "john@example.com",
      "phone": "+1-555-0123",
      "location": "Seattle, WA"
    },
    "experiences": [],
    "educations": [],
    "skills": ["C#", ".NET", "Azure"]
  }'
```

### Test ATS Optimization
```bash
curl -X POST https://localhost:5001/api/resume/optimize-ats \
  -H "Content-Type: application/json" \
  -d '{
    "resume": "{ \"personalInfo\": {...} }",
    "jobDescription": "Senior Software Engineer needed with C# and Azure experience..."
  }'
```

## Debugging

### Frontend Issues

**Problem**: "Cannot GET /api/resume/generate"
- ✅ Check backend is running on port 5000
- ✅ Verify proxy.conf.json target
- ✅ Restart dev server

**Problem**: "Cannot find module '@angular/core'"
- ✅ Run `npm install`
- ✅ Delete node_modules and try again

### Backend Issues

**Problem**: Application won't start
- ✅ Check .NET 8 is installed: `dotnet --version`
- ✅ Check ports (5000, 5001) are available
- ✅ Look for configuration issues in appsettings

**Problem**: "Unhandled exception: System.IO.FileNotFoundException"
- ✅ Restore packages: `dotnet restore`
- ✅ Ensure working directory is correct

## Next Steps

1. **Add Azure credentials** to appsettings.Development.json
2. **Install PDF/DOCX libraries** (QuestPDF, OpenXML SDK)
3. **Implement file extraction** (PDF/DOCX parsing)
4. **Add authentication** (Azure AD, Auth0)
5. **Deploy to Azure** (App Service + Static Web Apps)

## File Structure Quick Reference

```
ResumeBuilder/
├── backend/
│   └── src/
│       ├── Api/           ← Main API/Controllers
│       ├── Application/   ← Business logic
│       ├── Domain/        ← Core models
│       └── Infrastructure/← External services
│
└── frontend/
    └── src/
        └── app/
            ├── modules/   ← Feature pages
            └── shared/    ← Services & models
```

## Port Configuration

| Service | Port | URL |
|---------|------|-----|
| Backend API | 5001 | https://localhost:5001 |
| Frontend | 4200 | http://localhost:4200 |
| API Proxy (from frontend) | - | /api |
| Swagger UI | 5001 | https://localhost:5001 |

## Resources

- [ASP.NET Core Docs](https://learn.microsoft.com/en-us/aspnet/core/)
- [Angular Docs](https://angular.io/docs)
- [Azure OpenAI Docs](https://learn.microsoft.com/en-us/azure/cognitive-services/openai/)

---

**Need help?** Check the main [README.md](./README.md) for detailed setup & architecture.
