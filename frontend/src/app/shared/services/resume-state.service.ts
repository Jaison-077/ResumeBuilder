import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { ResumeModel, PersonalInfo } from '../models/resume.model';

/**
 * Service to manage global resume state.
 * Keeps the resume data synchronized across components using RxJS.
 */
@Injectable({
  providedIn: 'root',
})
export class ResumeStateService {
  private readonly resumeSubject = new BehaviorSubject<ResumeModel>(this.getInitialResume());
  public resume$: Observable<ResumeModel> = this.resumeSubject.asObservable();

  private readonly selectedTemplateSubject = new BehaviorSubject<string>('minimal');
  public selectedTemplate$: Observable<string> = this.selectedTemplateSubject.asObservable();

  constructor() {
    // Load resume from localStorage if available
    const saved = localStorage.getItem('resume');
    if (saved) {
      try {
        this.resumeSubject.next(JSON.parse(saved));
      } catch {
        console.warn('Failed to parse saved resume');
      }
    }
  }

  /**
   * Gets the current resume value.
   */
  getResume(): ResumeModel {
    return this.resumeSubject.value;
  }

  /**
   * Sets the resume and saves to localStorage.
   */
  setResume(resume: ResumeModel): void {
    this.resumeSubject.next(resume);
    localStorage.setItem('resume', JSON.stringify(resume));
  }

  /**
   * Updates personal info section.
   */
  updatePersonalInfo(personalInfo: PersonalInfo): void {
    const resume = this.getResume();
    resume.personalInfo = personalInfo;
    this.setResume(resume);
  }

  /**
   * Updates summary section.
   */
  updateSummary(summary: string): void {
    const resume = this.getResume();
    resume.summary = summary;
    this.setResume(resume);
  }

  /**
   * Sets the template ID.
   */
  setTemplate(templateId: string): void {
    this.selectedTemplateSubject.next(templateId);
  }

  /**
   * Gets the current template ID.
   */
  getTemplate(): string {
    return this.selectedTemplateSubject.value;
  }

  /**
   * Clears all resume data.
   */
  clearResume(): void {
    const empty = this.getInitialResume();
    this.setResume(empty);
  }

  private getInitialResume(): ResumeModel {
    return {
      personalInfo: {
        firstName: '',
        lastName: '',
        title: '',
        location: '',
        email: '',
        phone: '',
      },
      summary: '',
      experiences: [],
      educations: [],
      skills: [],
      projects: [],
      certifications: [],
    };
  }
}
