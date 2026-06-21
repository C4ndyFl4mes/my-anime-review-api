using FastEndpoints;
using Server.Data;

namespace Server.Routes.UserReport.POST;

public class PostUserReportEndpoint(AppDbContext ctx) : Endpoint<PostUserReportRequest, UserReportResponse>
{
    public override void Configure()
    {
        Post("/report/users");
        Roles("User", "Admin");
    }

    public override async Task<UserReportResponse> ExecuteAsync(PostUserReportRequest request, CancellationToken ct)
    {
        PostUserReportData data = new(ctx);
        return await data.ReportUserAsync(request, ct);
    }
}