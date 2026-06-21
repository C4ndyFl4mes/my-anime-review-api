using FastEndpoints;
using Server.Data;

namespace Server.Routes.UserReport.DELETE;

public class DeleteUserReportEndpoint(AppDbContext ctx) : EndpointWithoutRequest<UserReportResponse>
{
    public override void Configure()
    {
        Delete("/report/users/{id}");
        Roles("Admin");
    }

    public override async Task<UserReportResponse> ExecuteAsync(CancellationToken ct)
    {
        Guid reportId = Route<Guid>("id", isRequired: true);

        DeleteUserReportData data = new(ctx);
        return await data.DeleteUserReportAsync(reportId, ct);
    }
}