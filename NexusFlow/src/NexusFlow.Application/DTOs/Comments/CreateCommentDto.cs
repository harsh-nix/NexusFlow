using System;
using System.Collections.Generic;
using System.Text;

namespace NexusFlow.Application.DTOs.Comments
{
    public class CreateCommentDto
    {
        public string Content { get; set; } = string.Empty;
        public int TaskId { get; set; }
    }
}