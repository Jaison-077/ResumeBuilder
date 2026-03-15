# FREE Deployment Guide - Resume Builder

## Executive Summary

Your full-stack ResumeBuilder can be deployed completely FREE using:
- **Frontend**: Vercel or Netlify (free tier)
- **Backend**: Render.com (free tier)
- **Database**: PostgreSQL on Railway or Render (free tier)
- **File Storage**: Cloudinary (free tier)
- **AI Service**: Azure OpenAI (free tier) or use OpenAI API with free credits

**Total Cost: $0/month** (with fair use limits)

---

## Architecture Overview

```
┌────────────────────────────┐
│   Vercel/Netlify           │ ← Frontend (Angular)
│   (Free tier: 100GB/month)  │
└──────────┬─────────────────┘
           │ HTTPS
           │
┌──────────▼─────────────────┐
│   Render.com               │ ← Backend (.NET 8)
│   (Free: 1 GB RAM, sleep)   │
└──────────┬─────────────────┘
           │
    ┌──────┴──────┬──────────┬──────────┐
    │             │          │          │
┌───▼──┐      ┌───▼──┐  ┌──▼───┐  ┌───▼──┐
│Railroad│    │Cloud.│  │Azure │  │Render│
│/Railway│    │inary │  │OpenAI│  │Logs  │
│(Free DB)│   │(File)│  │(AI)  │  │      │
└────────┘    └──────┘  └──────┘  └──────┘
```

---

## Option 1: RECOMMENDED - Render.com + Railway + Vercel + Azure OpenAI

This is the easiest FREE setup with minimal configuration.

### Step 1: Frontend Deployment (Vercel)

**1.1 Prepare Frontend**
```bash
cd frontend
npm install --legacy-peer-deps
npm run build:prod
```

**1.2 Deploy to Vercel**
- Go to [vercel.com](https://vercel.com)
- Sign up with GitHub
- Click "New Project" → Import your GitHub repo
- Build Command: `npm run build`
- Output Directory: `dist`
- Environment: Set `ANGULAR_ENV=production`
- Click Deploy ✅

**Vercel Free Tier Benefits:**
- ✅ 100 GB bandwidth/month
- ✅ Auto-deploy on git push
- ✅ HTTPS included
- ✅ Global CDN
- ✅ Serverless Functions (not needed here)

**Result**: Frontend available at `https://resumebuilder-[random].vercel.app`

---

### Step 2: Backend Deployment (Render.com)

**2.1 Prepare Backend**

Create `backend/Dockerfile`:
```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /build
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Api.dll"]
```

Create `backend/.dockerignore`:
```
bin/
obj/
.git/
.gitignore
.vs/
.vscode/
*.user
*.suo
*.db
node_modules/
dist/
```

Update `appsettings.json` to use environment variables:
```json
{
  "Cors": {
    "AllowedOrigins": "${CORS_ORIGINS}"
  },
  "Database": {
    "ConnectionString": "${DATABASE_URL}"
  },
  "AzureOpenAI": {
    "Endpoint": "${AZURE_OPENAI_ENDPOINT}",
    "Key": "${AZURE_OPENAI_KEY}",
    "DeploymentName": "${AZURE_OPENAI_DEPLOYMENT}",
    "ApiVersion": "2024-02-15-preview"
  },
  "Cloudinary": {
    "CloudName": "${CLOUDINARY_CLOUD_NAME}",
    "ApiKey": "${CLOUDINARY_API_KEY}",
    "ApiSecret": "${CLOUDINARY_API_SECRET}"
  }
}
```

Update `Program.cs` to read environment variables:
```csharp
var corsOrigins = Environment.GetEnvironmentVariable("CORS_ORIGINS") 
    ?? "http://localhost:4200";
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL") 
    ?? builder.Configuration["Database:ConnectionString"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(corsOrigins.Split(';'))
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});
```

**2.2 Deploy to Render**
- Go to [render.com](https://render.com)
- Sign up with GitHub
- Click "New" → "Web Service"
- Connect your GitHub repository
- Settings:
  - **Name**: `resumebuilder-api`
  - **Environment**: `Docker`
  - **Plan**: `Free`
  - **Region**: `Virginia` (closest to US)
- Click "Deploy"

**2.3 Add Environment Variables on Render**
In the Render dashboard, go to your service → "Environment":
```
CORS_ORIGINS=https://resumebuilder-[vercel].vercel.app
DATABASE_URL=postgresql://[user]:[pass]@[host]:[port]/resumebuilder
AZURE_OPENAI_ENDPOINT=https://[resource].openai.azure.com/
AZURE_OPENAI_KEY=[your-key]
AZURE_OPENAI_DEPLOYMENT=gpt-4-turbo
CLOUDINARY_CLOUD_NAME=[your-cloud-name]
CLOUDINARY_API_KEY=[your-api-key]
CLOUDINARY_API_SECRET=[your-api-secret]
```

**Render Free Tier Benefits:**
- ✅ 750 hours/month of compute (enough for a web service)
- ✅ Automatic spinning down after 15 min of inactivity (cold starts ~30s)
- ✅ HTTPS included
- ✅ PostgreSQL database (optional, see next step)

**Result**: Backend available at `https://resumebuilder-api.onrender.com`

---

### Step 3: Database (Railway or Render)

**Option A: Railway (Recommended)**
- Go to [railway.app](https://railway.app)
- Sign up with GitHub
- Click "New Project" → "Provision PostgreSQL"
- Free tier: 5GB storage, limited compute
- Copy connection string:
  ```
  postgresql://user:password@host:port/database
  ```
- Add to Render env variable as `DATABASE_URL`

**Option B: Render PostgreSQL**
- From Render dashboard → New → PostgreSQL
- Free tier: 1 GB storage
- Copy connection string

---

### Step 4: File Storage (Cloudinary)

**4.1 Create Account**
- Go to [cloudinary.com](https://cloudinary.com)
- Sign up free
- Free tier: 25GB storage, 25GB bandwidth/month

**4.2 Get Credentials**
- Dashboard → Settings → API Keys
- Copy: `Cloud Name`, `API Key`, `API Secret`

**4.3 Update Backend Service**

Create `Infrastructure/ExternalServices/CloudinaryStorageService.cs`:
```csharp
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ResumeBuilder.Application.Interfaces;
using System.IO;
using System.Threading.Tasks;

namespace ResumeBuilder.Infrastructure.ExternalServices;

public class CloudinaryStorageService : IStorageService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryStorageService(IConfiguration configuration)
    {
        var cloudName = configuration["Cloudinary:CloudName"];
        var apiKey = configuration["Cloudinary:ApiKey"];
        var apiSecret = configuration["Cloudinary:ApiSecret"];

        var account = new Account(cloudName, apiKey, apiSecret);
        _cloudinary = new Cloudinary(account);
    }

    public async Task<string> UploadAsync(Stream fileStream, string fileName)
    {
        using (fileStream)
        {
            var uploadParams = new RawUploadParams
            {
                File = new FileDescription(fileName, fileStream),
                ResourceType = "raw"
            };

            var result = await _cloudinary.UploadAsync(uploadParams);
            return result.SecureUrl.ToString();
        }
    }

    public async Task DeleteAsync(string fileUrl)
    {
        var publicId = Path.GetFileNameWithoutExtension(fileUrl);
        var deleteParams = new DeletionParams(publicId) { ResourceType = ResourceType.Raw };
        await _cloudinary.DestroyAsync(deleteParams);
    }
}
```

Update `Program.cs` DI:
```csharp
builder.Services.AddScoped<IStorageService, CloudinaryStorageService>();
```

---

### Step 5: AI Service (Azure OpenAI Free Tier)

**5.1 Create Azure Free Account**
- Go to [azure.microsoft.com/en-us/free](https://azure.microsoft.com/en-us/free)
- Sign up with credit card (no charge)
- Get **$200 free credits** + free services for 12 months

**5.2 Create OpenAI Resource**
```bash
az group create --name resume-builder --location eastus

az cognitiveservices account create \
  --name resumebuilder-openai \
  --resource-group resume-builder \
  --kind OpenAI \
  --sku s0 \
  --location eastus

az cognitiveservices account deployment create \
  --name resumebuilder-openai \
  --resource-group resume-builder \
  --deployment-name gpt-35-turbo \
  --model-name gpt-3.5-turbo \
  --model-version 0613
```

**5.3 Get Credentials**
```bash
az cognitiveservices account keys list \
  --name resumebuilder-openai \
  --resource-group resume-builder

az cognitiveservices account show \
  --name resumebuilder-openai \
  --resource-group resume-builder
```

**FREE Tier Limits:**
- ✅ 1,000 TPM (tokens per minute) for free tier
- ✅ Pay-as-you-go after free credits expire ($0.002/1K tokens)

---

## Option 2: Alternative - Use Free OpenAI API Credits + Everything Else Free

Instead of Azure OpenAI, use OpenAI's free trial credits:

**Pros:**
- More model options (GPT-4, GPT-4o)
- Simpler setup

**Cons:**
- Free credits expire after 3 months
- Requires credit card

**Setup:**
1. Go to [platform.openai.com](https://platform.openai.com)
2. Get API key
3. Update backend to use OpenAI SDK instead of Azure

```bash
dotnet add package OpenAI --project src/Infrastructure
```

Update `Infrastructure/ExternalServices/OpenAiService.cs`:
```csharp
using OpenAI;
using ResumeBuilder.Application.Interfaces;

namespace ResumeBuilder.Infrastructure.ExternalServices;

public class OpenAiService : IAIService
{
    private readonly OpenAIClient _client;

    public OpenAiService(IConfiguration configuration)
    {
        var apiKey = configuration["OpenAI:ApiKey"];
        _client = new OpenAIClient(new System.ClientModel.ApiKeyCredential(apiKey));
    }

    public async Task<string> GenerateResumeAsync(string prompt)
    {
        var messages = new[] { new { role = "user", content = prompt } };
        var completionOptions = new CompletionOptions
        {
            Model = "gpt-3.5-turbo",
            Temperature = 0.7f,
            MaxTokens = 2000
        };

        var completion = await _client.GetChatCompletionsAsync(completionOptions);
        return completion.Value.Choices[0].Message.Content;
    }
}
```

---

## Cost Breakdown (Monthly)

| Service | Free Tier | Cost |
|---------|-----------|------|
| **Frontend** (Vercel) | 100 GB bandwidth | $0 |
| **Backend** (Render) | 750 hours/month | $0 |
| **Database** (Railway) | 5 GB storage | $0 |
| **File Storage** (Cloudinary) | 25 GB bandwidth | $0 |
| **AI** (Azure OpenAI) | $200 credits (3 months) | $0 (then ~$10/mo) |
| |||
| **TOTAL** | | **$0/month** |

---

## Step-by-Step Deployment Checklist

### Phase 1: Local Testing
- [ ] `npm install --legacy-peer-deps` in `frontend/`
- [ ] `npm run build:prod` in `frontend/`
- [ ] `dotnet build` in `backend/`
- [ ] Update `appsettings.json` with test credentials

### Phase 2: Prepare Code
- [ ] Create `backend/Dockerfile`
- [ ] Create `backend/.dockerignore`
- [ ] Update `appsettings.json` to use environment variables
- [ ] Update `Program.cs` to read environment variables
- [ ] Commit changes to GitHub

### Phase 3: Setup Services
- [ ] Create Vercel account & deploy frontend
- [ ] Create Render account & deploy backend
- [ ] Create Railway account & setup PostgreSQL
- [ ] Create Cloudinary account & get API keys
- [ ] Create Azure Free account & setup OpenAI

### Phase 4: Configure Everything
- [ ] Add environment variables to Render
- [ ] Update CORS origins in backend
- [ ] Update API endpoint in frontend `environment.ts`

### Phase 5: Test Production
- [ ] Test API endpoints from frontend
- [ ] Test file uploads
- [ ] Test AI generation

---

## Quick Environment Variables for Render

```bash
# Copy this template to Render dashboard

CORS_ORIGINS=https://your-vercel-domain.vercel.app
DATABASE_URL=postgresql://user:pass@host:port/database
AZURE_OPENAI_ENDPOINT=https://your-resource.openai.azure.com/
AZURE_OPENAI_KEY=your-key
AZURE_OPENAI_DEPLOYMENT=gpt-35-turbo
CLOUDINARY_CLOUD_NAME=your-cloud-name
CLOUDINARY_API_KEY=your-api-key
CLOUDINARY_API_SECRET=your-api-secret
ASPNETCORE_ENVIRONMENT=Production
```

---

## Monitoring & Logs

**Vercel**: Dashboard → Deployments → Logs
**Render**: Dashboard → Your Service → Logs
**Railway**: Dashboard → Your Project → Logs

---

## Important Notes

### Cold Start Times
- Render free tier spins down after 15 minutes of inactivity
- First request after spin-down takes ~30 seconds
- Paid tier ($7/month) keeps service always running

### Database Connections
- Free tier PostgreSQL limited to 4 concurrent connections
- Sufficient for small-medium traffic

### Bandwidth Limits
- Vercel: 100 GB/month (plenty for a resume builder)
- Cloudinary: 25 GB/month (plenty for document storage)

### Scaling Up (When You Need It)
- **Frontend**: Upgrade to Next.js (still free)
- **Backend**: Upgrade Render to Standard ($7/month, always running)
- **Database**: Add read replicas ($15/month each)
- **Storage**: Keep Cloudinary ($99/month for unlimited)

---

## Need Help?

Create a session note with any deployment errors, and I'll help troubleshoot!

