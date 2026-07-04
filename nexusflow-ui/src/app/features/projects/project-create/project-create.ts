import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { ProjectService } from '../../../core/services/project.service';

@Component({
  selector: 'app-project-create',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCardModule,
    MatDatepickerModule,
    MatNativeDateModule,
  ],
  templateUrl: './project-create.html',
  styleUrl: './project-create.css',
})
export class ProjectCreateComponent {
  form: FormGroup;
  errorMessage = signal<string | null>(null);
  isLoading = signal(false);

  // NOTE: OrganizationId is hardcoded to 1 for now — there's no Organization
  // Management screen yet, so every project is created under the single
  // demo organization you inserted via SSMS. This will become a dropdown
  // once that module exists.
  private readonly organizationId = 1;

  constructor(
    private fb: FormBuilder,
    private projectService: ProjectService,
    private router: Router
  ) {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(200)]],
      description: [''],
      startDate: [null],
      endDate: [null],
    });
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set(null);

    const raw = this.form.value;
    this.projectService
      .create({
        name: raw.name,
        description: raw.description || undefined,
        // Dates come from the datepicker as JS Date objects — convert to
        // ISO strings, which is the format ASP.NET Core expects for DateTime.
        startDate: raw.startDate ? new Date(raw.startDate).toISOString() : undefined,
        endDate: raw.endDate ? new Date(raw.endDate).toISOString() : undefined,
        organizationId: this.organizationId,
      })
      .subscribe({
        next: (res) => {
          this.isLoading.set(false);
          if (res.success) {
            this.router.navigate(['/projects']);
          } else {
            this.errorMessage.set(res.message || 'Could not create project.');
          }
        },
        error: (err) => {
          this.isLoading.set(false);
          const apiErrors = err.error?.errors as string[] | undefined;
          this.errorMessage.set(apiErrors?.join(' ') || err.error?.message || 'Something went wrong.');
        },
      });
  }
}