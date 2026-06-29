using System;
using System.Collections.Generic;
using System.Text;

using NexusFlow.Domain.Enums;

namespace NexusFlow.Domain.Entities
{
    public class Notification : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; } = false;
        public NotificationType Type { get; set; }
        public int UserId { get; set; }

        // Navigation
        public User? User { get; set; }
    }
}