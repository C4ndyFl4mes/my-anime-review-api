using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Entities;
using Server.Exceptions;

namespace Server.Routes.ReviewReport.POST;

public class PostReviewReportData(AppDbContext ctx)
{
    public async Task<ReviewReportResponse> PostReviewReportAsync(Guid reportedReviewId, CancellationToken ct)
    {
        if (!await ctx.Reviews.AnyAsync(r => r.Id == reportedReviewId, ct))
            throw new NotFoundException("The review doesn't exist.");
        
        ReportedReviewEntity report = new()
        {
            Id = Guid.NewGuid(),
            ReportedReviewId = reportedReviewId,
            CreatedAt = DateTime.UtcNow,
            ReportedReview = null! // Set by EF.  
        };

        await ctx.ReportedReviews.AddAsync(report, ct);

        await ctx.SaveChangesAsync(ct);

        return new()
        {
            Message = "The review has been reported."
        };
    }
}