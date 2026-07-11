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
import {
  priorityStringToEnum,
  statusStringToEnum,
  TASK_STATUS_OPTIONS,
  TaskPriority,
} from '../../../core/models/task.models';

@Component({
  selector: 'app-task-edit',
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
  templateUrl: './task-edit.html',
  styleUrl: './task-edit.css',
})
export class TaskEditComponent implements OnInit {
  form: FormGroup;
  errorMessage = signal<string | null>(null);
  isLoading = signal(false);
  isLoadingTask = signal(true);
  taskId!: number;
  projectId!: number;

  statusOptions = TASK_STATUS_OPTIONS;
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
    private router: Router
  ) {
    this.taskId = Number(this.route.snapshot.paramMap.get('taskId'));
    this.projectId = Number(this.route.snapshot.paramMap.get('projectId'));

    this.form = this.fb.group({
      title: ['', [Validators.required, Validators.maxLength(200)]],
      description: [''],
      status: [null, Validators.required],
      priority: [null, Validators.required],
      dueDate: [null],
    });
  }

  ngOnInit(): void {
    this.taskService.getById(this.taskId).subscribe({
      next: (res) => {
        this.isLoadingTask.set(false);
        if (res.success) {
          const t = res.data;
          this.form.patchValue({
            title: t.title,
            description: t.description,
            status: statusStringToEnum(t.status),
            priority: priorityStringToEnum(t.priority),
            dueDate: t.dueDate,
          });
        } else {
          this.errorMessage.set(res.message);
        }
      },
      error: () => {
        this.isLoadingTask.set(false);
        this.errorMessage.set('Could not load task.');
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
      .update(this.taskId, {
        title: raw.title,
        description: raw.description || undefined,
        status: raw.status,
        priority: raw.priority,
        dueDate: raw.dueDate ? new Date(raw.dueDate).toISOString() : undefined,
      })
      .subscribe({
        next: (res) => {
          this.isLoading.set(false);
          if (res.success) {
            this.router.navigate(['/projects', this.projectId, 'tasks']);
          } else {
            this.errorMessage.set(res.message || 'Could not update task.');
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