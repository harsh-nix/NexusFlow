using AutoMapper;
using NexusFlow.Application.DTOs.Common;
using NexusFlow.Application.DTOs.Users;
using NexusFlow.Application.Services.Interfaces;
using NexusFlow.Domain.Entities;
using NexusFlow.Domain.Interfaces;

namespace NexusFlow.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditLogService _auditLogService;
        private readonly IMapper _mapper;

        public UserService(
            IUnitOfWork unitOfWork,
            IAuditLogService auditLogService,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _auditLogService = auditLogService;
            _mapper = mapper;
        }

        public async Task<ApiResponse<List<UserDto>>> GetAllAsync()
        {
            var users = await _unitOfWork.Repository<User>()
                .FindAsync(u => !u.IsDeleted && u.IsActive);

            var result = _mapper.Map<List<UserDto>>(users.OrderBy(u => u.FullName));

            return ApiResponse<List<UserDto>>.Ok(result);
        }

        // Admin view — includes deactivated accounts, since re-activating
        // someone is the whole point of an admin user list.
        public async Task<ApiResponse<List<UserDto>>> GetAllForAdminAsync()
        {
            var users = await _unitOfWork.Repository<User>()
                .FindAsync(u => !u.IsDeleted);

            var result = _mapper.Map<List<UserDto>>(
                users.OrderBy(u => u.FullName));

            return ApiResponse<List<UserDto>>.Ok(result);
        }

        public async Task<ApiResponse<UserDto>> CreateAsync(
            CreateUserDto dto, int actingUserId)
        {
            var existing = await _unitOfWork.Repository<User>()
                .FindAsync(u => u.Email == dto.Email.ToLower().Trim() && !u.IsDeleted);

            if (existing.Any())
                return ApiResponse<UserDto>.Fail("Email already registered.", 400);

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email.ToLower().Trim(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role,
                PhoneNumber = dto.PhoneNumber,
                CreatedBy = actingUserId
            };

            await _unitOfWork.Repository<User>().AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "User", user.Id, "Created", null, user.Email, actingUserId);

            return ApiResponse<UserDto>.Created(
                _mapper.Map<UserDto>(user), "User created.");
        }

        public async Task<ApiResponse<UserDto>> UpdateAsync(
            int id, UpdateUserDto dto, int actingUserId)
        {
            var users = await _unitOfWork.Repository<User>()
                .FindAsync(u => u.Id == id && !u.IsDeleted);
            var user = users.FirstOrDefault();

            if (user == null)
                return ApiResponse<UserDto>.Fail("User not found.", 404);

            var oldRole = user.Role.ToString();

            user.FullName = dto.FullName;
            user.Role = dto.Role;
            user.PhoneNumber = dto.PhoneNumber;
            user.UpdatedBy = actingUserId;

            _unitOfWork.Repository<User>().Update(user);
            await _unitOfWork.SaveChangesAsync();

            if (oldRole != dto.Role.ToString())
            {
                await _auditLogService.LogAsync(
                    "User", user.Id, "RoleChanged", oldRole, dto.Role.ToString(), actingUserId);
            }

            return ApiResponse<UserDto>.Ok(
                _mapper.Map<UserDto>(user), "User updated.");
        }

        public async Task<ApiResponse<UserDto>> SetActiveStatusAsync(
            int id, bool isActive, int actingUserId)
        {
            var users = await _unitOfWork.Repository<User>()
                .FindAsync(u => u.Id == id && !u.IsDeleted);
            var user = users.FirstOrDefault();

            if (user == null)
                return ApiResponse<UserDto>.Fail("User not found.", 404);

            if (user.Id == actingUserId && !isActive)
                return ApiResponse<UserDto>.Fail(
                    "You cannot deactivate your own account.", 400);

            user.IsActive = isActive;
            user.UpdatedBy = actingUserId;

            _unitOfWork.Repository<User>().Update(user);
            await _unitOfWork.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "User", user.Id, isActive ? "Activated" : "Deactivated",
                (!isActive).ToString(), isActive.ToString(), actingUserId);

            return ApiResponse<UserDto>.Ok(
                _mapper.Map<UserDto>(user),
                isActive ? "User activated." : "User deactivated.");
        }
    }
}