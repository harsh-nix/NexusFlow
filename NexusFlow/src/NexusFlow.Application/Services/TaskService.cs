using AutoMapper;
using NexusFlow.Application.DTOs.Common;
using NexusFlow.Application.DTOs.Tasks;
using NexusFlow.Application.Services.Interfaces;
using NexusFlow.Domain.Entities;
using NexusFlow.Domain.Enums;
using NexusFlow.Domain.Interfaces;
using TaskStatus = NexusFlow.Domain.Enums.TaskStatus;

namespace NexusFlow.Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        private readonly IAuditLogService _auditLogService;
        private readonly IMapper _mapper;

        public TaskService(
            IUnitOfWork unitOfWork,
            INotificationService notificationService,
            IAuditLogService auditLogService,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _auditLogService = auditLogService;
            _mapper = mapper;
        }

        public async Task<ApiResponse<TaskDto>> CreateAsync(
            CreateTaskDto dto, int userId)
        {
            var task = _mapper.Map<ProjectTask>(dto);
            task.Status = TaskStatus.Todo;
            task.CreatedBy = userId;

            await _unitOfWork.Repository<ProjectTask>().AddAsync(task);
            await _unitOfWork.SaveChangesAsync();

            foreach (var assigneeId in dto.AssigneeIds)
            {
                await _unitOfWork.Repository<TaskAssignee>().AddAsync(new TaskAssignee
                {
                    TaskId = task.Id,
                    UserId = assigneeId,
                    CreatedBy = userId
                });

                // relatedTaskId is what makes this notification clickable —
                // the assignee lands directly on this task instead of just
                // seeing a message with nowhere to go.
                await _notificationService.CreateNotificationAsync(
                    assigneeId,
                    "New Task Assigned",
                    $"You have been assigned to task: {task.Title}",
                    NotificationType.TaskAssigned,
                    task.Id);
            }

            if (dto.AssigneeIds.Any())
                await _unitOfWork.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "ProjectTask", task.Id, "Created", null, task.Title, userId);

            return ApiResponse<TaskDto>.Created(
                await EnrichAsync(_mapper.Map<TaskDto>(task), task),
                "Task created successfully.");
        }

        public async Task<ApiResponse<TaskDto>> GetByIdAsync(int id)
        {
            var tasks = await _unitOfWork.Repository<ProjectTask>()
                .FindAsync(t => t.Id == id && !t.IsDeleted);

            var task = tasks.FirstOrDefault();

            if (task == null)
                return ApiResponse<TaskDto>.Fail("Task not found.", 404);

            var dto = await EnrichAsync(_mapper.Map<TaskDto>(task), task);

            return ApiResponse<TaskDto>.Ok(dto);
        }

        public async Task<ApiResponse<List<TaskDto>>> GetByProjectAsync(int projectId)
        {
            var tasks = await _unitOfWork.Repository<ProjectTask>()
                .FindAsync(t => t.ProjectId == projectId && !t.IsDeleted);

            var taskList = tasks.ToList();
            var result = new List<TaskDto>();

            // Project name is the same for every task in this list, so look
            // it up once instead of once per task.
            var projects = await _unitOfWork.Repository<Project>()
                .FindAsync(p => p.Id == projectId);
            var projectName = projects.FirstOrDefault()?.Name ?? string.Empty;

            foreach (var task in taskList)
            {
                var dto = _mapper.Map<TaskDto>(task);
                dto.ProjectName = projectName;
                result.Add(await EnrichAsync(dto, task, skipProjectLookup: true));
            }

            return ApiResponse<List<TaskDto>>.Ok(result);
        }

        public async Task<ApiResponse<TaskDto>> UpdateAsync(
            int id, UpdateTaskDto dto, int userId)
        {
            var tasks = await _unitOfWork.Repository<ProjectTask>()
                .FindAsync(t => t.Id == id && !t.IsDeleted);

            var task = tasks.FirstOrDefault();

            if (task == null)
                return ApiResponse<TaskDto>.Fail("Task not found.", 404);

            _mapper.Map(dto, task);
            task.UpdatedBy = userId;

            _unitOfWork.Repository<ProjectTask>().Update(task);
            await _unitOfWork.SaveChangesAsync();

            var resultDto = await EnrichAsync(_mapper.Map<TaskDto>(task), task);

            return ApiResponse<TaskDto>.Ok(resultDto, "Task updated successfully.");
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id, int userId)
        {
            var tasks = await _unitOfWork.Repository<ProjectTask>()
                .FindAsync(t => t.Id == id && !t.IsDeleted);

            var task = tasks.FirstOrDefault();

            if (task == null)
                return ApiResponse<bool>.Fail("Task not found.", 404);

            task.IsDeleted = true;
            task.UpdatedBy = userId;

            _unitOfWork.Repository<ProjectTask>().Update(task);
            await _unitOfWork.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "ProjectTask", task.Id, "Deleted", null, null, userId);

            return ApiResponse<bool>.Ok(true, "Task deleted successfully.");
        }

        public async Task<ApiResponse<TaskDto>> AcceptTaskAsync(int id, int userId)
        {
            var tasks = await _unitOfWork.Repository<ProjectTask>()
                .FindAsync(t => t.Id == id && !t.IsDeleted);
            var task = tasks.FirstOrDefault();

            if (task == null)
                return ApiResponse<TaskDto>.Fail("Task not found.", 404);

            var assignees = await _unitOfWork.Repository<TaskAssignee>()
                .FindAsync(a => a.TaskId == id && !a.IsDeleted);

            if (!assignees.Any(a => a.UserId == userId))
                return ApiResponse<TaskDto>.Fail(
                    "Only an assignee can accept this task.", 403);

            if (task.Status != TaskStatus.Todo)
                return ApiResponse<TaskDto>.Fail(
                    "This task has already been accepted or moved past Todo.", 400);

            var oldStatus = task.Status.ToString();
            task.Status = TaskStatus.InProgress;
            task.UpdatedBy = userId;

            _unitOfWork.Repository<ProjectTask>().Update(task);
            await _unitOfWork.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "ProjectTask", task.Id, "Accepted", oldStatus, task.Status.ToString(), userId);

            if (task.CreatedBy != userId)
            {
                await _notificationService.CreateNotificationAsync(
                    task.CreatedBy,
                    "Task Accepted",
                    $"Your task \"{task.Title}\" was accepted and is now in progress.",
                    NotificationType.TaskUpdated,
                    task.Id);
            }

            var dto = await EnrichAsync(_mapper.Map<TaskDto>(task), task);
            return ApiResponse<TaskDto>.Ok(dto, "Task accepted.");
        }

        public async Task<ApiResponse<TaskDto>> UpdateStatusAsync(
            int id, UpdateTaskStatusDto dto, int userId)
        {
            var tasks = await _unitOfWork.Repository<ProjectTask>()
                .FindAsync(t => t.Id == id && !t.IsDeleted);
            var task = tasks.FirstOrDefault();

            if (task == null)
                return ApiResponse<TaskDto>.Fail("Task not found.", 404);

            if (!await CanManageTaskAsync(task, userId))
                return ApiResponse<TaskDto>.Fail(
                    "You don't have permission to update this task's status.", 403);

            var oldStatus = task.Status.ToString();
            task.Status = dto.Status;
            task.UpdatedBy = userId;
            _unitOfWork.Repository<ProjectTask>().Update(task);

            if (!string.IsNullOrWhiteSpace(dto.Note))
            {
                await _unitOfWork.Repository<Comment>().AddAsync(new Comment
                {
                    Content = dto.Note,
                    TaskId = id,
                    UserId = userId,
                    Type = CommentType.WorkLog,
                    CreatedBy = userId
                });
            }

            await _unitOfWork.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "ProjectTask", task.Id, "StatusChanged", oldStatus, dto.Status.ToString(), userId);

            if (task.CreatedBy != userId)
            {
                await _notificationService.CreateNotificationAsync(
                    task.CreatedBy,
                    "Task Status Updated",
                    $"\"{task.Title}\" is now {dto.Status}.",
                    NotificationType.TaskUpdated,
                    task.Id);
            }

            var resultDto = await EnrichAsync(_mapper.Map<TaskDto>(task), task);
            return ApiResponse<TaskDto>.Ok(resultDto, "Status updated.");
        }

        public async Task<ApiResponse<List<TaskActivityDto>>> GetActivityAsync(int id)
        {
            var activity = await _auditLogService.GetActivityForEntityAsync("ProjectTask", id);
            return ApiResponse<List<TaskActivityDto>>.Ok(activity);
        }

        // True if the caller is an assignee, the task's creator, or a
        // manager (Admin / ProjectManager). Used by any action that
        // changes a task's state, so a random authenticated user can't
        // act on a task they have nothing to do with.
        private async Task<bool> CanManageTaskAsync(ProjectTask task, int userId)
        {
            if (task.CreatedBy == userId) return true;

            var assignees = await _unitOfWork.Repository<TaskAssignee>()
                .FindAsync(a => a.TaskId == task.Id && !a.IsDeleted);
            if (assignees.Any(a => a.UserId == userId)) return true;

            var users = await _unitOfWork.Repository<User>()
                .FindAsync(u => u.Id == userId);
            var caller = users.FirstOrDefault();

            return caller?.Role == UserRole.Admin || caller?.Role == UserRole.ProjectManager;
        }

        /// <summary>
        /// Fills in the TaskDto fields AutoMapper deliberately leaves
        /// blank (ProjectName, AssigneeNames, SubTaskCount, CommentCount)
        /// because they need extra queries beyond the ProjectTask row itself.
        /// </summary>
        private async Task<TaskDto> EnrichAsync(
            TaskDto dto, ProjectTask task, bool skipProjectLookup = false)
        {
            if (!skipProjectLookup)
            {
                var projects = await _unitOfWork.Repository<Project>()
                    .FindAsync(p => p.Id == task.ProjectId);
                dto.ProjectName = projects.FirstOrDefault()?.Name ?? string.Empty;
            }

            var assignees = await _unitOfWork.Repository<TaskAssignee>()
                .FindAsync(a => a.TaskId == task.Id && !a.IsDeleted);
            var assigneeUserIds = assignees.Select(a => a.UserId).ToList();

            if (assigneeUserIds.Any())
            {
                var users = await _unitOfWork.Repository<User>()
                    .FindAsync(u => assigneeUserIds.Contains(u.Id));
                dto.AssigneeNames = users.Select(u => u.FullName).ToList();
            }

            var subTasks = await _unitOfWork.Repository<SubTask>()
                .FindAsync(s => s.ParentTaskId == task.Id && !s.IsDeleted);
            dto.SubTaskCount = subTasks.Count();

            // Only count plain discussion comments here — Clarification /
            // ClarificationResponse / WorkLog entries are part of the same
            // thread but shouldn't inflate the "X comments" badge on the
            // task card, since they're shown with their own distinct tags.
            var comments = await _unitOfWork.Repository<Comment>()
                .FindAsync(c => c.TaskId == task.Id && !c.IsDeleted
                    && c.Type == CommentType.Comment);
            dto.CommentCount = comments.Count();

            return dto;
        }
    }
}