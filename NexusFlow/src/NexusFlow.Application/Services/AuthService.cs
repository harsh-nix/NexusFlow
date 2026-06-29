using System;
using System.Collections.Generic;
using System.Text;

using NexusFlow.Application.DTOs.Auth;
using NexusFlow.Application.DTOs.Common;
using NexusFlow.Application.Services.Interfaces;
using NexusFlow.Domain.Entities;
using NexusFlow.Domain.Enums;
using NexusFlow.Domain.Interfaces;

namespace NexusFlow.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthService(
            IUnitOfWork unitOfWork,
            IJwtTokenService jwtTokenService)
        {
            _unitOfWork = unitOfWork;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<ApiResponse<AuthResponseDto>> RegisterAsync(
            RegisterRequestDto request)
        {
            // Check if email already exists
            var existingUsers = await _unitOfWork
                .Repository<User>()
                .FindAsync(u => u.Email == request.Email && !u.IsDeleted);

            if (existingUsers.Any())
                return ApiResponse<AuthResponseDto>
                    .Fail("Email already registered.");

            // Validate passwords match
            if (request.Password != request.ConfirmPassword)
                return ApiResponse<AuthResponseDto>
                    .Fail("Passwords do not match.");

            // Create user
            var user = new User
            {
                FullName = request.FullName,
                Email = request.Email.ToLower().Trim(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                PhoneNumber = request.PhoneNumber,
                Role = UserRole.TeamMember,
                CreatedBy = 1
            };

            await _unitOfWork.Repository<User>().AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // Generate tokens
            var accessToken = _jwtTokenService.GenerateAccessToken(user);
            var refreshToken = _jwtTokenService.GenerateRefreshToken();

            // Save refresh token
            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedBy = user.Id
            };

            await _unitOfWork.Repository<RefreshToken>()
                .AddAsync(refreshTokenEntity);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<AuthResponseDto>.Created(new AuthResponseDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.ToString(),
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiry = _jwtTokenService.GetAccessTokenExpiry()
            }, "Registration successful.");
        }

        public async Task<ApiResponse<AuthResponseDto>> LoginAsync(
            LoginRequestDto request)
        {
            // Find user by email
            var users = await _unitOfWork
                .Repository<User>()
                .FindAsync(u => u.Email == request.Email.ToLower().Trim()
                             && !u.IsDeleted);

            var user = users.FirstOrDefault();

            if (user == null)
                return ApiResponse<AuthResponseDto>
                    .Fail("Invalid email or password.");

            if (!user.IsActive)
                return ApiResponse<AuthResponseDto>
                    .Fail("Account is deactivated. Contact admin.");

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return ApiResponse<AuthResponseDto>
                    .Fail("Invalid email or password.");

            // Generate tokens
            var accessToken = _jwtTokenService.GenerateAccessToken(user);
            var refreshToken = _jwtTokenService.GenerateRefreshToken();

            // Revoke old refresh tokens
            var oldTokens = await _unitOfWork
                .Repository<RefreshToken>()
                .FindAsync(t => t.UserId == user.Id && !t.IsRevoked);

            foreach (var token in oldTokens)
            {
                token.IsRevoked = true;
                _unitOfWork.Repository<RefreshToken>().Update(token);
            }

            // Save new refresh token
            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedBy = user.Id
            };

            await _unitOfWork.Repository<RefreshToken>()
                .AddAsync(refreshTokenEntity);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<AuthResponseDto>.Ok(new AuthResponseDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.ToString(),
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiry = _jwtTokenService.GetAccessTokenExpiry()
            }, "Login successful.");
        }

        public async Task<ApiResponse<AuthResponseDto>> RefreshTokenAsync(
            string refreshToken)
        {
            var tokens = await _unitOfWork
                .Repository<RefreshToken>()
                .FindAsync(t => t.Token == refreshToken
                             && !t.IsRevoked
                             && t.ExpiresAt > DateTime.UtcNow);

            var tokenEntity = tokens.FirstOrDefault();

            if (tokenEntity == null)
                return ApiResponse<AuthResponseDto>
                    .Fail("Invalid or expired refresh token.", 401);

            var users = await _unitOfWork
                .Repository<User>()
                .FindAsync(u => u.Id == tokenEntity.UserId);

            var user = users.FirstOrDefault();

            if (user == null)
                return ApiResponse<AuthResponseDto>
                    .Fail("User not found.", 404);

            // Revoke old token
            tokenEntity.IsRevoked = true;
            _unitOfWork.Repository<RefreshToken>().Update(tokenEntity);

            // Generate new tokens
            var newAccessToken = _jwtTokenService.GenerateAccessToken(user);
            var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

            await _unitOfWork.Repository<RefreshToken>().AddAsync(new RefreshToken
            {
                UserId = user.Id,
                Token = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedBy = user.Id
            });

            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<AuthResponseDto>.Ok(new AuthResponseDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.ToString(),
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                AccessTokenExpiry = _jwtTokenService.GetAccessTokenExpiry()
            }, "Token refreshed.");
        }

        public async Task<ApiResponse<bool>> LogoutAsync(string refreshToken)
        {
            var tokens = await _unitOfWork
                .Repository<RefreshToken>()
                .FindAsync(t => t.Token == refreshToken && !t.IsRevoked);

            var tokenEntity = tokens.FirstOrDefault();

            if (tokenEntity == null)
                return ApiResponse<bool>.Fail("Token not found.");

            tokenEntity.IsRevoked = true;
            _unitOfWork.Repository<RefreshToken>().Update(tokenEntity);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.Ok(true, "Logged out successfully.");
        }
    }
}