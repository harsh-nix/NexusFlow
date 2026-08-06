using NexusFlow.Application.DTOs.Common;
using NexusFlow.Application.DTOs.Organizations;

namespace NexusFlow.Application.Services.Interfaces
{
    public interface IOrganizationService
    {
        Task<ApiResponse<List<OrganizationDto>>> GetAllOrganizationsAsync();
        Task<ApiResponse<OrganizationDto>> CreateOrganizationAsync(CreateOrganizationDto dto, int userId);
        Task<ApiResponse<OrganizationDto>> UpdateOrganizationAsync(int id, UpdateOrganizationDto dto, int userId);
        Task<ApiResponse<bool>> DeleteOrganizationAsync(int id, int userId);

        Task<ApiResponse<List<DepartmentDto>>> GetDepartmentsAsync(int organizationId);
        Task<ApiResponse<DepartmentDto>> CreateDepartmentAsync(int organizationId, CreateDepartmentDto dto, int userId);
        Task<ApiResponse<DepartmentDto>> UpdateDepartmentAsync(int id, UpdateDepartmentDto dto, int userId);
        Task<ApiResponse<bool>> DeleteDepartmentAsync(int id, int userId);

        Task<ApiResponse<List<TeamDto>>> GetTeamsAsync(int departmentId);
        Task<ApiResponse<TeamDto>> CreateTeamAsync(int departmentId, CreateTeamDto dto, int userId);
        Task<ApiResponse<TeamDto>> UpdateTeamAsync(int id, UpdateTeamDto dto, int userId);
        Task<ApiResponse<bool>> DeleteTeamAsync(int id, int userId);

        Task<ApiResponse<TeamMemberDto>> AddTeamMemberAsync(int teamId, AddTeamMemberDto dto, int userId);
        Task<ApiResponse<bool>> RemoveTeamMemberAsync(int teamMemberId, int userId);
    }
}