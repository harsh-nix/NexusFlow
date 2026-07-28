import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/auth.models';
import { TaskFile } from '../models/file.models';

@Injectable({ providedIn: 'root' })
export class FileService {
  private readonly baseUrl = `${environment.apiUrl}/files`;

  constructor(private http: HttpClient) {}

  getByTask(taskId: number): Observable<ApiResponse<TaskFile[]>> {
    return this.http.get<ApiResponse<TaskFile[]>>(`${this.baseUrl}/task/${taskId}`);
  }

  upload(taskId: number, file: File): Observable<ApiResponse<TaskFile>> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<ApiResponse<TaskFile>>(`${this.baseUrl}/task/${taskId}`, formData);
  }

  download(id: number): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/${id}/download`, { responseType: 'blob' });
  }

  delete(id: number): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.baseUrl}/${id}`);
  }
}