using FastEndpoints;
using Server.Data;

namespace Server.Routes.BugReport.PUT;

public class ChangeStateEndpoint(AppDbContext ctx) : EndpointWithoutRequest<ChangeStateResponse>
{
    public override void Configure()
    {
        Put("/report/bugs/{id}");
        Roles("Admin");
    }

    public override async Task<ChangeStateResponse> ExecuteAsync(CancellationToken ct)
    {
        Guid reportId = Route<Guid>("id", isRequired: true);

        ChangeStateData data = new(ctx);
        return await data.ChangeStateAsync(reportId, ct);
    }
}