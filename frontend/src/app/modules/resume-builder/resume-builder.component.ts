import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ResumeApiService } from '../../shared/services/resume-api.service';
import { ResumeStateService } from '../../shared/services/resume-state.service';
import { ResumeModel, ResumeGeneratorRequest, PersonalInfoDto, ExperienceInputDto, EducationInputDto } from '../../shared/models/resume.model';

/**
 * Component for creating a resume from scratch.
 * Uses a multi-step form for guided resume building.
 * Steps: Personal Info → Experiences → Education → Skills → Review & Generate
 */
@Component({
  selector: 'app-resume-builder',
  templateUrl: './resume-builder.component.html',
  styleUrls: ['./resume-builder.component.css']
})
export class ResumeBuilderComponent implements OnInit {
  currentStep = 1;
  maxSteps = 5;
  isLoading = false;
  errorMessage = '';
  successMessage = '';

  // Forms for each step
  personalInfoForm!: FormGroup;
  experienceForm!: FormGroup;
  educationForm!: FormGroup;
  skillsForm!: FormGroup;

  // Data collections
  experiences: ExperienceInputDto[] = [];
  educations: EducationInputDto[] = [];
  skills: string[] = [];

  constructor(
    private fb: FormBuilder,
    private apiService: ResumeApiService,
    private stateService: ResumeStateService
  ) {}

  ngOnInit(): void {
    this.initializeForms();
  }

  private initializeForms(): void {
    // Personal Info Form
    this.personalInfoForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.minLength(2)]],
      lastName: ['', [Validators.required, Validators.minLength(2)]],
      title: ['', [Validators.required]],
      location: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      phone: ['', Validators.required],
      linkedInUrl: [''],
      portfolioUrl: [''],
      gitHubUrl: [''],
    });

    // Experience Form
    this.experienceForm = this.fb.group({
      company: ['', Validators.required],
      title: ['', Validators.required],
      location: ['', Validators.required],
      startDate: ['', Validators.required],
      endDate: [''],
      isCurrentRole: [false],
      description: ['', Validators.required],
    });

    // Education Form
    this.educationForm = this.fb.group({
      institution: ['', Validators.required],
      degree: ['', Validators.required],
      major: ['', Validators.required],
      location: ['', Validators.required],
      graduationDate: ['', Validators.required],
      gpa: [''],
    });

    // Skills Form
    this.skillsForm = this.fb.group({
      skillInput: [''],
    });
  }

  // ============ STEP NAVIGATION ============

  nextStep(): void {
    if (this.validateCurrentStep()) {
      this.currentStep++;
    }
  }

  previousStep(): void {
    if (this.currentStep > 1) {
      this.currentStep--;
    }
  }

  private validateCurrentStep(): boolean {
    let formToCheck: FormGroup | null = null;

    switch (this.currentStep) {
      case 1:
        formToCheck = this.personalInfoForm;
        break;
      case 2:
        if (this.experiences.length === 0) {
          this.errorMessage = 'Please add at least one experience entry';
          return false;
        }
        break;
      case 3:
        if (this.educations.length === 0) {
          this.errorMessage = 'Please add at least one education entry';
          return false;
        }
        break;
      case 4:
        if (this.skills.length === 0) {
          this.errorMessage = 'Please add at least one skill';
          return false;
        }
        break;
    }

    if (formToCheck && formToCheck.invalid) {
      this.errorMessage = 'Please fill in all required fields correctly';
      return false;
    }

    this.errorMessage = '';
    return true;
  }

  // ============ EXPERIENCE MANAGEMENT ============

  addExperience(): void {
    if (this.experienceForm.invalid) {
      this.errorMessage = 'Please fill in all experience fields';
      return;
    }

    const exp: ExperienceInputDto = {
      ...this.experienceForm.value,
      startDate: new Date(this.experienceForm.value.startDate),
      endDate: this.experienceForm.value.endDate ? new Date(this.experienceForm.value.endDate) : null,
    };

    this.experiences.push(exp);
    this.experienceForm.reset();
    this.successMessage = 'Experience added successfully';
    setTimeout(() => (this.successMessage = ''), 3000);
  }

  removeExperience(index: number): void {
    this.experiences.splice(index, 1);
  }

  // ============ EDUCATION MANAGEMENT ============

  addEducation(): void {
    if (this.educationForm.invalid) {
      this.errorMessage = 'Please fill in all education fields';
      return;
    }

    const edu: EducationInputDto = {
      ...this.educationForm.value,
      graduationDate: new Date(this.educationForm.value.graduationDate),
    };

    this.educations.push(edu);
    this.educationForm.reset();
    this.successMessage = 'Education added successfully';
    setTimeout(() => (this.successMessage = ''), 3000);
  }

  removeEducation(index: number): void {
    this.educations.splice(index, 1);
  }

  // ============ SKILLS MANAGEMENT ============

  addSkill(): void {
    const skill = this.skillsForm.get('skillInput')?.value?.trim();
    if (skill) {
      this.skills.push(skill);
      this.skillsForm.reset();
    }
  }

  removeSkill(index: number): void {
    this.skills.splice(index, 1);
  }

  // ============ RESUME GENERATION ============

  generateResume(): void {
    if (!this.validateCurrentStep()) {
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    const request: ResumeGeneratorRequest = {
      personalInfo: this.personalInfoForm.value,
      experiences: this.experiences,
      educations: this.educations,
      skills: this.skills,
    };

    this.apiService.generateResume(request).subscribe({
      next: (resume: ResumeModel) => {
        this.stateService.setResume(resume);
        this.isLoading = false;
        this.successMessage = 'Resume generated successfully!';
        // Navigate to preview or next step
        setTimeout(() => (this.currentStep = this.maxSteps), 1000);
      },
      error: (error: Error) => {
        this.isLoading = false;
        this.errorMessage = `Error generating resume: ${error.message}`;
        console.error(error);
      },
    });
  }
}
