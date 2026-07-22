using FluentValidation;
using NexusFlow.Application.DTOs.Tasks;

namespace NexusFlow.Application.Validators.Tasks
{
    public class UpdateTaskStatusDtoValidator : AbstractValidator<UpdateTaskStatusDto>
    {
        public UpdateTaskStatusDtoValidator()
        {
            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Status must be a valid task status.");

            RuleFor(x => x.Note)
                .MaximumLength(2000).WithMessage("Note cannot exceed 2000 characters.")
                .When(x => x.Note != null);
        }
    }
}