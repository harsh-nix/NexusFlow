using System;
using System.Collections.Generic;
using System.Text;

namespace NexusFlow.Domain.Entities
{
    public class Department : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int OrganizationId { get; set; }

        // Navigation
        public Organization? Organization { get; set; }
        public ICollection<Team> Teams { get; set; } = new List<Team>();
    }
}