export interface TaskFile {
  id: number;
  originalFileName: string;
  contentType: string;
  fileSizeBytes: number;
  taskId: number;
  uploadedBy: number;
  uploadedByName: string;
  createdAt: string;
}