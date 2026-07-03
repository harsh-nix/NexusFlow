using FluentValidation;
using NexusFlow.Application.DTOs.Comments;

namespace NexusFlow.Application.Validators.Comments
{
    public class CreateCommentDtoValidator : AbstractValidator<CreateCommentDto>
    {
        public CreateCommentDtoValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Comment content is required.")
                .MaximumLength(1000).WithMessage("Comment cannot exceed 1000 characters.");

            RuleFor(x => x.TaskId)
                .GreaterThan(0).WithMessage("A valid TaskId is required.");
        }
    }
}
