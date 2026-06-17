using FastEndpoints;
using FluentValidation;

namespace Server.Routes.Review.POST;

public class PostReviewValidation : Validator<ReviewPostRequest>
{
    public PostReviewValidation()
    {
        RuleFor(x => x.Text)
            .NotEmpty().WithMessage("A review must have content.")
            .MinimumLength(100).WithMessage("A review must have at least 100 characters.")
            .MaximumLength(4000).WithMessage("A review cannot be longer than 4000 characters.");
        
        RuleFor(x => x.Score)
            .InclusiveBetween(1, 10).WithMessage("Score must be between 1 and 10.");
    }
}