namespace ResumeBuilder.Domain.Models;

/// <summary>
/// Core domain model representing a resume structure.
/// This is ATS-friendly: no tables, no multi-column layouts, plain text with clear sections.
/// </summary>
public class ResumeModel
{
    public PersonalInfo PersonalInfo { get; set; } = new();
    public string? Summary { get; set; }
    public List<ExperienceEntry> Experiences { get; set; } = new();
    public List<EducationEntry> Educations { get; set; } = new();
    public List<Skill> Skills { get; set; } = new();
    public List<Project> Projects { get; set; } = new();
    public List<Certification> Certifications { get; set; } = new();
}

public class PersonalInfo
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

public class ExperienceEntry
{
    public string Company { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsCurrentRole { get; set; }
    public List<string> Bullets { get; set; } = new();
}

public class EducationEntry
{
    public string Institution { get; set; } = string.Empty;
    public string Degree { get; set; } = string.Empty;
    public string Major { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime GraduationDate { get; set; }
    public string? GPA { get; set; }
    public List<string> Highlights { get; set; } = new();
}

public class Skill
{
    public string Category { get; set; } = string.Empty; // e.g., "Languages", "Frameworks", "Tools"
    public List<string> Items { get; set; } = new();
}

public class Project
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<string> Highlights { get; set; } = new();
    public string? RepoUrl { get; set; }
    public string? LiveUrl { get; set; }
}

public class Certification
{
    public string Title { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public DateTime IssuedDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
}

/// <summary>
/// Represents ATS optimization analysis results.
/// </summary>
public class AtsAnalysisResult
{
    public int MatchScore { get; set; } // 0-100
    public List<string> MatchedKeywords { get; set; } = new();
    public List<string> MissingKeywords { get; set; } = new();
    public List<string> Suggestions { get; set; } = new();
    public ResumeModel? OptimizedResume { get; set; }
}

/// <summary>
/// Keywords extracted from a job description.
/// </summary>
public class JobDescriptionAnalysis
{
    public List<string> RequiredSkills { get; set; } = new();
    public List<string> PreferredSkills { get; set; } = new();
    public List<string> Tools { get; set; } = new();
    public List<string> Qualifications { get; set; } = new();
    public string? SummaryText { get; set; }
}
