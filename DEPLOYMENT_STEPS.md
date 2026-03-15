# Step-by-Step Deployment Guide - ResumeBuilder

Deploy your app for FREE in ~1 hour following these sequential steps.

---

## PHASE 1: LOCAL TESTING (10 minutes)

### Step 1: Install Frontend Dependencies
```bash
cd frontend
npm install --legacy-peer-deps
```
✅ **Verify**: No error messages, `node_modules/` folder created

---

### Step 2: Build Frontend
```bash
npm run build:prod
```
✅ **Verify**: `dist/` folder created with compiled files

---

### Step 3: Test Backend Locally
```bash
cd backend
dotnet build
```
✅ **Verify**: No compilation errors

---

### Step 4: Run Backend Locally
```bash
dotnet run --project src/Api/Api.csproj
```
✅ **Verify**: 
- Console output: "Now listening on: http://localhost:5000"
- Open http://localhost:5000/swagger - see API documentation

---

### Step 5: Run Frontend Locally (New Terminal)
```bash
cd frontend
npm start
```
✅ **Verify**: 
- Console output: "Application bundle generation complete"
- Open http://localhost:4200 - see resume builder UI

---

## PHASE 2: CLOUD ACCOUNT SETUP (20 minutes)

### Step 6: Create Vercel Account (Frontend Hosting)

1. Go to https://vercel.com
2. Click "Sign Up" → Select "GitHub"
3. Authorize Vercel to access your GitHub
4. Click "Skip" for team invitation
5. You're logged in! ✅

---

### Step 7: Create Render Account (Backend Hosting)

1. Go to https://render.com
2. Click "Get Started" → Select "GitHub"
3. Authorize Render to access your GitHub
4. Click "Create Dashboard"
5. You're logged in! ✅

---

### Step 8: Create Railway Account (Database)

1. Go to https://railway.app
2. Click "Login" → Select "GitHub"
3. Authorize Railway to access your GitHub
4. You're logged in! ✅

---

### Step 9: Create Cloudinary Account (File Storage)

1. Go to https://cloudinary.com
2. Click "Sign Up"
3. Fill in email, password, name
4. Verify email
5. You're logged in! ✅

**Get your credentials now:**
- Dashboard → Note your **Cloud Name** (top of page)
- Settings → API Keys → Copy **API Key** and **API Secret**
- Save these - you'll need them later

---

### Step 10: Create Azure Free Tier Account (AI Service)

1. Go to https://azure.microsoft.com/en-us/free
2. Click "Start Free"
3. Sign in with Microsoft account (or create one)
4. Add credit card (won't be charged)
5. Complete identity verification
6. Click "Next" through setup
7. Accept terms and create account
8. You're logged in! ✅

**Create OpenAI Resource:**
```bash
# Install Azure CLI if needed
# Windows: https://learn.microsoft.com/en-us/cli/azure/install-azure-cli-windows

az login

# Create resource group
az group create --name resume-builder --location eastus

# Create OpenAI resource
az cognitiveservices account create \
  --name resumebuilder-openai \
  --resource-group resume-builder \
  --kind OpenAI \
  --sku s0 \
  --location eastus

# Deploy GPT-3.5-turbo model
az cognitiveservices account deployment create \
  --name resumebuilder-openai \
  --resource-group resume-builder \
  --deployment-name gpt-35-turbo \
  --model-name gpt-3.5-turbo \
  --model-version 0613

# Get your credentials
az cognitiveservices account keys list \
  --name resumebuilder-openai \
  --resource-group resume-builder

az cognitiveservices account show \
  --name resumebuilder-openai \
  --resource-group resume-builder
```

Save the output - you need:
- **Key1** (API Key)
- **Endpoint** (URL with trailing slash)

---

## PHASE 3: DATABASE SETUP (5 minutes)

### Step 11: Create PostgreSQL Database on Railway

1. Log into Railway dashboard
2. Click "New Project"
3. Select "Provision PostgreSQL"
4. Wait for creation (1-2 minutes)
5. Click your PostgreSQL service
6. Click "Connect" tab
7. Copy the **Database URL** (starts with `postgresql://`)

✅ Save this connection string - you'll need it for Render

---

## PHASE 4: FRONTEND DEPLOYMENT (10 minutes)

### Step 12: Push Code to GitHub

```bash
# From your ResumeBuilder root directory
git add .
git commit -m "deploy: prepare for cloud deployment"
git push origin main
```

✅ Verify: Code appears on GitHub

---

### Step 13: Deploy Frontend to Vercel

1. Go to Vercel dashboard
2. Click "New Project" (or "Add New" → "Project")
3. Select "Import Git Repository"
4. Find and select `ResumeBuilder` from GitHub list
5. Click "Import"
6. **Configure Project:**
   - Framework: **Angular**
   - Root Directory: **frontend**
   - Build Command: `npm install --legacy-peer-deps && npm run build`
   - Output Directory: **dist/browser**
   - Environment Variables:
     - Key: `ANGULAR_ENV` → Value: `production`
7. Click "Deploy"
8. Wait 3-5 minutes... ✅

**Result**: You'll get a domain like `resumebuilder-abc123.vercel.app`

✅ **Copy this domain** - you need it for backend CORS configuration

---

### Step 14: Verify Frontend Deployment

1. Click your Vercel domain link
2. You should see the Resume Builder homepage ✅
3. If not, check Logs for errors

---

## PHASE 5: BACKEND DEPLOYMENT (15 minutes)

### Step 15: Create PostgreSQL Container

Before deploying backend, we need a database connection string.

Railway setup (already done in Step 11):
- You should have: `postgresql://user:password@host:5432/railway`

✅ Copy your Railway PostgreSQL connection string

---

### Step 16: Deploy Backend to Render

1. Go to Render dashboard
2. Click "New" → "Web Service"
3. Click "Build and deploy from a Git repository"
4. Find and select `ResumeBuilder` from GitHub
5. **Configure Service:**
   - Name: `resumebuilder-api`
   - Environment: **Docker**
   - Plan: **Free** ⭐ Important!
   - Region: **Virginia** (or closest to you)
   - Branch: **main**
6. Click "Create Web Service"
7. Wait for initial build (should show "Dockerfile detected")
8. **DO NOT wait for deployment to complete** - we need to add environment variables first

---

### Step 17: Add Environment Variables to Render

While backend is building:

1. In Render, click your `resumebuilder-api` service
2. Click "Environment" in left sidebar
3. Add each environment variable (click "Add Environment Variable"):

| Key | Value |
|-----|-------|
| `CORS_ORIGINS` | `https://your-vercel-domain.vercel.app` |
| `DATABASE_URL` | Your Railway PostgreSQL URL |
| `AZURE_OPENAI_ENDPOINT` | Your Azure endpoint (with trailing /) |
| `AZURE_OPENAI_KEY` | Your Azure API Key |
| `AZURE_OPENAI_DEPLOYMENT` | `gpt-35-turbo` |
| `CLOUDINARY_CLOUD_NAME` | Your Cloudinary cloud name |
| `CLOUDINARY_API_KEY` | Your Cloudinary API key |
| `CLOUDINARY_API_SECRET` | Your Cloudinary API secret |
| `ASPNETCORE_ENVIRONMENT` | `Production` |

4. Click "Save Changes"
5. Render automatically redeploys with new variables ✅

**Wait for deployment to complete** (5-10 minutes)

---

### Step 18: Verify Backend Deployment

1. Go to your Render service dashboard
2. Look for deployment status: "Live" ✅
3. Click the service URL (e.g., `resumebuilder-api.onrender.com`)
4. You should see: `{"status":"healthy","timestamp":"..."}`
5. Try Swagger: `https://your-render-domain/swagger`

✅ If you see Swagger UI, backend is working!

---

## PHASE 6: END-TO-END TESTING (10 minutes)

### Step 19: Test Frontend → Backend Connection

1. Go to your Vercel domain (frontend)
2. Open DevTools (F12) → Console tab
3. Navigate to "Resume Builder" section
4. Fill in some info and submit
5. Check Console for errors - should see API call to your Render domain
6. Should get response back ✅

---

### Step 20: Test File Upload

1. Go to "Resume Refactor" section
2. Try uploading a PDF or DOCX file
3. Should upload to Cloudinary
4. Check your Cloudinary dashboard → Media Library
5. Should see your file ✅

---

### Step 21: Test AI Generation

1. Fill out the Resume Builder form
2. Click "Generate" or submit
3. Should call Azure OpenAI
4. Should return AI-generated content
5. Check browser Network tab for Azure API call ✅

---

## PHASE 7: TROUBLESHOOTING

### Issue: Frontend gives CORS error

**Solution:**
1. Check `CORS_ORIGINS` in Render environment
2. Must match your Vercel domain exactly
3. Include `https://` protocol
4. No trailing slash on the domain
5. Redeploy Render service

---

### Issue: Backend takes 30 seconds to load first time

**This is normal!** Free tier on Render spins down after inactivity.

**Solution:** 
- Upgrade to Render Standard ($7/month) to keep always running
- Or just accept 30 second cold start

---

### Issue: Database connection fails

**Check:**
```bash
# Test connection locally first
psql postgresql://user:password@host:5432/railway
```

If connection works locally but not on Render:
1. Verify `DATABASE_URL` in Render environment
2. Check Railway database is running
3. Redeploy Render service

---

### Issue: Azure OpenAI returns error

**Check:**
1. Endpoint format: `https://resource.openai.azure.com/` (with trailing slash)
2. Key is correct and not expired
3. Deployment name matches (`gpt-35-turbo`)
4. Check Azure portal → OpenAI resource → Deployments

---

### Issue: Cloudinary upload failing

**Check:**
1. All three Cloudinary credentials are set correctly
2. You have quota available (free tier: 25 GB/month)
3. File type is allowed (PDF, DOCX, etc.)

---

## FINAL CHECKLIST

- [ ] ✅ Frontend deployed on Vercel (accessible, no errors)
- [ ] ✅ Backend deployed on Render (Swagger loads)
- [ ] ✅ Database connected (no connection errors)
- [ ] ✅ Environment variables set (all 9 values)
- [ ] ✅ CORS origin configured correctly
- [ ] ✅ Frontend can communicate with backend
- [ ] ✅ File upload working
- [ ] ✅ AI generation working
- [ ] ✅ App is live! 🎉

---

## NEXT STEPS

### Monitor Your App
- Check Render logs daily for errors: Dashboard → Logs
- Monitor Azure OpenAI token usage
- Watch Cloudinary bandwidth usage (25 GB/month limit)

### When to Upgrade
- **Backend cold starts too long?** Upgrade Render to Standard ($7/mo)
- **Hit database limits?** Upgrade Railway tier ($10+/mo)
- **Storage running out?** Upgrade Cloudinary ($99/mo unlimited)

### Continuous Deployment
From now on:
```bash
git push origin main
# Automatic redeploy on both Vercel + Render!
```

---

## Support Resources

- **Vercel Issues**: Dashboard → Deployments → Logs
- **Render Issues**: Dashboard → Your Service → Logs
- **Railway Issues**: Dashboard → Your Project → Logs
- **Azure Issues**: https://portal.azure.com
- **Cloudinary Issues**: Dashboard → Media Library

Good luck! You're deployed! 🚀

