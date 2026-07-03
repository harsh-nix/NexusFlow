using FluentValidation;
using NexusFlow.Application.DTOs.Tasks;

namespace NexusFlow.Application.Validators.Tasks
{
    public class UpdateTaskDtoValidator : AbstractValidator<UpdateTaskDto>
    {
        public UpdateTaskDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Task title is required.")
                .MaximumLength(200).WithMessage("Task title cannot exceed 200 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters.")
                .When(x => x.Description != null);

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Status must be a valid task status.");

            RuleFor(x => x.Priority)
                .IsInEnum().WithMessage("Priority must be a valid task priority.");
        }
    }
}