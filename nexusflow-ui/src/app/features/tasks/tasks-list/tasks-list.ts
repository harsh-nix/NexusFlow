import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { TaskService } from '../../../core/services/task.service';
import { CommentService } from '../../../core/services/comment.service';
import { AuthService } from '../../../core/services/auth.service';
import {
  priorityStringToEnum,
  ProjectTask,
  TASK_STATUS_OPTIONS,
  TaskStatusEnum,
  statusStringToEnum,
} from '../../../core/models/task.models';
import { TaskComment } from '../../../core/models/comment.models';

@Component({
  selector: 'app-tasks-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    FormsModule,
    MatCardModule,
    MatButtonModule,
    MatChipsModule,
    MatFormFieldModule,
    MatSelectModule,
    MatInputModule,
    MatIconModule,
  ],
  templateUrl: './tasks-list.html',
  styleUrl: './tasks-list.css',
})
export class TasksListComponent implements OnInit {
  tasks = signal<ProjectTask[]>([]);
  isLoading = signal(true);
  errorMessage = signal<string | null>(null);
  projectId!: number;

  statusOptions = TASK_STATUS_OPTIONS;
  statusStringToEnum = statusStringToEnum;

  // Which task's comment panel is currently open (only one at a time —
  // keeps the UI simple). null means none are open.
  expandedTaskId = signal<number | null>(null);

  // Comments already loaded, keyed by taskId, so we don't re-fetch every
  // time you toggle a panel open/closed.
  commentsByTask = signal<Record<number, TaskComment[]>>({});

  // The text currently typed into each task's "new comment" box, keyed by taskId.
  newCommentText: Record<number, string> = {};

  constructor(
    private route: ActivatedRoute,
    private taskService: TaskService,
    private commentService: CommentService,
    public authService: AuthService
  ) {}

  ngOnInit(): void {
    this.projectId = Number(this.route.snapshot.paramMap.get('projectId'));
    this.loadTasks();
  }

  loadTasks(): void {
    this.taskService.getByProject(this.projectId).subscribe({
      next: (res) => {
        this.isLoading.set(false);
        if (res.success) {
          this.tasks.set(res.data);
        } else {
          this.errorMessage.set(res.message);
        }
      },
      error: () => {
        this.isLoading.set(false);
        this.errorMessage.set('Could not load tasks.');
      },
    });
  }
  onDelete(task: ProjectTask): void {
  const confirmed = confirm(`Delete "${task.title}"? This cannot be undone.`);
  if (!confirmed) return;

  this.taskService.delete(task.id).subscribe({
    next: (res) => {
      if (res.success) {
        this.tasks.update((list) => list.filter((t) => t.id !== task.id));
      }
    },
  });
}

  onStatusChange(task: ProjectTask, newStatus: TaskStatusEnum): void {
    this.taskService
      .update(task.id, {
        title: task.title,
        description: task.description || undefined,
        status: newStatus,
        priority: priorityStringToEnum(task.priority),
        dueDate: task.dueDate || undefined,
      })
      .subscribe({
        next: (res) => {
          if (res.success) {
            this.tasks.update((list) => list.map((t) => (t.id === task.id ? res.data : t)));
          }
        },
      });
  }

  toggleComments(taskId: number): void {
    if (this.expandedTaskId() === taskId) {
      this.expandedTaskId.set(null); // clicking the same task again closes it
      return;
    }
    

    this.expandedTaskId.set(taskId);

    // Only fetch from the API the first time this task's panel is opened.
    if (!this.commentsByTask()[taskId]) {
      this.commentService.getByTask(taskId).subscribe({
        next: (res) => {
          if (res.success) {
            this.commentsByTask.update((map) => ({ ...map, [taskId]: res.data }));
          }
        },
      });
    }
  }

  addComment(taskId: number): void {
    const content = (this.newCommentText[taskId] || '').trim();
    if (!content) return;

    this.commentService.create({ content, taskId }).subscribe({
      next: (res) => {
        if (res.success) {
          const existing = this.commentsByTask()[taskId] || [];
          this.commentsByTask.update((map) => ({ ...map, [taskId]: [...existing, res.data] }));
          this.newCommentText[taskId] = '';

          // Keep the card's own comment count in sync without a full reload.
          this.tasks.update((list) =>
            list.map((t) => (t.id === taskId ? { ...t, commentCount: t.commentCount + 1 } : t))
          );
        }
      },
    });
  }
}