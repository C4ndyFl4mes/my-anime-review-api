using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Entities;
using Server.Enums;
using Server.Exceptions;

namespace Server.Routes.BugReport.PUT;

public class ChangeStateData(AppDbContext ctx)
{
    public async Task<ChangeStateResponse> ChangeStateAsync(Guid reportId, CancellationToken ct)
    {
        ReportedBugEntity report = await ctx.ReportedBugs.FirstOrDefaultAsync(b => b.Id == reportId, ct) ??
            throw new NotFoundException("The report doesn't exist.");
        
        BugState oldState = report.State;

        BugState newState = oldState switch
        {
            BugState.Pending => BugState.Planned,
            BugState.Planned => BugState.InProgress,
            BugState.InProgress => BugState.Completed,
            BugState.Completed => BugState.Rejected,
            BugState.Rejected => BugState.Pending,
            _ => throw new InvalidOperationException("Invalid bug state.")
        };

        report.State = newState;
        await ctx.SaveChangesAsync(ct);

        return new()
        {
            NewState = newState.ToString(),
            Message = $"The state changed from {oldState} to {newState}."
        };
    }
}