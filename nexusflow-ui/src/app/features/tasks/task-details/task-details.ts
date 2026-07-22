import { Component, OnInit, Signal, computed, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { TaskService } from '../../../core/services/task.service';
import { CommentService } from '../../../core/services/comment.service';
import { AuthService, StoredUser } from '../../../core/services/auth.service';

import {
  ProjectTask,
  TaskActivity,
  TASK_STATUS_OPTIONS,
  statusStringToEnum,
  taskPriorityChipClass,
  taskStatusChipClass,
} from '../../../core/models/task.models';
import { TaskComment } from '../../../core/models/comment.models';

@Component({
  selector: 'app-task-details',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    MatCardModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatIconModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './task-details.html',
  styleUrl: './task-details.css',
})
export class TaskDetailsComponent implements OnInit {
  taskId!: number;
  projectId!: number;

  task = signal<ProjectTask | null>(null);
  comments = signal<TaskComment[]>([]);
  activity = signal<TaskActivity[]>([]);

  isLoadingTask = signal(true);
  isLoadingComments = signal(true);
  isLoadingActivity = signal(true);

  isAccepting = signal(false);
  isUpdatingStatus = signal(false);
  isPostingComment = signal(false);
  isRequestingClarification = signal(false);
  isRespondingClarification = signal(false);

  errorMessage = signal<string | null>(null);
  actionMessage = signal<string | null>(null);

  statusOptions = TASK_STATUS_OPTIONS;
  taskStatusChipClass = taskStatusChipClass;
  taskPriorityChipClass = taskPriorityChipClass;

  statusForm: FormGroup;
  commentForm: FormGroup;
  clarificationForm: FormGroup;
  responseForm: FormGroup;

  currentUser!: Signal<StoredUser | null>;

  canAccept = computed(() => this.task()?.status === 'Todo');
  isCreator = computed(() => this.task()?.createdBy === this.currentUser()?.userId);
  isManager = computed(() => {
    const role = this.currentUser()?.role;
    return role === 'Admin' || role === 'ProjectManager';
  });
  canRespondToClarification = computed(() => this.isCreator() || this.isManager());

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private taskService: TaskService,
    private commentService: CommentService,
    private authService: AuthService
  ) {
    this.currentUser = this.authService.currentUser;
    this.taskId = Number(this.route.snapshot.paramMap.get('taskId'));
    this.projectId = Number(this.route.snapshot.paramMap.get('projectId'));

    this.statusForm = this.fb.group({
      status: [null, Validators.required],
      note: [''],
    });

    this.commentForm = this.fb.group({
      content: ['', Validators.required],
    });

    this.clarificationForm = this.fb.group({
      message: ['', Validators.required],
    });

    this.responseForm = this.fb.group({
      message: ['', Validators.required],
    });
  }

  ngOnInit(): void {
    this.loadTask();
    this.loadComments();
    this.loadActivity();
  }

  loadTask(): void {
    this.isLoadingTask.set(true);
    this.taskService.getById(this.taskId).subscribe({
      next: (res) => {
        this.isLoadingTask.set(false);
        if (res.success) {
          this.task.set(res.data);
          this.statusForm.patchValue({ status: statusStringToEnum(res.data.status) });
        } else {
          this.errorMessage.set(res.message);
        }
      },
      error: (err) => {
        this.isLoadingTask.set(false);
        this.errorMessage.set(this.extractError(err));
      },
    });
  }

  loadComments(): void {
    this.isLoadingComments.set(true);
    this.commentService.getByTask(this.taskId).subscribe({
      next: (res) => {
        this.isLoadingComments.set(false);
        if (res.success) {
          this.comments.set(res.data);
        }
      },
      error: () => this.isLoadingComments.set(false),
    });
  }

  loadActivity(): void {
    this.isLoadingActivity.set(true);
    this.taskService.getActivity(this.taskId).subscribe({
      next: (res) => {
        this.isLoadingActivity.set(false);
        if (res.success) {
          this.activity.set(res.data);
        }
      },
      error: () => this.isLoadingActivity.set(false),
    });
  }

  acceptTask(): void {
    this.isAccepting.set(true);
    this.errorMessage.set(null);
    this.actionMessage.set(null);

    this.taskService.accept(this.taskId).subscribe({
      next: (res) => {
        this.isAccepting.set(false);
        if (res.success) {
          this.actionMessage.set(res.message || 'Task accepted.');
          this.task.set(res.data);
          this.loadActivity();
        } else {
          this.errorMessage.set(res.message);
        }
      },
      error: (err) => {
        this.isAccepting.set(false);
        this.errorMessage.set(this.extractError(err));
      },
    });
  }

  submitStatusUpdate(): void {
    if (this.statusForm.invalid) {
      this.statusForm.markAllAsTouched();
      return;
    }

    this.isUpdatingStatus.set(true);
    this.errorMessage.set(null);
    this.actionMessage.set(null);

    const raw = this.statusForm.value;
    this.taskService
      .updateStatus(this.taskId, { status: raw.status, note: raw.note || undefined })
      .subscribe({
        next: (res) => {
          this.isUpdatingStatus.set(false);
          if (res.success) {
            this.actionMessage.set(res.message || 'Status updated.');
            this.task.set(res.data);
            this.statusForm.patchValue({ note: '' });
            this.loadComments();
            this.loadActivity();
          } else {
            this.errorMessage.set(res.message);
          }
        },
        error: (err) => {
          this.isUpdatingStatus.set(false);
          this.errorMessage.set(this.extractError(err));
        },
      });
  }

  submitComment(): void {
    if (this.commentForm.invalid) {
      this.commentForm.markAllAsTouched();
      return;
    }

    this.isPostingComment.set(true);
    this.errorMessage.set(null);

    this.commentService
      .create({ content: this.commentForm.value.content, taskId: this.taskId })
      .subscribe({
        next: (res) => {
          this.isPostingComment.set(false);
          if (res.success) {
            this.commentForm.reset();
            this.loadComments();
          } else {
            this.errorMessage.set(res.message);
          }
        },
        error: (err) => {
          this.isPostingComment.set(false);
          this.errorMessage.set(this.extractError(err));
        },
      });
  }

  submitClarification(): void {
    if (this.clarificationForm.invalid) {
      this.clarificationForm.markAllAsTouched();
      return;
    }

    this.isRequestingClarification.set(true);
    this.errorMessage.set(null);

    this.commentService
      .requestClarification(this.taskId, { message: this.clarificationForm.value.message })
      .subscribe({
        next: (res) => {
          this.isRequestingClarification.set(false);
          if (res.success) {
            this.clarificationForm.reset();
            this.loadComments();
            this.loadActivity();
          } else {
            this.errorMessage.set(res.message);
          }
        },
        error: (err) => {
          this.isRequestingClarification.set(false);
          this.errorMessage.set(this.extractError(err));
        },
      });
  }

  submitClarificationResponse(): void {
    if (this.responseForm.invalid) {
      this.responseForm.markAllAsTouched();
      return;
    }

    this.isRespondingClarification.set(true);
    this.errorMessage.set(null);

    this.commentService
      .respondToClarification(this.taskId, { message: this.responseForm.value.message })
      .subscribe({
        next: (res) => {
          this.isRespondingClarification.set(false);
          if (res.success) {
            this.responseForm.reset();
            this.loadComments();
            this.loadActivity();
          } else {
            this.errorMessage.set(res.message);
          }
        },
        error: (err) => {
          this.isRespondingClarification.set(false);
          this.errorMessage.set(this.extractError(err));
        },
      });
  }

  private extractError(err: any): string {
    const apiErrors = err.error?.errors as string[] | undefined;
    return apiErrors?.join(' ') || err.error?.message || 'Something went wrong.';
  }
}