using System;
using System.Collections.Generic;
using System.Text;

namespace NexusFlow.Application.DTOs.Comments
{
    public class CommentDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public int TaskId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // "Comment" | "Clarification" | "ClarificationResponse" | "WorkLog"
        public string Type { get; set; } = "Comment";
    }
}