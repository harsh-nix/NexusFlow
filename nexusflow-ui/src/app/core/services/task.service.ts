import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/auth.models';
import { CreateTaskRequest, ProjectTask, TaskActivity, UpdateTaskRequest, UpdateTaskStatusRequest } from '../models/task.models';

@Injectable({ providedIn: 'root' })
export class TaskService {
  private readonly baseUrl = `${environment.apiUrl}/tasks`;

  constructor(private http: HttpClient) {}

  getById(id: number): Observable<ApiResponse<ProjectTask>> {
    return this.http.get<ApiResponse<ProjectTask>>(`${this.baseUrl}/${id}`);
  }

  getByProject(projectId: number): Observable<ApiResponse<ProjectTask[]>> {
    return this.http.get<ApiResponse<ProjectTask[]>>(`${this.baseUrl}/project/${projectId}`);
  }
  getMyTasks(): Observable<ApiResponse<ProjectTask[]>> {
    return this.http.get<ApiResponse<ProjectTask[]>>(`${this.baseUrl}/my`);
  }

  create(request: CreateTaskRequest): Observable<ApiResponse<ProjectTask>> {
    return this.http.post<ApiResponse<ProjectTask>>(this.baseUrl, request);
  }

  update(id: number, request: UpdateTaskRequest): Observable<ApiResponse<ProjectTask>> {
    return this.http.put<ApiResponse<ProjectTask>>(`${this.baseUrl}/${id}`, request);
  }

  delete(id: number): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.baseUrl}/${id}`);
  }
  accept(id: number): Observable<ApiResponse<ProjectTask>> {
    return this.http.post<ApiResponse<ProjectTask>>(`${this.baseUrl}/${id}/accept`, {});
  }

  updateStatus(id: number, request: UpdateTaskStatusRequest): Observable<ApiResponse<ProjectTask>> {
    return this.http.put<ApiResponse<ProjectTask>>(`${this.baseUrl}/${id}/status`, request);
  }

  getActivity(id: number): Observable<ApiResponse<TaskActivity[]>> {
    return this.http.get<ApiResponse<TaskActivity[]>>(`${this.baseUrl}/${id}/activity`);
  }
}