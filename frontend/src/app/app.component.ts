import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';

/**
 * Root component for the Resume Builder application.
 */
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'AI Resume Generator & ATS Optimizer';

  navigationItems = [
    { label: 'Create Resume', path: '/builder', icon: '✏️' },
    { label: 'ATS Optimizer', path: '/ats-optimizer', icon: '📊' },
    { label: 'Preview & Export', path: '/preview', icon: '👁️' },
  ];
}
