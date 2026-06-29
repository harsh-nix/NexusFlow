using System;
using System.Collections.Generic;
using System.Text;

using NexusFlow.Application.DTOs.Common;
using NexusFlow.Application.DTOs.Projects;
using NexusFlow.Application.Services.Interfaces;
using NexusFlow.Domain.Entities;
using NexusFlow.Domain.Interfaces;

namespace NexusFlow.Application.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProjectService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<ProjectDto>> CreateAsync(
            CreateProjectDto dto, int userId)
        {
            var project = new Project
            {
                Name = dto.Name,
                Description = dto.Description,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                OrganizationId = dto.OrganizationId,
                CreatedBy = userId
            };

            await _unitOfWork.Repository<Project>().AddAsync(project);
            await _unitOfWork.SaveChangesAsync();

            // Add creator as project member
            var member = new ProjectMember
            {
                ProjectId = project.Id,
                UserId = userId,
                Role = Domain.Enums.UserRole.ProjectManager,
                CreatedBy = userId
            };

            await _unitOfWork.Repository<ProjectMember>().AddAsync(member);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<ProjectDto>.Created(
                MapToDto(project), "Project created successfully.");
        }

        public async Task<ApiResponse<ProjectDto>> GetByIdAsync(int id)
        {
            var projects = await _unitOfWork.Repository<Project>()
                .FindAsync(p => p.Id == id && !p.IsDeleted);

            var project = projects.FirstOrDefault();

            if (project == null)
                return ApiResponse<ProjectDto>.Fail("Project not found.", 404);

            var members = await _unitOfWork.Repository<ProjectMember>()
                .FindAsync(m => m.ProjectId == id);

            var tasks = await _unitOfWork.Repository<ProjectTask>()
                .FindAsync(t => t.ProjectId == id && !t.IsDeleted);

            var dto = MapToDto(project);
            dto.MemberCount = members.Count();
            dto.TaskCount = tasks.Count();

            return ApiResponse<ProjectDto>.Ok(dto);
        }

        public async Task<ApiResponse<List<ProjectDto>>> GetAllAsync(int userId)
        {
            // Get projects where user is a member
            var memberships = await _unitOfWork.Repository<ProjectMember>()
                .FindAsync(m => m.UserId == userId);

            var projectIds = memberships.Select(m => m.ProjectId).ToList();

            var projects = await _unitOfWork.Repository<Project>()
                .FindAsync(p => projectIds.Contains(p.Id) && !p.IsDeleted);

            var result = projects.Select(MapToDto).ToList();

            return ApiResponse<List<ProjectDto>>.Ok(result);
        }

        public async Task<ApiResponse<ProjectDto>> UpdateAsync(
            int id, UpdateProjectDto dto, int userId)
        {
            var projects = await _unitOfWork.Repository<Project>()
                .FindAsync(p => p.Id == id && !p.IsDeleted);

            var project = projects.FirstOrDefault();

            if (project == null)
                return ApiResponse<ProjectDto>.Fail("Project not found.", 404);

            project.Name = dto.Name;
            project.Description = dto.Description;
            project.Status = dto.Status;
            project.StartDate = dto.StartDate;
            project.EndDate = dto.EndDate;
            project.UpdatedBy = userId;

            _unitOfWork.Repository<Project>().Update(project);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<ProjectDto>.Ok(
                MapToDto(project), "Project updated successfully.");
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id, int userId)
        {
            var projects = await _unitOfWork.Repository<Project>()
                .FindAsync(p => p.Id == id && !p.IsDeleted);

            var project = projects.FirstOrDefault();

            if (project == null)
                return ApiResponse<bool>.Fail("Project not found.", 404);

            project.IsDeleted = true;
            project.UpdatedBy = userId;

            _unitOfWork.Repository<Project>().Update(project);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.Ok(true, "Project deleted successfully.");
        }

        private static ProjectDto MapToDto(Project project) => new()
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            Status = project.Status.ToString(),
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            OrganizationId = project.OrganizationId,
            CreatedBy = project.CreatedBy,
            CreatedAt = project.CreatedAt
        };
    }
}