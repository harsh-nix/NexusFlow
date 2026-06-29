using System;
using System.Collections.Generic;
using System.Text;

using NexusFlow.Application.DTOs.Auth;
using NexusFlow.Application.DTOs.Common;

namespace NexusFlow.Application.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterRequestDto request);
        Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginRequestDto request);
        Task<ApiResponse<AuthResponseDto>> RefreshTokenAsync(string refreshToken);
        Task<ApiResponse<bool>> LogoutAsync(string refreshToken);
    }
}