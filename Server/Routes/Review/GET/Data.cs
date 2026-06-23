using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Entities;
using Server.Exceptions;
using Server.Routes.Jikan;

namespace Server.Routes.Review.GET;

public class GetAnimeReviewsData(AppDbContext ctx)
{
    private const int PerPage = 10;

    public async Task<ReviewResponse> GetReviewsAsync(int malId, Guid currentUserId, int page, CancellationToken ct)
    {
        if (!await ctx.Animes.AnyAsync(a => a.Id == malId, ct))
            throw new NotFoundException("Anime not found.");

        IQueryable<ReviewEntity> baseQuery = ctx.Reviews
            .AsNoTracking()
            .Include(r => r.User)
            .Where(r => r.AnimeId == malId)
            .OrderByDescending(r => r.CreatedAt);
        
        int total = await baseQuery.CountAsync(ct);
        int lastVisiblePage = total == 0 ? 1 : (int)Math.Ceiling(total / (double)PerPage);
        int safePage = Math.Clamp(page, 1, lastVisiblePage);

        List<Review> reviews = await baseQuery
            .Skip((safePage - 1) * PerPage)
            .Take(PerPage)
            .Select(r => new Review
            {
                Id = r.Id,
                UserId = r.User.Id,
                Text = r.Text,
                Score = r.Score,
                Username = r.User.Username,
                ProfileImageURL = r.User.ProfileImageURL,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt,
                HelpfulCount = r.HelpfulByUsers.Count,
                IsHelpfulByCurrentUser = currentUserId != Guid.Empty && r.HelpfulByUsers.Any(h => h.UserId == currentUserId)
            })
            .ToListAsync(ct);
        
        return new ReviewResponse
        {
            Pagination = new Pagination
            {
                CurrentPage = safePage,
                LastVisiblePage = lastVisiblePage,
                HasNextPage = safePage < lastVisiblePage,
                Items = new Items
                {
                    Count = reviews.Count,
                    Total = total,
                    PerPage = PerPage
                }
            },
            Reviews = reviews
        };
    }
}