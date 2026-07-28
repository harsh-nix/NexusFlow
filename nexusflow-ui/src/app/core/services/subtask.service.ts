import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/auth.models';
import { CreateSubTaskRequest, SubTask, UpdateSubTaskRequest } from '../models/subtask.models';

@Injectable({ providedIn: 'root' })
export class SubTaskService {
  private readonly baseUrl = `${environment.apiUrl}/subtasks`;

  constructor(private http: HttpClient) {}

  getByTask(taskId: number): Observable<ApiResponse<SubTask[]>> {
    return this.http.get<ApiResponse<SubTask[]>>(`${this.baseUrl}/task/${taskId}`);
  }

  create(taskId: number, request: CreateSubTaskRequest): Observable<ApiResponse<SubTask>> {
    return this.http.post<ApiResponse<SubTask>>(`${this.baseUrl}/task/${taskId}`, request);
  }

  update(id: number, request: UpdateSubTaskRequest): Observable<ApiResponse<SubTask>> {
    return this.http.put<ApiResponse<SubTask>>(`${this.baseUrl}/${id}`, request);
  }

  delete(id: number): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.baseUrl}/${id}`);
  }
}