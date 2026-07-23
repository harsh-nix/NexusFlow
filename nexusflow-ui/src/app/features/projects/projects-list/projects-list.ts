import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { ProjectService } from '../../../core/services/project.service';
import { Project, projectStatusChipClass } from '../../../core/models/project.models';
import { SnackbarService } from '../../../core/services/snackbar.service';
import { ConfirmDialogComponent } from '../../../shared/confirm-dialog/confirm-dialog';

@Component({
  selector: 'app-projects-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatDialogModule,
  ],
  templateUrl: './projects-list.html',
  styleUrl: './projects-list.css',
})
export class ProjectsListComponent implements OnInit {
  projects = signal<Project[]>([]);
  isLoading = signal(true);
  errorMessage = signal<string | null>(null);

  // Exposed so the template can call this directly: [ngClass]="statusChipClass(project.status)"
  statusChipClass = projectStatusChipClass;
  progressPercent(project: Project): number {
    if (project.taskCount === 0) return 0;
    return Math.round((project.completedTaskCount / project.taskCount) * 100);
  }

  constructor(
    private projectService: ProjectService,
    private dialog: MatDialog,
    private snackbar: SnackbarService
  ) {}

  ngOnInit(): void {
    this.loadProjects();
  }

  loadProjects(): void {
    this.projectService.getAll().subscribe({
      next: (res) => {
        this.isLoading.set(false);
        if (res.success) {
          this.projects.set(res.data);
        } else {
          this.errorMessage.set(res.message);
        }
      },
      error: () => {
        this.isLoading.set(false);
        this.errorMessage.set('Could not load projects.');
      },
    });
  }

  onDelete(project: Project): void {
    // MatDialog.open() returns a reference to the open dialog. afterClosed()
    // fires once with whatever value the dialog was closed with — true if
    // the user clicked Delete, false (or undefined) for Cancel / clicking
    // outside. This replaces the old blocking confirm() popup.
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'Delete project?',
        message: `"${project.name}" will be permanently deleted. This cannot be undone.`,
        confirmText: 'Delete',
        destructive: true,
      },
    });

    dialogRef.afterClosed().subscribe((confirmed) => {
      if (!confirmed) return;

      this.projectService.delete(project.id).subscribe({
        next: (res) => {
          if (res.success) {
            this.projects.update((list) => list.filter((p) => p.id !== project.id));
            this.snackbar.success(`"${project.name}" was deleted.`);
          } else {
            this.snackbar.error(res.message || 'Could not delete project.');
          }
        },
        error: () => this.snackbar.error('Could not delete project.'),
      });
    });
  }
}
