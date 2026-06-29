using System;
using System.Collections.Generic;
using System.Text;

namespace NexusFlow.Domain.Entities
{
    public class Team : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int DepartmentId { get; set; }

        // Navigation
        public Department? Department { get; set; }
        public ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();
    }
}