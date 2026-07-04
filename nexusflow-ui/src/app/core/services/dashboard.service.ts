import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/auth.models';
import { DashboardStats } from '../models/dashboard.models';

@Injectable({ providedIn: 'root' })
export class DashboardService {
  private readonly baseUrl = `${environment.apiUrl}/dashboard`;

  constructor(private http: HttpClient) {}

  getStats(): Observable<ApiResponse<DashboardStats>> {
    // GET request — no body needed. The JWT is attached automatically
    // by authInterceptor (from Batch 1), so we don't set headers here.
    return this.http.get<ApiResponse<DashboardStats>>(this.baseUrl);
  }
}