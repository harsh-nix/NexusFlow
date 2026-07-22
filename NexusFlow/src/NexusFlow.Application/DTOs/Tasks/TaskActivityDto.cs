namespace NexusFlow.Application.DTOs.Tasks
{
    // One row in a task's Activity Timeline, read from AuditLog.
    public class TaskActivityDto
    {
        public string Action { get; set; } = string.Empty;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}