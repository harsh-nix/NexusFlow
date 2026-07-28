using FluentValidation;
using NexusFlow.Application.DTOs.Users;

namespace NexusFlow.Application.Validators.Users
{
    public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
    {
        public UpdateUserDtoValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(150).WithMessage("Full name cannot exceed 150 characters.");

            RuleFor(x => x.Role)
                .IsInEnum().WithMessage("Role must be a valid user role.");

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\+?[0-9\s-]{7,15}$")
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber))
                .WithMessage("Phone number format is invalid.");
        }
    }
}