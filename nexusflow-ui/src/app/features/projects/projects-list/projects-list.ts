import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { ProjectService } from '../../../core/services/project.service';
import { Project } from '../../../core/models/project.models';

@Component({
  selector: 'app-projects-list',
  standalone: true,
  imports: [CommonModule, RouterLink, MatCardModule, MatButtonModule, MatChipsModule],
  templateUrl: './projects-list.html',
  styleUrl: './projects-list.css',
})
export class ProjectsListComponent implements OnInit {
  projects = signal<Project[]>([]);
  isLoading = signal(true);
  errorMessage = signal<string | null>(null);

  constructor(private projectService: ProjectService) {}

  ngOnInit(): void {
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
}