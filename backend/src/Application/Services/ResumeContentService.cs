using Microsoft.Extensions.Logging;
using ResumeBuilder.Application.DTOs;
using ResumeBuilder.Application.Interfaces;
using ResumeBuilder.Domain.Models;
using System.Text.Json;

namespace ResumeBuilder.Application.Services;

/// <summary>
/// Service for core resume content operations.
/// Orchestrates AI calls and business logic for resume generation and refactoring.
/// </summary>
public class ResumeContentService : IResumeContentService
{
    private readonly IAIService _aiService;
    private readonly IFileExtractionService _fileExtractionService;
    private readonly ILogger<ResumeContentService> _logger;

    public ResumeContentService(
        IAIService aiService,
        IFileExtractionService fileExtractionService,
        ILogger<ResumeContentService> logger)
    {
        _aiService = aiService ?? throw new ArgumentNullException(nameof(aiService));
        _fileExtractionService = fileExtractionService ?? throw new ArgumentNullException(nameof(fileExtractionService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Generates a polished resume from structured form input using AI.
    /// </summary>
    public async Task<ResumeModel> GenerateResumeAsync(ResumeGeneratorRequest input, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting resume generation for {Name}", 
            $"{input.PersonalInfo.FirstName} {input.PersonalInfo.LastName}");

        if (input == null) throw new ArgumentNullException(nameof(input));

        // Build the prompt for Azure OpenAI
        var systemPrompt = BuildResumeGenerationSystemPrompt();
        var userPrompt = BuildResumeGenerationUserPrompt(input);

        try
        {
            // Call AI to get improved resume content
            var aiResponse = await _aiService.CallChatModelAsync(systemPrompt, userPrompt, cancellationToken);

            // Parse AI response into structured format (TODO: implement JSON parsing)
            var resume = ParseResumeFromAiResponse(input, aiResponse);

            _logger.LogInformation("Resume generation completed successfully");
            return resume;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during resume generation");
            throw;
        }
    }

    /// <summary>
    /// Takes raw text (from uploaded file or paste) and refactors into structured ResumeModel.
    /// </summary>
    public async Task<ResumeModel> RefactorResumeAsync(string rawText, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(rawText))
            throw new ArgumentException("Raw text cannot be empty", nameof(rawText));

        _logger.LogInformation("Starting resume refactoring from raw text");

        var systemPrompt = BuildResumeRefactorSystemPrompt();
        var userPrompt = $"Please refactor and structure the following resume:\n\n{rawText}";

        try
        {
            var aiResponse = await _aiService.CallChatModelAsync(systemPrompt, userPrompt, cancellationToken);
            var resume = ParseResumeFromAiResponse(aiResponse);

            _logger.LogInformation("Resume refactoring completed successfully");
            return resume;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during resume refactoring");
            throw;
        }
    }

    /// <summary>
    /// Validates that the resume adheres to ATS requirements.
    /// </summary>
    public bool ValidateAtsCompliance(ResumeModel resume)
    {
        if (resume == null) return false;

        // Check basic structure requirements
        var hasBasicInfo = !string.IsNullOrWhiteSpace(resume.PersonalInfo?.Email);
        var hasExperience = resume.Experiences?.Count > 0;
        var hasEducation = resume.Educations?.Count > 0;

        // Check for ATS-unfriendly elements
        // In a real implementation, you'd scan for complex formatting, images, etc.
        var noComplexFormatting = true; // Placeholder

        return hasBasicInfo && hasExperience && hasEducation && noComplexFormatting;
    }

    // ============ PRIVATE HELPER METHODS ============

    private string BuildResumeGenerationSystemPrompt()
    {
        return @"You are an expert resume writer specializing in ATS-friendly resumes for technical and engineering roles. 

Your guidelines:
- Use clear, action-oriented bullet points starting with strong verbs (e.g., 'Designed', 'Implemented', 'Led')
- Avoid tables, images, multi-column layouts, and special characters
- Quantify achievements whenever possible (%, time saved, defects reduced, cost savings, users impacted)
- Keep each bullet under 30 words
- Make everything truthful and provable
- Format output as valid JSON for parsing

When returning data, structure it as JSON with this schema:
{
  ""summary"": ""Professional summary paragraph"",
  ""experiences"": [
    {
      ""company"": """",
      ""title"": """",
      ""location"": """",
      ""startDate"": ""YYYY-MM-DD"",
      ""endDate"": ""YYYY-MM-DD"",
      ""isCurrentRole"": false,
      ""bullets"": [""bullet 1"", ""bullet 2""]
    }
  ],
  ""skills"": [
    {
      ""category"": ""Language|Framework|Tool"",
      ""items"": [""skill1"", ""skill2""]
    }
  ]
}";
    }

    private string BuildResumeRefactorSystemPrompt()
    {
        return BuildResumeGenerationSystemPrompt() + 
            "\n\nAdditionally: Clean up and restructure the provided resume into these sections: Summary, Experience, Education, Skills, Projects. Make it more polished while preserving truthfulness.";
    }

    private string BuildResumeGenerationUserPrompt(ResumeGeneratorRequest input)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Please generate a professional resume based on the following information:");
        sb.AppendLine();
        sb.AppendLine($"Name: {input.PersonalInfo.FirstName} {input.PersonalInfo.LastName}");
        sb.AppendLine($"Title: {input.PersonalInfo.Title}");
        sb.AppendLine($"Location: {input.PersonalInfo.Location}");

        if (input.Experiences?.Count > 0)
        {
            sb.AppendLine("\nExperience:");
            foreach (var exp in input.Experiences)
            {
                sb.AppendLine($"- {exp.Company} | {exp.Title} ({exp.StartDate:yyyy-MM} to {(exp.EndDate?.ToString("yyyy-MM") ?? "Present")})");
                sb.AppendLine($"  {exp.Description}");
            }
        }

        if (input.Skills?.Count > 0)
        {
            sb.AppendLine("\nSkills: " + string.Join(", ", input.Skills));
        }

        return sb.ToString();
    }

    private ResumeModel ParseResumeFromAiResponse(ResumeGeneratorRequest input, string aiResponse)
    {
        var resume = new ResumeModel();
        
        // Map personal info
        resume.PersonalInfo = new PersonalInfo
        {
            FirstName = input.PersonalInfo.FirstName,
            LastName = input.PersonalInfo.LastName,
            Title = input.PersonalInfo.Title,
            Location = input.PersonalInfo.Location,
            Email = input.PersonalInfo.Email,
            Phone = input.PersonalInfo.Phone,
            LinkedInUrl = input.PersonalInfo.LinkedInUrl,
            PortfolioUrl = input.PersonalInfo.PortfolioUrl,
            GitHubUrl = input.PersonalInfo.GitHubUrl,
        };

        // TODO: Parse AI response JSON and populate experiences, educations, skills
        // Use System.Text.Json to deserialize the structured response

        return resume;
    }

    private ResumeModel ParseResumeFromAiResponse(string aiResponse)
    {
        // TODO: Implement JSON parsing of AI response
        // The AI should return JSON that we parse into ResumeModel structure
        var resume = new ResumeModel();
        
        try
        {
            // var jsonOptions = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            // var parsed = System.Text.Json.JsonSerializer.Deserialize<dynamic>(aiResponse, jsonOptions);
            // Then map parsed data to ResumeModel
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not parse AI response as JSON, using fallback parsing");
        }

        return resume;
    }
}
