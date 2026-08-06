using FluentValidation;
using NexusFlow.Application.DTOs.Organizations;

namespace NexusFlow.Application.Validators.Organizations
{
    public class CreateTeamDtoValidator : AbstractValidator<CreateTeamDto>
    {
        public CreateTeamDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Team name is required.")
                .MaximumLength(150).WithMessage("Team name cannot exceed 150 characters.");
        }
    }

    public class UpdateTeamDtoValidator : AbstractValidator<UpdateTeamDto>
    {
        public UpdateTeamDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Team name is required.")
                .MaximumLength(150).WithMessage("Team name cannot exceed 150 characters.");
        }
    }

    public class AddTeamMemberDtoValidator : AbstractValidator<AddTeamMemberDto>
    {
        public AddTeamMemberDtoValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("A valid user must be selected.");
        }
    }
}