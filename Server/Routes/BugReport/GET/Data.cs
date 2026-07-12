using Server.Data;
using Server.Enums;
using Microsoft.EntityFrameworkCore;
using Server.Routes.Tenrai;
using Server.Entities;

namespace Server.Routes.BugReport.GET;

public class GetBugReportsData(AppDbContext ctx)
{
    private const int PerPage = 10;

    public async Task<GetBugReportsResponse> GetBugReportsAsync(BugState? state, int page, CancellationToken ct)
    {
        IQueryable<ReportedBugEntity> query = ctx.ReportedBugs.AsNoTracking();

        if (state.HasValue)
            query = query.Where(b => b.State == state.Value);

        int total = await query.CountAsync(ct);
        int lastVisiblePage = total == 0 ? 1 : (int)Math.Ceiling(total / (double)PerPage);
        int safePage = Math.Clamp(page, 1, lastVisiblePage);

        List<BugReport> bugs = await query
            .OrderByDescending(b => b.CreatedAt)
            .Skip((safePage - 1) * PerPage)
            .Take(PerPage)
            .Select(b => new BugReport
            {
                Id = b.Id,
                State = b.State,
                Details = b.Details,
                CreatedAt = b.CreatedAt
            })
            .ToListAsync(ct);

        return new GetBugReportsResponse
        {
            Pagination = new Pagination
            {
                CurrentPage = safePage,
                LastVisiblePage = lastVisiblePage,
                HasNextPage = safePage < lastVisiblePage,
                Items = new Items
                {
                    Count = bugs.Count,
                    Total = total,
                    PerPage = PerPage
                }
            },
            Reports = bugs
        };
    }
}
