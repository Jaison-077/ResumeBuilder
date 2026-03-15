using Microsoft.Extensions.Logging;
using ResumeBuilder.Application.Interfaces;
using ResumeBuilder.Domain.Models;
using System.Text.RegularExpressions;

namespace ResumeBuilder.Application.Services;

/// <summary>
/// Service for ATS (Applicant Tracking System) optimization.
/// Analyzes resume against job description and provides match score and suggestions.
/// </summary>
public class ATSService : IATSService
{
    private readonly IAIService _aiService;
    private readonly ILogger<ATSService> _logger;

    public ATSService(IAIService aiService, ILogger<ATSService> logger)
    {
        _aiService = aiService ?? throw new ArgumentNullException(nameof(aiService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Analyzes a resume against a job description.
    /// Returns match score, matched/missing keywords, and suggestions.
    /// </summary>
    public async Task<AtsAnalysisResult> AnalyzeAsync(ResumeModel resume, string jobDescription, CancellationToken cancellationToken = default)
    {
        if (resume == null) throw new ArgumentNullException(nameof(resume));
        if (string.IsNullOrWhiteSpace(jobDescription))
            throw new ArgumentException("Job description cannot be empty", nameof(jobDescription));

        _logger.LogInformation("Starting ATS analysis");

        try
        {
            // First, analyze the job description to extract keywords
            var jobAnalysis = await AnalyzeJobDescriptionAsync(jobDescription, cancellationToken);

            // Now analyze the resume against those keywords
            var resumeContent = SerializeResumeContent(resume);

            var systemPrompt = BuildATSAnalysisSystemPrompt();
            var userPrompt = BuildATSAnalysisUserPrompt(resumeContent, jobDescription, jobAnalysis);

            var aiResponse = await _aiService.CallChatModelAsync(systemPrompt, userPrompt, cancellationToken);

            // Parse AI response into AtsAnalysisResult
            var result = ParseATSAnalysisResponse(aiResponse, resume);

            _logger.LogInformation("ATS analysis completed. Match score: {MatchScore}", result.MatchScore);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during ATS analysis");
            throw;
        }
    }

    /// <summary>
    /// Analyzes job description and extracts key skills, tools, and qualifications.
    /// </summary>
    public async Task<JobDescriptionAnalysis> AnalyzeJobDescriptionAsync(string jobDescription, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(jobDescription))
            throw new ArgumentException("Job description cannot be empty", nameof(jobDescription));

        _logger.LogInformation("Analyzing job description");

        var systemPrompt = @"You are an expert at analyzing job descriptions and extracting key requirements.
Extract and categorize requirements from the job description into:
- requiredSkills: Must-have technical and soft skills
- preferredSkills: Nice-to-have skills
- tools: Specific technologies, frameworks, and tools mentioned
- qualifications: Education and experience requirements

Return as JSON:
{
  ""requiredSkills"": [""skill1"", ""skill2""],
  ""preferredSkills"": [""skill3""],
  ""tools"": [""tool1"", ""tool2""],
  ""qualifications"": [""qualification1""]
}";

        var userPrompt = $"Please analyze this job description:\n\n{jobDescription}";

        try
        {
            var aiResponse = await _aiService.CallChatModelAsync(systemPrompt, userPrompt, cancellationToken);
            var analysis = ParseJobDescriptionAnalysisResponse(aiResponse);
            return analysis;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing job description");
            return new JobDescriptionAnalysis(); // Return empty analysis on error
        }
    }

    // ============ PRIVATE HELPER METHODS ============

    private string BuildATSAnalysisSystemPrompt()
    {
        return @"You are an expert ATS (Applicant Tracking System) specialist and resume optimizer.
Your task is to:
1. Analyze the resume for presence of job description keywords
2. Calculate a match score (0-100) based on keyword presence, skills overlap, and experience relevance
3. Identify matched and missing keywords
4. Provide specific, actionable suggestions to improve the resume for this job

Return responses as JSON:
{
  ""matchScore"": 75,
  ""matchedKeywords"": [""keyword1"", ""keyword2""],
  ""missingKeywords"": [""keyword3"", ""keyword4""],
  ""suggestions"": [""Suggestion 1: Add X to strengthen Y"", ""Suggestion 2: ...""]
}";
    }

    private string BuildATSAnalysisUserPrompt(string resumeContent, string jobDescription, JobDescriptionAnalysis jobAnalysis)
    {
        return $@"Resume Content:
{resumeContent}

Job Description:
{jobDescription}

Job Analysis (extracted requirements):
Required Skills: {string.Join(", ", jobAnalysis.RequiredSkills)}
Preferred Skills: {string.Join(", ", jobAnalysis.PreferredSkills)}
Tools: {string.Join(", ", jobAnalysis.Tools)}

Please analyze how well this resume matches the job requirements and provide recommendations.";
    }

    private string SerializeResumeContent(ResumeModel resume)
    {
        var sb = new System.Text.StringBuilder();

        sb.AppendLine($"Name: {resume.PersonalInfo?.FirstName} {resume.PersonalInfo?.LastName}");
        sb.AppendLine($"Title: {resume.PersonalInfo?.Title}");

        if (!string.IsNullOrEmpty(resume.Summary))
            sb.AppendLine($"Summary: {resume.Summary}");

        if (resume.Experiences?.Count > 0)
        {
            sb.AppendLine("\nExperience:");
            foreach (var exp in resume.Experiences)
            {
                sb.AppendLine($"- {exp.Company} | {exp.Title}");
                foreach (var bullet in exp.Bullets)
                    sb.AppendLine($"  • {bullet}");
            }
        }

        if (resume.Skills?.Count > 0)
        {
            sb.AppendLine("\nSkills:");
            foreach (var skillGroup in resume.Skills)
            {
                sb.AppendLine($"- {skillGroup.Category}: {string.Join(", ", skillGroup.Items)}");
            }
        }

        return sb.ToString();
    }

    private AtsAnalysisResult ParseATSAnalysisResponse(string aiResponse, ResumeModel resume)
    {
        var result = new AtsAnalysisResult();

        try
        {
            // TODO: Parse JSON response from AI
            // Extract matchScore, matchedKeywords, missingKeywords, suggestions
            // For now, return a reasonable mock response

            result.MatchScore = ExtractNumberFromResponse(aiResponse, 65);
            result.MatchedKeywords = new List<string> { "C#", ".NET Core", "Azure" };
            result.MissingKeywords = new List<string> { "Kubernetes", "Docker" };
            result.Suggestions = new List<string>
            {
                "Add Docker and Kubernetes experience to your skills section",
                "Highlight cloud deployment experience",
                "Emphasize DevOps practices in your experience bullets"
            };
            result.OptimizedResume = resume; // In real implementation, return improved version
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to parse ATS analysis response, using defaults");
            result.MatchScore = 50;
        }

        return result;
    }

    private JobDescriptionAnalysis ParseJobDescriptionAnalysisResponse(string aiResponse)
    {
        var analysis = new JobDescriptionAnalysis();

        try
        {
            // TODO: Parse JSON response from AI
            // For now, perform basic keyword extraction
            analysis.RequiredSkills = ExtractKeywordsFromText(aiResponse, new[] { "require", "must" });
            analysis.PreferredSkills = ExtractKeywordsFromText(aiResponse, new[] { "prefer", "nice" });
            analysis.Tools = ExtractKeywordsFromText(aiResponse, new[] { "tool", "technology", "framework" });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to parse job description analysis");
        }

        return analysis;
    }

    private List<string> ExtractKeywordsFromText(string text, string[] keywords)
    {
        var results = new List<string>();
        var textLower = text.ToLower();

        foreach (var keyword in keywords)
        {
            var pattern = $@"{keyword}\s*[:\-]?\s*([^,\.;]+)";
            var matches = Regex.Matches(textLower, pattern);

            foreach (Match match in matches)
            {
                if (match.Groups.Count > 1)
                {
                    var extracted = match.Groups[1].Value.Trim();
                    if (!string.IsNullOrEmpty(extracted))
                        results.Add(extracted);
                }
            }
        }

        return results;
    }

    private int ExtractNumberFromResponse(string response, int defaultValue)
    {
        var match = Regex.Match(response, @"\b(\d{1,3})\b");
        return match.Success && int.TryParse(match.Groups[1].Value, out var score) ? score : defaultValue;
    }
}
