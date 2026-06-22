using Microsoft.EntityFrameworkCore;
using Server.Data;

namespace Server.Routes.ReviewReport.GET;

public class GetReviewReportsData(AppDbContext ctx)
{
    private const int PerPage = 10;

    public async Task<GetReviewReportsResponse> GetReviewReportsAsync(int page, CancellationToken ct)
    {
        int total = await ctx.ReportedReviews.CountAsync(ct);
        int lastVisiblePage = total == 0 ? 1 : (int)Math.Ceiling(total / (double)PerPage);
        int safePage = Math.Clamp(page, 1, lastVisiblePage);

        List<ReviewReport> reports = await ctx.ReportedReviews
            .Include(r => r.ReportedReview)
            .Skip((safePage - 1) * PerPage)
            .Take(PerPage)
            .Select(r => new ReviewReport
            {
                Id = r.Id,
                ReportedReviewId = r.ReportedReviewId,
                CreatedAt = r.CreatedAt,
                Text = r.ReportedReview.Text
            })
            .ToListAsync(ct);
        
        return new()
        {
            Pagination = new()
            {
                CurrentPage = page,
                HasNextPage = page < lastVisiblePage,
                LastVisiblePage = lastVisiblePage,
                Items = new()
                {
                    Count = reports.Count,
                    Total = total,
                    PerPage = PerPage
                }
            },
            Reports = reports
        };
    }
}