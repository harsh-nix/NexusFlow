using NexusFlow.Application.DTOs.Common;
using NexusFlow.Application.DTOs.Users;

namespace NexusFlow.Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<ApiResponse<List<UserDto>>> GetAllAsync();
    }
}