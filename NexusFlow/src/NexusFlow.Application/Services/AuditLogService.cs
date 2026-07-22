using NexusFlow.Application.DTOs.Tasks;
using NexusFlow.Application.Services.Interfaces;
using NexusFlow.Domain.Entities;
using NexusFlow.Domain.Interfaces;

namespace NexusFlow.Application.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuditLogService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task LogAsync(
            string entityName, int entityId, string action,
            string? oldValue, string? newValue, int userId)
        {
            await _unitOfWork.Repository<AuditLog>().AddAsync(new AuditLog
            {
                EntityName = entityName,
                EntityId = entityId,
                Action = action,
                OldValue = oldValue,
                NewValue = newValue,
                UserId = userId,
                CreatedBy = userId
            });

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<TaskActivityDto>> GetActivityForEntityAsync(
            string entityName, int entityId)
        {
            var logs = await _unitOfWork.Repository<AuditLog>()
                .FindAsync(a => a.EntityName == entityName && a.EntityId == entityId);

            var result = new List<TaskActivityDto>();

            // Newest first, so the timeline reads top-to-bottom like a feed.
            foreach (var log in logs.OrderByDescending(l => l.CreatedAt))
            {
                var users = await _unitOfWork.Repository<User>()
                    .FindAsync(u => u.Id == log.UserId);

                result.Add(new TaskActivityDto
                {
                    Action = log.Action,
                    OldValue = log.OldValue,
                    NewValue = log.NewValue,
                    UserId = log.UserId,
                    UserName = users.FirstOrDefault()?.FullName ?? "Unknown",
                    CreatedAt = log.CreatedAt
                });
            }

            return result;
        }
    }
}