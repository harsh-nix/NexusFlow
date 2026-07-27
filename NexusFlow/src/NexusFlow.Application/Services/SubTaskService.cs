using AutoMapper;
using NexusFlow.Application.DTOs.Common;
using NexusFlow.Application.DTOs.Tasks;
using NexusFlow.Application.Services.Interfaces;
using NexusFlow.Domain.Entities;
using NexusFlow.Domain.Enums;
using NexusFlow.Domain.Interfaces;

namespace NexusFlow.Application.Services
{
    public class SubTaskService : ISubTaskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditLogService _auditLogService;
        private readonly IMapper _mapper;

        public SubTaskService(
            IUnitOfWork unitOfWork,
            IAuditLogService auditLogService,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _auditLogService = auditLogService;
            _mapper = mapper;
        }

        public async Task<ApiResponse<List<SubTaskDto>>> GetByTaskAsync(int taskId)
        {
            var subTasks = await _unitOfWork.Repository<SubTask>()
                .FindAsync(s => s.ParentTaskId == taskId && !s.IsDeleted);

            var result = subTasks
                .OrderBy(s => s.CreatedAt)
                .Select(s => _mapper.Map<SubTaskDto>(s))
                .ToList();

            return ApiResponse<List<SubTaskDto>>.Ok(result);
        }

        public async Task<ApiResponse<SubTaskDto>> CreateAsync(
            int taskId, CreateSubTaskDto dto, int userId)
        {
            var tasks = await _unitOfWork.Repository<ProjectTask>()
                .FindAsync(t => t.Id == taskId && !t.IsDeleted);
            var task = tasks.FirstOrDefault();

            if (task == null)
                return ApiResponse<SubTaskDto>.Fail("Task not found.", 404);

            if (!await CanManageAsync(task, userId))
                return ApiResponse<SubTaskDto>.Fail(
                    "You are not authorized to add sub-tasks to this task.", 403);

            var subTask = new SubTask
            {
                Title = dto.Title,
                ParentTaskId = taskId,
                CreatedBy = userId
            };

            await _unitOfWork.Repository<SubTask>().AddAsync(subTask);
            await _unitOfWork.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "ProjectTask", taskId, "SubTaskAdded", null, subTask.Title, userId);

            return ApiResponse<SubTaskDto>.Created(
                _mapper.Map<SubTaskDto>(subTask), "Sub-task added.");
        }

        public async Task<ApiResponse<SubTaskDto>> UpdateAsync(
            int id, UpdateSubTaskDto dto, int userId)
        {
            var subTasks = await _unitOfWork.Repository<SubTask>()
                .FindAsync(s => s.Id == id && !s.IsDeleted);
            var subTask = subTasks.FirstOrDefault();

            if (subTask == null)
                return ApiResponse<SubTaskDto>.Fail("Sub-task not found.", 404);

            var parentTasks = await _unitOfWork.Repository<ProjectTask>()
                .FindAsync(t => t.Id == subTask.ParentTaskId && !t.IsDeleted);
            var task = parentTasks.FirstOrDefault();

            if (task == null)
                return ApiResponse<SubTaskDto>.Fail("Parent task not found.", 404);

            if (!await CanManageAsync(task, userId))
                return ApiResponse<SubTaskDto>.Fail(
                    "You are not authorized to update this sub-task.", 403);

            var wasCompleted = subTask.IsCompleted;

            subTask.Title = dto.Title;
            subTask.IsCompleted = dto.IsCompleted;
            subTask.UpdatedBy = userId;

            _unitOfWork.Repository<SubTask>().Update(subTask);
            await _unitOfWork.SaveChangesAsync();

            if (wasCompleted != dto.IsCompleted)
            {
                await _auditLogService.LogAsync(
                    "ProjectTask", task.Id,
                    dto.IsCompleted ? "SubTaskCompleted" : "SubTaskReopened",
                    wasCompleted.ToString(), dto.IsCompleted.ToString(), userId);
            }

            return ApiResponse<SubTaskDto>.Ok(
                _mapper.Map<SubTaskDto>(subTask), "Sub-task updated.");
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id, int userId)
        {
            var subTasks = await _unitOfWork.Repository<SubTask>()
                .FindAsync(s => s.Id == id && !s.IsDeleted);
            var subTask = subTasks.FirstOrDefault();

            if (subTask == null)
                return ApiResponse<bool>.Fail("Sub-task not found.", 404);

            var parentTasks = await _unitOfWork.Repository<ProjectTask>()
                .FindAsync(t => t.Id == subTask.ParentTaskId && !t.IsDeleted);
            var task = parentTasks.FirstOrDefault();

            if (task == null)
                return ApiResponse<bool>.Fail("Parent task not found.", 404);

            if (!await CanManageAsync(task, userId))
                return ApiResponse<bool>.Fail(
                    "You are not authorized to delete this sub-task.", 403);

            subTask.IsDeleted = true;
            subTask.UpdatedBy = userId;

            _unitOfWork.Repository<SubTask>().Update(subTask);
            await _unitOfWork.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "ProjectTask", task.Id, "SubTaskRemoved", subTask.Title, null, userId);

            return ApiResponse<bool>.Ok(true, "Sub-task removed.");
        }

        private async Task<bool> CanManageAsync(ProjectTask task, int userId)
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
    }
}