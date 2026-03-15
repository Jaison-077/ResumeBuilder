using ResumeBuilder.Domain.Models;

namespace ResumeBuilder.Application.Interfaces;

/// <summary>
/// Service for generating resume documents in various formats.
/// </summary>
public interface ITemplateService
{
    /// <summary>
    /// Generates a PDF from a ResumeModel using specified template.
    /// </summary>
    /// <param name="resume">Resume data model</param>
    /// <param name="templateId">Template name: "minimal", "modern", "classic"</param>
    Task<byte[]> GeneratePdfAsync(ResumeModel resume, string templateId = "minimal", CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a DOCX from a ResumeModel using specified template.
    /// </summary>
    /// <param name="resume">Resume data model</param>
    /// <param name="templateId">Template name: "minimal", "modern", "classic"</param>
    Task<byte[]> GenerateDocxAsync(ResumeModel resume, string templateId = "minimal", CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates HTML preview of the resume for UI display.
    /// </summary>
    string GenerateHtmlPreview(ResumeModel resume, string templateId = "minimal");
}
