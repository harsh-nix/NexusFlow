export interface SubTask {
  id: number;
  title: string;
  isCompleted: boolean;
  parentTaskId: number;
  createdAt: string;
}

export interface CreateSubTaskRequest {
  title: string;
}

export interface UpdateSubTaskRequest {
  title: string;
  isCompleted: boolean;
}