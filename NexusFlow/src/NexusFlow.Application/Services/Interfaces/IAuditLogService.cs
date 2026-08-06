using NexusFlow.Application.DTOs.AuditLogs;
using NexusFlow.Application.DTOs.Tasks;

namespace NexusFlow.Application.Services.Interfaces
{
    public interface IAuditLogService
    {
        Task LogAsync(
            string entityName, int entityId, string action,
            string? oldValue, string? newValue, int userId);

        Task<List<TaskActivityDto>> GetActivityForEntityAsync(
            string entityName, int entityId);

        Task<PagedAuditLogResult> GetAllAsync(
            string? entityName, string? action, int page, int pageSize);
    }
}