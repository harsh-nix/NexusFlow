import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/auth.models';
import { PagedAuditLogResult } from '../models/auditlog.models';

@Injectable({ providedIn: 'root' })
export class AuditLogService {
  private readonly baseUrl = `${environment.apiUrl}/auditlogs`;

  constructor(private http: HttpClient) {}

  getAll(
    entityName?: string,
    action?: string,
    page = 1,
    pageSize = 50
  ): Observable<ApiResponse<PagedAuditLogResult>> {
    let params = new HttpParams().set('page', page).set('pageSize', pageSize);
    if (entityName) params = params.set('entityName', entityName);
    if (action) params = params.set('action', action);

    return this.http.get<ApiResponse<PagedAuditLogResult>>(this.baseUrl, { params });
  }
}