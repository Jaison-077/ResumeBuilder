import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  ResumeModel,
  ResumeGeneratorRequest,
  RefactorResumeRequest,
  OptimizeForAtsRequest,
  ExportResumeRequest,
  AtsAnalysisResult,
  UploadResponse,
} from '../models/resume.model';

/**
 * API Service for all resume-related operations.
 * Communicates with ASP.NET Core backend via HTTP.
 * All DTOs are strongly-typed using TypeScript interfaces.
 */
@Injectable({
  providedIn: 'root',
})
export class ResumeApiService {
  private readonly apiUrl = '/api/resume';

  constructor(private http: HttpClient) {}

  /**
   * Generates a professional resume from structured form input.
   */
  generateResume(request: ResumeGeneratorRequest): Observable<ResumeModel> {
    return this.http.post<ResumeModel>(`${this.apiUrl}/generate`, request);
  }

  /**
   * Refactors an existing resume from raw text or file.
   */
  refactorResume(request: RefactorResumeRequest): Observable<ResumeModel> {
    return this.http.post<ResumeModel>(`${this.apiUrl}/refactor`, request);
  }

  /**
   * Optimizes resume for ATS against a job description.
   */
  optimizeForATS(request: OptimizeForAtsRequest): Observable<AtsAnalysisResult> {
    return this.http.post<AtsAnalysisResult>(`${this.apiUrl}/optimize-ats`, request);
  }

  /**
   * Exports resume to PDF.
   */
  exportToPdf(request: ExportResumeRequest): Observable<Blob> {
    return this.http.post(`${this.apiUrl}/export/pdf`, request, {
      responseType: 'blob',
    });
  }

  /**
   * Exports resume to DOCX.
   */
  exportToDocx(request: ExportResumeRequest): Observable<Blob> {
    return this.http.post(`${this.apiUrl}/export/docx`, request, {
      responseType: 'blob',
    });
  }

  /**
   * Generates HTML preview of resume.
   */
  getPreview(request: ExportResumeRequest): Observable<string> {
    return this.http.post(`${this.apiUrl}/preview`, request, {
      responseType: 'text',
    });
  }

  /**
   * Uploads a resume file to blob storage.
   */
  uploadResume(file: File): Observable<UploadResponse> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<UploadResponse>(`${this.apiUrl}/upload`, formData);
  }

  /**
   * Helper method to serialize a ResumeModel to JSON string.
   */
  serializeResume(resume: ResumeModel): string {
    return JSON.stringify(resume);
  }

  /**
   * Helper method to deserialize JSON string to ResumeModel.
   */
  deserializeResume(json: string): ResumeModel {
    return JSON.parse(json);
  }
}
