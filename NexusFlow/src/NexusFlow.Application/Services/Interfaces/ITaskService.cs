using NexusFlow.Application.DTOs.Common;
using NexusFlow.Application.DTOs.Tasks;

namespace NexusFlow.Application.Services.Interfaces
{
    public interface ITaskService
    {
        Task<ApiResponse<TaskDto>> CreateAsync(CreateTaskDto dto, int userId);
        Task<ApiResponse<TaskDto>> GetByIdAsync(int id);
        Task<ApiResponse<List<TaskDto>>> GetByProjectAsync(int projectId);
        Task<ApiResponse<TaskDto>> UpdateAsync(int id, UpdateTaskDto dto, int userId);
        Task<ApiResponse<bool>> DeleteAsync(int id, int userId);
    }
}