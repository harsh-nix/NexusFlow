using NexusFlow.Application.DTOs.Common;
using NexusFlow.Application.DTOs.Tasks;

namespace NexusFlow.Application.Services.Interfaces
{
    public interface ISubTaskService
    {
        Task<ApiResponse<List<SubTaskDto>>> GetByTaskAsync(int taskId);
        Task<ApiResponse<SubTaskDto>> CreateAsync(int taskId, CreateSubTaskDto dto, int userId);
        Task<ApiResponse<SubTaskDto>> UpdateAsync(int id, UpdateSubTaskDto dto, int userId);
        Task<ApiResponse<bool>> DeleteAsync(int id, int userId);
    }
}