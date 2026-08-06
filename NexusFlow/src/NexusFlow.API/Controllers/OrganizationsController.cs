using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusFlow.Application.DTOs.Organizations;
using NexusFlow.Application.Services.Interfaces;
using System.Security.Claims;

namespace NexusFlow.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class OrganizationsController : ControllerBase
    {
        private readonly IOrganizationService _organizationService;

        public OrganizationsController(IOrganizationService organizationService)
        {
            _organizationService = organizationService;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value
                ?? throw new UnauthorizedAccessException());

        [HttpGet]
        public async Task<IActionResult> GetAllOrganizations()
        {
            var result = await _organizationService.GetAllOrganizationsAsync();
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrganization([FromBody] CreateOrganizationDto dto)
        {
            var result = await _organizationService.CreateOrganizationAsync(dto, GetUserId());
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrganization(int id, [FromBody] UpdateOrganizationDto dto)
        {
            var result = await _organizationService.UpdateOrganizationAsync(id, dto, GetUserId());
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrganization(int id)
        {
            var result = await _organizationService.DeleteOrganizationAsync(id, GetUserId());
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{organizationId}/departments")]
        public async Task<IActionResult> GetDepartments(int organizationId)
        {
            var result = await _organizationService.GetDepartmentsAsync(organizationId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("{organizationId}/departments")]
        public async Task<IActionResult> CreateDepartment(
            int organizationId, [FromBody] CreateDepartmentDto dto)
        {
            var result = await _organizationService.CreateDepartmentAsync(organizationId, dto, GetUserId());
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("departments/{id}")]
        public async Task<IActionResult> UpdateDepartment(int id, [FromBody] UpdateDepartmentDto dto)
        {
            var result = await _organizationService.UpdateDepartmentAsync(id, dto, GetUserId());
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("departments/{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var result = await _organizationService.DeleteDepartmentAsync(id, GetUserId());
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("departments/{departmentId}/teams")]
        public async Task<IActionResult> GetTeams(int departmentId)
        {
            var result = await _organizationService.GetTeamsAsync(departmentId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("departments/{departmentId}/teams")]
        public async Task<IActionResult> CreateTeam(int departmentId, [FromBody] CreateTeamDto dto)
        {
            var result = await _organizationService.CreateTeamAsync(departmentId, dto, GetUserId());
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("teams/{id}")]
        public async Task<IActionResult> UpdateTeam(int id, [FromBody] UpdateTeamDto dto)
        {
            var result = await _organizationService.UpdateTeamAsync(id, dto, GetUserId());
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("teams/{id}")]
        public async Task<IActionResult> DeleteTeam(int id)
        {
            var result = await _organizationService.DeleteTeamAsync(id, GetUserId());
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("teams/{teamId}/members")]
        public async Task<IActionResult> AddTeamMember(int teamId, [FromBody] AddTeamMemberDto dto)
        {
            var result = await _organizationService.AddTeamMemberAsync(teamId, dto, GetUserId());
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("teams/members/{teamMemberId}")]
        public async Task<IActionResult> RemoveTeamMember(int teamMemberId)
        {
            var result = await _organizationService.RemoveTeamMemberAsync(teamMemberId, GetUserId());
            return StatusCode(result.StatusCode, result);
        }
    }
}