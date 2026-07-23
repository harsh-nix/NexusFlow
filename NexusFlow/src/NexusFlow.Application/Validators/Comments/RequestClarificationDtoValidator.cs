using FluentValidation;
using NexusFlow.Application.DTOs.Comments;

namespace NexusFlow.Application.Validators.Comments
{
    public class RequestClarificationDtoValidator : AbstractValidator<RequestClarificationDto>
    {
        public RequestClarificationDtoValidator()
        {
            RuleFor(x => x.Message)
                .NotEmpty().WithMessage("Please describe what needs clarification.")
                .MaximumLength(1000).WithMessage("Message cannot exceed 1000 characters.");
        }
    }
}