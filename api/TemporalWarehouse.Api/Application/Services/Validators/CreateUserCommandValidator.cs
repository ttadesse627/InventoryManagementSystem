


using FluentValidation;
using TemporalWarehouse.Api.Contracts.RequestDtos;

namespace TemporalWarehouse.Api.Application.Services.Validators;

public class CreateUserCommandValidator : AbstractValidator<RegisterRequest>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email is required.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.");

        RuleFor(x => x.Roles)
            .NotEmpty().WithMessage("User role is required.");
    }
}