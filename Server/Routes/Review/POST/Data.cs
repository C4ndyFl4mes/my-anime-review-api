using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Entities;
using Server.Exceptions;

namespace Server.Routes.Review.POST;

public class PostReviewData(AppDbContext ctx)
{
    public async Task<ReviewPostResponse> PostReviewAsync(int malId, Guid currentUserId, ReviewPostRequest request, CancellationToken ct)
    {
        if (!await ctx.Animes.AnyAsync(a => a.Id == malId, ct))
            throw new NotFoundException("Anime not found.");

        if (currentUserId == Guid.Empty)
            throw new UnauthorizedException("You're not allowed to post a review.");
        
        if (await ctx.Reviews.Where(r => r.UserId == currentUserId).AnyAsync(r => r.AnimeId == malId, ct))
            throw new ConflictException("You cannot post more than one review per anime.");

        ReviewEntity newReview = new()
        {
            Id = new Guid(),
            AnimeId = malId,
            UserId = currentUserId,
            Text = request.Text,
            Score = request.Score,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null,
            Anime = null!, // Will be set by EF Core
            User = null!, // Will be set by EF Core
            HelpfulByUsers = []
        };

        await ctx.Reviews.AddAsync(newReview, ct);

        await ctx.SaveChangesAsync(ct);

        return new()
        {
            Message = "Review successfully posted."
        };
    }
}