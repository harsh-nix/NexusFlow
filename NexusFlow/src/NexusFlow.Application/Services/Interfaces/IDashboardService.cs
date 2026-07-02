using NexusFlow.Application.DTOs.Common;
using NexusFlow.Application.DTOs.Dashboard;

namespace NexusFlow.Application.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<ApiResponse<DashboardStatsDto>> GetStatsAsync(int userId);
    }
}