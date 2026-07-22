using System;
using System.Collections.Generic;
using System.Text;

using NexusFlow.Domain.Enums;

namespace NexusFlow.Domain.Entities
{
    public class Comment : BaseEntity
    {
        public string Content { get; set; } = string.Empty;
        public int TaskId { get; set; }
        public int UserId { get; set; }

        // Discriminates what kind of "note" this row is. Comment is a plain
        // discussion message; the other three are written by the new
        // task-workflow actions (RequestClarification / RespondToClarification /
        // UpdateStatus's optional work-log note) but stored in this same table
        // so the existing comment endpoint, service, and UI panel keep working
        // completely unchanged — the workflow actions just tag their entries
        // differently.
        public CommentType Type { get; set; } = CommentType.Comment;

        // Navigation
        public ProjectTask? Task { get; set; }
        public User? User { get; set; }
    }
}