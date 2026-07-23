import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { TaskService } from '../../../core/services/task.service';
import {
  ProjectTask,
  taskPriorityChipClass,
  taskStatusChipClass,
} from '../../../core/models/task.models';

@Component({
  selector: 'app-my-tasks',
  standalone: true,
  imports: [CommonModule, RouterLink, MatCardModule, MatIconModule, MatProgressSpinnerModule],
  templateUrl: './my-tasks.html',
  styleUrl: './my-tasks.css',
})
export class MyTasksComponent implements OnInit {
  tasks = signal<ProjectTask[]>([]);
  isLoading = signal(true);
  errorMessage = signal<string | null>(null);

  statusChipClass = taskStatusChipClass;
  priorityChipClass = taskPriorityChipClass;

  constructor(private taskService: TaskService) {}

  ngOnInit(): void {
    this.taskService.getMyTasks().subscribe({
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
        this.errorMessage.set('Could not load your tasks.');
      },
    });
  }
}