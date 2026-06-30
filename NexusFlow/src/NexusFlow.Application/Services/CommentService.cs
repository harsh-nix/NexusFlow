using System;
using System.Collections.Generic;
using System.Text;

using NexusFlow.Application.DTOs.Comments;
using NexusFlow.Application.DTOs.Common;
using NexusFlow.Application.Services.Interfaces;
using NexusFlow.Domain.Entities;
using NexusFlow.Domain.Interfaces;

namespace NexusFlow.Application.Services
{
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CommentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<CommentDto>> CreateAsync(
            CreateCommentDto dto, int userId)
        {
            var comment = new Comment
            {
                Content = dto.Content,
                TaskId = dto.TaskId,
                UserId = userId,
                CreatedBy = userId
            };

            await _unitOfWork.Repository<Comment>().AddAsync(comment);
            await _unitOfWork.SaveChangesAsync();

            var users = await _unitOfWork.Repository<User>()
                .FindAsync(u => u.Id == userId);
            var user = users.FirstOrDefault();

            return ApiResponse<CommentDto>.Created(new CommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                TaskId = comment.TaskId,
                UserId = comment.UserId,
                UserName = user?.FullName ?? string.Empty,
                CreatedAt = comment.CreatedAt
            }, "Comment added successfully.");
        }

        public async Task<ApiResponse<List<CommentDto>>> GetByTaskAsync(int taskId)
        {
            var comments = await _unitOfWork.Repository<Comment>()
                .FindAsync(c => c.TaskId == taskId && !c.IsDeleted);

            var result = new List<CommentDto>();

            foreach (var comment in comments.OrderBy(c => c.CreatedAt))
            {
                var users = await _unitOfWork.Repository<User>()
                    .FindAsync(u => u.Id == comment.UserId);
                var user = users.FirstOrDefault();

                result.Add(new CommentDto
                {
                    Id = comment.Id,
                    Content = comment.Content,
                    TaskId = comment.TaskId,
                    UserId = comment.UserId,
                    UserName = user?.FullName ?? string.Empty,
                    CreatedAt = comment.CreatedAt
                });
            }

            return ApiResponse<List<CommentDto>>.Ok(result);
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id, int userId)
        {
            var comments = await _unitOfWork.Repository<Comment>()
                .FindAsync(c => c.Id == id && !c.IsDeleted);

            var comment = comments.FirstOrDefault();

            if (comment == null)
                return ApiResponse<bool>.Fail("Comment not found.", 404);

            comment.IsDeleted = true;
            comment.UpdatedBy = userId;

            _unitOfWork.Repository<Comment>().Update(comment);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.Ok(true, "Comment deleted.");
        }
    }
}