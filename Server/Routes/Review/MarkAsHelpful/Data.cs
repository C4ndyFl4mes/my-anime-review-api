using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Entities;
using Server.Exceptions;

namespace Server.Routes.Review.MarkAsHelpful;

public class MarkAsHelpfulData(AppDbContext ctx)
{
    public async Task<MarkAsHelpfulResponse> MarkReviewAsHelpfulAsync(MarkAsHelpfulRequest request, Guid currentUserId, CancellationToken ct)
    {
        if (!await ctx.Reviews.AnyAsync(r => r.Id == request.ReviewId, ct))
            throw new NotFoundException("The review to mark as helpful doesn't exist.");

        if (request.IsHelpful)
        {
            HelpfulEntity helpful = new()
            {
                UserId = currentUserId,
                ReviewId = request.ReviewId,
                User = null!, // Will be set by EF Core
                Review = null! // Will be set by EF Core
            };

            await ctx.AddAsync(helpful, ct);
        }
        else
        {
            HelpfulEntity? helpful = await ctx.HelpfulMarks.FirstOrDefaultAsync(h => h.UserId == currentUserId && h.ReviewId == request.ReviewId, ct);
            if (helpful != null)
                ctx.HelpfulMarks.Remove(helpful);
            
        }

        await ctx.SaveChangesAsync(ct);

        return new()
        {
            IsHelpful = request.IsHelpful
        };
    }
}