using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Entities;
using Server.Exceptions;

namespace Server.Routes.BugReport.DELETE;

public class DeleteBugReportData(AppDbContext ctx)
{
    public async Task<BugReportMessageResponse> DeleteReportAsync(Guid reportId, CancellationToken ct)
    {
        ReportedBugEntity report = await ctx.ReportedBugs.FirstOrDefaultAsync(b => b.Id == reportId, ct) ??
            throw new NotFoundException("The report doesn't exist.");
        
        ctx.Remove(report);

        await ctx.SaveChangesAsync(ct);

        return new()
        {
            Message = "The bug report has been removed."
        };
    }
}