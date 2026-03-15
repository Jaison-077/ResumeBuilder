# Deployment Guide

## Overview

This guide covers deploying the AI Resume Generator to Azure in production.

## Prerequisites

- Azure subscription
- Azure CLI installed (`az --version`)
- GitHub account (for CI/CD pipeline)
- Domain name (optional, for custom domain)

## Architecture Diagram

```
┌────────────┐
│   Client   │ (Browser)
└──────┬─────┘
       │ HTTPS
┌──────┴─────────────────────────┐
│  Azure Static Web Apps          │ ← Frontend (Angular)
│  (Azure CDN)                    │
└──────┬─────────────────────────┘
       │ HTTPS (Private Endpoint)
┌──────┴────────────────────────────────┐
│  Azure App Service                     │ ← Backend (.NET 8)
│  (Autoscale, 2-4 instances)           │
└──────┬────────────────────────────────┘
       │
   ┌───┴──────┬──────────┬──────────┐
   │          │          │          │
┌──┴──┐  ┌───▼──┐  ┌───▼──┐  ┌───▼──┐
│Azure│  │Azure │  │Azure │  │App   │
│SQL  │  │Blob  │  │OpenAI│  │Insights│
│(DB) │  │Store │  │(AI)  │  │(Logs)│
└─────┘  └──────┘  └──────┘  └──────┘
```

## Step 1: Create Azure Resources

### 1.1 Resource Group
```bash
az group create \
  --name resume-builder-rg \
  --location eastus
```

### 1.2 Azure SQL Database
```bash
# Create SQL Server
az sql server create \
  --resource-group resume-builder-rg \
  --name resumebuilder-sql \
  --admin-user sqladmin \
  --admin-password "YourSecurePassword123!"

# Create Database
az sql db create \
  --resource-group resume-builder-rg \
  --server resumebuilder-sql \
  --name ResumeBuilder \
  --service-objective S1

# Configure firewall (allow Azure services)
az sql server firewall-rule create \
  --name AllowAzureServices \
  --resource-group resume-builder-rg \
  --server resumebuilder-sql \
  --start-ip-address 0.0.0.0 \
  --end-ip-address 0.0.0.0
```

**Get Connection String**:
```bash
az sql db show-connection-string \
  --server resumebuilder-sql \
  --name ResumeBuilder \
  --client ado.net
```

### 1.3 Azure Storage (Blob)
```bash
# Create Storage Account
az storage account create \
  --name resumebuilderstorage \
  --resource-group resume-builder-rg \
  --location eastus \
  --sku Standard_LRS

# Create Blob Container
az storage container create \
  --name resumes \
  --account-name resumebuilderstorage \
  --public-access off

# Get Connection String
az storage account show-connection-string \
  --name resumebuilderstorage \
  --resource-group resume-builder-rg \
  --query connectionString -o tsv
```

### 1.4 Azure OpenAI
```bash
# Create Cognitive Services Account
az cognitiveservices account create \
  --name resumebuilder-openai \
  --resource-group resume-builder-rg \
  --kind OpenAI \
  --sku s0 \
  --location eastus

# Deploy Model
az cognitiveservices account deployment create \
  --name resumebuilder-openai \
  --resource-group resume-builder-rg \
  --deployment-name gpt-4-turbo \
  --model-name gpt-4-turbo \
  --model-version 2024-04-09

# Get Keys
az cognitiveservices account keys list \
  --name resumebuilder-openai \
  --resource-group resume-builder-rg
```

### 1.5 App Service (Backend)
```bash
# Create App Service Plan
az appservice plan create \
  --name resume-builder-plan \
  --resource-group resume-builder-rg \
  --sku B1 \
  --is-linux

# Create App Service
az webapp create \
  --name resumebuilder-api \
  --resource-group resume-builder-rg \
  --plan resume-builder-plan \
  --runtime "DOTNET|8.0"

# Enable HTTPS only
az webapp update \
  --name resumebuilder-api \
  --resource-group resume-builder-rg \
  --https-only true
```

### 1.6 Static Web Apps (Frontend)
```bash
# Create Static Web App
az staticwebapp create \
  --name resumebuilder-app \
  --resource-group resume-builder-rg \
  --location eastus \
  --source https://github.com/<your-repo>/resume-builder.git \
  --branch main \
  --login-with-github
```

## Step 2: Configure Application Settings

### Backend Configuration

```bash
# Set App Service Configuration
az webapp config appsettings set \
  --name resumebuilder-api \
  --resource-group resume-builder-rg \
  --settings \
    AzureOpenAI__Endpoint="https://resumebuilder-openai.openai.azure.com/" \
    AzureOpenAI__Key="your-api-key" \
    AzureOpenAI__DeploymentName="gpt-4-turbo" \
    AzureBlob__ConnectionString="DefaultEndpointsProtocol=https;..." \
    AzureBlob__ContainerName="resumes" \
    Database__ConnectionString="Server=resumebuilder-sql.database.windows.net;..." \
    Cors__AllowedOrigins="https://resumebuilder-app.azurestaticapps.net" \
    ASPNETCORE_ENVIRONMENT="Production"
```

### Frontend Configuration

Create `src/environments/environment.prod.ts`:
```typescript
export const environment = {
  production: true,
  apiUrl: 'https://resumebuilder-api.azurewebsites.net/api'
};
```

## Step 3: Build & Deploy Backend

### 3.1 Build .NET Application
```bash
cd backend
dotnet publish -c Release -o ./publish
```

### 3.2 Deploy to App Service

**Option A: ZIP Deploy**
```bash
cd publish
zip -r ../publish.zip *
cd ..

az webapp deployment source config-zip \
  --resource-group resume-builder-rg \
  --name resumebuilder-api \
  --src-path publish.zip
```

**Option B: GitHub Actions (Recommended)**

Create `.github/workflows/deploy-backend.yml`:
```yaml
name: Deploy Backend

on:
  push:
    branches: [main]
    paths:
      - 'backend/**'

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest

    steps:
      - uses: actions/checkout@v3
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'
      
      - name: Build
        run: dotnet publish -c Release -o ./publish
        working-directory: backend
      
      - name: Deploy
        uses: azure/webapps-deploy@v2
        with:
          app-name: 'resumebuilder-api'
          publish-profile: ${{ secrets.AZURE_PUBLISH_PROFILE }}
          package: './backend/publish'
```

## Step 4: Build & Deploy Frontend

### 4.1 Build Angular Application
```bash
cd frontend
npm run build:prod
```

### 4.2 Deploy to Static Web Apps

**Option A: Portal**
1. Go to Static Web Apps resource
2. Click "Deployment details"
3. Copy deployment token
4. Run:
```bash
cd frontend/dist
az staticwebapp secrets list \
  --name resumebuilder-app \
  --resource-group resume-builder-rg
```

**Option B: GitHub Actions (Automatic)**

The portal will auto-generate `.github/workflows/azure-static-web-apps.yml`

## Step 5: Run Database Migrations

```bash
cd backend

# Create migration
dotnet ef migrations add Initial \
  --project src/Infrastructure/Infrastructure.csproj \
  --startup-project src/Api/Api.csproj

# Apply migrations to production database
dotnet ef database update \
  --project src/Infrastructure/Infrastructure.csproj \
  --startup-project src/Api/Api.csproj \
  --configuration Release \
  --connection "Server=resumebuilder-sql.database.windows.net;Database=ResumeBuilder;User Id=sqladmin;Password=YourPassword;"
```

## Step 6: Configure Custom Domain

```bash
# Add custom domain to Static Web Apps
az staticwebapp hostname set \
  --name resumebuilder-app \
  --resource-group resume-builder-rg \
  --hostname yourdomain.com

# Add custom domain to App Service
az webapp config hostname add \
  --webapp-name resumebuilder-api \
  --resource-group resume-builder-rg \
  --hostname api.yourdomain.com
```

## Step 7: Configure SSL/TLS

```bash
# Managed certificates (automatic via Azure)
az webapp config ssl bind \
  --name resumebuilder-api \
  --resource-group resume-builder-rg \
  --certificate-name Default
```

## Monitoring & Logging

### Application Insights

```bash
# Create Application Insights
az monitor app-insights component create \
  --app resumebuilder-insights \
  --location eastus \
  --resource-group resume-builder-rg \
  --application-type web

# Connect to App Service
az webapp config appsettings set \
  --name resumebuilder-api \
  --resource-group resume-builder-rg \
  --settings APPLICATIONINSIGHTS_CONNECTION_STRING="<connection-string>"
```

### View Logs

```bash
# Stream logs
az webapp log tail \
  --name resumebuilder-api \
  --resource-group resume-builder-rg

# Download logs
az webapp log download \
  --name resumebuilder-api \
  --resource-group resume-builder-rg \
  --log-file ~/resume-api-logs.zip
```

## Cost Optimization

### Auto-Scaling
```bash
# Create auto-scale rule
az monitor autoscale create \
  --resource-group resume-builder-rg \
  --resource-type "Microsoft.Web/serverFarms" \
  --resource-name resume-builder-plan \
  --min-count 2 \
  --max-count 4 \
  --count 2
```

### Usage Alerts
```bash
# Alert on high costs
az monitor metrics alert create \
  --name HighAPIUsage \
  --resource-group resume-builder-rg \
  --description "Alert when OpenAI usage is high"
```

## Maintenance

### Backup Database
```bash
az sql db copy \
  --dest-server resumebuilder-sql \
  --dest-name ResumeBuilder-Backup \
  --name ResumeBuilder \
  --resource-group resume-builder-rg \
  --server resumebuilder-sql
```

### Update Application
```bash
# Pull latest code
git pull origin main

# Build & deploy
dotnet publish -c Release
az webapp deployment source config-zip \
  --name resumebuilder-api \
  --resource-group resume-builder-rg \
  --src-path publish.zip
```

## Troubleshooting

**App Service won't start**
```bash
az webapp log configuration \
  --name resumebuilder-api \
  --resource-group resume-builder-rg \
  --application-logging filesystem --level verbose

az webapp log tail \
  --name resumebuilder-api \
  --resource-group resume-builder-rg
```

**Database connection fails**
- Check firewall rules allow your IP
- Verify connection string in appsettings
- Ensure migrations were applied

**Frontend can't reach API**
- Check CORS configuration
- Verify API endpoint in environment.prod.ts
- Check Static Web Apps networking

## Production Checklist

- ✅ Database backups configured
- ✅ Monitoring & logging enabled
- ✅ Auto-scaling configured
- ✅ SSL/TLS certificates active
- ✅ CORS properly configured
- ✅ Rate limiting enabled on OpenAI calls
- ✅ Environment variables secured (no hardcoded secrets)
- ✅ Database indexed for performance
- ✅ API documentation (Swagger) enabled
- ✅ Error tracking (Application Insights) active
- ✅ Disaster recovery plan in place

---

**For questions**: Refer to [Azure Docs](https://learn.microsoft.com/en-us/azure/)
