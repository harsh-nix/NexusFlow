using NexusFlow.Application.DTOs.Comments;
using NexusFlow.Application.DTOs.Common;

namespace NexusFlow.Application.Services.Interfaces
{
    public interface ICommentService
    {
        Task<ApiResponse<CommentDto>> CreateAsync(CreateCommentDto dto, int userId);
        Task<ApiResponse<List<CommentDto>>> GetByTaskAsync(int taskId);
        Task<ApiResponse<bool>> DeleteAsync(int id, int userId);

        // Assignee asks a question about the task. Only an assignee may call this.
        Task<ApiResponse<CommentDto>> RequestClarificationAsync(
            int taskId, RequestClarificationDto dto, int userId);

        // Task creator or a manager answers. Notifies every assignee.
        Task<ApiResponse<CommentDto>> RespondToClarificationAsync(
            int taskId, RespondClarificationDto dto, int userId);
    }
}