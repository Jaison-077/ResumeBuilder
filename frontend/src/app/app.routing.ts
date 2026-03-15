import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ResumeBuilderComponent } from './modules/resume-builder/resume-builder.component';
import { AtsOptimizerComponent } from './modules/ats-optimizer/ats-optimizer.component';
import { ResumePreviewComponent } from './modules/preview/resume-preview.component';

/**
 * Main routing configuration for the Resume Builder application.
 */
export const routes: Routes = [
  {
    path: '',
    redirectTo: '/builder',
    pathMatch: 'full'
  },
  {
    path: 'builder',
    component: ResumeBuilderComponent,
    data: { title: 'Resume Builder - Create from Scratch' }
  },
  {
    path: 'ats-optimizer',
    component: AtsOptimizerComponent,
    data: { title: 'ATS Optimizer - Optimize for Job' }
  },
  {
    path: 'preview',
    component: ResumePreviewComponent,
    data: { title: 'Preview & Export Resume' }
  },
  {
    path: '**',
    redirectTo: '/builder'
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
