export interface AppNotification {
  id: number;
  title: string;
  message: string;
  isRead: boolean;
  type: string;
  createdAt: string;
  relatedTaskId: number | null;
}