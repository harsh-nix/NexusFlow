using System;
using System.Collections.Generic;
using System.Text;

namespace NexusFlow.Domain.Entities
{
    public class Organization : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? LogoUrl { get; set; }
        public string? Website { get; set; }

        // Navigation
        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<Department> Departments { get; set; } = new List<Department>();
        public ICollection<Project> Projects { get; set; } = new List<Project>();
    }
}
