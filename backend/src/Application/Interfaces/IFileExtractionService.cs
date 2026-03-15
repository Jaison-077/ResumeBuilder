namespace ResumeBuilder.Application.Interfaces;

/// <summary>
/// Service for extracting text from various file formats.
/// </summary>
public interface IFileExtractionService
{
    /// <summary>
    /// Extracts all text from a PDF file.
    /// </summary>
    Task<string> ExtractTextFromPdfAsync(Stream fileStream, CancellationToken cancellationToken = default);

    /// <summary>
    /// Extracts all text from a DOCX file.
    /// </summary>
    Task<string> ExtractTextFromDocxAsync(Stream fileStream, CancellationToken cancellationToken = default);
}
