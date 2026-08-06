using System;

namespace NexusFlow.Application.DTOs.Reports
{
    public class ProjectReportRowDto
    {
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int MemberCount { get; set; }
        public int TaskCount { get; set; }
        public int CompletedTaskCount { get; set; }
        public int ProgressPercent { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class TaskReportRowDto
    {
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string Assignees { get; set; } = string.Empty;
        public string? DueDate { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
    }
}