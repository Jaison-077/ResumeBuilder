# FREE Deployment Implementation Checklist

Complete these steps in order to deploy your application for FREE.

## ✅ Pre-Deployment Setup (Local)

### 1. Update Backend Configuration
- [ ] Review `backend/src/Api/Program.cs` 
- [ ] Compare with `Program.cs.free-deployment` template
- [ ] Update CORS policy to read from environment variables
- [ ] Add `using ResumeBuilder.Infrastructure.ExternalServices;`
- [ ] Register `CloudinaryStorageService` and `ConfigurationExtensions`

### 2. Create Docker Configuration
- [ ] Create `backend/Dockerfile` (✅ Already done)
- [ ] Create `backend/.dockerignore` (✅ Already done)
- [ ] Test locally: `docker build -t resumebuilder-api .`

### 3. Update appsettings.json
- [ ] Backup `appsettings.json`
- [ ] Check all sections use `${VAR_NAME}` or sensible defaults
- [ ] Key sections needed:
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
      "DeploymentName": "${AZURE_OPENAI_DEPLOYMENT}"
    },
    "Cloudinary": {
      "CloudName": "${CLOUDINARY_CLOUD_NAME}",
      "ApiKey": "${CLOUDINARY_API_KEY}",
      "ApiSecret": "${CLOUDINARY_API_SECRET}"
    }
  }
  ```

### 4. Update Frontend Environment
- [ ] Open `frontend/src/environments/environment.prod.ts`
- [ ] Add: `apiUrl: 'https://resumebuilder-api.onrender.com/api'`
- [ ] Update `environment.ts` for local dev (keep localhost:5000)

### 5. Test Locally
```bash
# Build frontend
cd frontend
npm install --legacy-peer-deps
npm run build:prod

# Run backend
cd backend
dotnet run --project src/Api/Api.csproj
```
- [ ] Backend running on `http://localhost:5000`
- [ ] Frontend accessible on `http://localhost:4200`
- [ ] Swagger API docs available at `http://localhost:5000`

---

## 🎯 Cloud Service Setup

### 6. Create Vercel Account (Frontend)
- [ ] Go to https://vercel.com
- [ ] Sign up with GitHub
- [ ] Click "New Project"
- [ ] Import ResumeBuilder GitHub repository
- [ ] Settings:
  - Build Command: `npm install --legacy-peer-deps && npm run build`
  - Output Directory: `dist`
  - Root Directory: `frontend`
- [ ] Click "Deploy"
- [ ] **Note**: Copy your Vercel domain (e.g., `resumebuilder-abc123.vercel.app`)

### 7. Create Railway Database (PostgreSQL)
- [ ] Go to https://railway.app
- [ ] Sign up with GitHub
- [ ] Click "New Project" → "Provision PostgreSQL"
- [ ] Wait for deployment (2-3 minutes)
- [ ] Go to PostgreSQL service → "Connect"
- [ ] Copy **Database URL** connection string
- [ ] Format: `postgresql://user:password@host:port/database`

### 8. Create Cloudinary Account (File Storage)
- [ ] Go to https://cloudinary.com
- [ ] Sign up free
- [ ] Go to Dashboard
- [ ] Note your **Cloud Name** (displayed at top)
- [ ] Click Settings → API Keys
- [ ] Copy:
  - [ ] Cloud Name
  - [ ] API Key
  - [ ] API Secret

### 9. Create Azure Free Account (AI)
- [ ] Go to https://azure.microsoft.com/en-us/free
- [ ] Sign up with credit card (no charge, get $200 credits)
- [ ] Create resource group: `resume-builder`
- [ ] Create OpenAI resource
- [ ] Deploy model: `gpt-35-turbo`
- [ ] Go to Keys and Endpoint
- [ ] Copy:
  - [ ] **Endpoint** URL (with trailing slash)
  - [ ] **Key 1**
  - [ ] **Deployment Name** (gpt-35-turbo)

### 10. Create Render Account (Backend)
- [ ] Go to https://render.com
- [ ] Sign up with GitHub
- [ ] Click "New" → "Web Service"
- [ ] Connect your GitHub repository (ResumeBuilder)
- [ ] Settings:
  - Service Name: `resumebuilder-api`
  - Environment: **Docker**
  - Plan: **Free**
  - Region: **Virginia**
- [ ] **DO NOT deploy yet** - need to add environment variables first

---

## 🔧 Configure Cloud Services

### 11. Add Environment Variables to Render
In Render dashboard for your `resumebuilder-api` service:
- [ ] Click "Environment" in left sidebar
- [ ] Add the following environment variables:

```
CORS_ORIGINS=https://your-vercel-domain.vercel.app
DATABASE_URL=postgresql://user:pass@host:port/database
AZURE_OPENAI_ENDPOINT=https://your-resource.openai.azure.com/
AZURE_OPENAI_KEY=your-key-here
AZURE_OPENAI_DEPLOYMENT=gpt-35-turbo
CLOUDINARY_CLOUD_NAME=your-cloud-name
CLOUDINARY_API_KEY=your-api-key
CLOUDINARY_API_SECRET=your-api-secret
ASPNETCORE_ENVIRONMENT=Production
```

### 12. Deploy Backend on Render
- [ ] In Render dashboard, click "Deploy" button
- [ ] Wait for build and deployment (5-10 minutes)
- [ ] Check logs for errors
- [ ] **Note**: Copy your Render domain (e.g., `resumebuilder-api.onrender.com`)

### 13. Update Frontend API Endpoint
- [ ] Go to Vercel project settings
- [ ] Add environment variable:
  - [ ] Key: `ANGULAR_API_URL`
  - [ ] Value: `https://resumebuilder-api.onrender.com`
- [ ] Redeploy frontend

---

## 🧪 Testing & Verification

### 14. Test API Endpoints
- [ ] Visit `https://resumebuilder-api.onrender.com/swagger/index.html`
- [ ] Check /health endpoint returns `{ status: "healthy" }`
- [ ] Test GET /swagger endpoints populated

### 15. Test Frontend
- [ ] Visit `https://your-frontend.vercel.app`
- [ ] Check browser console for API errors
- [ ] Try: Home → Resume Builder → Fill form
- [ ] Verify API calls succeed (Network tab in DevTools)

### 16. Test File Upload
- [ ] Navigate to Resume Refactor section
- [ ] Try uploading a PDF file
- [ ] Verify file appears in Cloudinary dashboard

### 17. Test AI Generation
- [ ] Fill resume form, submit
- [ ] Check if Azure OpenAI is called
- [ ] Verify response appears in preview

### 18. Monitor Logs
- [ ] Render Dashboard → Your Service → Logs
- [ ] Check for any errors
- [ ] Verify database connections

---

## 🚀 Final Checks

- [ ] Frontend accessible at your Vercel domain
- [ ] Backend accessible at your Render domain
- [ ] API endpoint calls working
- [ ] Database connection established
- [ ] File uploads working
- [ ] AI generation working
- [ ] No CORS errors in console

---

## 📊 Cost Verification

Verify you're on free tiers:

**Vercel**
- [ ] Dashboard → Settings → Billing
- [ ] Plan: **Free**

**Render**
- [ ] Dashboard → Your Service
- [ ] Plan: **Free**

**Railway**
- [ ] Dashboard → Your Project
- [ ] Check usage under 5 GB

**Cloudinary**
- [ ] Dashboard → Settings → Billing
- [ ] Plan: **Free**
- [ ] Usage tracking visible

**Azure**
- [ ] Azure Portal → Cost Management
- [ ] Verify you have free credits
- [ ] Monitor OpenAI usage

---

## 🔄 What Happens Next

### Cold Starts (Expected - Free Tier Behavior)
- First request to backend after 15 min inactivity takes ~30 seconds
- This is normal for Render free tier
- Upgrade to Standard ($7/month) to eliminate cold starts

### Monitoring
- Check Render logs daily for errors
- Monitor Azure OpenAI token usage
- Verify Cloudinary bandwidth stays under 25 GB/month

### Scaling
When you're ready to upgrade:
- Frontend: Keep Vercel (always free for SPA)
- Backend: Upgrade Render to Standard ($7/month)
- Database: Upgrade Railway to paid tier ($10 GB/month)
- Storage: Upgrade Cloudinary ($99/month unlimited)

---

## 🆘 Troubleshooting

### Frontend Won't Load
- Check Vercel logs: Dashboard → Deployments → Logs
- Verify environment variables set
- Check CORS_ORIGINS on backend

### Backend API Errors
- Check Render logs for exceptions
- Verify environment variables in Render dashboard
- Check Azure OpenAI key validity
- Verify DATABASE_URL connection string format

### Cold Start Too Long
- This is expected (30 seconds) on free tier
- Upgrade to Render Standard ($7/month) to fix
- Or use a 3rd-party service to ping your API every 10 minutes

### Database Connection Failed
- Verify DATABASE_URL in Render environment
- Check Railway PostgreSQL is running
- Verify firewall/network settings

---

## Need Help?

If you encounter errors:
1. Check Render logs for detailed error messages
2. Verify all environment variables are set correctly
3. Test API locally before deploying
4. Check GitHub issues for similar problems

Good luck with your deployment! 🎉

