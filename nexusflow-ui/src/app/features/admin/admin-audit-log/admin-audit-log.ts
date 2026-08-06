import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { AuditLogService } from '../../../core/services/auditlog.service';
import { AuditLogEntry } from '../../../core/models/auditlog.models';

@Component({
  selector: 'app-admin-audit-log',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatCardModule,
    MatButtonModule,
    MatFormFieldModule,
    MatSelectModule,
    MatIconModule,
  ],
  templateUrl: './admin-audit-log.html',
  styleUrl: './admin-audit-log.css',
})
export class AdminAuditLogComponent implements OnInit {
  logs = signal<AuditLogEntry[]>([]);
  totalCount = signal(0);
  isLoading = signal(true);
  errorMessage = signal<string | null>(null);

  entityNameFilter = '';
  actionFilter = '';
  page = 1;
  pageSize = 50;

  entityOptions = ['ProjectTask', 'User', 'Project'];

  constructor(private auditLogService: AuditLogService) {}

  ngOnInit(): void {
    this.loadLogs();
  }

  loadLogs(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.auditLogService
      .getAll(
        this.entityNameFilter || undefined,
        this.actionFilter || undefined,
        this.page,
        this.pageSize
      )
      .subscribe({
        next: (res) => {
          this.isLoading.set(false);
          if (res.success) {
            this.logs.set(res.data.items);
            this.totalCount.set(res.data.totalCount);
          } else {
            this.errorMessage.set(res.message);
          }
        },
        error: () => {
          this.isLoading.set(false);
          this.errorMessage.set('Could not load the audit log.');
        },
      });
  }

  applyFilters(): void {
    this.page = 1;
    this.loadLogs();
  }

  nextPage(): void {
    if (this.page * this.pageSize >= this.totalCount()) return;
    this.page++;
    this.loadLogs();
  }

  previousPage(): void {
    if (this.page <= 1) return;
    this.page--;
    this.loadLogs();
  }

  get totalPages(): number {
    return Math.max(1, Math.ceil(this.totalCount() / this.pageSize));
  }
}