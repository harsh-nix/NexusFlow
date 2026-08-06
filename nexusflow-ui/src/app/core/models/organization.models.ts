export interface Organization {
  id: number;
  name: string;
  description: string | null;
  website: string | null;
  departmentCount: number;
  userCount: number;
  createdAt: string;
}

export interface CreateOrganizationRequest {
  name: string;
  description?: string;
  website?: string;
}

export interface UpdateOrganizationRequest {
  name: string;
  description?: string;
  website?: string;
}

export interface Department {
  id: number;
  name: string;
  description: string | null;
  organizationId: number;
  organizationName: string;
  teamCount: number;
  createdAt: string;
}

export interface CreateDepartmentRequest {
  name: string;
  description?: string;
}

export interface UpdateDepartmentRequest {
  name: string;
  description?: string;
}

export interface TeamMember {
  id: number;
  userId: number;
  userName: string;
  userEmail: string;
  joinedAt: string;
}

export interface Team {
  id: number;
  name: string;
  description: string | null;
  departmentId: number;
  departmentName: string;
  members: TeamMember[];
  createdAt: string;
}

export interface CreateTeamRequest {
  name: string;
  description?: string;
}

export interface UpdateTeamRequest {
  name: string;
  description?: string;
}

export interface AddTeamMemberRequest {
  userId: number;
}