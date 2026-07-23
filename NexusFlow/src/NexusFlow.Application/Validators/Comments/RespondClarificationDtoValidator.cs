using FluentValidation;
using NexusFlow.Application.DTOs.Comments;

namespace NexusFlow.Application.Validators.Comments
{
    public class RespondClarificationDtoValidator : AbstractValidator<RespondClarificationDto>
    {
        public RespondClarificationDtoValidator()
        {
            RuleFor(x => x.Message)
                .NotEmpty().WithMessage("Response cannot be empty.")
                .MaximumLength(1000).WithMessage("Response cannot exceed 1000 characters.");
        }
    }
}