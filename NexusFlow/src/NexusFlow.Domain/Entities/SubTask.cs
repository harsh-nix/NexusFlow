using System;
using System.Collections.Generic;
using System.Text;

namespace NexusFlow.Domain.Entities
{
    public class SubTask : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public bool IsCompleted { get; set; } = false;
        public int ParentTaskId { get; set; }

        // Navigation
        public ProjectTask? ParentTask { get; set; }
    }
}