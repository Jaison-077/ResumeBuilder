using Microsoft.Extensions.Logging;
using ResumeBuilder.Application.Interfaces;
using ResumeBuilder.Domain.Models;
using System.Text;

namespace ResumeBuilder.Infrastructure.DocumentGeneration;

/// <summary>
/// Service for generating PDFs and DOCX files from ResumeModel.
/// Also generates HTML preview for the frontend.
/// </summary>
public class TemplateService : ITemplateService
{
    private readonly ILogger<TemplateService> _logger;

    public TemplateService(ILogger<TemplateService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Generates a PDF from ResumeModel using QuestPDF or similar library.
    /// TODO: Install NuGet: QuestPDF
    /// </summary>
    public async Task<byte[]> GeneratePdfAsync(ResumeModel resume, string templateId = "minimal", CancellationToken cancellationToken = default)
    {
        if (resume == null)
            throw new ArgumentNullException(nameof(resume));

        try
        {
            _logger.LogInformation("Generating PDF with template '{TemplateId}'", templateId);

            // TODO: Implement QuestPDF generation
            // Example structure:
            // using (var document = Document.Create(container =>
            // {
            //     container.Page(page =>
            //     {
            //         page.Size(PageSizes.Letter);
            //         page.Margin(0.5f, Unit.Inch);
            //         
            //         page.Content().Column(column =>
            //         {
            //             // Add header with personal info
            //             column.Item().Text(resume.PersonalInfo.FirstName + " " + resume.PersonalInfo.LastName)
            //                 .Bold().FontSize(16);
            //             
            //             // Add sections...
            //         });
            //     });
            // }))
            // {
            //     return document.GeneratePdf();
            // }

            await Task.Delay(100, cancellationToken);
            return Encoding.UTF8.GetBytes("Mock PDF content");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating PDF");
            throw;
        }
    }

    /// <summary>
    /// Generates a DOCX from ResumeModel using OpenXML SDK.
    /// TODO: Install NuGet: DocumentFormat.OpenXml
    /// </summary>
    public async Task<byte[]> GenerateDocxAsync(ResumeModel resume, string templateId = "minimal", CancellationToken cancellationToken = default)
    {
        if (resume == null)
            throw new ArgumentNullException(nameof(resume));

        try
        {
            _logger.LogInformation("Generating DOCX with template '{TemplateId}'", templateId);

            // TODO: Implement OpenXML DOCX generation
            // Example structure:
            // using (var ms = new MemoryStream())
            // {
            //     using (var doc = WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document))
            //     {
            //         var mainPart = doc.AddMainDocumentPart();
            //         mainPart.Document = new Document();
            //         var body = mainPart.Document.AppendChild(new Body());
            //         
            //         // Add header with name
            //         var heading = body.AppendChild(new Paragraph());
            //         var run = heading.AppendChild(new Run());
            //         run.AppendChild(new Text($"{resume.PersonalInfo.FirstName} {resume.PersonalInfo.LastName}"));
            //         
            //         // Add sections...
            //     }
            //     return ms.ToArray();
            // }

            await Task.Delay(100, cancellationToken);
            return Encoding.UTF8.GetBytes("Mock DOCX content");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating DOCX");
            throw;
        }
    }

    /// <summary>
    /// Generates HTML preview of resume for frontend display.
    /// Returns plain HTML with basic styling.
    /// </summary>
    public string GenerateHtmlPreview(ResumeModel resume, string templateId = "minimal")
    {
        if (resume == null)
            throw new ArgumentNullException(nameof(resume));

        var html = new StringBuilder();
        html.AppendLine("<!DOCTYPE html>");
        html.AppendLine("<html>");
        html.AppendLine("<head>");
        html.AppendLine("  <meta charset='UTF-8'>");
        html.AppendLine("  <meta name='viewport' content='width=device-width, initial-scale=1.0'>");
        html.AppendLine("  <title>Resume Preview</title>");
        html.AppendLine("  <style>");

        // Add CSS based on template
        html.AppendLine(GetTemplateStyles(templateId));

        html.AppendLine("  </style>");
        html.AppendLine("</head>");
        html.AppendLine("<body>");

        // Header with personal info
        html.AppendLine("<div class='header'>");
        html.AppendLine($"  <h1>{resume.PersonalInfo?.FirstName} {resume.PersonalInfo?.LastName}</h1>");
        if (!string.IsNullOrEmpty(resume.PersonalInfo?.Title))
            html.AppendLine($"  <h2 class='title'>{resume.PersonalInfo.Title}</h2>");

        // Contact info
        html.AppendLine("  <div class='contact'>");
        if (!string.IsNullOrEmpty(resume.PersonalInfo?.Email))
            html.AppendLine($"    <span>{resume.PersonalInfo.Email}</span> | ");
        if (!string.IsNullOrEmpty(resume.PersonalInfo?.Phone))
            html.AppendLine($"    <span>{resume.PersonalInfo.Phone}</span> | ");
        if (!string.IsNullOrEmpty(resume.PersonalInfo?.Location))
            html.AppendLine($"    <span>{resume.PersonalInfo.Location}</span>");
        html.AppendLine("  </div>");
        html.AppendLine("</div>");

        // Summary
        if (!string.IsNullOrEmpty(resume.Summary))
        {
            html.AppendLine("<section>");
            html.AppendLine("  <h3>Professional Summary</h3>");
            html.AppendLine($"  <p>{resume.Summary}</p>");
            html.AppendLine("</section>");
        }

        // Experience
        if (resume.Experiences?.Count > 0)
        {
            html.AppendLine("<section>");
            html.AppendLine("  <h3>Experience</h3>");
            foreach (var exp in resume.Experiences)
            {
                html.AppendLine("  <div class='experience-item'>");
                html.AppendLine($"    <h4>{exp.Title} <span class='company'>@ {exp.Company}</span></h4>");
                html.AppendLine($"    <p class='date'>{exp.StartDate:MMM yyyy} - {(exp.IsCurrentRole ? "Present" : exp.EndDate?.ToString("MMM yyyy"))} | {exp.Location}</p>");
                html.AppendLine("    <ul>");
                foreach (var bullet in exp.Bullets)
                {
                    html.AppendLine($"      <li>{bullet}</li>");
                }
                html.AppendLine("    </ul>");
                html.AppendLine("  </div>");
            }
            html.AppendLine("</section>");
        }

        // Education
        if (resume.Educations?.Count > 0)
        {
            html.AppendLine("<section>");
            html.AppendLine("  <h3>Education</h3>");
            foreach (var edu in resume.Educations)
            {
                html.AppendLine("  <div class='education-item'>");
                html.AppendLine($"    <h4>{edu.Degree} in {edu.Major}</h4>");
                html.AppendLine($"    <p class='school'>{edu.Institution} - {edu.Location}</p>");
                html.AppendLine($"    <p class='date'>{edu.GraduationDate:MMM yyyy}</p>");
                html.AppendLine("  </div>");
            }
            html.AppendLine("</section>");
        }

        // Skills
        if (resume.Skills?.Count > 0)
        {
            html.AppendLine("<section>");
            html.AppendLine("  <h3>Skills</h3>");
            foreach (var skillGroup in resume.Skills)
            {
                html.AppendLine("  <div class='skill-group'>");
                html.AppendLine($"    <strong>{skillGroup.Category}:</strong> {string.Join(", ", skillGroup.Items)}");
                html.AppendLine("  </div>");
            }
            html.AppendLine("</section>");
        }

        html.AppendLine("</body>");
        html.AppendLine("</html>");

        return html.ToString();
    }

    private string GetTemplateStyles(string templateId)
    {
        return templateId switch
        {
            "modern" => @"
    body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; line-height: 1.6; color: #333; max-width: 8.5in; margin: 0 auto; padding: 20px; }
    .header { border-bottom: 3px solid #007bff; padding-bottom: 10px; margin-bottom: 20px; }
    h1 { margin: 0; font-size: 28px; color: #007bff; }
    h2.title { margin: 5px 0 0 0; color: #666; font-size: 14px; }
    .contact { font-size: 12px; color: #666; margin-top: 5px; }
    h3 { font-size: 16px; color: #007bff; border-bottom: 2px solid #007bff; padding-bottom: 5px; margin-top: 15px; }
    h4 { margin: 10px 0 2px 0; font-size: 13px; }
    .company { font-weight: normal; color: #666; }
    .date { font-size: 11px; color: #888; margin: 2px 0; }
    section { margin-bottom: 15px; }
    ul { margin: 5px 0; padding-left: 20px; }
    li { margin: 3px 0; font-size: 12px; }",

            "classic" => @"
    body { font-family: 'Times New Roman', serif; line-height: 1.5; color: #000; max-width: 8.5in; margin: 0 auto; padding: 20px; }
    .header { text-align: center; margin-bottom: 15px; }
    h1 { margin: 0; font-size: 24px; }
    h2.title { margin: 3px 0 0 0; font-size: 12px; }
    .contact { font-size: 11px; margin-top: 5px; }
    h3 { font-size: 14px; text-transform: uppercase; border-bottom: 1px solid #000; margin-top: 12px; }
    h4 { margin: 8px 0 2px 0; font-size: 12px; }
    .company { font-weight: normal; }
    .date { font-size: 11px; margin: 2px 0; }
    section { margin-bottom: 12px; }
    ul { margin: 4px 0; padding-left: 20px; }
    li { margin: 2px 0; font-size: 11px; }",

            _ => @" /* Minimal Theme */
    body { font-family: Arial, sans-serif; line-height: 1.5; color: #333; max-width: 8.5in; margin: 0 auto; padding: 20px; }
    .header { margin-bottom: 15px; }
    h1 { margin: 0; font-size: 20px; }
    h2.title { margin: 3px 0 0 0; font-size: 12px; color: #555; }
    .contact { font-size: 11px; color: #666; margin-top: 5px; }
    h3 { font-size: 13px; font-weight: bold; margin-top: 12px; margin-bottom: 5px; }
    h4 { margin: 8px 0 2px 0; font-size: 12px; font-weight: bold; }
    .company { font-weight: normal; color: #555; }
    .date { font-size: 11px; color: #777; margin: 2px 0; }
    section { margin-bottom: 10px; }
    ul { margin: 3px 0; padding-left: 20px; }
    li { margin: 2px 0; font-size: 11px; }
    .skill-group { margin: 3px 0; font-size: 11px; }"
        };
    }
}
