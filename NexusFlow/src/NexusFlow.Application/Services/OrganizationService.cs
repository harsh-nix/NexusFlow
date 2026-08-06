using NexusFlow.Application.DTOs.Common;
using NexusFlow.Application.DTOs.Organizations;
using NexusFlow.Application.Services.Interfaces;
using NexusFlow.Domain.Entities;
using NexusFlow.Domain.Interfaces;

namespace NexusFlow.Application.Services
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditLogService _auditLogService;

        public OrganizationService(IUnitOfWork unitOfWork, IAuditLogService auditLogService)
        {
            _unitOfWork = unitOfWork;
            _auditLogService = auditLogService;
        }

        // ---------- Organizations ----------

        public async Task<ApiResponse<List<OrganizationDto>>> GetAllOrganizationsAsync()
        {
            var orgs = await _unitOfWork.Repository<Organization>()
                .FindAsync(o => !o.IsDeleted);

            var result = new List<OrganizationDto>();
            foreach (var org in orgs.OrderBy(o => o.Name))
            {
                result.Add(await ToOrgDtoAsync(org));
            }

            return ApiResponse<List<OrganizationDto>>.Ok(result);
        }

        public async Task<ApiResponse<OrganizationDto>> CreateOrganizationAsync(
            CreateOrganizationDto dto, int userId)
        {
            var org = new Organization
            {
                Name = dto.Name,
                Description = dto.Description,
                Website = dto.Website,
                CreatedBy = userId
            };

            await _unitOfWork.Repository<Organization>().AddAsync(org);
            await _unitOfWork.SaveChangesAsync();

            await _auditLogService.LogAsync("Organization", org.Id, "Created", null, org.Name, userId);

            return ApiResponse<OrganizationDto>.Created(await ToOrgDtoAsync(org), "Organization created.");
        }

        public async Task<ApiResponse<OrganizationDto>> UpdateOrganizationAsync(
            int id, UpdateOrganizationDto dto, int userId)
        {
            var orgs = await _unitOfWork.Repository<Organization>()
                .FindAsync(o => o.Id == id && !o.IsDeleted);
            var org = orgs.FirstOrDefault();

            if (org == null)
                return ApiResponse<OrganizationDto>.Fail("Organization not found.", 404);

            org.Name = dto.Name;
            org.Description = dto.Description;
            org.Website = dto.Website;
            org.UpdatedBy = userId;

            _unitOfWork.Repository<Organization>().Update(org);
            await _unitOfWork.SaveChangesAsync();

            await _auditLogService.LogAsync("Organization", org.Id, "Updated", null, org.Name, userId);

            return ApiResponse<OrganizationDto>.Ok(await ToOrgDtoAsync(org), "Organization updated.");
        }

        public async Task<ApiResponse<bool>> DeleteOrganizationAsync(int id, int userId)
        {
            var orgs = await _unitOfWork.Repository<Organization>()
                .FindAsync(o => o.Id == id && !o.IsDeleted);
            var org = orgs.FirstOrDefault();

            if (org == null)
                return ApiResponse<bool>.Fail("Organization not found.", 404);

            org.IsDeleted = true;
            org.UpdatedBy = userId;
            _unitOfWork.Repository<Organization>().Update(org);
            await _unitOfWork.SaveChangesAsync();

            await _auditLogService.LogAsync("Organization", org.Id, "Deleted", null, null, userId);

            return ApiResponse<bool>.Ok(true, "Organization deleted.");
        }

        // ---------- Departments ----------

        public async Task<ApiResponse<List<DepartmentDto>>> GetDepartmentsAsync(int organizationId)
        {
            var departments = await _unitOfWork.Repository<Department>()
                .FindAsync(d => d.OrganizationId == organizationId && !d.IsDeleted);

            var result = new List<DepartmentDto>();
            foreach (var dept in departments.OrderBy(d => d.Name))
            {
                result.Add(await ToDeptDtoAsync(dept));
            }

            return ApiResponse<List<DepartmentDto>>.Ok(result);
        }

        public async Task<ApiResponse<DepartmentDto>> CreateDepartmentAsync(
            int organizationId, CreateDepartmentDto dto, int userId)
        {
            var orgs = await _unitOfWork.Repository<Organization>()
                .FindAsync(o => o.Id == organizationId && !o.IsDeleted);

            if (!orgs.Any())
                return ApiResponse<DepartmentDto>.Fail("Organization not found.", 404);

            var dept = new Department
            {
                Name = dto.Name,
                Description = dto.Description,
                OrganizationId = organizationId,
                CreatedBy = userId
            };

            await _unitOfWork.Repository<Department>().AddAsync(dept);
            await _unitOfWork.SaveChangesAsync();

            await _auditLogService.LogAsync("Department", dept.Id, "Created", null, dept.Name, userId);

            return ApiResponse<DepartmentDto>.Created(await ToDeptDtoAsync(dept), "Department created.");
        }

        public async Task<ApiResponse<DepartmentDto>> UpdateDepartmentAsync(
            int id, UpdateDepartmentDto dto, int userId)
        {
            var departments = await _unitOfWork.Repository<Department>()
                .FindAsync(d => d.Id == id && !d.IsDeleted);
            var dept = departments.FirstOrDefault();

            if (dept == null)
                return ApiResponse<DepartmentDto>.Fail("Department not found.", 404);

            dept.Name = dto.Name;
            dept.Description = dto.Description;
            dept.UpdatedBy = userId;

            _unitOfWork.Repository<Department>().Update(dept);
            await _unitOfWork.SaveChangesAsync();

            await _auditLogService.LogAsync("Department", dept.Id, "Updated", null, dept.Name, userId);

            return ApiResponse<DepartmentDto>.Ok(await ToDeptDtoAsync(dept), "Department updated.");
        }

        public async Task<ApiResponse<bool>> DeleteDepartmentAsync(int id, int userId)
        {
            var departments = await _unitOfWork.Repository<Department>()
                .FindAsync(d => d.Id == id && !d.IsDeleted);
            var dept = departments.FirstOrDefault();

            if (dept == null)
                return ApiResponse<bool>.Fail("Department not found.", 404);

            dept.IsDeleted = true;
            dept.UpdatedBy = userId;
            _unitOfWork.Repository<Department>().Update(dept);
            await _unitOfWork.SaveChangesAsync();

            await _auditLogService.LogAsync("Department", dept.Id, "Deleted", null, null, userId);

            return ApiResponse<bool>.Ok(true, "Department deleted.");
        }

        // ---------- Teams ----------

        public async Task<ApiResponse<List<TeamDto>>> GetTeamsAsync(int departmentId)
        {
            var teams = await _unitOfWork.Repository<Team>()
                .FindAsync(t => t.DepartmentId == departmentId && !t.IsDeleted);

            var result = new List<TeamDto>();
            foreach (var team in teams.OrderBy(t => t.Name))
            {
                result.Add(await ToTeamDtoAsync(team));
            }

            return ApiResponse<List<TeamDto>>.Ok(result);
        }

        public async Task<ApiResponse<TeamDto>> CreateTeamAsync(
            int departmentId, CreateTeamDto dto, int userId)
        {
            var departments = await _unitOfWork.Repository<Department>()
                .FindAsync(d => d.Id == departmentId && !d.IsDeleted);

            if (!departments.Any())
                return ApiResponse<TeamDto>.Fail("Department not found.", 404);

            var team = new Team
            {
                Name = dto.Name,
                Description = dto.Description,
                DepartmentId = departmentId,
                CreatedBy = userId
            };

            await _unitOfWork.Repository<Team>().AddAsync(team);
            await _unitOfWork.SaveChangesAsync();

            await _auditLogService.LogAsync("Team", team.Id, "Created", null, team.Name, userId);

            return ApiResponse<TeamDto>.Created(await ToTeamDtoAsync(team), "Team created.");
        }

        public async Task<ApiResponse<TeamDto>> UpdateTeamAsync(int id, UpdateTeamDto dto, int userId)
        {
            var teams = await _unitOfWork.Repository<Team>()
                .FindAsync(t => t.Id == id && !t.IsDeleted);
            var team = teams.FirstOrDefault();

            if (team == null)
                return ApiResponse<TeamDto>.Fail("Team not found.", 404);

            team.Name = dto.Name;
            team.Description = dto.Description;
            team.UpdatedBy = userId;

            _unitOfWork.Repository<Team>().Update(team);
            await _unitOfWork.SaveChangesAsync();

            await _auditLogService.LogAsync("Team", team.Id, "Updated", null, team.Name, userId);

            return ApiResponse<TeamDto>.Ok(await ToTeamDtoAsync(team), "Team updated.");
        }

        public async Task<ApiResponse<bool>> DeleteTeamAsync(int id, int userId)
        {
            var teams = await _unitOfWork.Repository<Team>()
                .FindAsync(t => t.Id == id && !t.IsDeleted);
            var team = teams.FirstOrDefault();

            if (team == null)
                return ApiResponse<bool>.Fail("Team not found.", 404);

            team.IsDeleted = true;
            team.UpdatedBy = userId;
            _unitOfWork.Repository<Team>().Update(team);
            await _unitOfWork.SaveChangesAsync();

            await _auditLogService.LogAsync("Team", team.Id, "Deleted", null, null, userId);

            return ApiResponse<bool>.Ok(true, "Team deleted.");
        }

        // ---------- Team Members ----------

        public async Task<ApiResponse<TeamMemberDto>> AddTeamMemberAsync(
            int teamId, AddTeamMemberDto dto, int userId)
        {
            var teams = await _unitOfWork.Repository<Team>()
                .FindAsync(t => t.Id == teamId && !t.IsDeleted);

            if (!teams.Any())
                return ApiResponse<TeamMemberDto>.Fail("Team not found.", 404);

            var existing = await _unitOfWork.Repository<TeamMember>()
                .FindAsync(m => m.TeamId == teamId && m.UserId == dto.UserId && !m.IsDeleted);

            if (existing.Any())
                return ApiResponse<TeamMemberDto>.Fail("User is already on this team.", 400);

            var member = new TeamMember
            {
                TeamId = teamId,
                UserId = dto.UserId,
                JoinedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            await _unitOfWork.Repository<TeamMember>().AddAsync(member);
            await _unitOfWork.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "Team", teamId, "MemberAdded", null, dto.UserId.ToString(), userId);

            var users = await _unitOfWork.Repository<User>().FindAsync(u => u.Id == dto.UserId);
            var user = users.FirstOrDefault();

            return ApiResponse<TeamMemberDto>.Created(new TeamMemberDto
            {
                Id = member.Id,
                UserId = member.UserId,
                UserName = user?.FullName ?? "Unknown",
                UserEmail = user?.Email ?? "",
                JoinedAt = member.JoinedAt
            }, "Member added.");
        }

        public async Task<ApiResponse<bool>> RemoveTeamMemberAsync(int teamMemberId, int userId)
        {
            var members = await _unitOfWork.Repository<TeamMember>()
                .FindAsync(m => m.Id == teamMemberId && !m.IsDeleted);
            var member = members.FirstOrDefault();

            if (member == null)
                return ApiResponse<bool>.Fail("Team member not found.", 404);

            member.IsDeleted = true;
            member.UpdatedBy = userId;
            _unitOfWork.Repository<TeamMember>().Update(member);
            await _unitOfWork.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "Team", member.TeamId, "MemberRemoved", member.UserId.ToString(), null, userId);

            return ApiResponse<bool>.Ok(true, "Member removed.");
        }

        // ---------- Helpers ----------

        private async Task<OrganizationDto> ToOrgDtoAsync(Organization org)
        {
            var departments = await _unitOfWork.Repository<Department>()
                .FindAsync(d => d.OrganizationId == org.Id && !d.IsDeleted);
            var users = await _unitOfWork.Repository<User>()
                .FindAsync(u => u.OrganizationId == org.Id && !u.IsDeleted);

            return new OrganizationDto
            {
                Id = org.Id,
                Name = org.Name,
                Description = org.Description,
                Website = org.Website,
                DepartmentCount = departments.Count(),
                UserCount = users.Count(),
                CreatedAt = org.CreatedAt
            };
        }

        private async Task<DepartmentDto> ToDeptDtoAsync(Department dept)
        {
            var teams = await _unitOfWork.Repository<Team>()
                .FindAsync(t => t.DepartmentId == dept.Id && !t.IsDeleted);
            var orgs = await _unitOfWork.Repository<Organization>()
                .FindAsync(o => o.Id == dept.OrganizationId);

            return new DepartmentDto
            {
                Id = dept.Id,
                Name = dept.Name,
                Description = dept.Description,
                OrganizationId = dept.OrganizationId,
                OrganizationName = orgs.FirstOrDefault()?.Name ?? "",
                TeamCount = teams.Count(),
                CreatedAt = dept.CreatedAt
            };
        }

        private async Task<TeamDto> ToTeamDtoAsync(Team team)
        {
            var members = await _unitOfWork.Repository<TeamMember>()
                .FindAsync(m => m.TeamId == team.Id && !m.IsDeleted);
            var departments = await _unitOfWork.Repository<Department>()
                .FindAsync(d => d.Id == team.DepartmentId);

            var memberDtos = new List<TeamMemberDto>();
            foreach (var member in members)
            {
                var users = await _unitOfWork.Repository<User>()
                    .FindAsync(u => u.Id == member.UserId);
                var user = users.FirstOrDefault();

                memberDtos.Add(new TeamMemberDto
                {
                    Id = member.Id,
                    UserId = member.UserId,
                    UserName = user?.FullName ?? "Unknown",
                    UserEmail = user?.Email ?? "",
                    JoinedAt = member.JoinedAt
                });
            }

            return new TeamDto
            {
                Id = team.Id,
                Name = team.Name,
                Description = team.Description,
                DepartmentId = team.DepartmentId,
                DepartmentName = departments.FirstOrDefault()?.Name ?? "",
                Members = memberDtos,
                CreatedAt = team.CreatedAt
            };
        }
    }
}