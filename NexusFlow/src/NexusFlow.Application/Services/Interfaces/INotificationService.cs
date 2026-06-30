using NexusFlow.Application.DTOs.Common;
using NexusFlow.Application.DTOs.Notifications;
using NexusFlow.Domain.Enums;

namespace NexusFlow.Application.Services.Interfaces
{
    public interface INotificationService
    {
        Task CreateNotificationAsync(
            int userId, string title, string message, NotificationType type);
        Task<ApiResponse<List<NotificationDto>>> GetByUserAsync(int userId);
        Task<ApiResponse<bool>> MarkAsReadAsync(int id, int userId);
        Task<ApiResponse<bool>> MarkAllAsReadAsync(int userId);
    }
}