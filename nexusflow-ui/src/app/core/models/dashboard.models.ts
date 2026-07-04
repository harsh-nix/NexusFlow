export interface RecentActivity {
  title: string;
  type: string;
  date: string;
}

export interface DashboardStats {
  totalProjects: number;
  activeProjects: number;
  totalTasks: number;
  completedTasks: number;
  pendingTasks: number;
  inProgressTasks: number;
  unreadNotifications: number;
  recentActivities: RecentActivity[];
}