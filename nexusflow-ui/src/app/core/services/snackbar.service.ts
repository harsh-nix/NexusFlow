import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

// A single, reusable place to show toast-style confirmations across the
// whole app (Login, Register, Create Project, Delete Task, etc). Any
// component just injects this and calls snackbar.success('Saved!')
// instead of each component configuring MatSnackBar's duration,
// position, and styling on its own.
@Injectable({ providedIn: 'root' })
export class SnackbarService {
  constructor(private snackBar: MatSnackBar) {}

  success(message: string): void {
    this.show(message, 'nf-snackbar--success');
  }

  error(message: string): void {
    this.show(message, 'nf-snackbar--error');
  }

  info(message: string): void {
    this.show(message, 'nf-snackbar--info');
  }

  private show(message: string, panelClass: string): void {
    this.snackBar.open(message, 'Close', {
      duration: 4000,
      horizontalPosition: 'end',
      verticalPosition: 'top',
      panelClass: ['nf-snackbar', panelClass],
    });
  }
}