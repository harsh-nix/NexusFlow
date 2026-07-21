import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatBadgeModule } from '@angular/material/badge';
import { MatButtonModule } from '@angular/material/button';
import { MatDividerModule } from '@angular/material/divider';
import { AuthService } from '../../core/services/auth.service';
import { NotificationService } from '../../core/services/notification.service';
import { AppNotification } from '../../core/models/notification.models';
import { FooterComponent } from '../footer/footer';

// This component now owns the toolbar + notification bell that used to
// live only inside dashboard.ts. Because it has a <router-outlet> of its
// own (see shell.html), every child route (dashboard, projects, tasks)
// renders INSIDE this shell automatically — one toolbar, every page.
@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    RouterLinkActive,
    RouterOutlet,
    MatToolbarModule,
    MatIconModule,
    MatMenuModule,
    MatBadgeModule,
    MatButtonModule,
    MatDividerModule,
    FooterComponent,
  ],
  templateUrl: './shell.html',
  styleUrl: './shell.css',
})
export class ShellComponent implements OnInit {
  notifications = signal<AppNotification[]>([]);
  unreadCount = computed(() => this.notifications().filter((n) => !n.isRead).length);

  // Two-letter initials for the avatar circle, e.g. "Harsh Mishra" -> "HM"
  userInitials = computed(() => {
    const user = this.authService.currentUser();
    if (!user?.fullName) return '?';
    const parts = user.fullName.trim().split(/\s+/);
    const first = parts[0]?.[0] ?? '';
    const last = parts.length > 1 ? parts[parts.length - 1][0] : '';
    return (first + last).toUpperCase();
  });

  constructor(public authService: AuthService, private notificationService: NotificationService) {}

  ngOnInit(): void {
    this.loadNotifications();
  }

  loadNotifications(): void {
    this.notificationService.getAll().subscribe({
      next: (res) => {
        if (res.success) this.notifications.set(res.data);
      },
    });
  }

  onNotificationClick(notification: AppNotification): void {
    if (notification.isRead) return;

    this.notificationService.markAsRead(notification.id).subscribe({
      next: (res) => {
        if (res.success) {
          this.notifications.update((list) =>
            list.map((n) => (n.id === notification.id ? { ...n, isRead: true } : n))
          );
        }
      },
    });
  }

  onMarkAllAsRead(): void {
    this.notificationService.markAllAsRead().subscribe({
      next: (res) => {
        if (res.success) {
          this.notifications.update((list) => list.map((n) => ({ ...n, isRead: true })));
        }
      },
    });
  }

  onLogout(): void {
    this.authService.logout();
  }
}