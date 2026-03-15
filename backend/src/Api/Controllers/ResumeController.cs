using Microsoft.AspNetCore.Mvc;
using ResumeBuilder.Application.DTOs;
using ResumeBuilder.Application.Interfaces;
using ResumeBuilder.Domain.Models;

namespace ResumeBuilder.Api.Controllers;

/// <summary>
/// Main API controller for resume operations.
/// Orchestrates all resume-related endpoints: generate, refactor, optimize, export.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ResumeController : ControllerBase
{
    private readonly IResumeContentService _resumeContentService;
    private readonly IATSService _atsService;
    private readonly ITemplateService _templateService;
    private readonly IFileExtractionService _fileExtractionService;
    private readonly IStorageService _storageService;
    private readonly ILogger<ResumeController> _logger;

    public ResumeController(
        IResumeContentService resumeContentService,
        IATSService atsService,
        ITemplateService templateService,
        IFileExtractionService fileExtractionService,
        IStorageService storageService,
        ILogger<ResumeController> logger)
    {
        _resumeContentService = resumeContentService ?? throw new ArgumentNullException(nameof(resumeContentService));
        _atsService = atsService ?? throw new ArgumentNullException(nameof(atsService));
        _templateService = templateService ?? throw new ArgumentNullException(nameof(templateService));
        _fileExtractionService = fileExtractionService ?? throw new ArgumentNullException(nameof(fileExtractionService));
        _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Generates a professional resume from structured user input.
    /// Uses Azure OpenAI to improve content and suggestions.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// POST /api/resume/generate
    /// {
    ///   "personalInfo": {
    ///     "firstName": "John",
    ///     "lastName": "Doe",
    ///     "title": "Senior Software Engineer",
    ///     "location": "Seattle, WA",
    ///     "email": "john@example.com",
    ///     "phone": "+1-555-0123"
    ///   },
    ///   "experiences": [
    ///     {
    ///       "company": "Tech Corp",
    ///       "title": "Senior Developer",
    ///       "startDate": "2020-01-15",
    ///       "endDate": null,
    ///       "isCurrentRole": true,
    ///       "description": "Led development of cloud services..."
    ///     }
    ///   ]
    /// }
    /// </remarks>
    [HttpPost("generate")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResumeModel))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResumeModel>> GenerateResume(
        [FromBody] ResumeGeneratorRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            _logger.LogInformation("Resume generation requested for {Name}", 
                $"{request.PersonalInfo.FirstName} {request.PersonalInfo.LastName}");

            var resume = await _resumeContentService.GenerateResumeAsync(request, cancellationToken);

            return Ok(resume);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating resume");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "Error generating resume", error = ex.Message });
        }
    }

    /// <summary>
    /// Refactors an existing resume by uploading a file (PDF/DOCX) or raw text.
    /// Extracts text, cleans it up, and restructures into a professional resume.
    /// </summary>
    [HttpPost("refactor")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResumeModel))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ResumeModel>> RefactorResume(
        [FromBody] RefactorResumeRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RawText) && string.IsNullOrWhiteSpace(request.FileUrl))
            return BadRequest(new { message = "Either RawText or FileUrl must be provided" });

        try
        {
            _logger.LogInformation("Resume refactoring requested");

            string textContent = request.RawText ?? string.Empty;

            // If FileUrl is provided, download and extract text
            if (!string.IsNullOrWhiteSpace(request.FileUrl))
            {
                // Parse file URL to determine format
                var isDocx = request.FileUrl.EndsWith(".docx", StringComparison.OrdinalIgnoreCase);
                var isPdf = request.FileUrl.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase);

                // TODO: Download from blob storage and extract text
                // Stream fileStream = await _storageService.DownloadAsync(blobName, cancellationToken);
                // textContent = isDocx 
                //     ? await _fileExtractionService.ExtractTextFromDocxAsync(fileStream, cancellationToken)
                //     : await _fileExtractionService.ExtractTextFromPdfAsync(fileStream, cancellationToken);
            }

            var resume = await _resumeContentService.RefactorResumeAsync(textContent, cancellationToken);

            return Ok(resume);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refactoring resume");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Error refactoring resume", error = ex.Message });
        }
    }

    /// <summary>
    /// Optimizes a resume for ATS (Applicant Tracking Systems) against a job description.
    /// Returns match score, keyword analysis, and suggestions.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// POST /api/resume/optimize-ats
    /// {
    ///   "resume": "{serialized ResumeModel JSON}",
    ///   "jobDescription": "Sr. Software Engineer... Required Skills: C#, .NET, Azure..."
    /// }
    /// </remarks>
    [HttpPost("optimize-ats")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AtsAnalysisResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AtsAnalysisResult>> OptimizeForATS(
        [FromBody] OptimizeForAtsRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Resume) || string.IsNullOrWhiteSpace(request.JobDescription))
            return BadRequest(new { message = "Resume and JobDescription cannot be empty" });

        try
        {
            _logger.LogInformation("ATS optimization requested");

            // Deserialize resume from JSON
            var resumeModel = System.Text.Json.JsonSerializer.Deserialize<ResumeModel>(request.Resume);
            if (resumeModel == null)
                return BadRequest(new { message = "Invalid resume JSON" });

            var result = await _atsService.AnalyzeAsync(resumeModel, request.JobDescription, cancellationToken);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error optimizing resume for ATS");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Error optimizing resume", error = ex.Message });
        }
    }

    /// <summary>
    /// Exports a resume to PDF format.
    /// </summary>
    [HttpPost("export/pdf")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> ExportToPdf(
        [FromBody] ExportResumeRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Resume))
            return BadRequest(new { message = "Resume cannot be empty" });

        try
        {
            _logger.LogInformation("PDF export requested with template '{TemplateId}'", request.TemplateId);

            var resumeModel = System.Text.Json.JsonSerializer.Deserialize<ResumeModel>(request.Resume);
            if (resumeModel == null)
                return BadRequest(new { message = "Invalid resume JSON" });

            var pdfBytes = await _templateService.GeneratePdfAsync(
                resumeModel,
                request.TemplateId ?? "minimal",
                cancellationToken);

            return File(pdfBytes, "application/pdf", $"resume.pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting to PDF");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Error exporting resume", error = ex.Message });
        }
    }

    /// <summary>
    /// Exports a resume to DOCX format.
    /// </summary>
    [HttpPost("export/docx")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> ExportToDocx(
        [FromBody] ExportResumeRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Resume))
            return BadRequest(new { message = "Resume cannot be empty" });

        try
        {
            _logger.LogInformation("DOCX export requested with template '{TemplateId}'", request.TemplateId);

            var resumeModel = System.Text.Json.JsonSerializer.Deserialize<ResumeModel>(request.Resume);
            if (resumeModel == null)
                return BadRequest(new { message = "Invalid resume JSON" });

            var docxBytes = await _templateService.GenerateDocxAsync(
                resumeModel,
                request.TemplateId ?? "minimal",
                cancellationToken);

            return File(docxBytes, 
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                $"resume.docx");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting to DOCX");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Error exporting resume", error = ex.Message });
        }
    }

    /// <summary>
    /// Generates HTML preview of the resume for frontend display.
    /// Useful for live preview during editing.
    /// </summary>
    [HttpPost("preview")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<string> GetPreview([FromBody] ExportResumeRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Resume))
            return BadRequest(new { message = "Resume cannot be empty" });

        try
        {
            var resumeModel = System.Text.Json.JsonSerializer.Deserialize<ResumeModel>(request.Resume);
            if (resumeModel == null)
                return BadRequest(new { message = "Invalid resume JSON" });

            var html = _templateService.GenerateHtmlPreview(resumeModel, request.TemplateId ?? "minimal");

            return Ok(html);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating preview");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Error generating preview", error = ex.Message });
        }
    }

    /// <summary>
    /// Uploads a resume file (PDF or DOCX) to blob storage.
    /// Returns the blob URL for later reference.
    /// </summary>
    [HttpPost("upload")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UploadResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UploadResponse>> UploadResume(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { message = "File cannot be empty" });

        var validExtensions = new[] { ".pdf", ".docx", ".doc" };
        var fileExtension = Path.GetExtension(file.FileName).ToLower();

        if (!validExtensions.Contains(fileExtension))
            return BadRequest(new { message = "Only PDF and DOCX files are supported" });

        try
        {
            _logger.LogInformation("File upload requested: {FileName}", file.FileName);

            using var stream = file.OpenReadStream();
            var blobUrl = await _storageService.UploadAsync(stream, file.FileName, file.ContentType, cancellationToken);

            return Ok(new UploadResponse { Url = blobUrl, FileName = file.FileName });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Error uploading file", error = ex.Message });
        }
    }
}

/// <summary>
/// Response model for file upload.
/// </summary>
public class UploadResponse
{
    public string Url { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
}
