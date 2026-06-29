using System;
using System.Collections.Generic;
using System.Text;

namespace NexusFlow.Application.DTOs.Tasks
{
    public class TaskDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> AssigneeNames { get; set; } = new();
        public int SubTaskCount { get; set; }
        public int CommentCount { get; set; }
    }
}