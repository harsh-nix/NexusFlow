using NexusFlow.Application.DTOs.Common;
using NexusFlow.Application.DTOs.Tasks;

namespace NexusFlow.Application.Services.Interfaces
{
    public interface ITaskService
    {
        Task<ApiResponse<TaskDto>> CreateAsync(CreateTaskDto dto, int userId);
        Task<ApiResponse<TaskDto>> GetByIdAsync(int id);
        Task<ApiResponse<List<TaskDto>>> GetByProjectAsync(int projectId);
        Task<ApiResponse<List<TaskDto>>> GetMyTasksAsync(int userId);
        Task<ApiResponse<TaskDto>> UpdateAsync(int id, UpdateTaskDto dto, int userId);
        Task<ApiResponse<bool>> DeleteAsync(int id, int userId);

        // Assignee acknowledges the task and it moves Todo -> InProgress.
        // Only an actual assignee may call this.
        Task<ApiResponse<TaskDto>> AcceptTaskAsync(int id, int userId);

        // Dedicated status-change path (separate from the full edit form)
        // so every change is ownership-checked, audit-logged, and can carry
        // an optional work-log note.
        Task<ApiResponse<TaskDto>> UpdateStatusAsync(int id, UpdateTaskStatusDto dto, int userId);

        Task<ApiResponse<List<TaskActivityDto>>> GetActivityAsync(int id);
    }
}