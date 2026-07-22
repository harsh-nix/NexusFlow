using System;
using System.Collections.Generic;
using System.Text;

using NexusFlow.Domain.Enums;
using TaskStatus = NexusFlow.Domain.Enums.TaskStatus;

namespace NexusFlow.Application.DTOs.Tasks
{
    public class CreateTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        public DateTime? DueDate { get; set; }
        public int ProjectId { get; set; }
        public List<int> AssigneeIds { get; set; } = new();

        // Optional. Shown to the assignee above the discussion thread so
        // they see *why* they got this task before reading any comments.
        public string? AssignmentNote { get; set; }
    }
}