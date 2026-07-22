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

        // Nullable on purpose — not every notification is about a task (a
        // future "ProjectCreated" notification wouldn't be). When this is
        // set, the frontend makes the notification clickable and jumps
        // straight to that task instead of just displaying a message.
        public int? RelatedTaskId { get; set; }

        // Navigation
        public User? User { get; set; }
        public ProjectTask? RelatedTask { get; set; }
    }
}