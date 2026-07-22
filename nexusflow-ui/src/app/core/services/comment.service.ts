import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/auth.models';
import { CreateCommentRequest, RequestClarificationRequest, RespondClarificationRequest, TaskComment } from '../models/comment.models';

@Injectable({ providedIn: 'root' })
export class CommentService {
  private readonly baseUrl = `${environment.apiUrl}/comments`;

  constructor(private http: HttpClient) {}

  getByTask(taskId: number): Observable<ApiResponse<TaskComment[]>> {
    return this.http.get<ApiResponse<TaskComment[]>>(`${this.baseUrl}/task/${taskId}`);
  }

  create(request: CreateCommentRequest): Observable<ApiResponse<TaskComment>> {
    return this.http.post<ApiResponse<TaskComment>>(this.baseUrl, request);
  }

  delete(id: number): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.baseUrl}/${id}`);
  }
  requestClarification(taskId: number, request: RequestClarificationRequest): Observable<ApiResponse<TaskComment>> {
    return this.http.post<ApiResponse<TaskComment>>(`${this.baseUrl}/task/${taskId}/clarification`, request);
  }

  respondToClarification(taskId: number, request: RespondClarificationRequest): Observable<ApiResponse<TaskComment>> {
    return this.http.post<ApiResponse<TaskComment>>(`${this.baseUrl}/task/${taskId}/clarification/respond`, request);
  }
}