using FastEndpoints;
using Server.Data;

namespace Server.Routes.UserReport.GET;

public class GetUserReportsEndpoint(AppDbContext ctx) : EndpointWithoutRequest<GetUserReportsResponse>
{
    public override void Configure()
    {
        Get("/report/users");
        Roles("Admin");
    }

    public override async Task<GetUserReportsResponse> ExecuteAsync(CancellationToken ct)
    {
        int page = Query<int>("page", isRequired: false);
        if (page < 1)
            page = 1;

        GetUserReportsData data = new(ctx);
        return await data.GetUserReportsAsync(page, ct);
    }
}