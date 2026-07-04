export interface TaskComment {
  id: number;
  content: string;
  taskId: number;
  userId: number;
  userName: string;
  createdAt: string;
}

export interface CreateCommentRequest {
  content: string;
  taskId: number;
}