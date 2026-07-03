import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, AuthResponse, LoginRequest, RegisterRequest } from '../models/auth.models';

const ACCESS_TOKEN_KEY = 'nf_access_token';
const REFRESH_TOKEN_KEY = 'nf_refresh_token';
const USER_KEY = 'nf_user';

export interface StoredUser {
  userId: number;
  fullName: string;
  email: string;
  role: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly baseUrl = `${environment.apiUrl}/auth`;

  // signal-based reactive current-user state (Angular 20 idiomatic)
  private readonly _currentUser = signal<StoredUser | null>(this.readStoredUser());
  readonly currentUser = this._currentUser.asReadonly();
  readonly isLoggedIn = computed(() => this._currentUser() !== null);

  constructor(private http: HttpClient, private router: Router) {}

  register(request: RegisterRequest): Observable<ApiResponse<AuthResponse>> {
    return this.http
      .post<ApiResponse<AuthResponse>>(`${this.baseUrl}/register`, request)
      .pipe(tap((res) => this.handleAuthSuccess(res)));
  }

  login(request: LoginRequest): Observable<ApiResponse<AuthResponse>> {
    return this.http
      .post<ApiResponse<AuthResponse>>(`${this.baseUrl}/login`, request)
      .pipe(tap((res) => this.handleAuthSuccess(res)));
  }

  refreshToken(): Observable<ApiResponse<AuthResponse>> {
    const refreshToken = this.getRefreshToken();
    // NOTE: the API's RefreshToken/Logout endpoints take [FromBody] string,
    // so the body must be a raw JSON string literal, not { refreshToken }.
    return this.http
      .post<ApiResponse<AuthResponse>>(`${this.baseUrl}/refresh-token`, JSON.stringify(refreshToken), {
        headers: { 'Content-Type': 'application/json' },
      })
      .pipe(tap((res) => this.handleAuthSuccess(res)));
  }

  logout(): void {
    const refreshToken = this.getRefreshToken();
    if (refreshToken) {
      this.http
        .post(`${this.baseUrl}/logout`, JSON.stringify(refreshToken), {
          headers: { 'Content-Type': 'application/json' },
        })
        .subscribe({ complete: () => this.clearSession(), error: () => this.clearSession() });
    } else {
      this.clearSession();
    }
  }

  getAccessToken(): string | null {
    return localStorage.getItem(ACCESS_TOKEN_KEY);
  }

  getRefreshToken(): string | null {
    return localStorage.getItem(REFRESH_TOKEN_KEY);
  }

  private handleAuthSuccess(res: ApiResponse<AuthResponse>): void {
    if (!res.success || !res.data) return;

    localStorage.setItem(ACCESS_TOKEN_KEY, res.data.accessToken);
    localStorage.setItem(REFRESH_TOKEN_KEY, res.data.refreshToken);

    const user: StoredUser = {
      userId: res.data.userId,
      fullName: res.data.fullName,
      email: res.data.email,
      role: res.data.role,
    };
    localStorage.setItem(USER_KEY, JSON.stringify(user));
    this._currentUser.set(user);
  }

  private clearSession(): void {
    localStorage.removeItem(ACCESS_TOKEN_KEY);
    localStorage.removeItem(REFRESH_TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    this._currentUser.set(null);
    this.router.navigate(['/login']);
  }

  private readStoredUser(): StoredUser | null {
    const raw = localStorage.getItem(USER_KEY);
    return raw ? (JSON.parse(raw) as StoredUser) : null;
  }
}