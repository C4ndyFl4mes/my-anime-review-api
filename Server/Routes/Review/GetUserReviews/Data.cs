using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Exceptions;

namespace Server.Routes.Review.GetUserReviews;

public class GetUserReviewsData(AppDbContext ctx)
{
    private const int PerPage = 10;

    public async Task<ReviewExtendedResponse> GetUserReviewsAsync(Guid targetUserId, Guid currentUserId, int page, CancellationToken ct)
    {
        if (!await ctx.Users.AnyAsync(u => u.Id == targetUserId, ct))
            throw new NotFoundException("The user doesn't exist.");
        
        int total = await ctx.Reviews.CountAsync(r => r.UserId == targetUserId, ct);
        int lastVisiblePage = total == 0 ? 1 : (int)Math.Ceiling(total / (double)PerPage);
        int safePage = Math.Clamp(page, 1, lastVisiblePage);

        List<ReviewExtended> reviews = await ctx.Reviews
            .AsNoTracking()
            .Include(r => r.Anime)
            .Include(r => r.User)
            .Where(r => r.UserId == targetUserId)
            .OrderByDescending(r => r.CreatedAt)
            .Skip((safePage - 1) * PerPage)
            .Take(PerPage)
            .Select(r => new ReviewExtended
            {
                Review = new()
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
                },
                AnimeId = r.AnimeId,
                Title = r.Anime.Title
            })
            .ToListAsync(ct);
        
        return new()
        {
            Pagination = new()
            {
                CurrentPage = page,
                HasNextPage = page < lastVisiblePage,
                LastVisiblePage = lastVisiblePage,
                Items = new()
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