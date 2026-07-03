using AutoMapper;
using NexusFlow.Application.DTOs.Tasks;
using NexusFlow.Domain.Entities;

namespace NexusFlow.Application.Mappings
{
    public class TaskProfile : Profile
    {
        public TaskProfile()
        {
            CreateMap<ProjectTask, TaskDto>()
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Priority,
                    opt => opt.MapFrom(src => src.Priority.ToString()))
                .ForMember(dest => dest.ProjectName,
                    opt => opt.Ignore()) // set manually in service — requires Project include/lookup
                .ForMember(dest => dest.AssigneeNames,
                    opt => opt.Ignore()) // set manually in service — requires Assignees include/lookup
                .ForMember(dest => dest.SubTaskCount,
                    opt => opt.Ignore()) // set manually in service — requires separate query
                .ForMember(dest => dest.CommentCount,
                    opt => opt.Ignore()); // set manually in service — requires separate query

            CreateMap<CreateTaskDto, ProjectTask>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.Project, opt => opt.Ignore())
                .ForMember(dest => dest.SubTasks, opt => opt.Ignore())
                .ForMember(dest => dest.Assignees, opt => opt.Ignore())
                .ForMember(dest => dest.Comments, opt => opt.Ignore());

            CreateMap<UpdateTaskDto, ProjectTask>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ProjectId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.Project, opt => opt.Ignore())
                .ForMember(dest => dest.SubTasks, opt => opt.Ignore())
                .ForMember(dest => dest.Assignees, opt => opt.Ignore())
                .ForMember(dest => dest.Comments, opt => opt.Ignore());
        }
    }
}