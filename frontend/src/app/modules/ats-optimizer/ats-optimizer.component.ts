import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ResumeApiService } from '../../shared/services/resume-api.service';
import { ResumeStateService } from '../../shared/services/resume-state.service';
import { AtsAnalysisResult, OptimizeForAtsRequest, ResumeModel } from '../../shared/models/resume.model';

/**
 * Component for ATS (Applicant Tracking System) optimization.
 * User pastes a job description, and we analyze resume match score, keywords, and suggestions.
 */
@Component({
  selector: 'app-ats-optimizer',
  templateUrl: './ats-optimizer.component.html',
  styleUrls: ['./ats-optimizer.component.css']
})
export class AtsOptimizerComponent implements OnInit {
  form!: FormGroup;
  isLoading = false;
  errorMessage = '';
  successMessage = '';

  currentResume: ResumeModel | null = null;
  analysisResult: AtsAnalysisResult | null = null;
  matchScoreStatus: 'poor' | 'fair' | 'good' | 'excellent' = 'fair';

  constructor(
    private fb: FormBuilder,
    private apiService: ResumeApiService,
    private stateService: ResumeStateService
  ) {}

  ngOnInit(): void {
    this.initializeForm();
    this.currentResume = this.stateService.getResume();
  }

  private initializeForm(): void {
    this.form = this.fb.group({
      jobDescription: ['', [Validators.required, Validators.minLength(50)]],
    });
  }

  analyzeForATS(): void {
    if (this.form.invalid) {
      this.errorMessage = 'Please paste a job description (at least 50 characters)';
      return;
    }

    if (!this.currentResume) {
      this.errorMessage = 'No resume found. Please create or upload a resume first.';
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';
    this.successMessage = '';

    const request: OptimizeForAtsRequest = {
      resume: this.apiService.serializeResume(this.currentResume),
      jobDescription: this.form.get('jobDescription')?.value,
    };

    this.apiService.optimizeForATS(request).subscribe({
      next: (result: AtsAnalysisResult) => {
        this.analysisResult = result;
        this.isLoading = false;
        this.successMessage = 'ATS analysis complete!';
        this.updateMatchScoreStatus();

        // Update resume if optimized version is provided
        if (result.optimizedResume) {
          this.stateService.setResume(result.optimizedResume);
          this.currentResume = result.optimizedResume;
        }
      },
      error: (error: Error) => {
        this.isLoading = false;
        this.errorMessage = `Error analyzing resume: ${error.message}`;
        console.error(error);
      },
    });
  }

  private updateMatchScoreStatus(): void {
    if (!this.analysisResult) return;

    const score = this.analysisResult.matchScore;
    if (score >= 80) this.matchScoreStatus = 'excellent';
    else if (score >= 60) this.matchScoreStatus = 'good';
    else if (score >= 40) this.matchScoreStatus = 'fair';
    else this.matchScoreStatus = 'poor';
  }

  useOptimizedResume(): void {
    if (this.analysisResult?.optimizedResume) {
      this.stateService.setResume(this.analysisResult.optimizedResume);
      this.successMessage = 'Optimized resume saved!';
      setTimeout(() => (this.successMessage = ''), 3000);
    }
  }
}
