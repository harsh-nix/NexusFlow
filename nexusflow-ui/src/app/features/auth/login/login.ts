import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { AuthService } from '../../../core/services/auth.service';
import { SnackbarService } from '../../../core/services/snackbar.service';

// "standalone: true" means this component does NOT need to be declared
// inside an NgModule — it lists everything it needs (forms, Material
// components, router links) directly in its own "imports" array below.
@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCardModule,
    MatProgressSpinnerModule,
    MatIconModule,
  ],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class LoginComponent {
  // A FormGroup bundles multiple form fields (FormControls) together so
  // we can validate and read them as one object. This is Angular's
  // "Reactive Forms" approach — the form's state lives in TypeScript,
  // not scattered across template attributes.
  loginForm: FormGroup;

  // signal() creates a reactive value. Whenever we call .set() on it,
  // any part of the template reading it (e.g. @if (errorMessage())) updates
  // automatically — no manual DOM manipulation needed.
  errorMessage = signal<string | null>(null);
  isLoading = signal(false);

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private snackbar: SnackbarService
  ) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required]],
    });
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched(); // forces validation messages to show
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set(null);

    // .subscribe() runs the HTTP call. "next" fires on success,
    // "error" fires if the API returns a 4xx/5xx status code.
    this.authService.login(this.loginForm.value).subscribe({
      next: (res) => {
        this.isLoading.set(false);
        if (res.success) {
          this.snackbar.success('Welcome back!');
          this.router.navigate(['/dashboard']);
        } else {
          const message = res.message || 'Login failed.';
          this.errorMessage.set(message);
          this.snackbar.error(message);
        }
      },
      error: (err) => {
        this.isLoading.set(false);
        const message = err.error?.message || 'Something went wrong. Please try again.';
        this.errorMessage.set(message);
        this.snackbar.error(message);
      },
    });
  }
}