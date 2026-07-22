namespace NexusFlow.Application.DTOs.Notifications
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public string Type { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // When set, the frontend makes the notification clickable and
        // navigates straight to this task instead of just marking it read.
        public int? RelatedTaskId { get; set; }
    }
}