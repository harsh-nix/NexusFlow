using System;
using System.Collections.Generic;
using System.Text;

namespace NexusFlow.Domain.Enums
{
    public enum NotificationType
    {
        TaskAssigned = 1,
        TaskUpdated = 2,
        ProjectCreated = 3,
        CommentAdded = 4,
        DeadlineApproaching = 5,
        MentionedInComment = 6,
        ClarificationRequested = 7,
        ClarificationResponded = 8
    }
}