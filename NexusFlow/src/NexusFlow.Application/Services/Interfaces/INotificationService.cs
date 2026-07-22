using NexusFlow.Application.DTOs.Common;
using NexusFlow.Application.DTOs.Notifications;
using NexusFlow.Domain.Enums;

namespace NexusFlow.Application.Services.Interfaces
{
    public interface INotificationService
    {
        // relatedTaskId defaults to null so every existing call site keeps
        // compiling unchanged; callers that ARE about a specific task pass
        // it in to make the notification clickable on the frontend.
        Task CreateNotificationAsync(
            int userId, string title, string message, NotificationType type,
            int? relatedTaskId = null);
        Task<ApiResponse<List<NotificationDto>>> GetByUserAsync(int userId);
        Task<ApiResponse<bool>> MarkAsReadAsync(int id, int userId);
        Task<ApiResponse<bool>> MarkAllAsReadAsync(int userId);
    }
}