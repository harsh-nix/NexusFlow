using NexusFlow.Application.DTOs.Comments;
using NexusFlow.Application.DTOs.Common;
using NexusFlow.Application.Services.Interfaces;
using NexusFlow.Domain.Entities;
using NexusFlow.Domain.Enums;
using NexusFlow.Domain.Interfaces;

namespace NexusFlow.Application.Services
{
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        private readonly IAuditLogService _auditLogService;

        public CommentService(
            IUnitOfWork unitOfWork,
            INotificationService notificationService,
            IAuditLogService auditLogService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _auditLogService = auditLogService;
        }

        public async Task<ApiResponse<CommentDto>> CreateAsync(
            CreateCommentDto dto, int userId)
        {
            var comment = new Comment
            {
                Content = dto.Content,
                TaskId = dto.TaskId,
                UserId = userId,
                Type = CommentType.Comment,
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
                CreatedAt = comment.CreatedAt,
                Type = comment.Type.ToString()
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
                    CreatedAt = comment.CreatedAt,
                    Type = comment.Type.ToString()
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

        public async Task<ApiResponse<CommentDto>> RequestClarificationAsync(
            int taskId, RequestClarificationDto dto, int userId)
        {
            var tasks = await _unitOfWork.Repository<ProjectTask>()
                .FindAsync(t => t.Id == taskId && !t.IsDeleted);
            var task = tasks.FirstOrDefault();

            if (task == null)
                return ApiResponse<CommentDto>.Fail("Task not found.", 404);

            var assignees = await _unitOfWork.Repository<TaskAssignee>()
                .FindAsync(a => a.TaskId == taskId && !a.IsDeleted);

            if (!assignees.Any(a => a.UserId == userId))
                return ApiResponse<CommentDto>.Fail(
                    "Only an assignee can request clarification.", 403);

            var comment = new Comment
            {
                Content = dto.Message,
                TaskId = taskId,
                UserId = userId,
                Type = CommentType.Clarification,
                CreatedBy = userId
            };

            await _unitOfWork.Repository<Comment>().AddAsync(comment);
            await _unitOfWork.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "ProjectTask", taskId, "ClarificationRequested", null, dto.Message, userId);

            if (task.CreatedBy != userId)
            {
                await _notificationService.CreateNotificationAsync(
                    task.CreatedBy,
                    "Clarification Requested",
                    $"A question was raised on \"{task.Title}\".",
                    NotificationType.ClarificationRequested,
                    taskId);
            }

            var users = await _unitOfWork.Repository<User>()
                .FindAsync(u => u.Id == userId);
            var user = users.FirstOrDefault();

            return ApiResponse<CommentDto>.Created(new CommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                TaskId = taskId,
                UserId = userId,
                UserName = user?.FullName ?? string.Empty,
                CreatedAt = comment.CreatedAt,
                Type = comment.Type.ToString()
            }, "Clarification requested.");
        }

        public async Task<ApiResponse<CommentDto>> RespondToClarificationAsync(
            int taskId, RespondClarificationDto dto, int userId)
        {
            var tasks = await _unitOfWork.Repository<ProjectTask>()
                .FindAsync(t => t.Id == taskId && !t.IsDeleted);
            var task = tasks.FirstOrDefault();

            if (task == null)
                return ApiResponse<CommentDto>.Fail("Task not found.", 404);

            var users = await _unitOfWork.Repository<User>()
                .FindAsync(u => u.Id == userId);
            var caller = users.FirstOrDefault();
            var isManager = caller?.Role == UserRole.Admin || caller?.Role == UserRole.ProjectManager;

            if (task.CreatedBy != userId && !isManager)
                return ApiResponse<CommentDto>.Fail(
                    "Only the task creator or a manager can respond.", 403);

            var comment = new Comment
            {
                Content = dto.Message,
                TaskId = taskId,
                UserId = userId,
                Type = CommentType.ClarificationResponse,
                CreatedBy = userId
            };

            await _unitOfWork.Repository<Comment>().AddAsync(comment);
            await _unitOfWork.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "ProjectTask", taskId, "ClarificationResponded", null, dto.Message, userId);

            // Notify every assignee (not just the one who originally asked) —
            // simplest correct behavior since clarifications aren't linked
            // to a specific requester in this table structure.
            var assignees = await _unitOfWork.Repository<TaskAssignee>()
                .FindAsync(a => a.TaskId == taskId && !a.IsDeleted);

            foreach (var assignee in assignees.Where(a => a.UserId != userId))
            {
                await _notificationService.CreateNotificationAsync(
                    assignee.UserId,
                    "Clarification Answered",
                    $"Your question on \"{task.Title}\" was answered.",
                    NotificationType.ClarificationResponded,
                    taskId);
            }

            return ApiResponse<CommentDto>.Created(new CommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                TaskId = taskId,
                UserId = userId,
                UserName = caller?.FullName ?? string.Empty,
                CreatedAt = comment.CreatedAt,
                Type = comment.Type.ToString()
            }, "Response sent.");
        }
    }
}