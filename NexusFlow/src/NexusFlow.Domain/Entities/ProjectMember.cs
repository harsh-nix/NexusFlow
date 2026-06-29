using System;
using System.Collections.Generic;
using System.Text;

using NexusFlow.Domain.Enums;

namespace NexusFlow.Domain.Entities
{
    public class ProjectMember : BaseEntity
    {
        public int ProjectId { get; set; }
        public int UserId { get; set; }
        public UserRole Role { get; set; } = UserRole.TeamMember;
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Project? Project { get; set; }
        public User? User { get; set; }
    }
}