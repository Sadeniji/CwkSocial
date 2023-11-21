using CwkSocial.Domain.Aggregates.UserProfileAggregate;
using FluentValidation;

namespace CwkSocial.Domain.Validators.UserProfileValidator;

public class BasicInfoValidator : AbstractValidator<BasicInfo>
{
    public BasicInfoValidator()
    {
        RuleFor(info => info.FirstName)
            .NotNull().WithMessage("First name is required. It cannot be null")
            .NotEmpty().WithMessage("First name is required. It cannot be empty")
            .MinimumLength(3).WithMessage("First name must be at least 3  characters long")
            .MaximumLength(100).WithMessage("First name must be at most 100  characters long");

        RuleFor(info => info.LastName)
            .NotNull().WithMessage("Last name is required. It cannot be null")
            .NotEmpty().WithMessage("Last name is required. It cannot be empty")
            .MinimumLength(3).WithMessage("Last name must be at least 3  characters long")
            .MaximumLength(100).WithMessage("Last name must be at most 100  characters long");

        RuleFor(info => info.EmailAddress)
            .NotNull().WithMessage("Email address is required. It cannot be null")
            .NotEmpty().WithMessage("Email address is required. It cannot be empty")
            .EmailAddress().WithMessage("Provided string is not a correct email format");

        RuleFor(info => info.DateOfBirth)
            .InclusiveBetween(new DateTime(DateTime.Now.AddYears(-125).Ticks),
                new DateTime(DateTime.Now.AddYears(-18).Ticks))
            .WithMessage("Age needs to be between 18 and 125");
    }
}