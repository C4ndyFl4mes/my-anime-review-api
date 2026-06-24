using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Entities;
using Server.Exceptions;

namespace Server.Routes.UserReport.DELETE;

public class DeleteUserReportData(AppDbContext ctx)
{
    public async Task<UserReportResponse> DeleteUserReportAsync(Guid reportId, CancellationToken ct)
    {
        ReportedUserEntity report = await ctx.ReportedUsers.FirstOrDefaultAsync(r => r.Id == reportId, ct) ??
            throw new NotFoundException("The report doesn't exist.");
        
        ctx.ReportedUsers.Remove(report);

        await ctx.SaveChangesAsync(ct);

        return new()
        {
            Message = "The user report has been removed."
        };
    }
}