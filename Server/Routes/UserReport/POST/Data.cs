using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Entities;
using Server.Exceptions;

namespace Server.Routes.UserReport.POST;

public class PostUserReportData(AppDbContext ctx)
{
    public async Task<UserReportResponse> ReportUserAsync(PostUserReportRequest request, CancellationToken ct)
    {
        if (!await ctx.Users.AnyAsync(u => u.Id == request.ReportedUserId, ct))
            throw new NotFoundException("The user doesn't exist.");
        
        ReportedUserEntity report = new()
        {
            Id = Guid.NewGuid(),
            ReportedUserId = request.ReportedUserId,
            Reason = request.Reason,
            CreatedAt = DateTime.UtcNow,
            ReportedUser = null! // Set by EF.
        };

        await ctx.ReportedUsers.AddAsync(report, ct);

        await ctx.SaveChangesAsync(ct);

        return new()
        {
            Message = "You've successfully reported a user."
        };
    }
}