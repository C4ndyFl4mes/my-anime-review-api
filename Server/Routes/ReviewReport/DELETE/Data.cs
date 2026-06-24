using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Entities;
using Server.Exceptions;

namespace Server.Routes.ReviewReport.DELETE;

public class DeleteReviewReportData(AppDbContext ctx)
{
    public async Task<ReviewReportResponse> DeleteReviewReportAsync(Guid reportId, CancellationToken ct)
    {
        ReportedReviewEntity report = await ctx.ReportedReviews.FirstOrDefaultAsync(r => r.Id == reportId, ct) ??
            throw new NotFoundException("The report doesn't exist.");
        
        ctx.Remove(report);

        await ctx.SaveChangesAsync(ct);

        return new()
        {
            Message = "The review report has been removed."  
        };
    }
}