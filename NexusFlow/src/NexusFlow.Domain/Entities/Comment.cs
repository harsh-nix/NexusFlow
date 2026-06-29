using System;
using System.Collections.Generic;
using System.Text;

namespace NexusFlow.Domain.Entities
{
    public class Comment : BaseEntity
    {
        public string Content { get; set; } = string.Empty;
        public int TaskId { get; set; }
        public int UserId { get; set; }

        // Navigation
        public ProjectTask? Task { get; set; }
        public User? User { get; set; }
    }
}