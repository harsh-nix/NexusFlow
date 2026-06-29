using NexusFlow.Domain.Enums;
using TaskStatus = NexusFlow.Domain.Enums.TaskStatus;

namespace NexusFlow.Domain.Entities
{
    public class ProjectTask : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TaskStatus Status { get; set; } = TaskStatus.Todo;
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        public DateTime? DueDate { get; set; }
        public int ProjectId { get; set; }

        // Navigation
        public Project? Project { get; set; }
        public ICollection<SubTask> SubTasks { get; set; } = new List<SubTask>();
        public ICollection<TaskAssignee> Assignees { get; set; } = new List<TaskAssignee>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}