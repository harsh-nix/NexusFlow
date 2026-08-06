import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/auth.models';
import {
  AddTeamMemberRequest,
  CreateDepartmentRequest,
  CreateOrganizationRequest,
  CreateTeamRequest,
  Department,
  Organization,
  Team,
  TeamMember,
  UpdateDepartmentRequest,
  UpdateOrganizationRequest,
  UpdateTeamRequest,
} from '../models/organization.models';

@Injectable({ providedIn: 'root' })
export class OrganizationService {
  private readonly baseUrl = `${environment.apiUrl}/organizations`;

  constructor(private http: HttpClient) {}

  getAllOrganizations(): Observable<ApiResponse<Organization[]>> {
    return this.http.get<ApiResponse<Organization[]>>(this.baseUrl);
  }

  createOrganization(request: CreateOrganizationRequest): Observable<ApiResponse<Organization>> {
    return this.http.post<ApiResponse<Organization>>(this.baseUrl, request);
  }

  updateOrganization(
    id: number,
    request: UpdateOrganizationRequest
  ): Observable<ApiResponse<Organization>> {
    return this.http.put<ApiResponse<Organization>>(`${this.baseUrl}/${id}`, request);
  }

  deleteOrganization(id: number): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.baseUrl}/${id}`);
  }

  getDepartments(organizationId: number): Observable<ApiResponse<Department[]>> {
    return this.http.get<ApiResponse<Department[]>>(
      `${this.baseUrl}/${organizationId}/departments`
    );
  }

  createDepartment(
    organizationId: number,
    request: CreateDepartmentRequest
  ): Observable<ApiResponse<Department>> {
    return this.http.post<ApiResponse<Department>>(
      `${this.baseUrl}/${organizationId}/departments`,
      request
    );
  }

  updateDepartment(
    id: number,
    request: UpdateDepartmentRequest
  ): Observable<ApiResponse<Department>> {
    return this.http.put<ApiResponse<Department>>(`${this.baseUrl}/departments/${id}`, request);
  }

  deleteDepartment(id: number): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.baseUrl}/departments/${id}`);
  }

  getTeams(departmentId: number): Observable<ApiResponse<Team[]>> {
    return this.http.get<ApiResponse<Team[]>>(`${this.baseUrl}/departments/${departmentId}/teams`);
  }

  createTeam(departmentId: number, request: CreateTeamRequest): Observable<ApiResponse<Team>> {
    return this.http.post<ApiResponse<Team>>(
      `${this.baseUrl}/departments/${departmentId}/teams`,
      request
    );
  }

  updateTeam(id: number, request: UpdateTeamRequest): Observable<ApiResponse<Team>> {
    return this.http.put<ApiResponse<Team>>(`${this.baseUrl}/teams/${id}`, request);
  }

  deleteTeam(id: number): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`${this.baseUrl}/teams/${id}`);
  }

  addTeamMember(
    teamId: number,
    request: AddTeamMemberRequest
  ): Observable<ApiResponse<TeamMember>> {
    return this.http.post<ApiResponse<TeamMember>>(
      `${this.baseUrl}/teams/${teamId}/members`,
      request
    );
  }

  removeTeamMember(teamMemberId: number): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(
      `${this.baseUrl}/teams/members/${teamMemberId}`
    );
  }
}