using System;
using System.Collections.Generic;
using System.Text;

using NexusFlow.Domain.Enums;

namespace NexusFlow.Domain.Entities
{
    public class Project : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public ProjectStatus Status { get; set; } = ProjectStatus.Planning;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int OrganizationId { get; set; }

        // Navigation
        public Organization? Organization { get; set; }
        public ICollection<ProjectMember> ProjectMembers { get; set; } = new List<ProjectMember>();
        public ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
    }
}