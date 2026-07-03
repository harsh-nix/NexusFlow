using FluentValidation;
using NexusFlow.Application.DTOs.Tasks;

namespace NexusFlow.Application.Validators.Tasks
{
    public class CreateTaskDtoValidator : AbstractValidator<CreateTaskDto>
    {
        public CreateTaskDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Task title is required.")
                .MaximumLength(200).WithMessage("Task title cannot exceed 200 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters.")
                .When(x => x.Description != null);

            RuleFor(x => x.Priority)
                .IsInEnum().WithMessage("Priority must be a valid task priority.");

            RuleFor(x => x.ProjectId)
                .GreaterThan(0).WithMessage("A valid ProjectId is required.");

            RuleForEach(x => x.AssigneeIds)
                .GreaterThan(0).WithMessage("Assignee IDs must be valid user IDs.");
        }
    }
}