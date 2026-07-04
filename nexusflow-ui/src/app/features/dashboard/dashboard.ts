import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { AuthService } from '../../core/services/auth.service';
import { DashboardService } from '../../core/services/dashboard.service';
import { DashboardStats } from '../../core/models/dashboard.models';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  // RouterLink MUST be listed here, not just imported at the top of the file.
  // Angular standalone components only "see" a directive if it's in this array —
  // that's the whole reason routerLink wasn't working.
  imports: [CommonModule, RouterLink, MatCardModule, MatButtonModule, MatToolbarModule, MatIconModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class DashboardComponent implements OnInit {
  stats = signal<DashboardStats | null>(null);
  isLoading = signal(true);
  errorMessage = signal<string | null>(null);

  constructor(
    public authService: AuthService,
    private dashboardService: DashboardService
  ) {}

  ngOnInit(): void {
    this.dashboardService.getStats().subscribe({
      next: (res) => {
        this.isLoading.set(false);
        if (res.success) {
          this.stats.set(res.data);
        } else {
          this.errorMessage.set(res.message);
        }
      },
      error: () => {
        this.isLoading.set(false);
        this.errorMessage.set('Could not load dashboard data.');
      },
    });
  }

  onLogout(): void {
    this.authService.logout();
  }
}