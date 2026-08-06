using FluentValidation;
using NexusFlow.Application.DTOs.Organizations;

namespace NexusFlow.Application.Validators.Organizations
{
    public class CreateOrganizationDtoValidator : AbstractValidator<CreateOrganizationDto>
    {
        public CreateOrganizationDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Organization name is required.")
                .MaximumLength(150).WithMessage("Organization name cannot exceed 150 characters.");

            RuleFor(x => x.Website)
                .MaximumLength(255)
                .When(x => !string.IsNullOrWhiteSpace(x.Website));
        }
    }

    public class UpdateOrganizationDtoValidator : AbstractValidator<UpdateOrganizationDto>
    {
        public UpdateOrganizationDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Organization name is required.")
                .MaximumLength(150).WithMessage("Organization name cannot exceed 150 characters.");

            RuleFor(x => x.Website)
                .MaximumLength(255)
                .When(x => !string.IsNullOrWhiteSpace(x.Website));
        }
    }
}