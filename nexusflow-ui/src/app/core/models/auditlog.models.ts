export interface AuditLogEntry {
  id: number;
  entityName: string;
  entityId: number;
  action: string;
  oldValue: string | null;
  newValue: string | null;
  userId: number;
  userName: string;
  createdAt: string;
}

export interface PagedAuditLogResult {
  items: AuditLogEntry[];
  totalCount: number;
  page: number;
  pageSize: number;
}