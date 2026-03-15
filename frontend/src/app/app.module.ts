import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { AppRoutingModule, routes } from './app.routing';
import { AppComponent } from './app.component';

// Feature Modules
import { ResumeBuilderComponent } from './modules/resume-builder/resume-builder.component';
import { AtsOptimizerComponent } from './modules/ats-optimizer/ats-optimizer.component';
import { ResumePreviewComponent } from './modules/preview/resume-preview.component';

// Shared Services
import { ResumeApiService } from './shared/services/resume-api.service';
import { ResumeStateService } from './shared/services/resume-state.service';

// HTTP Interceptor for error handling
import { ErrorInterceptor } from './shared/interceptors/error.interceptor';

@NgModule({
  declarations: [
    AppComponent,
    ResumeBuilderComponent,
    AtsOptimizerComponent,
    ResumePreviewComponent,
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    HttpClientModule,
    ReactiveFormsModule,
    FormsModule,
  ],
  providers: [
    ResumeApiService,
    ResumeStateService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: ErrorInterceptor,
      multi: true
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
