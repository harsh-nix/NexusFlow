using System;
using System.Collections.Generic;
using System.Text;

namespace NexusFlow.Domain.Entities
{
    public class AuditLog : BaseEntity
    {
        public string EntityName { get; set; } = string.Empty;
        public int EntityId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public int UserId { get; set; }
    }
}