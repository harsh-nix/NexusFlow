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

// Maps the backend's status string (e.g. "OnHold") to one of the 5
// reusable ".status-chip--*" classes defined globally in styles.css,
// so every status pill in the app shares the same color language.
const STATUS_TO_CHIP_CLASS: Record<string, string> = {
  Planning: 'status-chip--todo',
  Active: 'status-chip--in-progress',
  OnHold: 'status-chip--in-review',
  Completed: 'status-chip--done',
  Cancelled: 'status-chip--cancelled',
};

export function projectStatusChipClass(status: string): string {
  return STATUS_TO_CHIP_CLASS[status] ?? 'status-chip--todo';
}

export interface UpdateProjectRequest {
  name: string;
  description?: string;
  status: ProjectStatusEnum;
  startDate?: string;
  endDate?: string;
}