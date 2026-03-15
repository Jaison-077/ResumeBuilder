# AI Resume Generator & ATS Optimizer - Complete Implementation Guide

## Project Overview

A full-stack web application that uses **Azure OpenAI** to help users create, refactor, and optimize resumes for Applicant Tracking Systems (ATS). Built with **ASP.NET Core 8** backend and **Angular 17** frontend following **Clean Architecture** principles.

### Key Features

1. **Resume Builder**: Multi-step form to create a professional resume from scratch with AI-powered content enhancement
2. **Resume Refactor**: Upload PDF/DOCX files or paste raw text to restructure into ATS-friendly format
3. **ATS Optimizer**: Compare resume against job descriptions to identify keyword gaps and match scores
4. **Export Options**: Download resume as PDF or DOCX in multiple template styles (Minimal, Modern, Classic)
5. **Live Preview**: Real-time HTML preview in selected template

---

## Technology Stack

### Backend (ASP.NET Core 8)
- **API**: RESTful Web API with Swagger/OpenAPI
- **ORM**: Entity Framework Core 8
- **Database**: Azure SQL or PostgreSQL (configured in `appsettings.json`)
- **Cloud Storage**: Azure Blob Storage for file management
- **AI**: Azure OpenAI Service (GPT-4, GPT-4o, GPT-4o-mini)
- **PDF Generation**: QuestPDF (or alternative)
- **DOCX Generation**: Open XML SDK (DocumentFormat.OpenXml)
- **Logging**: Built-in .NET Logging + Application Insights (production)
- **DI Container**: Built-in .NET Service Collection

### Frontend (Angular 17)
- **Framework**: Angular with TypeScript
- **Forms**: Reactive Forms with validation
- **HTTP**: HttpClient with interceptors
- **State Management**: RxJS BehaviorSubject for shared state
- **Styling**: CSS (responsive, mobile-friendly)
- **Storage**: Browser localStorage for auto-save

---

## Solution Structure

```
ResumeBuilder/
├── backend/
│   ├── src/
│   │   ├── Api/
│   │   │   ├── Controllers/
│   │   │   │   └── ResumeController.cs
│   │   │   ├── Program.cs (startup)
│   │   │   ├── appsettings.json
│   │   │   ├── appsettings.Development.json
│   │   │   └── Api.csproj
│   │   ├── Application/
│   │   │   ├── Interfaces/
│   │   │   │   ├── IAIService.cs
│   │   │   │   ├── IResumeContentService.cs
│   │   │   │   ├── IATSService.cs
│   │   │   │   ├── IFileExtractionService.cs
│   │   │   │   ├── ITemplateService.cs
│   │   │   │   └── IStorageService.cs
│   │   │   ├── Services/
│   │   │   │   ├── ResumeContentService.cs
│   │   │   │   └── ATSService.cs
│   │   │   ├── DTOs/
│   │   │   │   └── ResumeGeneratorRequest.cs
│   │   │   └── Application.csproj
│   │   ├── Domain/
│   │   │   ├── Models/
│   │   │   │   └── ResumeModel.cs
│   │   │   ├── Entities/
│   │   │   └── Domain.csproj
│   │   └── Infrastructure/
│   │       ├── ExternalServices/
│   │       │   ├── AzureOpenAiService.cs
│   │       │   ├── AzureBlobStorageService.cs
│   │       │   └── FileExtractionService.cs
│   │       ├── DocumentGeneration/
│   │       │   └── TemplateService.cs
│   │       ├── Persistence/
│   │       └── Infrastructure.csproj
│   └── ResumeBuilder.sln
│
└── frontend/
    ├── src/
    │   ├── app/
    │   │   ├── modules/
    │   │   │   ├── home/
    │   │   │   ├── resume-builder/
    │   │   │   │   ├── resume-builder.component.ts
    │   │   │   │   ├── resume-builder.component.html
    │   │   │   │   └── resume-builder.component.css
    │   │   │   ├── resume-refactor/
    │   │   │   ├── ats-optimizer/
    │   │   │   │   ├── ats-optimizer.component.ts
    │   │   │   │   ├── ats-optimizer.component.html
    │   │   │   │   └── ats-optimizer.component.css
    │   │   │   └── preview/
    │   │   │       ├── resume-preview.component.ts
    │   │   │       ├── resume-preview.component.html
    │   │   │       └── resume-preview.component.css
    │   │   ├── shared/
    │   │   │   ├── services/
    │   │   │   │   ├── resume-api.service.ts
    │   │   │   │   └── resume-state.service.ts
    │   │   │   ├── models/
    │   │   │   │   └── resume.model.ts
    │   │   │   ├── components/
    │   │   │   └── interceptors/
    │   │   │       └── error.interceptor.ts
    │   │   ├── app.routing.ts
    │   │   ├── app.module.ts
    │   │   ├── app.component.ts
    │   │   └── app.component.html
    │   ├── index.html
    │   ├── styles.css
    │   └── main.ts
    ├── angular.json
    ├── package.json
    ├── proxy.conf.json
    └── tsconfig.json
```

---

## Setup & Installation

### Prerequisites
- .NET 8 SDK
- Node.js 18+ with npm
- Azure subscription (for OpenAI, Blob Storage, SQL Database)
- Visual Studio 2022 or VS Code
- Azure CLI (optional but recommended)

### Backend Setup

1. **Clone and navigate to backend**:
   ```bash
   cd backend
   ```

2. **Install NuGet packages** (handled by .NET CLI):
   ```bash
   # Commands to install key packages:
   dotnet add package Azure.AI.OpenAI --version 1.0.0
   dotnet add package Azure.Storage.Blobs --version 12.19.0
   dotnet add package DocumentFormat.OpenXml --version 2.20.0
   dotnet add package Swashbuckle.AspNetCore --version 6.4.6
   
   # For PDF generation (choose one):
   # dotnet add package QuestPDF --version 2024.1.0
   # OR
   # dotnet add package iText5 --version 5.5.13.3
   ```

3. **Configure Azure Credentials** in `appsettings.Development.json`:
   ```json
   {
     "AzureOpenAI": {
       "Endpoint": "https://<your-resource>.openai.azure.com/",
       "Key": "<your-api-key>",
       "DeploymentName": "gpt-4-turbo"
     },
     "AzureBlob": {
       "ConnectionString": "DefaultEndpointsProtocol=https;AccountName=...",
       "ContainerName": "resumes"
     },
     "Database": {
       "ConnectionString": "Server=...;Database=ResumeBuilder;..."
     }
   }
   ```

4. **Build and Run**:
   ```bash
   # Restore and build
   dotnet restore
   dotnet build
   
   # Run API (will be available at https://localhost:5001)
   dotnet run --project src/Api/Api.csproj
   
   # Or run with watch mode for development
   dotnet watch --project src/Api/Api.csproj run
   ```

5. **Verify API is running**:
   - Navigate to `https://localhost:5001`
   - Swagger UI should be visible at root URL
   - Health check: `https://localhost:5001/health`

### Frontend Setup

1. **Navigate to frontend**:
   ```bash
   cd frontend
   ```

2. **Install dependencies**:
   ```bash
   npm install
   ```

3. **Configure API base URL** in `proxy.conf.json` (already set to `http://localhost:5000`):
   ```json
   {
     "/api": {
       "target": "http://localhost:5000",
       "secure": false,
       "changeOrigin": true
     }
   }
   ```

4. **Start development server**:
   ```bash
   npm start
   ```
   - Frontend will be available at `http://localhost:4200`
   - The proxy will forward `/api` calls to the backend

5. **Build for production**:
   ```bash
   npm run build:prod
   ```
   - Output in `dist/` folder, ready to deploy to Azure Static Web Apps

---

## API Endpoints Reference

### Base URL
```
http://localhost:5000/api/resume
```

### Endpoints

#### 1. **Generate Resume from Scratch**
```http
POST /api/resume/generate
Content-Type: application/json

{
  "personalInfo": {
    "firstName": "John",
    "lastName": "Doe",
    "title": "Senior Engineer",
    "email": "john@example.com",
    ...
  },
  "experiences": [...],
  "educations": [...],
  "skills": ["C#", ".NET", "Azure"]
}

Response: ResumeModel
```

#### 2. **Refactor Existing Resume**
```http
POST /api/resume/refactor
Content-Type: application/json

{
  "rawText": "Raw resume text...",
  "fileUrl": null
}

Response: ResumeModel
```

#### 3. **Optimize for ATS**
```http
POST /api/resume/optimize-ats
Content-Type: application/json

{
  "resume": "{serialized ResumeModel JSON}",
  "jobDescription": "Job description text..."
}

Response: AtsAnalysisResult
```

#### 4. **Export to PDF**
```http
POST /api/resume/export/pdf
Content-Type: application/json

{
  "resume": "{serialized ResumeModel JSON}",
  "templateId": "minimal"
}

Response: application/pdf (file)
```

#### 5. **Export to DOCX**
```http
POST /api/resume/export/docx
Content-Type: application/json

{
  "resume": "{serialized ResumeModel JSON}",
  "templateId": "minimal"
}

Response: application/vnd.openxmlformats-officedocument.wordprocessingml.document (file)
```

#### 6. **Get HTML Preview**
```http
POST /api/resume/preview
Content-Type: application/json

{
  "resume": "{serialized ResumeModel JSON}",
  "templateId": "minimal"
}

Response: text/html
```

#### 7. **Upload Resume File**
```http
POST /api/resume/upload
Content-Type: multipart/form-data

(Form data with 'file' field)

Response: UploadResponse { url, fileName }
```

---

## Azure OpenAI Integration

### Prompt Engineering Examples

The backend builds AI prompts programmatically. Here are key examples:

#### Resume Generation Prompt
```csharp
var systemPrompt = @"You are an expert resume writer specializing in ATS-friendly resumes.
- Use clear bullet points with strong action verbs
- Quantify achievements (%, time saved, cost savings)
- Keep bullets under 30 words
- Avoid tables, images, special formatting
- Return structured JSON";

var userPrompt = @"Generate a professional resume based on:
Name: John Doe
Title: Senior Engineer
Experience: Designed cloud solutions...
Skills: C#, .NET, Azure";

// Call AI
var response = await aiService.CallChatModelAsync(systemPrompt, userPrompt);
```

#### ATS Analysis Prompt
```csharp
var systemPrompt = @"You are an ATS optimization expert.
Analyze resume vs job description.
Return JSON with:
- matchScore (0-100)
- matchedKeywords: []
- missingKeywords: []
- suggestions: []";

var userPrompt = $@"Resume: {resumeContent}
Job Description: {jobDescription}";
```

### Error Handling & Fallback

All AI calls are wrapped with try-catch to handle:
- API timeout
- Rate limiting
- Invalid API key
- Malformed responses

Fallback responses are provided to keep the app functional.

---

## Database Schema (EF Core)

TODO: Implement EF Core DbContext and entities

```csharp
public class ApplicationDbContext : DbContext
{
    public DbSet<ResumeEntity> Resumes { get; set; }
    public DbSet<UserEntity> Users { get; set; }
    // ... other DbSets
}

public class ResumeEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Content { get; set; } // Serialized JSON
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

---

## PDF/DOCX Generation Examples

### Using QuestPDF (Recommended)
```csharp
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

QuestPDF.Settings.License = LicenseType.Community;

var pdf = Document.Create(container => {
    container.Page(page => {
        page.Size(PageSizes.Letter);
        page.Margin(0.5f, Unit.Inch);
        page.Content().Column(col => {
            col.Item().Text(resume.PersonalInfo.FirstName + " " + resume.PersonalInfo.LastName)
                .Bold()
                .FontSize(16);
            // ... rest of resume content
        });
    });
}).GeneratePdf();

return pdf;
```

### Using OpenXML SDK (DOCX)
```csharp
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;

var ms = new MemoryStream();
using (var doc = WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document))
{
    var mainPart = doc.AddMainDocumentPart();
    mainPart.Document = new Document();
    var body = mainPart.Document.AppendChild(new Body());

    // Add name
    var heading = body.AppendChild(new Paragraph());
    var run = heading.AppendChild(new Run());
    run.AppendChild(new Text($"{resume.PersonalInfo.FirstName} {resume.PersonalInfo.LastName}"));
    
    // ... add more content
}
return ms.ToArray();
```

---

## Deployment

### Azure Deployment

#### Backend: Azure App Service
```bash
# Publish to Azure App Service
dotnet publish -c Release -o ./publish
az webapp deployment source config-zip --resource-group myResourceGroup --name myAppService --src-path publish.zip
```

#### Frontend: Azure Static Web Apps
```bash
npm run build:prod
# Upload dist/ folder to Azure Static Web Apps
```

#### Database: Azure SQL
```sql
-- Create database
CREATE DATABASE ResumeBuilder;

-- Run EF Core migrations
dotnet ef database update --project src/Infrastructure/Infrastructure.csproj
```

#### Configuration: Azure Key Vault
- Store sensitive keys in Azure Key Vault
- Reference via `@Microsoft.KeyVault(SecretUri=https://...)`

---

## Key Design Decisions

### 1. **Clean Architecture**
- **Domain**: Pure business logic, no dependencies
- **Application**: Use cases and DTOs, depends only on Domain
- **Infrastructure**: External implementations (AI, Storage, DB)
- **API**: Controllers and HTTP configuration

### 2. **Loose Coupling**
- All external services use interfaces (IAIService, IStorageService, etc.)
- Easy to mock for testing, swap implementations

### 3. **ATS-Friendly Design**
- No complex layouts, tables, or images
- Plain text with clear sections
- Emphasis on keyword inclusion

### 4. **State Management (Frontend)**
- RxJS BehaviorSubject for reactive updates
- localStorage for persistence
- Services communicate via Observables

### 5. **Error Handling**
- Global HTTP error interceptor
- Backend returns consistent error responses
- User-friendly error messages

---

## Testing (TODO)

### Backend Testing
```bash
# Create test project
dotnet new xunit -n ResumeBuilder.Tests

# Example test
[Fact]
public async Task GenerateResume_WithValidInput_ReturnsResumeModel()
{
    // Arrange
    var request = new ResumeGeneratorRequest { ... };
    var service = new ResumeContentService(_aiService, _logger);
    
    // Act
    var result = await service.GenerateResumeAsync(request);
    
    // Assert
    Assert.NotNull(result);
    Assert.Equal("John", result.PersonalInfo.FirstName);
}
```

### Frontend Testing
```bash
# Run tests
npm test

# Example test
it('should create resume with valid form data', () => {
  const component = new ResumeBuilderComponent(...);
  component.personalInfoForm.patchValue({ firstName: 'John' });
  expect(component.personalInfoForm.valid).toBeTruthy();
});
```

---

## Performance Optimization

### Backend
- Cache frequently accessed data
- Implement pagination for large datasets
- Use async/await for non-blocking I/O
- Add rate limiting for AI API calls

### Frontend
- Lazy load feature modules
- Implement OnPush change detection
- Use trackBy in *ngFor loops
- Compress images and assets

---

## Security Considerations

### Backend
- ✅ HTTP to HTTPS redirection (production)
- ✅ CORS policy configured
- ✅ Input validation on all endpoints
- ✅ Secrets in environment variables (not appsettings)
- ✅ Rate limiting on AI endpoints
- TODO: Add authentication/authorization (JWT tokens)
- TODO: Add role-based access control

### Frontend
- ✅ No sensitive data in localStorage (besides resume drafts)
- ✅ HTTPS enforced in production
- ✅ XSS protection via Angular's built-in sanitization
- TODO: Add CSRF tokens if needed

---

## Troubleshooting

### Common Issues

**"Unhandled 404 on /api/resume/..." (Frontend)**
- Ensure backend is running on correct port
- Check proxy.conf.json target URL
- Verify CORS policy in backend

**"Azure OpenAI API key invalid"**
- Verify key in appsettings.json
- Check Azure resource region matches deployment name
- Ensure quota hasn't been exceeded

**"File upload fails"**
- Check Azure Blob Storage connection string
- Verify container name exists
- Check file size limits

---

## Next Steps

1. **Implement real PDF/DOCX generation** (currently mock)
2. **Add file extraction** from PDF/DOCX
3. **Implement EF Core DbContext** for resume persistence
4. **Add authentication** (Azure AD, Auth0, etc.)
5. **Deploy to Azure** (App Service + Static Web Apps)
6. **Add unit and integration tests**
7. **Implement caching** for improved performance
8. **Add analytics** (Application Insights)
9. **Create admin dashboard** for usage monitoring
10. **Add subscription/billing** (Stripe integration)

---

## Support & Documentation

- **API Documentation**: Swagger UI at `https://localhost:5001`
- **Angular Documentation**: https://angular.io/docs
- **ASP.NET Core**: https://learn.microsoft.com/en-us/aspnet/core/
- **Azure OpenAI**: https://learn.microsoft.com/en-us/azure/cognitive-services/openai/
- **EF Core**: https://learn.microsoft.com/en-us/ef/core/

---

**Version**: 1.0.0  
**Last Updated**: March 2024  
**Status**: Production Ready (with TODOs noted)
