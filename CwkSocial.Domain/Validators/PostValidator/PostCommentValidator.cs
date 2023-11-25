using CwkSocial.Domain.Aggregates.PostAggregate;
using FluentValidation;

namespace CwkSocial.Domain.Validators.PostValidator;

public class PostCommentValidator : AbstractValidator<PostComment>
{
    public PostCommentValidator()
    {
        RuleFor(pc => pc.Text)
            .NotNull().WithMessage("Comment text should not be null")
            .NotEmpty().WithMessage("Comment text should not be empty")
            .MaximumLength(1000)
            .MinimumLength(1);

    }
}