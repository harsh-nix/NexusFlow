using NexusFlow.Application.DTOs.Common;
using NexusFlow.Application.DTOs.Users;

namespace NexusFlow.Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<ApiResponse<List<UserDto>>> GetAllAsync();
        Task<ApiResponse<List<UserDto>>> GetAllForAdminAsync();
        Task<ApiResponse<UserDto>> CreateAsync(CreateUserDto dto, int actingUserId);
        Task<ApiResponse<UserDto>> UpdateAsync(int id, UpdateUserDto dto, int actingUserId);
        Task<ApiResponse<UserDto>> SetActiveStatusAsync(int id, bool isActive, int actingUserId);
    }
}