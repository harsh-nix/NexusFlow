using System;

namespace NexusFlow.Application.DTOs.Tasks
{
    public class SubTaskDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public int ParentTaskId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}