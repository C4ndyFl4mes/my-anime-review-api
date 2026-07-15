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
            .ThenInclude(r => r.User)
            .Skip((safePage - 1) * PerPage)
            .Take(PerPage)
            .Select(r => new ReviewReport
            {
                Id = r.Id,
                CreatedAt = r.CreatedAt,
                ReportedReview = new()
                {
                    Id = r.ReportedReview.Id,
                    UserId = r.ReportedReview.UserId,
                    Text = r.ReportedReview.Text,
                    Score = r.ReportedReview.Score,
                    Username = r.ReportedReview.User.Username,
                    ProfileImageURL = r.ReportedReview.User.ProfileImageURL,
                    CreatedAt = r.ReportedReview.CreatedAt,
                    UpdatedAt = r.ReportedReview.UpdatedAt,
                    HelpfulCount = r.ReportedReview.HelpfulByUsers.Count,
                    IsHelpfulByCurrentUser = false
                }
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