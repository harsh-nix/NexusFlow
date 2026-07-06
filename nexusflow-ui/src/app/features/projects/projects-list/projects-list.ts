import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { ProjectService } from '../../../core/services/project.service';
import { Project } from '../../../core/models/project.models';

@Component({
  selector: 'app-projects-list',
  standalone: true,
  imports: [CommonModule, RouterLink, MatCardModule, MatButtonModule, MatChipsModule, MatIconModule],
  templateUrl: './projects-list.html',
  styleUrl: './projects-list.css',
})
export class ProjectsListComponent implements OnInit {
  projects = signal<Project[]>([]);
  isLoading = signal(true);
  errorMessage = signal<string | null>(null);

  constructor(private projectService: ProjectService) {}

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
    // A plain browser confirm() dialog — simple and blocking, appropriate
    // for a destructive action like this rather than a silent click.
    const confirmed = confirm(`Delete "${project.name}"? This cannot be undone.`);
    if (!confirmed) return;

    this.projectService.delete(project.id).subscribe({
      next: (res) => {
        if (res.success) {
          this.projects.update((list) => list.filter((p) => p.id !== project.id));
        }
      },
    });
  }
}