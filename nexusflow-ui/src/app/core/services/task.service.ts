import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/auth.models';
import { CreateTaskRequest, ProjectTask, UpdateTaskRequest } from '../models/task.models';

@Injectable({ providedIn: 'root' })
export class TaskService {
  private readonly baseUrl = `${environment.apiUrl}/tasks`;

  constructor(private http: HttpClient) {}

  getByProject(projectId: number): Observable<ApiResponse<ProjectTask[]>> {
    return this.http.get<ApiResponse<ProjectTask[]>>(`${this.baseUrl}/project/${projectId}`);
  }

  create(request: CreateTaskRequest): Observable<ApiResponse<ProjectTask>> {
    return this.http.post<ApiResponse<ProjectTask>>(this.baseUrl, request);
  }

  update(id: number, request: UpdateTaskRequest): Observable<ApiResponse<ProjectTask>> {
    return this.http.put<ApiResponse<ProjectTask>>(`${this.baseUrl}/${id}`, request);
  }
}