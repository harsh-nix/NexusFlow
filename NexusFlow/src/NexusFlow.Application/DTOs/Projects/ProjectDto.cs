using System;
using System.Collections.Generic;
using System.Text;

namespace NexusFlow.Application.DTOs.Projects
{
    public class ProjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int OrganizationId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int MemberCount { get; set; }
        public int TaskCount { get; set; }
    }
}