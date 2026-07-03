using AutoMapper;
using NexusFlow.Application.DTOs.Users;
using NexusFlow.Domain.Entities;

namespace NexusFlow.Application.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Role,
                    opt => opt.MapFrom(src => src.Role.ToString()));
        }
    }
}