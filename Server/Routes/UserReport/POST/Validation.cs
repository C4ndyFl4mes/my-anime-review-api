using FastEndpoints;
using FluentValidation;

namespace Server.Routes.UserReport.POST;

public class PostUserReportValidation : Validator<PostUserReportRequest>
{
    public PostUserReportValidation()
    {
        RuleFor(x => x.ReportedUserId)
            .Must(x => Guid.TryParse(Convert.ToString(x), out _))
            .WithMessage("Invalid GUID format for ReportedUserId.");
        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Reason is required.")
            .MinimumLength(100)
            .WithMessage("Reason must be at least 100 characters.")
            .MaximumLength(1000)
            .WithMessage("Reason cannot exceed 1000 characters.");
    }
}