using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Entities;
using Server.Exceptions;

namespace Server.Routes.Review.DELETE;

public class DeleteReviewData(AppDbContext ctx)
{
    public async Task<ReviewPostResponse> DeleteReviewAsync(Guid reviewId, Guid currentUserId, string currentUserRole, CancellationToken ct)
    {
        ReviewEntity review = await ctx.Reviews.FirstOrDefaultAsync(r => r.Id == reviewId, ct) ??
            throw new NotFoundException("Review not found.");
        
        // Only allow deletion if the review belongs to the current user or if the user is an admin
        if (review.UserId != currentUserId && currentUserRole != "Admin")
            throw new UnauthorizedException("You cannot delete another user's review.");
        
        ctx.Reviews.Remove(review);
        await ctx.SaveChangesAsync(ct);

        return new()
        {
            Message = "Review successfully deleted."
        };
    }
}