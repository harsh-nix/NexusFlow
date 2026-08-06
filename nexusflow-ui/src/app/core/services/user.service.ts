import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/auth.models';
import { AppUser, CreateUserRequest, UpdateUserRequest } from '../models/user.models';

@Injectable({ providedIn: 'root' })
export class UserService {
  private readonly baseUrl = `${environment.apiUrl}/users`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<ApiResponse<AppUser[]>> {
    return this.http.get<ApiResponse<AppUser[]>>(this.baseUrl);
  }
  getAllForAdmin(): Observable<ApiResponse<AppUser[]>> {
    return this.http.get<ApiResponse<AppUser[]>>(`${this.baseUrl}/admin`);
  }

  create(request: CreateUserRequest): Observable<ApiResponse<AppUser>> {
    return this.http.post<ApiResponse<AppUser>>(this.baseUrl, request);
  }

  update(id: number, request: UpdateUserRequest): Observable<ApiResponse<AppUser>> {
    return this.http.put<ApiResponse<AppUser>>(`${this.baseUrl}/${id}`, request);
  }

  setActiveStatus(id: number, isActive: boolean): Observable<ApiResponse<AppUser>> {
    return this.http.put<ApiResponse<AppUser>>(`${this.baseUrl}/${id}/status`, isActive);
  }
}