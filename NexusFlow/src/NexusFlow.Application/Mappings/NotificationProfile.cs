using AutoMapper;
using NexusFlow.Application.DTOs.Notifications;
using NexusFlow.Domain.Entities;

namespace NexusFlow.Application.Mappings
{
    public class NotificationProfile : Profile
    {
        public NotificationProfile()
        {
            CreateMap<Notification, NotificationDto>()
                .ForMember(dest => dest.Type,
                    opt => opt.MapFrom(src => src.Type.ToString()));
        }
    }
}