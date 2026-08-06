using System.Collections.Generic;

namespace NexusFlow.Application.DTOs.AuditLogs
{
    public class PagedAuditLogResult
    {
        public List<AuditLogDto> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}