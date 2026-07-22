export interface TaskComment {
  id: number;
  content: string;
  taskId: number;
  userId: number;
  userName: string;
  createdAt: string;
  type: string;
}
export interface CreateCommentRequest {
  content: string;
  taskId: number;
}
export interface RequestClarificationRequest {
  message: string;
}

export interface RespondClarificationRequest {
  message: string;
}