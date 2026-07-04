export interface Project {
  id: number;
  name: string;
  description: string | null;
  status: string;
  startDate: string | null;
  endDate: string | null;
  organizationId: number;
  createdBy: number;
  createdAt: string;
  memberCount: number;
  taskCount: number;
}

export interface CreateProjectRequest {
  name: string;
  description?: string;
  startDate?: string;
  endDate?: string;
  organizationId: number;
}