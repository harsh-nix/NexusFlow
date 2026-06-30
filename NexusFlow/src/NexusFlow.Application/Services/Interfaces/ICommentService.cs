using System;
using System.Collections.Generic;
using System.Text;

using NexusFlow.Application.DTOs.Comments;
using NexusFlow.Application.DTOs.Common;

namespace NexusFlow.Application.Services.Interfaces
{
    public interface ICommentService
    {
        Task<ApiResponse<CommentDto>> CreateAsync(CreateCommentDto dto, int userId);
        Task<ApiResponse<List<CommentDto>>> GetByTaskAsync(int taskId);
        Task<ApiResponse<bool>> DeleteAsync(int id, int userId);
    }
}