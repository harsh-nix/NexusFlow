export interface ProjectTask {
  id: number;
  title: string;
  description: string | null;
  status: string;
  priority: string;
  dueDate: string | null;
  projectId: number;
  projectName: string;
  createdBy: number;
  createdAt: string;
  assigneeNames: string[];
  subTaskCount: number;
  commentCount: number;
  assignmentNote: string | null;
}

// Must match NexusFlow.Domain.Enums.TaskPriority exactly:
// Low = 1, Medium = 2, High = 3, Critical = 4
export enum TaskPriority {
  Low = 1,
  Medium = 2,
  High = 3,
  Critical = 4,
}

// Must match NexusFlow.Domain.Enums.TaskStatus exactly:
// Todo = 1, InProgress = 2, InReview = 3, Done = 4, Cancelled = 5
export enum TaskStatusEnum {
  Todo = 1,
  InProgress = 2,
  InReview = 3,
  Done = 4,
  Cancelled = 5,
}

export const TASK_STATUS_OPTIONS = [
  { value: TaskStatusEnum.Todo, label: 'Todo' },
  { value: TaskStatusEnum.InProgress, label: 'In Progress' },
  { value: TaskStatusEnum.InReview, label: 'In Review' },
  { value: TaskStatusEnum.Done, label: 'Done' },
  { value: TaskStatusEnum.Cancelled, label: 'Cancelled' },
];

// The backend sends status/priority back as strings (e.g. "InProgress"),
// but the UpdateTaskDto expects the raw numeric enum on the way in.
// These two lookups convert the string we received back into the number
// we need to send.
const PRIORITY_STRING_TO_ENUM: Record<string, TaskPriority> = {
  Low: TaskPriority.Low,
  Medium: TaskPriority.Medium,
  High: TaskPriority.High,
  Critical: TaskPriority.Critical,
};

const STATUS_STRING_TO_ENUM: Record<string, TaskStatusEnum> = {
  Todo: TaskStatusEnum.Todo,
  InProgress: TaskStatusEnum.InProgress,
  InReview: TaskStatusEnum.InReview,
  Done: TaskStatusEnum.Done,
  Cancelled: TaskStatusEnum.Cancelled,
};

export function priorityStringToEnum(value: string): TaskPriority {
  return PRIORITY_STRING_TO_ENUM[value] ?? TaskPriority.Medium;
}

export function statusStringToEnum(value: string): TaskStatusEnum {
  return STATUS_STRING_TO_ENUM[value] ?? TaskStatusEnum.Todo;
}

// Maps backend status/priority strings to the reusable chip classes
// defined globally in styles.css, so task cards, project cards, and
// anywhere else chips appear all share one color language.
const STATUS_TO_CHIP_CLASS: Record<string, string> = {
  Todo: 'status-chip--todo',
  InProgress: 'status-chip--in-progress',
  InReview: 'status-chip--in-review',
  Done: 'status-chip--done',
  Cancelled: 'status-chip--cancelled',
};

const PRIORITY_TO_CHIP_CLASS: Record<string, string> = {
  Low: 'priority-chip--low',
  Medium: 'priority-chip--medium',
  High: 'priority-chip--high',
  Critical: 'priority-chip--critical',
};

export function taskStatusChipClass(status: string): string {
  return STATUS_TO_CHIP_CLASS[status] ?? 'status-chip--todo';
}

export function taskPriorityChipClass(priority: string): string {
  return PRIORITY_TO_CHIP_CLASS[priority] ?? 'priority-chip--medium';
}

export interface CreateTaskRequest {
  title: string;
  description?: string;
  priority: TaskPriority;
  dueDate?: string;
  projectId: number;
  assigneeIds: number[];
}

export interface UpdateTaskRequest {
  title: string;
  description?: string;
  status: TaskStatusEnum;
  priority: TaskPriority;
  dueDate?: string;
}
export interface UpdateTaskStatusRequest {
  status: TaskStatusEnum;
  note?: string;
}

export interface TaskActivity {
  action: string;
  oldValue: string | null;
  newValue: string | null;
  userId: number;
  userName: string;
  createdAt: string;
}