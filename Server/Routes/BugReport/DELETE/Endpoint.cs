using FastEndpoints;
using Server.Data;

namespace Server.Routes.BugReport.DELETE;

public class DeleteBugReportEndpoint(AppDbContext ctx) : EndpointWithoutRequest<BugReportMessageResponse>
{
    public override void Configure()
    {
        Delete("/report/bugs/{id}");
        Roles("Admin");
    }

    public override async Task<BugReportMessageResponse> ExecuteAsync(CancellationToken ct)
    {
        Guid reportId = Route<Guid>("id");

        DeleteBugReportData data = new(ctx);
        return await data.DeleteReportAsync(reportId, ct);
    }
}