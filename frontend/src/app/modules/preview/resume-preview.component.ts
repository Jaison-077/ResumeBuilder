import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ResumeApiService } from '../../shared/services/resume-api.service';
import { ResumeStateService } from '../../shared/services/resume-state.service';
import { ResumeModel, ExportResumeRequest } from '../../shared/models/resume.model';

/**
 * Component for previewing and exporting resume.
 * Shows live preview in selected template, allows PDF/DOCX download.
 */
@Component({
  selector: 'app-resume-preview',
  templateUrl: './resume-preview.component.html',
  styleUrls: ['./resume-preview.component.css']
})
export class ResumePreviewComponent implements OnInit {
  currentResume: ResumeModel | null = null;
  previewHtml: string | null = null;
  selectedTemplate = 'minimal';
  isLoadingPreview = false;
  isExporting = false;
  errorMessage = '';
  successMessage = '';

  templates = [
    { id: 'minimal', label: 'Minimal', description: 'Clean and simple' },
    { id: 'modern', label: 'Modern', description: 'Contemporary design' },
    { id: 'classic', label: 'Classic', description: 'Traditional format' },
  ];

  exportForm!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private apiService: ResumeApiService,
    private stateService: ResumeStateService
  ) {}

  ngOnInit(): void {
    this.initializeForm();
    this.currentResume = this.stateService.getResume();
    this.stateService.resume$.subscribe((resume: ResumeModel | null) => {
      this.currentResume = resume;
      this.refreshPreview();
    });
    this.refreshPreview();
  }

  private initializeForm(): void {
    this.exportForm = this.fb.group({
      format: ['pdf'], // pdf or docx
    });
  }

  refreshPreview(): void {
    if (!this.currentResume) {
      this.errorMessage = 'No resume loaded. Please create or upload a resume first.';
      return;
    }

    this.isLoadingPreview = true;
    this.errorMessage = '';

    const request: ExportResumeRequest = {
      resume: this.apiService.serializeResume(this.currentResume),
      templateId: this.selectedTemplate,
    };

    this.apiService.getPreview(request).subscribe({
      next: (html: string) => {
        this.previewHtml = html;
        this.isLoadingPreview = false;
      },
      error: (error: Error) => {
        this.isLoadingPreview = false;
        this.errorMessage = `Error generating preview: ${error.message}`;
        console.error(error);
      },
    });
  }

  selectTemplate(templateId: string): void {
    this.selectedTemplate = templateId;
    this.stateService.setTemplate(templateId);
    this.refreshPreview();
  }

  exportPdf(): void {
    if (!this.currentResume) {
      this.errorMessage = 'No resume to export';
      return;
    }

    this.isExporting = true;
    this.errorMessage = '';

    const request: ExportResumeRequest = {
      resume: this.apiService.serializeResume(this.currentResume),
      templateId: this.selectedTemplate,
    };

    this.apiService.exportToPdf(request).subscribe({
      next: (blob: Blob) => {
        this.downloadFile(blob, 'resume.pdf');
        this.isExporting = false;
        this.successMessage = 'Resume exported as PDF!';
        setTimeout(() => (this.successMessage = ''), 3000);
      },
      error: (error: Error) => {
        this.isExporting = false;
        this.errorMessage = `Error exporting PDF: ${error.message}`;
      },
    });
  }

  exportDocx(): void {
    if (!this.currentResume) {
      this.errorMessage = 'No resume to export';
      return;
    }

    this.isExporting = true;
    this.errorMessage = '';

    const request: ExportResumeRequest = {
      resume: this.apiService.serializeResume(this.currentResume),
      templateId: this.selectedTemplate,
    };

    this.apiService.exportToDocx(request).subscribe({
      next: (blob: Blob) => {
        this.downloadFile(blob, 'resume.docx');
        this.isExporting = false;
        this.successMessage = 'Resume exported as DOCX!';
        setTimeout(() => (this.successMessage = ''), 3000);
      },
      error: (error: Error) => {
        this.isExporting = false;
        this.errorMessage = `Error exporting DOCX: ${error.message}`;
      },
    });
  }

  private downloadFile(blob: Blob, filename: string): void {
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = filename;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    window.URL.revokeObjectURL(url);
  }
}
