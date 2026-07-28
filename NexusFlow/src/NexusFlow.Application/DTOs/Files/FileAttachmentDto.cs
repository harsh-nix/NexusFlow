using System;

namespace NexusFlow.Application.DTOs.Files
{
    public class FileAttachmentDto
    {
        public int Id { get; set; }
        public string OriginalFileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSizeBytes { get; set; }
        public int TaskId { get; set; }
        public int UploadedBy { get; set; }
        public string UploadedByName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}