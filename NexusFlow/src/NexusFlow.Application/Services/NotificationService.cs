using NexusFlow.Application.DTOs.Common;
using NexusFlow.Application.DTOs.Notifications;
using NexusFlow.Application.Services.Interfaces;
using NexusFlow.Domain.Entities;
using NexusFlow.Domain.Enums;
using NexusFlow.Domain.Interfaces;

namespace NexusFlow.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotificationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CreateNotificationAsync(
            int userId, string title, string message, NotificationType type,
            int? relatedTaskId = null)
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
                IsRead = false,
                RelatedTaskId = relatedTaskId,
                CreatedBy = userId
            };

            await _unitOfWork.Repository<Notification>().AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<ApiResponse<List<NotificationDto>>> GetByUserAsync(
            int userId)
        {
            var notifications = await _unitOfWork.Repository<Notification>()
                .FindAsync(n => n.UserId == userId && !n.IsDeleted);

            var result = notifications
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new NotificationDto
                {
                    Id = n.Id,
                    Title = n.Title,
                    Message = n.Message,
                    IsRead = n.IsRead,
                    Type = n.Type.ToString(),
                    CreatedAt = n.CreatedAt,
                    RelatedTaskId = n.RelatedTaskId
                })
                .ToList();

            return ApiResponse<List<NotificationDto>>.Ok(result);
        }

        public async Task<ApiResponse<bool>> MarkAsReadAsync(int id, int userId)
        {
            var notifications = await _unitOfWork.Repository<Notification>()
                .FindAsync(n => n.Id == id && n.UserId == userId);

            var notification = notifications.FirstOrDefault();

            if (notification == null)
                return ApiResponse<bool>.Fail("Notification not found.", 404);

            notification.IsRead = true;
            _unitOfWork.Repository<Notification>().Update(notification);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.Ok(true, "Marked as read.");
        }

        public async Task<ApiResponse<bool>> MarkAllAsReadAsync(int userId)
        {
            var notifications = await _unitOfWork.Repository<Notification>()
                .FindAsync(n => n.UserId == userId && !n.IsRead);

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                _unitOfWork.Repository<Notification>().Update(notification);
            }

            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.Ok(true, "All marked as read.");
        }
    }
}