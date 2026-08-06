using FluentValidation;
using NexusFlow.Application.DTOs.Organizations;

namespace NexusFlow.Application.Validators.Organizations
{
    public class CreateDepartmentDtoValidator : AbstractValidator<CreateDepartmentDto>
    {
        public CreateDepartmentDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Department name is required.")
                .MaximumLength(150).WithMessage("Department name cannot exceed 150 characters.");
        }
    }

    public class UpdateDepartmentDtoValidator : AbstractValidator<UpdateDepartmentDto>
    {
        public UpdateDepartmentDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Department name is required.")
                .MaximumLength(150).WithMessage("Department name cannot exceed 150 characters.");
        }
    }
}