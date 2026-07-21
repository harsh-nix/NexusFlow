import { Component, OnInit, signal, ViewChild, ElementRef, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Chart, registerables } from 'chart.js';
import { DashboardService } from '../../core/services/dashboard.service';
import { DashboardStats } from '../../core/models/dashboard.models';

// Chart.js ships its chart types (bar, pie, etc.) as separate pieces you
// opt into, to keep bundle size small. registerables just turns all of
// them on — simplest option for a project this size.
Chart.register(...registerables);

interface StatCard {
  icon: string;
  label: string;
  value: number;
  accent: 'primary' | 'done' | 'inprogress' | 'todo' | 'inreview';
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatIconModule, MatProgressSpinnerModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class DashboardComponent implements OnInit {
  // ViewChild grabs a direct reference to the <canvas> elements in the
  // template so Chart.js (a plain JS library, not an Angular one) can
  // draw into them. The "!" tells TypeScript "this will be set before
  // it's used" — safe here because we only touch it after the view renders.
  @ViewChild('taskStatusCanvas') taskStatusCanvas!: ElementRef<HTMLCanvasElement>;
  @ViewChild('projectSplitCanvas') projectSplitCanvas!: ElementRef<HTMLCanvasElement>;

  stats = signal<DashboardStats | null>(null);
  isLoading = signal(true);
  errorMessage = signal<string | null>(null);

  statCards = computed<StatCard[]>(() => {
    const s = this.stats();
    if (!s) return [];
    return [
      { icon: 'folder_open', label: 'Total Projects', value: s.totalProjects, accent: 'primary' },
      { icon: 'trending_up', label: 'In Progress', value: s.inProgressTasks, accent: 'inprogress' },
      { icon: 'check_circle', label: 'Completed', value: s.completedTasks, accent: 'done' },
      { icon: 'pending_actions', label: 'Pending', value: s.pendingTasks, accent: 'todo' },
      { icon: 'notifications_active', label: 'Notifications', value: s.unreadNotifications, accent: 'inreview' },
    ];
  });

  private taskChart?: Chart;
  private projectChart?: Chart;

  constructor(private dashboardService: DashboardService) {}

  ngOnInit(): void {
    this.dashboardService.getStats().subscribe({
      next: (res) => {
        this.isLoading.set(false);
        if (res.success) {
          this.stats.set(res.data);
          // The canvas elements only exist in the DOM once the @if(stats())
          // block in the template renders. setTimeout(0) waits one tick so
          // Angular finishes that render before Chart.js looks for them.
          setTimeout(() => this.renderCharts(res.data), 0);
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

  activityIcon(type: string): string {
    return type === 'Project' ? 'folder_open' : 'task_alt';
  }

  private renderCharts(s: DashboardStats): void {
    const styles = getComputedStyle(document.documentElement);
    const color = (name: string) => styles.getPropertyValue(name).trim();

    this.taskChart?.destroy();
    this.projectChart?.destroy();

    if (this.taskStatusCanvas) {
      this.taskChart = new Chart(this.taskStatusCanvas.nativeElement, {
        type: 'bar',
        data: {
          labels: ['Completed', 'In Progress', 'Pending'],
          datasets: [
            {
              data: [s.completedTasks, s.inProgressTasks, s.pendingTasks],
              backgroundColor: [
                color('--nf-status-done'),
                color('--nf-status-inprogress'),
                color('--nf-status-todo'),
              ],
              borderRadius: 6,
              barThickness: 40,
            },
          ],
        },
        options: {
          responsive: true,
          maintainAspectRatio: false,
          plugins: { legend: { display: false } },
          scales: {
            y: { beginAtZero: true, ticks: { precision: 0 }, grid: { color: color('--nf-border') } },
            x: { grid: { display: false } },
          },
        },
      });
    }

    if (this.projectSplitCanvas) {
      const other = Math.max(s.totalProjects - s.activeProjects, 0);
      this.projectChart = new Chart(this.projectSplitCanvas.nativeElement, {
        type: 'pie',
        data: {
          // "Other" covers any project not currently Active (on hold,
          // completed, cancelled, etc). The API only exposes the active
          // count directly, so this is the honest breakdown available.
          labels: ['Active', 'Other'],
          datasets: [
            {
              data: [s.activeProjects, other],
              backgroundColor: [color('--nf-status-inprogress'), color('--nf-status-todo')],
              borderWidth: 0,
            },
          ],
        },
        options: {
          responsive: true,
          maintainAspectRatio: false,
          plugins: { legend: { position: 'bottom', labels: { boxWidth: 12, padding: 16 } } },
        },
      });
    }
  }
}
