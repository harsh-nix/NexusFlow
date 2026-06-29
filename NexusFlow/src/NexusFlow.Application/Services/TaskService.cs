using System;
using System.Collections.Generic;
using System.Text;

using NexusFlow.Application.DTOs.Common;
using NexusFlow.Application.DTOs.Tasks;
using NexusFlow.Application.Services.Interfaces;
using NexusFlow.Domain.Entities;
using NexusFlow.Domain.Interfaces;
using TaskStatus = NexusFlow.Domain.Enums.TaskStatus;

namespace NexusFlow.Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TaskService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<TaskDto>> CreateAsync(
            CreateTaskDto dto, int userId)
        {
            var task = new ProjectTask
            {
                Title = dto.Title,
                Description = dto.Description,
                Priority = dto.Priority,
                DueDate = dto.DueDate,
                ProjectId = dto.ProjectId,
                Status = TaskStatus.Todo,
                CreatedBy = userId
            };

            await _unitOfWork.Repository<ProjectTask>().AddAsync(task);
            await _unitOfWork.SaveChangesAsync();

            // Assign users
            foreach (var assigneeId in dto.AssigneeIds)
            {
                await _unitOfWork.Repository<TaskAssignee>().AddAsync(new TaskAssignee
                {
                    TaskId = task.Id,
                    UserId = assigneeId,
                    CreatedBy = userId
                });
            }

            if (dto.AssigneeIds.Any())
                await _unitOfWork.SaveChangesAsync();

            return ApiResponse<TaskDto>.Created(
                MapToDto(task), "Task created successfully.");
        }

        public async Task<ApiResponse<TaskDto>> GetByIdAsync(int id)
        {
            var tasks = await _unitOfWork.Repository<ProjectTask>()
                .FindAsync(t => t.Id == id && !t.IsDeleted);

            var task = tasks.FirstOrDefault();

            if (task == null)
                return ApiResponse<TaskDto>.Fail("Task not found.", 404);

            return ApiResponse<TaskDto>.Ok(MapToDto(task));
        }

        public async Task<ApiResponse<List<TaskDto>>> GetByProjectAsync(int projectId)
        {
            var tasks = await _unitOfWork.Repository<ProjectTask>()
                .FindAsync(t => t.ProjectId == projectId && !t.IsDeleted);

            var result = tasks.Select(MapToDto).ToList();

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

            task.Title = dto.Title;
            task.Description = dto.Description;
            task.Status = dto.Status;
            task.Priority = dto.Priority;
            task.DueDate = dto.DueDate;
            task.UpdatedBy = userId;

            _unitOfWork.Repository<ProjectTask>().Update(task);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<TaskDto>.Ok(
                MapToDto(task), "Task updated successfully.");
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

        private static TaskDto MapToDto(ProjectTask task) => new()
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status.ToString(),
            Priority = task.Priority.ToString(),
            DueDate = task.DueDate,
            ProjectId = task.ProjectId,
            CreatedBy = task.CreatedBy,
            CreatedAt = task.CreatedAt
        };
    }
}