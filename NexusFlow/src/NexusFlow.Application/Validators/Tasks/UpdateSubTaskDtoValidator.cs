using FluentValidation;
using NexusFlow.Application.DTOs.Tasks;

namespace NexusFlow.Application.Validators.Tasks
{
    public class UpdateSubTaskDtoValidator : AbstractValidator<UpdateSubTaskDto>
    {
        public UpdateSubTaskDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Sub-task title is required.")
                .MaximumLength(200).WithMessage("Sub-task title cannot exceed 200 characters.");
        }
    }
}