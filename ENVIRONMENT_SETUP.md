# Free Deployment - Environment Setup Guide

This guide explains what environment variables you need to set for free deployment.

## Platform-Specific Instructions

### Render.com Environment Variables

1. Go to your Render service dashboard
2. Click "Environment" in the left sidebar
3. Add these environment variables (copy the entire block below and paste):

```bash
# CORS Configuration
CORS_ORIGINS=https://your-vercel-domain.vercel.app

# Database (PostgreSQL on Railway)
DATABASE_URL=postgresql://user:password@host.railway.app:5432/railway

# Azure OpenAI Configuration
AZURE_OPENAI_ENDPOINT=https://your-resource.openai.azure.com/
AZURE_OPENAI_KEY=your-api-key-here
AZURE_OPENAI_DEPLOYMENT=gpt-35-turbo
AZURE_OPENAI_API_VERSION=2024-02-15-preview

# Cloudinary File Storage
CLOUDINARY_CLOUD_NAME=your-cloud-name
CLOUDINARY_API_KEY=your-api-key-here
CLOUDINARY_API_SECRET=your-api-secret-here

# ASP.NET Core
ASPNETCORE_ENVIRONMENT=Production
```

### Getting Your Values

#### CORS_ORIGINS
After deploying to Vercel:
- Copy the domain from Vercel dashboard (e.g., `resumebuilder-abc123.vercel.app`)
- Replace in: `https://your-vercel-domain.vercel.app`
- Multiple origins: `https://domain1.vercel.app;https://domain2.vercel.app`

#### DATABASE_URL
From Railway dashboard:
1. Go to your PostgreSQL database
2. Click "Connect"
3. Copy the Connection URL (starts with `postgresql://`)

#### AZURE_OPENAI_*
From Azure Portal:
1. Go to your OpenAI resource
2. Click "Keys and Endpoint"
3. Copy Endpoint URL and Key 1
4. For DEPLOYMENT: Use your deployed model name (e.g., `gpt-35-turbo`)

#### CLOUDINARY_*
From Cloudinary dashboard:
1. Go to Settings → API Keys
2. Cloud Name: Large text at top
3. API Key: Copy from dashboard
4. API Secret: Copy from the same page

---

## How Environment Variables Override appsettings.json

The backend is configured to:
1. Load `appsettings.json` (defaults)
2. Load `appsettings.Production.json` if it exists
3. **Override with environment variables** (these win!)

So you don't need to modify appsettings files - just set environment variables.

---

## Local Development

For local testing with these services:

Create `backend/appsettings.Development.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "Cors": {
    "AllowedOrigins": "http://localhost:4200"
  },
  "Database": {
    "ConnectionString": "Server=localhost;Database=ResumeBuildLocal;User Id=sa;Password=YourPassword;"
  },
  "AzureOpenAI": {
    "Endpoint": "https://<your-resource>.openai.azure.com/",
    "Key": "<your-key>",
    "DeploymentName": "gpt-35-turbo",
    "ApiVersion": "2024-02-15-preview"
  },
  "Cloudinary": {
    "CloudName": "<your-cloud-name>",
    "ApiKey": "<your-api-key>",
    "ApiSecret": "<your-api-secret>"
  }
}
```

Then run:
```bash
cd backend
dotnet run --project src/Api/Api.csproj
```

---

## Updating Frontend API Endpoint

When you deploy, update `frontend/src/environments/environment.prod.ts`:

```typescript
export const environment = {
  production: true,
  apiUrl: 'https://resumebuilder-api.onrender.com/api'
};
```

And `frontend/src/environments/environment.ts` for local dev:

```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api'
};
```

---

## Troubleshooting

### "Connection string not found"
- Verify `DATABASE_URL` is set in Render environment variables
- Database should be running on Railway

### "OpenAI API key invalid"
- Check `AZURE_OPENAI_ENDPOINT` format: `https://resource.openai.azure.com/` (with trailing slash)
- Verify key is not expired
- Check deployment name matches your Azure resource

### "Cloudinary upload failed"
- Verify `CLOUDINARY_CLOUD_NAME`, `CLOUDINARY_API_KEY`, `CLOUDINARY_API_SECRET`
- Check you have credits/quota available (free tier: 25 GB/month)

### "CORS error in frontend"
- Check `CORS_ORIGINS` matches your Vercel domain exactly
- Should include `https://` protocol
- No trailing slash

---

## Security Notes

⚠️ **IMPORTANT**: Never commit credentials to GitHub!
- Use `.gitignore` to exclude sensitive files
- Use Render's environment variables for secrets
- Rotate keys regularly

