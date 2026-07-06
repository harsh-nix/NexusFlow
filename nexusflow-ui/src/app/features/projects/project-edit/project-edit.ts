import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { ProjectService } from '../../../core/services/project.service';
import { PROJECT_STATUS_OPTIONS, projectStatusStringToEnum } from '../../../core/models/project.models';

@Component({
  selector: 'app-project-edit',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatCardModule,
    MatDatepickerModule,
    MatNativeDateModule,
  ],
  templateUrl: './project-edit.html',
  styleUrl: './project-edit.css',
})
export class ProjectEditComponent implements OnInit {
  form: FormGroup;
  errorMessage = signal<string | null>(null);
  isLoading = signal(false);
  isLoadingProject = signal(true);
  projectId!: number;
  statusOptions = PROJECT_STATUS_OPTIONS;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private projectService: ProjectService,
    private router: Router
  ) {
    this.projectId = Number(this.route.snapshot.paramMap.get('id'));

    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(200)]],
      description: [''],
      status: [null, Validators.required],
      startDate: [null],
      endDate: [null],
    });
  }

  ngOnInit(): void {
    // Pre-fill the form with the project's current values before letting
    // the user edit anything — otherwise submitting would overwrite fields
    // with blanks.
    this.projectService.getById(this.projectId).subscribe({
      next: (res) => {
        this.isLoadingProject.set(false);
        if (res.success) {
          const p = res.data;
          this.form.patchValue({
            name: p.name,
            description: p.description,
            status: projectStatusStringToEnum(p.status),
            startDate: p.startDate,
            endDate: p.endDate,
          });
        } else {
          this.errorMessage.set(res.message);
        }
      },
      error: () => {
        this.isLoadingProject.set(false);
        this.errorMessage.set('Could not load project.');
      },
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
      .update(this.projectId, {
        name: raw.name,
        description: raw.description || undefined,
        status: raw.status,
        startDate: raw.startDate ? new Date(raw.startDate).toISOString() : undefined,
        endDate: raw.endDate ? new Date(raw.endDate).toISOString() : undefined,
      })
      .subscribe({
        next: (res) => {
          this.isLoading.set(false);
          if (res.success) {
            this.router.navigate(['/projects']);
          } else {
            this.errorMessage.set(res.message || 'Could not update project.');
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