using FastEndpoints;
using FluentValidation;

namespace Server.Routes.BugReport.POST;

public class PostBugReportValidation : Validator<PostBugReportRequest>
{
    public PostBugReportValidation()
    {
        RuleFor(x => x.Details)
            .NotEmpty()
            .WithMessage("Details cannot be empty.")
            .MinimumLength(100)
            .WithMessage("Details must be at least 100 characters.")
            .MaximumLength(1000)
            .WithMessage("Details cannot exceed 1000 characters.");
    }
}