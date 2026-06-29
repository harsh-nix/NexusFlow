using System;
using System.Collections.Generic;
using System.Text;

namespace NexusFlow.Domain.Entities
{
    public class TaskAssignee : BaseEntity
    {
        public int TaskId { get; set; }
        public int UserId { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ProjectTask? Task { get; set; }
        public User? User { get; set; }
    }
}