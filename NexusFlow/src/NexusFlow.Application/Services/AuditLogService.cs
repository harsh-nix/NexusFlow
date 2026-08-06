using NexusFlow.Application.DTOs.Tasks;
using NexusFlow.Application.Services.Interfaces;
using NexusFlow.Domain.Entities;
using NexusFlow.Domain.Interfaces;
using NexusFlow.Application.DTOs.AuditLogs;

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
        public async Task<PagedAuditLogResult> GetAllAsync(
            string? entityName, string? action, int page, int pageSize)
        {
            var logs = await _unitOfWork.Repository<AuditLog>().GetAllAsync();

            var filtered = logs.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(entityName))
                filtered = filtered.Where(a =>
                    a.EntityName.Equals(entityName, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(action))
                filtered = filtered.Where(a =>
                    a.Action.Equals(action, StringComparison.OrdinalIgnoreCase));

            var ordered = filtered.OrderByDescending(a => a.CreatedAt).ToList();
            var totalCount = ordered.Count;

            var pageItems = ordered
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var items = new List<AuditLogDto>();
            foreach (var log in pageItems)
            {
                var users = await _unitOfWork.Repository<User>()
                    .FindAsync(u => u.Id == log.UserId);

                items.Add(new AuditLogDto
                {
                    Id = log.Id,
                    EntityName = log.EntityName,
                    EntityId = log.EntityId,
                    Action = log.Action,
                    OldValue = log.OldValue,
                    NewValue = log.NewValue,
                    UserId = log.UserId,
                    UserName = users.FirstOrDefault()?.FullName ?? "Unknown",
                    CreatedAt = log.CreatedAt
                });
            }

            return new PagedAuditLogResult
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }
    }
}