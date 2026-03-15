using ResumeBuilder.Domain.Models;

namespace ResumeBuilder.Application.Interfaces;

/// <summary>
/// Service for ATS (Applicant Tracking System) optimization.
/// Analyzes resume vs job description and suggests improvements.
/// </summary>
public interface IATSService
{
    /// <summary>
    /// Analyzes resume against a job description.
    /// Returns matched keywords, missing keywords, match score, and suggestions for improvement.
    /// </summary>
    Task<AtsAnalysisResult> AnalyzeAsync(ResumeModel resume, string jobDescription, CancellationToken cancellationToken = default);

    /// <summary>
    /// Extracts and analyzes keywords from a job description.
    /// </summary>
    Task<JobDescriptionAnalysis> AnalyzeJobDescriptionAsync(string jobDescription, CancellationToken cancellationToken = default);
}
