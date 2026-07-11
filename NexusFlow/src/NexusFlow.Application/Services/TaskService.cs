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
        private readonly IMapper _mapper;

        public TaskService(
            IUnitOfWork unitOfWork,
            INotificationService notificationService,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
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

                await _notificationService.CreateNotificationAsync(
                    assigneeId,
                    "New Task Assigned",
                    $"You have been assigned to task: {task.Title}",
                    NotificationType.TaskAssigned);
            }

            if (dto.AssigneeIds.Any())
                await _unitOfWork.SaveChangesAsync();

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

            return ApiResponse<bool>.Ok(true, "Task deleted successfully.");
        }

        /// <summary>
        /// Fills in the four TaskDto fields AutoMapper deliberately leaves
        /// blank (ProjectName, AssigneeNames, SubTaskCount, CommentCount)
        /// because they need extra queries beyond the ProjectTask row itself.
        /// This was a known, flagged gap since the AutoMapper wiring — it's
        /// what was causing comment counts to reset to 0 on reload.
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

            var comments = await _unitOfWork.Repository<Comment>()
                .FindAsync(c => c.TaskId == task.Id && !c.IsDeleted);
            dto.CommentCount = comments.Count();

            return dto;
        }
    }
}