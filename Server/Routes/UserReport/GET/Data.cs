using Microsoft.EntityFrameworkCore;
using Server.Data;

namespace Server.Routes.UserReport.GET;

public class GetUserReportsData(AppDbContext ctx)
{
    private const int PerPage = 10;

    public async Task<GetUserReportsResponse> GetUserReportsAsync(int page, CancellationToken ct)
    {
        int total = await ctx.ReportedUsers.CountAsync(ct);
        int lastVisiblePage = total == 0 ? 1 : (int)Math.Ceiling(total / (double)PerPage);
        int safePage = Math.Clamp(page, 1, lastVisiblePage);

        List<UserReport> reports = await ctx.ReportedUsers
            .Skip((safePage - 1) * PerPage)
            .Take(PerPage)
            .Select(r => new UserReport
            {
                Id = r.Id,
                ReportedUserId = r.ReportedUserId,
                Reason = r.Reason,
                CreatedAt = r.CreatedAt
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