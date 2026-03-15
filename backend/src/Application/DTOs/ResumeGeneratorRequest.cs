namespace ResumeBuilder.Application.DTOs;

/// <summary>
/// DTO for creating a resume from scratch.
/// Sent from frontend as structured form input.
/// </summary>
public class ResumeGeneratorRequest
{
    public PersonalInfoDto PersonalInfo { get; set; } = new();
    public string? DesiredJobTitle { get; set; }
    public List<ExperienceInputDto> Experiences { get; set; } = new();
    public List<EducationInputDto> Educations { get; set; } = new();
    public List<string> Skills { get; set; } = new();
}

public class PersonalInfoDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? LinkedInUrl { get; set; }
    public string? PortfolioUrl { get; set; }
    public string? GitHubUrl { get; set; }
}

public class ExperienceInputDto
{
    public string Company { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsCurrentRole { get; set; }
    public string? Description { get; set; } // Raw text that AI will convert to bullets
}

public class EducationInputDto
{
    public string Institution { get; set; } = string.Empty;
    public string Degree { get; set; } = string.Empty;
    public string Major { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime GraduationDate { get; set; }
    public string? GPA { get; set; }
}

/// <summary>
/// DTO for resume refactoring request.
/// </summary>
public class RefactorResumeRequest
{
    public string? RawText { get; set; }
    public string? FileUrl { get; set; } // URL to file stored in Blob Storage
}

/// <summary>
/// DTO for ATS optimization request.
/// </summary>
public class OptimizeForAtsRequest
{
    public string Resume { get; set; } = string.Empty; // Serialized ResumeModel JSON
    public string JobDescription { get; set; } = string.Empty;
}

/// <summary>
/// DTO for export request.
/// </summary>
public class ExportResumeRequest
{
    public string Resume { get; set; } = string.Empty; // Serialized ResumeModel JSON
    public string TemplateId { get; set; } = "minimal"; // minimal, modern, classic
}
