using System;
using System.Collections.Generic;
using System.Text;

using NexusFlow.Application.DTOs.Common;
using NexusFlow.Application.DTOs.Projects;

namespace NexusFlow.Application.Services.Interfaces
{
    public interface IProjectService
    {
        Task<ApiResponse<ProjectDto>> CreateAsync(CreateProjectDto dto, int userId);
        Task<ApiResponse<ProjectDto>> GetByIdAsync(int id);
        Task<ApiResponse<List<ProjectDto>>> GetAllAsync(int userId);
        Task<ApiResponse<ProjectDto>> UpdateAsync(int id, UpdateProjectDto dto, int userId);
        Task<ApiResponse<bool>> DeleteAsync(int id, int userId);
    }
}