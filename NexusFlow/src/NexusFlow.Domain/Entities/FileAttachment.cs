namespace NexusFlow.Domain.Entities
{
    public class FileAttachment : BaseEntity
    {
        public string StoredFileName { get; set; } = string.Empty;
        public string OriginalFileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSizeBytes { get; set; }
        public int TaskId { get; set; }

        // Navigation
        public ProjectTask? Task { get; set; }
    }
}