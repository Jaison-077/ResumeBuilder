using ResumeBuilder.Application.DTOs;
using ResumeBuilder.Domain.Models;

namespace ResumeBuilder.Application.Interfaces;

/// <summary>
/// Service for core resume content operations.
/// Responsible for generating, refactoring, and structuring resumes.
/// </summary>
public interface IResumeContentService
{
    /// <summary>
    /// Generates a polished resume from structured user input.
    /// Calls Azure OpenAI to improve summary, experience bullets, and suggest skills.
    /// </summary>
    Task<ResumeModel> GenerateResumeAsync(ResumeGeneratorRequest input, CancellationToken cancellationToken = default);

    /// <summary>
    /// Takes raw text (from uploaded file or user paste) and refactors it into structured ResumeModel.
    /// Cleans up formatting, improves clarity, and organizes into sections.
    /// </summary>
    Task<ResumeModel> RefactorResumeAsync(string rawText, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates that resume is ATS-friendly (no complex formatting, proper structure).
    /// </summary>
    bool ValidateAtsCompliance(ResumeModel resume);
}
