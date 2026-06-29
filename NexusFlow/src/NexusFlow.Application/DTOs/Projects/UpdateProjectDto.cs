using System;
using System.Collections.Generic;
using System.Text;

using NexusFlow.Domain.Enums;

namespace NexusFlow.Application.DTOs.Projects
{
    public class UpdateProjectDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public ProjectStatus Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}