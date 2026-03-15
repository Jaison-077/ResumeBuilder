# AI Resume Generator - Azure OpenAI Integration Guide

## Overview

This document explains how the application uses Azure OpenAI for content generation and optimization.

## Azure OpenAI Configuration

### Service Setup

1. **Create Azure OpenAI Resource**:
   ```bash
   az cognitiveservices account create \
     --name my-openai-resource \
     --resource-group my-resource-group \
     --kind OpenAI \
     --sku s0 \
     --location eastus
   ```

2. **Deploy a Model**:
   ```bash
   az cognitiveservices account deployment create \
     --name my-openai-resource \
     --resource-group my-resource-group \
     --deployment-name gpt-4-turbo \
     --model-name gpt-4-turbo \
     --model-version 2024-04-09
   ```

3. **Get Credentials**:
   ```bash
   az cognitiveservices account keys list \
     --name my-openai-resource \
     --resource-group my-resource-group
   ```

### Configuration in appsettings.json

```json
{
  "AzureOpenAI": {
    "Endpoint": "https://my-resource.openai.azure.com/",
    "Key": "your-api-key",
    "DeploymentName": "gpt-4-turbo",
    "ApiVersion": "2024-02-15-preview"
  }
}
```

## Prompt Engineering

### 1. Resume Generation Process

**Goal**: Create a polished, professional resume from user input

```csharp
var systemPrompt = @"You are an award-winning resume writer with 20+ years of experience.
Your expertise:
- Converting raw experience into compelling bullet points
- Quantifying achievements and impact
- Optimizing for both ATS systems and human readers
- Following modern resume best practices

Guidelines:
- Each bullet point should start with a strong action verb
- Include specific metrics, percentages, or outcomes when possible
- Keep bullets between 15-30 words
- Write from the perspective of the candidate
- Ensure all content is truthful and verifiable
- Avoid generic phrases and clichés
- Focus on impact and results, not just duties

Return ONLY valid JSON with no markdown formatting or code blocks.";

var userPrompt = $@"{{
  'candidateName': '{input.PersonalInfo.FirstName} {input.PersonalInfo.LastName}',
  'desiredTitle': '{input.DesiredJobTitle}',
  'experiences': [
    {{
      'company': 'Company Name',
      'title': 'Job Title',
      'period': 'Jan 2020 - Present',
      'description': 'Detailed description of what I did...'
    }}
  ],
  'skills': {JsonSerializer.Serialize(input.Skills)}
}}

Please generate an enhanced resume with:
1. A compelling 3-4 sentence professional summary
2. Improved experience bullets (4-6 per role) using action verbs and metrics
3. Organized skills by category (Languages, Frameworks, Tools, Soft Skills)
4. Return as JSON matching this schema:
{{
  'summary': 'string',
  'experience_bullets': [string],
  'skills_by_category': {{
    'category_name': [string]
  }}
}}";
```

### 2. ATS Optimization Process

**Goal**: Compare resume with job description and suggest improvements

```csharp
var systemPrompt = @"You are an expert in ATS (Applicant Tracking System) optimization.
Your role is to help resumes pass ATS filters by:
- Identifying key requirements from job descriptions
- Comparing against resume content
- Suggesting improvements that remain truthful
- Providing actionable recommendations

Return JSON ONLY with this structure:
{
  'match_score': <0-100>,
  'required_skills_found': [string],
  'required_skills_missing': [string],
  'suggested_improvements': [string],
  'priority_changes': [string]
}";

var userPrompt = $@"RESUME CONTENT:
{resumeText}

---

JOB DESCRIPTION:
{jobDescription}

---

Please analyze and provide:
1. Overall ATS match score (0-100)
2. Which required skills from the job description are present in the resume
3. Which required skills are missing
4. Specific, actionable suggestions to improve the match
5. Top 3 priority changes to maximize ATS match

Be specific - provide exact phrases or keywords to add/modify.";
```

### 3. Resume Refactoring Process

**Goal**: Transform raw/messy resume into structured ATS-friendly format

```csharp
var systemPrompt = @"You are a resume formatting and content improvement specialist.
Given a resume in any format (raw text, poorly formatted, outdated style):
1. Extract all relevant information
2. Reorganize into standard sections
3. Improve clarity and professionalism
4. Apply ATS best practices
5. Enhance bullet points while maintaining accuracy

IMPORTANT: Do NOT add false information. Only improve what's there.

Return as JSON with this structure:
{
  'personal_info': {
    'name': 'string',
    'title': 'string',
    'location': 'string',
    'email': 'string',
    'phone': 'string'
  },
  'professional_summary': 'string',
  'experience': [
    {
      'company': 'string',
      'title': 'string',
      'period': 'string',
      'bullets': [string]
    }
  ],
  'education': [
    {
      'school': 'string',
      'degree': 'string',
      'field': 'string',
      'graduation': 'string'
    }
  ],
  'skills': {
    'category': [string]
  }
}";

var userPrompt = $@"Please clean up and restructure this resume:

---
{rawResumeText}
---

Focus on:
1. Clear, logical organization
2. Professional language and strong action verbs
3. Quantifiable achievements where possible
4. ATS-friendly formatting (no complex structures)
5. Proper chronological order";
```

## Cost Optimization

### Request Pricing (as of 2024)

- **GPT-4 Turbo**: ~$0.01-0.03 per 1K tokens (input/output)
- **GPT-4o**: ~$0.003-0.006 per 1K tokens

### Recommendations

1. **Use Models Efficiently**:
   - Use GPT-4o-mini for simple tasks (cost-effective)
   - Use GPT-4 Turbo only for complex optimization

2. **Cache Frequently Used Prompts**:
   ```csharp
   // Same system prompt for many requests = cache hits
   private const string SYSTEM_PROMPT_RESUME_GEN = @"...";
   ```

3. **Limit Token Usage**:
   - Set `max_tokens` parameter
   - Truncate responses if too long
   - Use structured outputs to reduce reparsing

4. **Batch Requests When Possible**:
   ```csharp
   // Process multiple resumes in single request
   var batch = new List<Resume> { ... };
   var prompt = BuildBatchAnalysisPrompt(batch);
   ```

## Error Handling

### Common Errors

```csharp
try
{
    var response = await aiService.CallChatModelAsync(systemPrompt, userPrompt);
}
catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.RateLimited)
{
    // Implement exponential backoff
    await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, retryCount)));
}
catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
{
    // Log and alert - credentials issue
    _logger.LogCritical("Azure OpenAI authentication failed");
}
catch (TaskCanceledException)
{
    // Timeout - fall back to default or cached response
    return GetDefaultResponse();
}
```

## Testing Prompts

### Manual Testing

1. **Resume Generation**:
   - Input: Basic work experience details
   - Expected: Polished, quantified bullet points in JSON

2. **ATS Analysis**:
   - Input: Resume + Tech job description
   - Expected: Match score with missing keywords

3. **Refactoring**:
   - Input: Poorly formatted resume text
   - Expected: Structured JSON output

## Monitoring & Logging

```csharp
public class AzureOpenAiService : IAIService
{
    private readonly ILogger<AzureOpenAiService> _logger;

    public async Task<string> CallChatModelAsync(string systemPrompt, string userPrompt)
    {
        var stopwatch = Stopwatch.StartNew();
        
        _logger.LogInformation("Calling OpenAI: Model={Model}, InputTokens={Tokens}", 
            _deploymentName, UserPrompt.Length / 4);

        try
        {
            var response = await client.GetChatCompletionsAsync(options);
            
            stopwatch.Stop();
            _logger.LogInformation("OpenAI call completed in {Ms}ms, OutputTokens={Tokens}",
                stopwatch.ElapsedMilliseconds, response.Usage.CompletionTokens);

            return response.Choices[0].Message.Content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OpenAI call failed");
            throw;
        }
    }
}
```

## Best Practices

1. ✅ **Always validate outputs** before using them
2. ✅ **Implement timeout handling** (default 30 seconds)
3. ✅ **Log all API interactions** for debugging
4. ✅ **Cache responses** when possible
5. ✅ **Use structured outputs** to reduce token usage
6. ✅ **Monitor costs** and set budget alerts
7. ✅ **Implement rate limiting** on frontend calls
8. ✅ **Provide fallback options** for failures

## References

- [Azure OpenAI Service Documentation](https://learn.microsoft.com/en-us/azure/cognitive-services/openai/)
- [Prompt Engineering Best Practices](https://learn.microsoft.com/en-us/azure/cognitive-services/openai/concepts/prompt-engineering)
- [OpenAI Cookbook](https://github.com/openai/openai-cookbook)
- [ATS Optimization Guide](https://www.ats-friendly-resume.com/)
