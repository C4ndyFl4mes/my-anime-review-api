using FastEndpoints;
using Server.Data;

namespace Server.Routes.ReviewReport.DELETE;

public class DeleteReviewReportEndpoint(AppDbContext ctx) : EndpointWithoutRequest<ReviewReportResponse>
{
    public override void Configure()
    {
        Delete("/report/reviews/{id}");
        Roles("Admin");
    }

    public override async Task<ReviewReportResponse> ExecuteAsync(CancellationToken ct)
    {
        Guid reportId = Route<Guid>("id", isRequired: true);

        DeleteReviewReportData data = new(ctx);
        return await data.DeleteReviewReportAsync(reportId, ct);
    }
}