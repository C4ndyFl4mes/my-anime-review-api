using FastEndpoints;
using Server.Data;

namespace Server.Routes.ReviewReport.GET;

public class GetReviewReportsEndpoint(AppDbContext ctx) : EndpointWithoutRequest<GetReviewReportsResponse>
{
    public override void Configure()
    {
        Get("/report/reviews");
        Roles("Admin");
    }

    public override async Task<GetReviewReportsResponse> ExecuteAsync(CancellationToken ct)
    {
        int page = Query<int>("page", isRequired: false);
        if (page < 1)
            page = 1;
        
        GetReviewReportsData data = new(ctx);
        return await data.GetReviewReportsAsync(page, ct);
    }
}