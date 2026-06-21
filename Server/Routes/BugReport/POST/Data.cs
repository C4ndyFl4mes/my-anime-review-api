using Server.Data;
using Server.Entities;
using Server.Enums;

namespace Server.Routes.BugReport.POST;

public class PostBugReportData(AppDbContext ctx)
{
    public async Task<BugReportMessageResponse> PostBugAsync(PostBugReportRequest request, CancellationToken ct)
    {
        ReportedBugEntity reportedBug = new()
        {
            Id = Guid.NewGuid(),
            State = BugState.Pending,
            Details = request.Details,
            CreatedAt = DateTime.UtcNow
        };

        await ctx.ReportedBugs.AddAsync(reportedBug, ct);

        await ctx.SaveChangesAsync(ct);

        return new()
        {
            Message = "You've successfully reported a bug."
        };
    }
}