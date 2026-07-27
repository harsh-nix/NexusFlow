using AutoMapper;
using NexusFlow.Application.DTOs.Tasks;
using NexusFlow.Domain.Entities;

namespace NexusFlow.Application.Mappings
{
    public class SubTaskProfile : Profile
    {
        public SubTaskProfile()
        {
            CreateMap<SubTask, SubTaskDto>();
        }
    }
}