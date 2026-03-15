/**
 * Shared domain models for Angular frontend.
 * These match the backend DTOs and domain models.
 */

export interface ResumeModel {
  personalInfo: PersonalInfo;
  summary?: string;
  experiences: ExperienceEntry[];
  educations: EducationEntry[];
  skills: Skill[];
  projects: Project[];
  certifications: Certification[];
}

export interface PersonalInfo {
  firstName: string;
  lastName: string;
  title: string;
  location: string;
  email: string;
  phone: string;
  linkedInUrl?: string;
  portfolioUrl?: string;
  gitHubUrl?: string;
}

export interface ExperienceEntry {
  company: string;
  title: string;
  location: string;
  startDate: Date;
  endDate?: Date;
  isCurrentRole: boolean;
  bullets: string[];
}

export interface EducationEntry {
  institution: string;
  degree: string;
  major: string;
  location: string;
  graduationDate: Date;
  gpa?: string;
  highlights: string[];
}

export interface Skill {
  category: string; // e.g., "Languages", "Frameworks", "Tools"
  items: string[];
}

export interface Project {
  title: string;
  description?: string;
  highlights: string[];
  repoUrl?: string;
  liveUrl?: string;
}

export interface Certification {
  title: string;
  issuer: string;
  issuedDate: Date;
  expiryDate?: Date;
}

// ============ REQUEST/RESPONSE DTOs ============

export interface ResumeGeneratorRequest {
  personalInfo: PersonalInfoDto;
  desiredJobTitle?: string;
  experiences: ExperienceInputDto[];
  educations: EducationInputDto[];
  skills: string[];
}

export interface PersonalInfoDto {
  firstName: string;
  lastName: string;
  title: string;
  location: string;
  email: string;
  phone: string;
  linkedInUrl?: string;
  portfolioUrl?: string;
  gitHubUrl?: string;
}

export interface ExperienceInputDto {
  company: string;
  title: string;
  location: string;
  startDate: Date;
  endDate?: Date;
  isCurrentRole: boolean;
  description?: string; // Raw text that AI will convert to bullets
}

export interface EducationInputDto {
  institution: string;
  degree: string;
  major: string;
  location: string;
  graduationDate: Date;
  gpa?: string;
}

export interface RefactorResumeRequest {
  rawText?: string;
  fileUrl?: string;
}

export interface OptimizeForAtsRequest {
  resume: string; // Serialized ResumeModel JSON
  jobDescription: string;
}

export interface ExportResumeRequest {
  resume: string; // Serialized ResumeModel JSON
  templateId: string; // minimal, modern, classic
}

export interface AtsAnalysisResult {
  matchScore: number; // 0-100
  matchedKeywords: string[];
  missingKeywords: string[];
  suggestions: string[];
  optimizedResume?: ResumeModel;
}

export interface JobDescriptionAnalysis {
  requiredSkills: string[];
  preferredSkills: string[];
  tools: string[];
  qualifications: string[];
  summaryText?: string;
}

export interface UploadResponse {
  url: string;
  fileName: string;
}
