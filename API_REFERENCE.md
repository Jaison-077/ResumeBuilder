# API Reference & Developer Guide

## Base URL

```
Production: https://api.yourdomain.com
Development: http://localhost:5000
```

## Authentication

**Current Status**: No authentication required  
**TODO**: Implement JWT tokens with Azure AD/Auth0

## Content-Type
All requests must include:
```
Content-Type: application/json
```

---

## Endpoints

### 1. Resume Generation
**Create a polished resume from structured form input**

```http
POST /api/resume/generate
```

**Request Body**:
```json
{
  "personalInfo": {
    "firstName": "John",
    "lastName": "Doe",
    "title": "Senior Software Engineer",
    "location": "Seattle, WA",
    "email": "john@example.com",
    "phone": "+1-206-555-0123",
    "linkedInUrl": "https://linkedin.com/in/johndoe",
    "portfolioUrl": "https://johndoe.dev",
    "gitHubUrl": "https://github.com/johndoe"
  },
  "desiredJobTitle": "Staff Engineer",
  "experiences": [
    {
      "company": "Tech Corp",
      "title": "Senior Developer",
      "location": "Seattle, WA",
      "startDate": "2020-01-15T00:00:00",
      "endDate": null,
      "isCurrentRole": true,
      "description": "Led development of microservices architecture serving 10M users. Reduced API latency by 40% through optimization."
    }
  ],
  "educations": [
    {
      "institution": "University of Washington",
      "degree": "Bachelor",
      "major": "Computer Science",
      "location": "Seattle, WA",
      "graduationDate": "2016-06-01T00:00:00",
      "gpa": "3.85"
    }
  ],
  "skills": ["C#", ".NET Core", "Azure", "Docker", "Kubernetes"]
}
```

**Response** (200 OK):
```json
{
  "personalInfo": {...},
  "summary": "Results-driven Senior Software Engineer with 8+ years of experience...",
  "experiences": [
    {
      "company": "Tech Corp",
      "title": "Senior Developer",
      "location": "Seattle, WA",
      "startDate": "2020-01-15T00:00:00",
      "endDate": null,
      "isCurrentRole": true,
      "bullets": [
        "Led microservices architecture redesign, reducing API latency 40% and improving reliability to 99.99%",
        "Mentored team of 5 engineers, conducting code reviews and establishing best practices",
        "Optimized database queries and implemented Redis caching, cutting response times by 60%"
      ]
    }
  ],
  "educations": [...],
  "skills": [
    {
      "category": "Languages",
      "items": ["C#", "TypeScript", "Python"]
    },
    {
      "category": "Cloud & Infrastructure",
      "items": ["Azure", "Docker", "Kubernetes"]
    }
  ],
  "projects": [],
  "certifications": []
}
```

**Error Responses**:
- `400 Bad Request`: Invalid input
- `500 Internal Server Error`: AI service failed

---

### 2. Resume Refactoring
**Transform raw/messy resume into ATS-friendly format**

```http
POST /api/resume/refactor
```

**Request Body** (Option A - Raw Text):
```json
{
  "rawText": "John Doe\nEmail: john@example.com\nSenior Software Engineer\n\nWork Experience\nTech Corp - Senior Developer (2020-Present)\nWorked on cloud infrastructure stuff. Made things faster.",
  "fileUrl": null
}
```

**Request Body** (Option B - File):
```json
{
  "rawText": null,
  "fileUrl": "https://resumebuilderstorage.blob.core.windows.net/resumes/resume-upload-abc123.pdf"
}
```

**Response** (200 OK):
```json
{
  "personalInfo": {
    "firstName": "John",
    "lastName": "Doe",
    "title": "Senior Software Engineer",
    "email": "john@example.com",
    "phone": "",
    "location": ""
  },
  "summary": "Experienced Senior Software Engineer with proven track record in cloud infrastructure...",
  "experiences": [...],
  "educations": [],
  "skills": [...],
  "projects": [],
  "certifications": []
}
```

---

### 3. ATS Optimization
**Analyze resume match against job description**

```http
POST /api/resume/optimize-ats
```

**Request Body**:
```json
{
  "resume": "{\"personalInfo\":{...}, \"experiences\":[...]}",
  "jobDescription": "We are looking for a Senior Software Engineer with 5+ years of experience in C# and Azure. Required: microservices, Docker, Kubernetes. Nice to have: ML/AI experience."
}
```

**Response** (200 OK):
```json
{
  "matchScore": 78,
  "matchedKeywords": [
    "Senior Software Engineer",
    "C#",
    "Azure",
    "microservices",
    "Docker",
    "Kubernetes",
    "5+ years"
  ],
  "missingKeywords": [
    "Machine Learning",
    "TensorFlow",
    "AWS",
    "Agile Scrum certification"
  ],
  "suggestions": [
    "Add 'Machine Learning' to your skills if you have any ML project experience",
    "Highlight your microservices architecture work more explicitly in experience bullets",
    "Consider adding a summary statement emphasizing your Azure expertise",
    "Add any certifications or training in AI/ML to stand out"
  ],
  "optimizedResume": {
    "personalInfo": {...},
    "summary": "Results-driven Senior Software Engineer with 8+ years specializing in C# development, Azure cloud architecture, and microservices...",
    "experiences": [...]
  }
}
```

**Match Score Interpretation**:
- 0-30: Poor match - significant gaps
- 31-60: Fair match - some keywords missing
- 61-80: Good match - well aligned
- 81-100: Excellent match - highly compatible

---

### 4. PDF Export
**Download resume as PDF**

```http
POST /api/resume/export/pdf
```

**Request Body**:
```json
{
  "resume": "{\"personalInfo\":{...}, \"experiences\":[...]}",
  "templateId": "minimal"
}
```

**Template Options**:
- `minimal`: Clean, ATS-friendly, no colors
- `modern`: Contemporary design with subtle colors
- `classic`: Traditional resume format

**Response** (200 OK):
- Content-Type: `application/pdf`
- Body: Binary PDF file

**Curl Example**:
```bash
curl -X POST http://localhost:5000/api/resume/export/pdf \
  -H "Content-Type: application/json" \
  -d '{...}' \
  --output resume.pdf
```

---

### 5. DOCX Export
**Download resume as Word document**

```http
POST /api/resume/export/docx
```

**Request Body**:
```json
{
  "resume": "{\"personalInfo\":{...}, \"experiences\":[...]}",
  "templateId": "minimal"
}
```

**Response** (200 OK):
- Content-Type: `application/vnd.openxmlformats-officedocument.wordprocessingml.document`
- Body: Binary DOCX file

---

### 6. HTML Preview
**Get HTML preview of resume**

```http
POST /api/resume/preview
```

**Request Body**:
```json
{
  "resume": "{\"personalInfo\":{...}, \"experiences\":[...]}",
  "templateId": "minimal"
}
```

**Response** (200 OK):
```html
<!DOCTYPE html>
<html>
<head>
  <style>...resume-specific styles...</style>
</head>
<body>
  <div class="header">
    <h1>John Doe</h1>
    <h2>Senior Software Engineer</h2>
    <div class="contact">john@example.com | +1-206-555-0123 | Seattle, WA</div>
  </div>
  ...
</body>
</html>
```

---

### 7. File Upload
**Upload resume file to blob storage**

```http
POST /api/resume/upload
Content-Type: multipart/form-data
```

**Form Data**:
```
file: <binary PDF or DOCX>
```

**Response** (200 OK):
```json
{
  "url": "https://resumebuilderstorage.blob.core.windows.net/resumes/resume-20240315-abc123.pdf",
  "fileName": "resume.pdf"
}
```

**Supported Formats**:
- `.pdf` - PDF documents
- `.docx` - Word documents
- `.doc` - Legacy Word documents

**Upload Limits**:
- Maximum file size: 10 MB
- Supported MIME types: application/pdf, application/vnd.openxmlformats-officedocument.wordprocessingml.document

---

### 8. Health Check
**Check API health status**

```http
GET /health
```

**Response** (200 OK):
```json
{
  "status": "healthy",
  "timestamp": "2024-03-15T10:30:45.123Z"
}
```

---

## Error Handling

### Standard Error Response Format

```json
{
  "message": "User-friendly error message",
  "error": "Detailed error information",
  "timestamp": "2024-03-15T10:30:45.123Z"
}
```

### Common Status Codes

| Code | Meaning | Example |
|------|---------|---------|
| 200 | Success | Request processed successfully |
| 400 | Bad Request | Invalid JSON or missing required fields |
| 401 | Unauthorized | Authentication required (when implemented) |
| 403 | Forbidden | Access denied to resource |
| 404 | Not Found | Endpoint doesn't exist |
| 422 | Unprocessable Entity | Input validation failed |
| 429 | Too Many Requests | Rate limit exceeded |
| 500 | Server Error | Internal error (check logs) |
| 503 | Unavailable | Azure OpenAI service down |

### Example Error Responses

**Missing Required Field** (400):
```json
{
  "message": "Validation failed",
  "error": "Field 'email' is required",
  "timestamp": "2024-03-15T10:30:45.123Z"
}
```

**AI Service Unavailable** (503):
```json
{
  "message": "Error generating resume",
  "error": "Azure OpenAI service is currently unavailable",
  "timestamp": "2024-03-15T10:30:45.123Z"
}
```

---

## Rate Limiting

**Current**: No rate limiting (development)  
**Production** (TODO): 
- 100 requests per minute per IP
- 10 AI generation requests per hour per user

---

## Data Models

### ResumeModel
```typescript
{
  personalInfo: PersonalInfo;
  summary?: string;
  experiences: ExperienceEntry[];
  educations: EducationEntry[];
  skills: Skill[];
  projects: Project[];
  certifications: Certification[];
}
```

### PersonalInfo
```typescript
{
  firstName: string;
  lastName: string;
  title: string;
  location: string;
  email: string;
  phone: string;
  linkedInUrl?: string;
  portfolioUrl?: string;
  gitHubUrl?: string;
}
```

### ExperienceEntry
```typescript
{
  company: string;
  title: string;
  location: string;
  startDate: Date;
  endDate?: Date;
  isCurrentRole: boolean;
  bullets: string[];
}
```

### Skill
```typescript
{
  category: string; // "Languages", "Frameworks", "Tools", etc.
  items: string[];
}
```

---

## Example Workflows

### Workflow 1: Complete Resume Generation
```json
// Step 1: Generate resume
POST /api/resume/generate
Request: {...personal info, experiences, education, skills}
Response: First version of polished resume

// Step 2: Preview resume
POST /api/resume/preview
Request: {resume, templateId: "modern"}
Response: HTML preview

// Step 3: Optimize for job
POST /api/resume/optimize-ats
Request: {resume, jobDescription}
Response: Match score and suggestions

// Step 4: Export to PDF
POST /api/resume/export/pdf
Request: {resume, templateId: "modern"}
Response: PDF file download
```

### Workflow 2: Refactor Existing Resume
```json
// Step 1: Upload existing resume
POST /api/resume/upload
Request: multipart/form-data with file
Response: {url, fileName}

// Step 2: Refactor uploaded resume
POST /api/resume/refactor
Request: {fileUrl: "https://..."}
Response: Structured, polished resume

// Step 3: Preview and export (same as Workflow 1)
```

---

## SDKs & Client Libraries

### JavaScript/TypeScript
```typescript
import { ResumeApiService } from './shared/services/resume-api.service';

// Inject service
constructor(private apiService: ResumeApiService) {}

// Use methods
this.apiService.generateResume(request).subscribe(
  resume => console.log(resume),
  error => console.error(error)
);
```

### C# (.NET)
```csharp
using ResumeBuilder.Application.Interfaces;

// Inject dependencies
public class MyService
{
    private readonly IResumeContentService _resumeService;
    public MyService(IResumeContentService resumeService)
    {
        _resumeService = resumeService;
    }

    public async Task Test()
    {
        var resume = await _resumeService.GenerateResumeAsync(request);
    }
}
```

### cURL
See individual endpoint examples above

---

## Testing the API

### Using Swagger UI
1. Navigate to `https://localhost:5001`
2. Click on endpoint
3. Click "Try it out"
4. Fill in request body
5. Click "Execute"

### Using Postman
1. Import OpenAPI spec from `https://localhost:5001/swagger/v1/swagger.json`
2. All endpoints will be pre-configured
3. Set environment variables for base URL
4. Send requests

### Using VS Code REST Client
Create `test.http`:
```http
### Test Generate Resume
POST http://localhost:5000/api/resume/generate
Content-Type: application/json

{
  "personalInfo": {...},
  "experiences": [...],
  "educations": [...],
  "skills": [...]
}
```

---

## Support & Debugging

### Enable Detailed Logging
In `appsettings.Development.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft": "Debug"
    }
  }
}
```

### Check API Logs
```bash
# Local development
dotnet run --project src/Api/Api.csproj

# Production (Azure)
az webapp log tail --name resumebuilder-api --resource-group resume-builder-rg
```

---

**API Version**: 1.0  
**Last Updated**: March 2024  
**Status**: Active
