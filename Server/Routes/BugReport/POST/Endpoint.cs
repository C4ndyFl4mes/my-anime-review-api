using FastEndpoints;
using Server.Data;

namespace Server.Routes.BugReport.POST;

public class PostBugReportEndpoint(AppDbContext ctx) : Endpoint<PostBugReportRequest, BugReportMessageResponse>
{
    public override void Configure()
    {
        Post("/report/bugs");
        Roles("User", "Admin");
    }

    public override async Task<BugReportMessageResponse> ExecuteAsync(PostBugReportRequest request, CancellationToken ct)
    {
        PostBugReportData data = new(ctx);
        return await data.PostBugAsync(request, ct);
    }
}