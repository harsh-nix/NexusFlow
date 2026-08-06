import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { UserService } from '../../../core/services/user.service';
import { AuthService } from '../../../core/services/auth.service';
import { AppUser } from '../../../core/models/user.models';
import { ConfirmDialogComponent } from '../../../shared/confirm-dialog/confirm-dialog';

@Component({
  selector: 'app-admin-users',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatIconModule,
    MatDialogModule,
  ],
  templateUrl: './admin-users.html',
  styleUrl: './admin-users.css',
})
export class AdminUsersComponent implements OnInit {
  users = signal<AppUser[]>([]);
  isLoading = signal(true);
  errorMessage = signal<string | null>(null);
  successMessage = signal<string | null>(null);

  showCreateForm = signal(false);
  isSaving = signal(false);

  editingUserId = signal<number | null>(null);

  roles = ['Admin', 'ProjectManager', 'TeamMember', 'Client'];

  createForm: FormGroup;
  editForm: FormGroup;

  currentUserId!: number;

  constructor(
    private fb: FormBuilder,
    private userService: UserService,
    private authService: AuthService,
    private dialog: MatDialog
  ) {
    this.currentUserId = this.authService.currentUser()?.userId ?? 0;
    this.createForm = this.fb.group({
      fullName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8)]],
      role: ['TeamMember', Validators.required],
      phoneNumber: [''],
    });

    this.editForm = this.fb.group({
      fullName: ['', Validators.required],
      role: ['', Validators.required],
      phoneNumber: [''],
    });
  }

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.isLoading.set(true);
    this.userService.getAllForAdmin().subscribe({
      next: (res) => {
        this.isLoading.set(false);
        if (res.success) {
          this.users.set(res.data);
        } else {
          this.errorMessage.set(res.message);
        }
      },
      error: (err) => {
        this.isLoading.set(false);
        this.errorMessage.set(this.extractError(err));
      },
    });
  }

  toggleCreateForm(): void {
    this.showCreateForm.set(!this.showCreateForm());
    this.createForm.reset({ role: 'TeamMember' });
  }

  submitCreate(): void {
    if (this.createForm.invalid) {
      this.createForm.markAllAsTouched();
      return;
    }

    this.isSaving.set(true);
    this.clearMessages();

    this.userService.create(this.createForm.value).subscribe({
      next: (res) => {
        this.isSaving.set(false);
        if (res.success) {
          this.successMessage.set('User created.');
          this.showCreateForm.set(false);
          this.createForm.reset({ role: 'TeamMember' });
          this.loadUsers();
        } else {
          this.errorMessage.set(res.message);
        }
      },
      error: (err) => {
        this.isSaving.set(false);
        this.errorMessage.set(this.extractError(err));
      },
    });
  }

  startEdit(user: AppUser): void {
    this.editingUserId.set(user.id);
    this.editForm.setValue({
      fullName: user.fullName,
      role: user.role,
      phoneNumber: user.phoneNumber || '',
    });
  }

  cancelEdit(): void {
    this.editingUserId.set(null);
  }

  submitEdit(userId: number): void {
    if (this.editForm.invalid) {
      this.editForm.markAllAsTouched();
      return;
    }

    this.isSaving.set(true);
    this.clearMessages();

    this.userService.update(userId, this.editForm.value).subscribe({
      next: (res) => {
        this.isSaving.set(false);
        if (res.success) {
          this.successMessage.set('User updated.');
          this.editingUserId.set(null);
          this.loadUsers();
        } else {
          this.errorMessage.set(res.message);
        }
      },
      error: (err) => {
        this.isSaving.set(false);
        this.errorMessage.set(this.extractError(err));
      },
    });
  }

  toggleActiveStatus(user: AppUser): void {
    const activating = !user.isActive;

    if (activating) {
      this.applyStatusChange(user, true);
      return;
    }

    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'Deactivate user?',
        message: `${user.fullName} will no longer be able to log in.`,
        confirmText: 'Deactivate',
        destructive: true,
      },
    });

    dialogRef.afterClosed().subscribe((confirmed) => {
      if (confirmed) {
        this.applyStatusChange(user, false);
      }
    });
  }

  private applyStatusChange(user: AppUser, isActive: boolean): void {
    this.clearMessages();
    this.userService.setActiveStatus(user.id, isActive).subscribe({
      next: (res) => {
        if (res.success) {
          this.successMessage.set(isActive ? 'User activated.' : 'User deactivated.');
          this.loadUsers();
        } else {
          this.errorMessage.set(res.message);
        }
      },
      error: (err) => this.errorMessage.set(this.extractError(err)),
    });
  }

  private clearMessages(): void {
    this.errorMessage.set(null);
    this.successMessage.set(null);
  }

  private extractError(err: any): string {
    const apiErrors = err.error?.errors as string[] | undefined;
    return apiErrors?.join(' ') || err.error?.message || 'Something went wrong.';
  }
}