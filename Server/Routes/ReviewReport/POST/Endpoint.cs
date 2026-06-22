using FastEndpoints;
using Server.Data;

namespace Server.Routes.ReviewReport.POST;

public class PostReviewReportEndpoint(AppDbContext ctx) : Endpoint<PostReviewReportRequest, ReviewReportResponse>
{
    public override void Configure()
    {
        Post("/report/reviews");
        Roles("User", "Admin");
    }

    public override async Task<ReviewReportResponse> ExecuteAsync(PostReviewReportRequest request, CancellationToken ct)
    {
        PostReviewReportData data = new(ctx);
        return await data.PostReviewReportAsync(request.ReportedReviewId, ct);
    }
}