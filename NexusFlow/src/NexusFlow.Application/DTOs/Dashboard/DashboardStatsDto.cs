namespace NexusFlow.Application.DTOs.Dashboard
{
    public class DashboardStatsDto
    {
        public int TotalProjects { get; set; }
        public int ActiveProjects { get; set; }
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int PendingTasks { get; set; }
        public int InProgressTasks { get; set; }
        public int UnreadNotifications { get; set; }
        public List<RecentActivityDto> RecentActivities { get; set; } = new();
    }

    public class RecentActivityDto
    {
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }
}