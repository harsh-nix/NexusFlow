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
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResponse<List<UserDto>>> GetAllAsync()
        {
            var users = await _unitOfWork.Repository<User>()
                .FindAsync(u => !u.IsDeleted && u.IsActive);

            var result = _mapper.Map<List<UserDto>>(users.OrderBy(u => u.FullName));

            return ApiResponse<List<UserDto>>.Ok(result);
        }
    }
}   