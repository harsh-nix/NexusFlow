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
import { TaskService } from '../../../core/services/task.service';
import { UserService } from '../../../core/services/user.service';
import { TaskPriority } from '../../../core/models/task.models';
import { AppUser } from '../../../core/models/user.models';

@Component({
  selector: 'app-task-create',
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
  templateUrl: './task-create.html',
  styleUrl: './task-create.css',
})
export class TaskCreateComponent implements OnInit {
  form: FormGroup;
  errorMessage = signal<string | null>(null);
  isLoading = signal(false);
  projectId!: number;

  users = signal<AppUser[]>([]);

  priorityOptions = [
    { value: TaskPriority.Low, label: 'Low' },
    { value: TaskPriority.Medium, label: 'Medium' },
    { value: TaskPriority.High, label: 'High' },
    { value: TaskPriority.Critical, label: 'Critical' },
  ];

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private taskService: TaskService,
    private userService: UserService,
    private router: Router
  ) {
    this.projectId = Number(this.route.snapshot.paramMap.get('projectId'));

    this.form = this.fb.group({
      title: ['', [Validators.required, Validators.maxLength(200)]],
      description: [''],
      priority: [TaskPriority.Medium, Validators.required],
      dueDate: [null],
      assigneeIds: [[]], // holds an array of selected user IDs
      assignmentNote: [''], // holds an array of selected user IDs
    });
  }

  ngOnInit(): void {
    // Populates the assignee dropdown. If this fails, the form still works —
    // it just means the dropdown will be empty (assignment is optional).
    this.userService.getAll().subscribe({
      next: (res) => {
        if (res.success) this.users.set(res.data);
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
    this.taskService
      .create({
        title: raw.title,
        description: raw.description || undefined,
        priority: raw.priority,
        dueDate: raw.dueDate ? new Date(raw.dueDate).toISOString() : undefined,
        projectId: this.projectId,
        assigneeIds: raw.assigneeIds || [],
        assignmentNote: raw.assignmentNote || undefined,
      })
      .subscribe({
        next: (res) => {
          this.isLoading.set(false);
          if (res.success) {
            this.router.navigate(['/projects', this.projectId, 'tasks']);
          } else {
            this.errorMessage.set(res.message || 'Could not create task.');
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