using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Entities;
using Server.Exceptions;

namespace Server.Routes.Review.PUT;

public class UpdateReviewData(AppDbContext ctx)
{
    public async Task<ReviewPostResponse> UpdateReviewAsync(Guid reviewId, Guid currentUserId, ReviewPostRequest request, CancellationToken ct)
    {
        ReviewEntity review = await ctx.Reviews.FirstOrDefaultAsync(r => r.Id == reviewId, ct) ??
            throw new NotFoundException("Review not found.");
        
        if (review.UserId != currentUserId)
            throw new UnauthorizedException("You cannot edit another user's review.");
        
        review.Score = request.Score;
        review.Text = request.Text;

        if (ctx.ChangeTracker.HasChanges())
        {
            review.UpdatedAt = DateTime.UtcNow;
            await ctx.SaveChangesAsync(ct);
        }

        return new()
        {
            Message = "The review has been updated."
        };
    }
}