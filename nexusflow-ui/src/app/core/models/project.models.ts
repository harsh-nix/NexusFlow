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

// Must match NexusFlow.Domain.Enums.ProjectStatus exactly:
// Planning = 1, Active = 2, OnHold = 3, Completed = 4, Cancelled = 5
export enum ProjectStatusEnum {
  Planning = 1,
  Active = 2,
  OnHold = 3,
  Completed = 4,
  Cancelled = 5,
}

export const PROJECT_STATUS_OPTIONS = [
  { value: ProjectStatusEnum.Planning, label: 'Planning' },
  { value: ProjectStatusEnum.Active, label: 'Active' },
  { value: ProjectStatusEnum.OnHold, label: 'On Hold' },
  { value: ProjectStatusEnum.Completed, label: 'Completed' },
  { value: ProjectStatusEnum.Cancelled, label: 'Cancelled' },
];

const STATUS_STRING_TO_ENUM: Record<string, ProjectStatusEnum> = {
  Planning: ProjectStatusEnum.Planning,
  Active: ProjectStatusEnum.Active,
  OnHold: ProjectStatusEnum.OnHold,
  Completed: ProjectStatusEnum.Completed,
  Cancelled: ProjectStatusEnum.Cancelled,
};

export function projectStatusStringToEnum(value: string): ProjectStatusEnum {
  return STATUS_STRING_TO_ENUM[value] ?? ProjectStatusEnum.Planning;
}

export interface UpdateProjectRequest {
  name: string;
  description?: string;
  status: ProjectStatusEnum;
  startDate?: string;
  endDate?: string;
}